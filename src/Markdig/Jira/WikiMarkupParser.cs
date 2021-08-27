using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Markdig.Extensions.CustomContainers;
using Markdig.Helpers;
using Markdig.Collections;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Jira
{
    public class WikiMarkupParser
    {
        private MarkdownDocument _currentDoc;
        private Block _currentBlock;
        private InlineList<ListBlock> _listBlocks;
        private InlineList<Line> _lines;
        private InlineList<BraceIndex> _openBraceIndices;
        private InlineList<WikiBlock> _wikiBlocks;
        private InlineList<TextPart> _textParts;
        private InlineList<uint> _openIndices;
        private uint _wikiBlockIndex;
        private string _text;
        private uint _currentLineIndex;
        private Action? _currentLineBlockAction;

        public WikiMarkupParser()
        {
            _listBlocks = new InlineList<ListBlock>(4);
            _lines = new InlineList<Line>(4);
            _openBraceIndices = new InlineList<BraceIndex>(4);
            _wikiBlocks = new InlineList<WikiBlock>(4);
            _textParts = new InlineList<TextPart>(4);
            _openIndices = new InlineList<uint>(4);
        }

        public MarkdownDocument Parse(string text)
        {
            try
            {
                _text = text;
                _currentDoc = new MarkdownDocument();
                _currentBlock = _currentDoc;
                _wikiBlockIndex = 0;

                PrepareLines();
                CollectBlocks();
                CloseOpenedBlocks();
                ProcessLines();
                return _currentDoc;
            }
            finally
            {
                // Cleanup, don't hold any references
                _text = null;
                _currentDoc = null;
                _currentBlock = null;
                _lines.Count = 0;
                _listBlocks.Count = 0;
                _openBraceIndices.Count = 0;
                _wikiBlocks.Count = 0;
                _textParts.Count = 0;
                _openIndices.Count = 0;
                _lines.Count = 0;
            }
        }

        private void PrepareLines()
        {
            _lines.Clear();
            _openBraceIndices.Clear();
            var text = _text;
            
            // Rough evaluate the number of lines (assuming they are all normal \r\n or \n)
            uint lineCount = 0;
            int index = 0;
            while (true)
            {
                var nextIndex = text.IndexOf('\n', index);
                if (nextIndex <= -1) break;
                lineCount++;
                index = nextIndex + 1;
            }

            // +1 for the last line that might not have a new line
            if (lineCount + 1 > _lines.Capacity)
            {
                _lines.Capacity = lineCount + 1;
            }

            // Read all lines and collect their positions
            var reader = new InternalLineReader(text);
            while (true)
            {
                var line = reader.ReadLine(out var firstBraceIndex
                );
                if (line.IsEmpty) break;
                if (firstBraceIndex >= 0)
                {
                    _openBraceIndices.Add(new BraceIndex(_lines.Count, firstBraceIndex));
                }
                _lines.Add(new Line(line.Start, line.End, line.NewLine));
            }
        }

        /// <summary>
        /// Collect all blocks (e.g {code}) to <see cref="_wikiBlocks"/>
        /// </summary>
        /// <param name="text"></param>
        private void CollectBlocks()
        {
            _wikiBlocks.Clear();
            var text = _text;

            // Adjust capacity (heuristic with the number of existing braces `{`
            var capacity = _openBraceIndices.Count * 3 / 2;
            if (capacity > _wikiBlocks.Capacity)
            {
                _wikiBlocks.Capacity = capacity;
            }

            // Iterate on each brace and try to detect blocks
            foreach (var openBraceIndex in _openBraceIndices)
            {
                var line = _lines[openBraceIndex.LineIndex];
                var endLine = line.End;
                var i = openBraceIndex.CharIndex;
                while (i >= 0)
                {
                    var pc = i == 0 ? '\0' : text[i - 1];
                    if (pc != '{' && pc != '\\')
                    {
                        var ni = i + 1;
                        var slice = new StringSlice(text, ni, endLine);
                        //     012345
                        // +5: {code}
                        // parse code
                        var matchCode = slice.Match("code");
                        if (matchCode || slice.Match("panel"))
                        {
                            slice.Start += matchCode ? 4 : 5;
                            var end = slice.Start;
                            var c = slice.NextChar();
                            var blockKind = WikiBlockKind.Default;
                            if (c == ':')
                            {
                                var endOfBlockIndex = slice.IndexOfAbsolute('}', slice.Start);
                                if (endOfBlockIndex > 0 && endOfBlockIndex <= endLine)
                                {
                                    blockKind = matchCode ? WikiBlockKind.CodeWithArgs : WikiBlockKind.PanelWithArgs;
                                    end = endOfBlockIndex;
                                }
                                else
                                {
                                    goto next;
                                }
                            }
                            else if (c == '}')
                            {
                                // valid
                                blockKind = matchCode ? WikiBlockKind.Code : WikiBlockKind.Panel;
                            }

                            AddWikiBlock(openBraceIndex.LineIndex, i, end, blockKind);
                            i = end;
                        }
                        else if (slice.Match("noformat}"))
                        {
                            var end = i + 9;
                            AddWikiBlock(openBraceIndex.LineIndex, i, end, WikiBlockKind.NoFormat);
                            i = end;
                        }
                        else if (slice.Match("quote}"))
                        {
                            var end = i + 6;
                            AddWikiBlock(openBraceIndex.LineIndex, i, end, WikiBlockKind.Quote);
                            i = end;
                        }
                        else
                        {
                            // Grab any 
                            var c = slice.NextChar();
                            if (c.IsAlphaNumeric())
                            {
                                var endOfBlockIndex = slice.IndexOfAbsolute('}', slice.Start);
                                if (endOfBlockIndex > 0 && endOfBlockIndex <= endLine)
                                {
                                    AddWikiBlock(openBraceIndex.LineIndex, i, endOfBlockIndex, WikiBlockKind.Default);
                                    i = endOfBlockIndex;
                                }
                            }
                        }
                    }

                    next:
                    // Collect on the same line other blocks
                    var length = endLine - i - 1;
                    if (length < 0) break;
                    i = text.IndexOf('{', i + 1, length);
                }
            }
        }

        private void AddWikiBlock(uint lineIndex, int start, int end, WikiBlockKind kind)
        {

            var blockFlags = kind == WikiBlockKind.Default ? TextPartFlags.Default : TextPartFlags.Open;

            int? otherOpenIndex = null;

            // Process previous blocks
            if (kind != WikiBlockKind.Default)
            {
                var wikiBlocks = _wikiBlocks;
                for (uint i = 0; i < _openIndices.Count; i++)
                {
                    var index = _openIndices[i];
                    ref var previousBlock = ref wikiBlocks[index];
                    var previousKind = previousBlock.Kind;
                    var previousFlags = previousBlock.Flags;
                    // Valid cases:
                    // ------------
                    // {quote}+ / {quote}+
                    // {noformat}+ / {noformat}+
                    // {code}+ / {code}+
                    // {code:args}- / {code}+
                    // {panel}+ / {panel}+
                    // {panel:args}- / {panel}+
                    //
                    // Invalid cases:
                    // ------------
                    // {code}+ / {code:args}-
                    // {panel}+ / {panel:args}-
                    // {code:args}- / {code:args}-

                    if (previousBlock.OtherBlockIndex < 0 && previousFlags == TextPartFlags.Open && kind > 0 && (previousKind == kind || (int)previousKind + kind == 0))
                    {
                        // If we have a valid noformat/code block, disable any kind of blocks in between
                        bool isFormatOrCode = kind == WikiBlockKind.NoFormat || kind == WikiBlockKind.Code;

                        for (uint j = i + 1; j < _openIndices.Count; j++)
                        {
                            var nextIndex = _openIndices[j];
                            ref var innerWikiBlock = ref wikiBlocks[nextIndex];
                            DeactivateBlockOpenAndClose(ref innerWikiBlock, isFormatOrCode);
                        }
                        blockFlags = TextPartFlags.Close;

                        // Attach the previous block with 
                        previousBlock.OtherBlockIndex = (int)_wikiBlocks.Count;
                        otherOpenIndex = (int)i;

                        if (i == 0)
                        {
                            _openIndices.Count = 0;
                        }

                        break;
                    }
                }
            }

            var newBlock = new WikiBlock(lineIndex, start, end, kind, blockFlags);
            if (otherOpenIndex.HasValue)
            {
                newBlock.OtherBlockIndex = otherOpenIndex.Value;
            }

            if (blockFlags == TextPartFlags.Open)
            {
                _openIndices.Add(_wikiBlocks.Count);
            }

            _wikiBlocks.AddByRef(newBlock);
        }

        private void CloseOpenedBlocks()
        {
            var wikiBlocks = _wikiBlocks;
            for (uint i = 0; i < _openIndices.Count; i++)
            {
                var index = _openIndices[i];
                ref var previousBlock = ref wikiBlocks[index];
                var previousKind = previousBlock.Kind;
                var previousFlags = previousBlock.Flags;
                if (previousBlock.OtherBlockIndex < 0 && previousFlags == TextPartFlags.Open)
                {
                    // If we have a valid noformat/code block, disable any kind of blocks in between
                    DeactivateBlockOpenAndClose(ref previousBlock, false);
                }
            }
            _openIndices.Count = 0;
        }

        private void DeactivateBlockOpenAndClose(ref WikiBlock block, bool format)
        {
            var nextIndex = block.OtherBlockIndex;
            DeactivateBlock(ref block, format);
            if (nextIndex >= 0)
            {
                DeactivateBlock(ref _wikiBlocks[(uint)nextIndex], format);
            }
        }

        private static void DeactivateBlock(ref WikiBlock block, bool format)
        {
            if (format)
            {
                // In format, all blocks in between will be skip
                block.Kind = WikiBlockKind.Default;
                block.Flags = TextPartFlags.Default;
                block.OtherBlockIndex = -1;
            }
            else if (block.OtherBlockIndex < 0)
            {
                // Any block in-between that is not part of a pair open-close blocks
                // becomes a single open-close block
                block.Flags = TextPartFlags.Open | TextPartFlags.Close;
                block.OtherBlockIndex = -1;
            }
        }

        private void ProcessLines()
        {
            _currentLineIndex = 0;
            for (; _currentLineIndex < _lines.Count; _currentLineIndex++)
            {
                var line = _lines[_currentLineIndex];
                var slice = new StringSlice(_text, line.Start, line.End, line.NewLine);
                ProcessLine(slice);
            }
        }

        private void ProcessLine(StringSlice line)
        {
            _currentLineBlockAction = null;
            var lineStart = line.Start;
            line.TrimStart();

            var position = line.Start;
            var c = line.CurrentChar;

            // Maximum number of whitespace characters before we start to have some content
            // (to avoid code blocks in markdown when indented at 4 whitespace or more)
            var spaceLengthBefore = Math.Min(position - lineStart, 3);
            var triviaBefore = position > lineStart ? new StringSlice(_text, lineStart, lineStart + spaceLengthBefore - 1) : default;

            // Process headings
            if (c == 'h')
            {
                var nc = line.PeekChar();
                if (nc >= '1' && nc <= '6' && line.PeekChar(2) == '.' && line.PeekChar(3).IsWhitespace())
                {
                    line.Start += 4; // skip "h1. "

                    _currentLineBlockAction = () =>
                    {
                        
                        var block = new HeadingBlock()
                        {
                            HeaderChar = '#',
                            HeaderCharCount = (int) (nc - '0'),
                            IsSetext = false,
                            NewLine = line.NewLine,
                            TriviaBefore = triviaBefore,
                            TriviaAfterAtxHeaderChar = new StringSlice(_text, line.Start - 1, line.Start - 1),
                        };

                        PushBlock(block, true);
                    };

                    // Execute the heading
                    _currentLineBlockAction();
                }
            }
            else if (c == '-' && line.PeekChar(2) == '-' && line.PeekChar(3) == '-' && line.PeekChar(4) == '-')
            {
                // Process break

                bool isWhiteSpace = true;
                for (int i = line.Start + 4; i <= line.End; i++)
                {
                    if (!line[i].IsWhitespace())
                    {
                        isWhiteSpace = false;
                        break;
                    }
                }

                if (isWhiteSpace)
                {
                    var block = new ThematicBreakBlock()
                    {
                        ThematicChar = '-',
                        ThematicCharCount = 3,
                        TriviaBefore = triviaBefore,
                        Content = new StringSlice(line.Text, line.Start, line.End),
                        NewLine = line.NewLine,
                    };

                    PushBlock(block, false);
                    // Break is a whole, we proceed on the next line
                    return;
                }
            }
            else if (c == 'b' && line.PeekChar(2) == 'q' && line.PeekChar(3) == '.' && line.PeekChar(4).IsWhitespace())
            {
                line.Start += 4; // skip "bq. "

                _currentLineBlockAction = () =>
                {
                    var quote = new QuoteBlock() {QuoteChar = '>'};
                    PushBlock(quote, true);
                };

                // continue parsing the line
            }
            else if (c == '*' || c == '-' || c == '#')
            {
                bool valid = false;
                for (int i = line.Start; i <= line.End; i++)
                {
                    var nc = line[i];
                    if (nc.IsWhitespace())
                    {
                        valid = true;
                        break;
                    }
                    if (nc != '*' && nc != '-' && nc != '#')
                    {
                        break;
                    }
                }

                if (valid)
                {
                    _currentLineBlockAction = () =>
                    {
                        // Collect active list blocks
                        _listBlocks.Clear();
                        var visitBlock = _currentBlock;
                        while (true)
                        {
                            if (visitBlock is ListBlock listBlock)
                            {
                                _listBlocks.Add(listBlock);
                                visitBlock = visitBlock.Parent;
                                continue;
                            }
                            else if (visitBlock is ListItemBlock)
                            {
                                visitBlock = visitBlock.Parent;
                                continue;
                            }

                            break;
                        }

                        uint blockIndex = 0;
                        for (int i = line.Start; i <= line.End; i++, blockIndex++)
                        {
                            var nc = line[i];
                            if (nc.IsWhitespace())
                            {
                                line.Start = i + 1;
                                break;
                            }

                            var kind = nc == '*' || nc == '-' ? nc : '1';

                            var currentListBlock = blockIndex < _listBlocks.Count ? _listBlocks[blockIndex] : null;
                            if (currentListBlock == null || currentListBlock.BulletType != kind)
                            {
                                var nextListBlock = new ListBlock()
                                {
                                    BulletType = kind,
                                };

                                if (kind == '#')
                                {
                                    nextListBlock.IsOrdered = true;
                                    nextListBlock.DefaultOrderedStart = "1";
                                    nextListBlock.BulletType = '1';
                                    nextListBlock.OrderedStart = "1";
                                }

                                var containerBlock = currentListBlock?.Parent ?? _currentDoc;
                                containerBlock.Add(nextListBlock);
                                _currentDoc.Add(nextListBlock);

                                currentListBlock = nextListBlock;
                            }

                            var nextListItemBlock = new ListItemBlock();
                            currentListBlock.Add(nextListItemBlock);
                            _currentBlock = nextListItemBlock;
                        }
                    };
                }
            }

            // We might have a pending Block that we need to create for this line
            // We keep _currentLineBlockAction in case we need to call it again for the same
            // line. It happens if a block is inlined in the line (e.g {noformat}...{noformat})
            _currentLineBlockAction?.Invoke();

            try
            {
                // Process inlines
                CollectTextPartsForLine(line);

                FlushTextPartsToMarkdownObjects(line.NewLine);
            }
            finally
            {
                // Clear the block action to not have it around in memory after we have processed a line
                _currentLineBlockAction = null;
            }
        }

        private const string SpecialChars = "*_?-+^~{!";

        private static bool IsSpecialChar(char c) => SpecialChars.IndexOf(c) >= 0;
        
        private void CollectTextPartsForLine(StringSlice line)
        {
            // Reset text parts for line (don't clear to avoid a costly memset)
            _textParts.Count = 0;

            var c = line.CurrentChar;

            var pc = '\0';

            while (c != '\0')
            {
                var nc = line.PeekChar();

                TextEffectKind effectKind = TextEffectKind.Default;
                switch (c)
                {
                    case '*':
                        effectKind = TextEffectKind.Strong;
                        break;
                    case '_':
                        effectKind = TextEffectKind.Emphasis;
                        break;
                    case '?':
                        effectKind = nc == '?' ? TextEffectKind.Citation : TextEffectKind.Default;
                        break;
                    case '-':
                        effectKind = TextEffectKind.Deleted;
                        break;
                    case '+':
                        effectKind = TextEffectKind.Inserted;
                        break;
                    case '^':
                        effectKind = TextEffectKind.Superscript;
                        break;
                    case '~':
                        effectKind = TextEffectKind.Subscript;
                        break;
                    case '{':
                        if (TryFindNextBlock(line.Start, out var blockIndex))
                        {
                            var block = _wikiBlocks[blockIndex];

                            var nextPosition = PushBlock(ref block, ref line);
                            if (nextPosition < 0) return;

                            c = line.PeekCharAbsolute(line.Start);
                            nc = line.PeekCharAbsolute(line.Start + 1);
                            goto nextChar;
                        }

                        effectKind = nc == '{' ? TextEffectKind.Monospaced : TextEffectKind.Default;
                        break;
                    case '[':
                        effectKind = TextEffectKind.Link;
                        break;
                    case '\\':
                        effectKind = nc == '\\' ? TextEffectKind.Hardline : TextEffectKind.MaybeEscape;
                        break;
                    case '!':
                        effectKind = TextEffectKind.Attachment;
                        break;
                    default:
                        effectKind = TextEffectKind.Default;
                        break;
                }

                if (effectKind != TextEffectKind.Default)
                {
                    if (effectKind == TextEffectKind.MaybeEscape)
                    {
                        if (IsSpecialChar(nc))
                        {
                            var end = line.Start + 1;
                            PushTextPart(new TextPart(line.Start, end, TextEffectKind.Escape, TextPartFlags.Default));
                            nc = line.PeekCharAbsolute(end + 1);
                            line.Start = end;
                            goto nextChar;
                        }
                    }
                    else if (effectKind == TextEffectKind.Hardline)
                    {
                        var end = line.Start + 1;
                        PushTextPart(new TextPart(line.Start, end, TextEffectKind.Hardline, TextPartFlags.Default));
                        nc = line.PeekCharAbsolute(end + 1);
                        line.Start = end;
                        goto nextChar;
                    }
                    else if (effectKind == TextEffectKind.Link)
                    {
                        var end = TryParseLink(line, out effectKind);
                        if (end > 0 && !ContainsBlock(line.Start, end))
                        {
                            PushTextPart(new TextPart(line.Start, end, effectKind, TextPartFlags.Default));
                            c = '}';
                            nc = line.PeekCharAbsolute(end + 1);
                            line.Start = end;
                            goto nextChar;
                        }
                    }
                    else
                    {
                        // The characters after the delimiter {
                        // "x{y" => pc = "x", nc = ac = "y"
                        // "x{{y" => pc = "x", nc = "{", ac = "y"
                        var nnc = effectKind == TextEffectKind.Citation || effectKind == TextEffectKind.Monospaced ? line.PeekChar(2) : nc;
                        var partFlags = CheckOpenCloseDelimiter(pc, nnc);

                        if (partFlags != TextPartFlags.Default)
                        {
                            if (effectKind == TextEffectKind.Monospaced)
                            {
                                // If we have a valid monospaced block, we grab the entire block
                                var end = MatchClosingMonospaced(line);
                                if (end > 0)
                                {
                                    if (!ContainsBlock(line.Start, end))
                                    {
                                        PushTextPart(new TextPart(line.Start, end, TextEffectKind.Monospaced, TextPartFlags.Default));
                                        c = '}';
                                        nc = line.PeekCharAbsolute(end + 1);
                                        line.Start = end;
                                        goto nextChar;
                                    }
                                }
                            }
                            else
                            {
                                var end = line.Start + (effectKind == TextEffectKind.Citation ? 2 : 1);
                                PushTextPart(new TextPart(line.Start, end, effectKind, partFlags));

                                if (effectKind == TextEffectKind.Citation)
                                {
                                    nc = line.PeekCharAbsolute(end + 1);
                                    line.Start = end;
                                }

                                goto nextChar;
                            }
                        }
                    }
                }

                // The current char is a plain text
                AddLiteralCharToCurrentTextPart(line.Start);

                nextChar:
                pc = c;
                c = nc;
                line.Start++;
            }
        }

        private ContainerBlock FindContainerForNewBlock()
        {
            var parentBlock = _currentBlock;
            while (parentBlock != null)
            {
                if (parentBlock is not ContainerBlock || parentBlock is ListItemBlock || parentBlock is ListBlock)
                {
                    parentBlock = parentBlock.Parent;
                }
                else
                {
                    break;
                }
            }

            return (ContainerBlock) parentBlock;
        }

        private void PushBlock(Block block, bool setCurrentBlock)
        {
            var parentContainerBlock = FindContainerForNewBlock();

            // If 2 paragraph blocks are following, add an empty line in between
            if (parentContainerBlock.LastChild is ParagraphBlock previousParagraphBlock && block is ParagraphBlock)
            {
                previousParagraphBlock.LinesAfter = new List<StringSlice> {new(string.Empty)};
            }

            parentContainerBlock.Add(block);
            _currentBlock = setCurrentBlock ? block : parentContainerBlock;
        }

        private void PushTextPart(TextPart part)
        {
            switch (part.EffectKind)
            {
                case TextEffectKind.Default:
                    // If we can concat with a previous text entry, we do it
                    if (_textParts.Count > 0)
                    {
                        ref var previous = ref _textParts[_textParts.Count - 1];
                        if (previous.EffectKind == TextEffectKind.Default)
                        {
                            previous.End = part.End;
                            return;
                        }
                    }
                    _textParts.AddByRef(part);
                    break;
                case TextEffectKind.Strong:
                case TextEffectKind.Emphasis:
                case TextEffectKind.Citation:
                case TextEffectKind.Deleted:
                case TextEffectKind.Inserted:
                case TextEffectKind.Superscript:
                case TextEffectKind.Subscript:
                case TextEffectKind.Attachment:
                    bool added = false;
                    if (part.Flags == TextPartFlags.Open)
                    {
                        _openIndices.Add(_textParts.Count);
                        _textParts.Add(part);
                        added = true;
                    }
                    else if (part.Flags == TextPartFlags.Close)
                    {
                        // Let's try to find a matching open
                        for (uint i = 0; i < _openIndices.Count; i++)
                        {
                            var previousPartIndex = _openIndices[i];
                            ref var previousPart = ref _textParts[previousPartIndex];

                            // If we find a matching open
                            if (previousPart.Flags == TextPartFlags.Open && previousPart.EffectKind == part.EffectKind)
                            {
                                // match open <=> close
                                previousPart.OtherTextPartIndex = (int)_textParts.Count;
                                part.OtherTextPartIndex = (int)previousPartIndex;
                                _textParts.Add(part);
                                added = true;

                                // convert all remaining open to plain text
                                for (uint j = i + 1; j < _openIndices.Count; j++)
                                {
                                    ref var discardPart = ref _textParts[_openIndices[j]];
                                    discardPart.EffectKind = TextEffectKind.Default;
                                    discardPart.Flags = TextPartFlags.Default;
                                }

                                // We keep only what was not matched (before the open)
                                _openIndices.Count = i;
                                break;
                            }
                        }
                    }

                    // If the parts was not added, that means that it is converted to a plain text
                    if (!added)
                    {
                        part.EffectKind = TextEffectKind.Default;
                        part.Flags = TextPartFlags.Default;
                        goto case TextEffectKind.Default;
                    }

                    break;
                case TextEffectKind.Monospaced:
                case TextEffectKind.Link:
                case TextEffectKind.LinkExternal:
                case TextEffectKind.LinkExternalWithTitle:
                case TextEffectKind.LinkAnchor:
                case TextEffectKind.LinkAttachment:
                case TextEffectKind.LinkUser:
                case TextEffectKind.Hardline:
                case TextEffectKind.Escape:
                    _textParts.AddByRef(part);
                    break;
            }
        }

        private void FlushTextPartsToMarkdownObjects(NewLine newLine)
        {
            // Transform all opened parts into plain text
            for (uint i = 0; i < _openIndices.Count; i++)
            {
                var partIndex = _openIndices[i];
                ref var part = ref _textParts[partIndex];
                part.EffectKind = TextEffectKind.Default;
                part.Flags = TextPartFlags.Default;
            }
            _openIndices.Count = 0;

            ParagraphBlock? paragraphBlock = null;
            ContainerInline? currentContainerInline = null;
            for (uint i = 0; i < _textParts.Count; i++)
            {
                ref var part = ref _textParts[i];

                if (currentContainerInline == null)
                {
                    var leafBlock = _currentBlock as LeafBlock;
                    if (leafBlock == null)
                    {
                        paragraphBlock = new ParagraphBlock();
                        leafBlock = paragraphBlock;
                        PushBlock(leafBlock, true);
                    }

                    currentContainerInline = leafBlock.Inline;
                    if (currentContainerInline == null)
                    {
                        currentContainerInline = new ContainerInline();
                        leafBlock.Inline = currentContainerInline;
                    }
                }

                var lastInline = currentContainerInline.LastChild;

                switch (part.EffectKind)
                {
                    case TextEffectKind.Default:
                        if (lastInline is LiteralInline previousLiteralInline)
                        {
                            previousLiteralInline.Content = new StringSlice(_text, previousLiteralInline.Content.Start, part.End);
                        }
                        else
                        {
                            currentContainerInline.AppendChild(new LiteralInline(part.ToSlice(_text)));
                        }
                        break;
                    case TextEffectKind.Strong:
                    case TextEffectKind.Emphasis:
                    case TextEffectKind.Citation:
                    case TextEffectKind.Deleted:
                    case TextEffectKind.Inserted:
                    case TextEffectKind.Superscript:
                    case TextEffectKind.Subscript:
                        if (part.Flags == TextPartFlags.Open)
                        {
                            var newContainer = new EmphasisInline();
                            var emphasisAndCount = GetEmphasisDelimiter(part.EffectKind);
                            newContainer.DelimiterChar = emphasisAndCount.Character;
                            newContainer.DelimiterCount = emphasisAndCount.Count;
                            currentContainerInline.AppendChild(newContainer);
                            currentContainerInline = newContainer;
                        }
                        else
                        {
                            Debug.Assert(part.Flags == TextPartFlags.Close);
                            currentContainerInline = currentContainerInline.Parent!;
                        }
                        break;
                    case TextEffectKind.Attachment:
                    case TextEffectKind.Monospaced:
                    case TextEffectKind.Link:
                    case TextEffectKind.LinkExternal:
                    case TextEffectKind.LinkExternalWithTitle:
                    case TextEffectKind.LinkAnchor:
                    case TextEffectKind.LinkAttachment:
                    case TextEffectKind.LinkUser:
                    case TextEffectKind.Hardline:
                    case TextEffectKind.Escape:
                        _textParts.AddByRef(part);
                        break;
                }
            }

            if (paragraphBlock != null)
            {
                paragraphBlock.Inline!.AppendChild(new LineBreakInline() {NewLine = newLine});
            }

            _textParts.Count = 0;
        }

        private static EmphasisCount GetEmphasisDelimiter(TextEffectKind kind)
        {
            return kind switch
            {
                TextEffectKind.Strong => new ('*', 2),
                TextEffectKind.Emphasis => new('_', 1),
                TextEffectKind.Citation => new('~', 2),
                TextEffectKind.Deleted => new('~', 2),
                TextEffectKind.Inserted => new('+', 2),
                TextEffectKind.Superscript => new('^', 1),
                TextEffectKind.Subscript => new('~', 1),
                _ => new('\0', 0)
            };
        }

        private int PushBlock(ref WikiBlock block, ref StringSlice newLine)
        {
            // If we have any pending text parts, we flush them before processing a block
            FlushTextPartsToMarkdownObjects(newLine.NewLine);

            FindContainerForNewBlock();
            
            switch (block.Kind)
            {
                case WikiBlockKind.Code:
                case WikiBlockKind.CodeWithArgs:
                case WikiBlockKind.NoFormat:

                    var endBlock = _wikiBlocks[(uint)block.OtherBlockIndex];

                    var markdownCodeBlock = new FencedCodeBlock
                    {
                        InfoNewLine = NewLine.LineFeed,
                        NewLine = NewLine.LineFeed,
                        Lines = new StringLineGroup((int)(endBlock.LineIndex - block.LineIndex) + 1)
                    };

                    if (block.Kind == WikiBlockKind.CodeWithArgs)
                    {
                        // "{code:" => Length = 6
                        var slice = new StringSlice(_text, block.Start + 6, block.End - 1);
                        DecodeInfoAndArgumentsForCode(slice, out var info, out var arguments);
                        markdownCodeBlock.Info = info;
                        markdownCodeBlock.Arguments = arguments;
                    }


                    markdownCodeBlock.FencedChar = '`';
                    markdownCodeBlock.OpeningFencedCharCount = 3;
                    markdownCodeBlock.ClosingFencedCharCount = markdownCodeBlock.OpeningFencedCharCount;

                    // Exception if the block is on the same line
                    if (block.LineIndex == endBlock.LineIndex)
                    {
                        var currentLine = new StringSlice(_text, block.End + 1, block.Start - 1, NewLine.LineFeed);
                        markdownCodeBlock.Lines.Add(currentLine);
                    }
                    else
                    {
                        // Add first line if necessary
                        var firstLine = _lines[block.LineIndex].ToSlice(_text);
                        firstLine.Start = block.End + 1;
                        if (!firstLine.IsEmptyOrWhitespace())
                        {
                            markdownCodeBlock.Lines.Add(firstLine);
                        }

                        // Add all intermediate lines
                        for (uint lineIndex = block.LineIndex + 1; lineIndex < endBlock.LineIndex; lineIndex++)
                        {
                            var line = _lines[lineIndex];
                            var slice = line.ToSlice(_text);
                            markdownCodeBlock.Lines.Add(slice);
                        }

                        // Add last line if necessary
                        var lastLine = _lines[endBlock.LineIndex].ToSlice(_text);
                        lastLine.End = endBlock.Start - 1;
                        if (!lastLine.IsEmptyOrWhitespace())
                        {
                            markdownCodeBlock.Lines.Add(lastLine);
                        }
                    }

                    // Push this block
                    PushBlock(markdownCodeBlock, false);

                    _wikiBlockIndex = (uint)block.OtherBlockIndex + 1;
                    _currentLineIndex = endBlock.LineIndex;

                    var afterLine = _lines[endBlock.LineIndex].ToSlice(_text);
                    afterLine.Start = endBlock.End + 1;
                    afterLine.Trim();
                    if (!afterLine.IsEmptyOrWhitespace())
                    {
                        newLine.Start = afterLine.Start - 1;
                        return afterLine.Start;
                    }

                    // We don't have to process anymore
                    break;

                case WikiBlockKind.Quote:

                    if (block.Flags == TextPartFlags.Open)
                    {
                        var quote = new QuoteBlock {QuoteChar = '>'};
                        PushBlock(quote, true);
                    }
                    else
                    {
                        Debug.Assert(block.Flags == TextPartFlags.Close);

                        var quoteBlock = (QuoteBlock) FindContainerForNewBlock();
                        _currentBlock = quoteBlock.Parent;
                    }

                    newLine.Start = block.End;
                    return newLine.Start;

                case WikiBlockKind.Panel:
                case WikiBlockKind.PanelWithArgs:
                    if (block.Flags == TextPartFlags.Open)
                    {
                        var panel = new CustomContainer
                        {
                            FencedChar = ':',
                            OpeningFencedCharCount = 3,
                            ClosingFencedCharCount = 3,
                            Info = "panel"
                        };

                        // Extract arguments
                        if (block.Kind == WikiBlockKind.PanelWithArgs)
                        {
                            // "{panel:" => Length = 7
                            var slice = new StringSlice(_text, block.Start + 7, block.End - 1);
                            var arguments = slice.ToString().Replace('|', ' ');
                            panel.Arguments = arguments;
                        }

                        PushBlock(panel, true);
                    }
                    else
                    {
                        Debug.Assert(block.Flags == TextPartFlags.Close);
                        var panelBlock = (CustomContainer)FindContainerForNewBlock();
                        _currentBlock = panelBlock.Parent;
                    }
                    newLine.Start = block.End;
                    return newLine.Start;

                case WikiBlockKind.Default:
                    var defaultBlock = new ParagraphBlock();
                    var containerInline = new ContainerInline();
                    containerInline.AppendChild(new LiteralInline(block.ToSlice(_text)));
                    containerInline.AppendChild(new LineBreakInline() { NewLine = newLine.NewLine });
                    PushBlock(defaultBlock, false);

                    newLine.Start = block.End;
                    return newLine.Start;
            }

            // We are done processing the line
            return -1;
        }

        private void DecodeInfoAndArgumentsForCode(StringSlice slice, out string info, out string? arguments)
        {
            // slice after : and ends before }
            var infoBuilder = new StringBuilder();
            StringBuilder? argumentsBuilder = null;

            while (!slice.IsEmpty)
            {
                var nextEqual = slice.IndexOf('=');
                var nextPipe = slice.IndexOf('|');
                var hasEqual = nextEqual > 0 && (nextPipe < 0 || nextEqual < nextPipe);
                var hasPipe = nextPipe > 0;
                bool addAsArgument = true;
                if (infoBuilder.Length == 0)
                {
                    if (slice.Match("title="))
                    {
                        var start = slice.Start + 6;
                        var length = (hasPipe ? nextPipe : slice.End + 1) - start;
                        if (length > 0)
                        {
                            var fileName = slice.Text.Substring(start, length);
                            var extension = (Path.GetExtension(fileName) ?? ".txt");
                            infoBuilder.Append(extension, 1, extension.Length - 1);
                        }
                    }
                    else if (!hasPipe)
                    {
                        infoBuilder.Append(slice.Text.Substring(slice.Start, slice.Length));
                        break;
                    }
                    else if (!hasEqual)
                    {
                        infoBuilder.Append(slice.Text.Substring(slice.Start, nextPipe - slice.Start));
                        slice.Start = nextPipe + 1;
                        addAsArgument = false;
                    }
                }

                if (addAsArgument)
                {
                    var length = (hasPipe ? nextPipe : slice.End + 1) - slice.Start;
                    if (length > 0)
                    {
                        argumentsBuilder ??= new StringBuilder();
                        argumentsBuilder.Append(slice.Text.Substring(slice.Start, length));
                        slice.Start = (hasPipe ? nextPipe : slice.End) + 1;
                    }
                }
            }

            info = infoBuilder.Length == 0 ? "txt" : infoBuilder.ToString();
            arguments = argumentsBuilder?.ToString();
        }


        private bool TryFindNextBlock(int start, out uint blockIndex)
        {
            blockIndex = 0;
            for (; _wikiBlockIndex < _wikiBlocks.Count; _wikiBlockIndex++)
            {
                var block = _wikiBlocks[_wikiBlockIndex];
                if (block.Start == start)
                {
                    blockIndex = _wikiBlockIndex;
                    return true;
                }

                if (start > block.Start)
                {
                    break;
                }
            }

            return false;
        }

        private void AddLiteralCharToCurrentTextPart(int index)
        {
            // If we can concat with a previous text entry, we do it
            if (_textParts.Count > 0)
            {
                ref var previous = ref _textParts[_textParts.Count - 1];
                if (previous.EffectKind == TextEffectKind.Default)
                {
                    previous.End = index;
                    return;
                }
            }
            _textParts.Add(new TextPart(index, index, TextEffectKind.Default, TextPartFlags.Default));
        }
        
        private bool ContainsBlock(int start, int end)
        {
            uint blockIndex = _wikiBlockIndex;
            var wikiBlocks = _wikiBlocks;
            for (; blockIndex < wikiBlocks.Count; blockIndex++)
            {
                var blockStart = wikiBlocks[blockIndex].Start;
                if (blockStart >= start && blockStart <= end) return true;

                if (start > blockStart) break;
            }

            return false;

        }

        private static bool IsSupportedLink(ref StringSlice line)
        {
            return line.Match("http://") || line.Match("https://") || line.Match("file://") || line.Match("ftp://") || line.Match("mailto:");
        }

        private static int TryParseLink(StringSlice line, out TextEffectKind kind)
        {
            kind = TextEffectKind.Default;
            line.Start++; // skip [

            // Early check that we have a ]
            var indexOfCloseBracket = line.IndexOf(']');
            if (indexOfCloseBracket < 0) return -1;

            var c = line.CurrentChar;
            if (c == '#')
            {
                kind = TextEffectKind.LinkAnchor;
            }
            else if (c == '^')
            {
                kind = TextEffectKind.LinkAttachment;
            }
            else if (c == '~')
            {
                kind = TextEffectKind.LinkUser;
            }
            else if (IsSupportedLink(ref line))
            {
                kind = TextEffectKind.LinkExternal;
            }
            else
            {
                var barIndex = line.IndexOf('|');
                // Case where [text]|
                if (barIndex < 0 || barIndex > indexOfCloseBracket)
                {
                    return -1;
                }

                line.Start = barIndex + 1;
                if (!IsSupportedLink(ref line))
                {
                    return -1;
                }
                kind = TextEffectKind.LinkExternalWithTitle;
            }

            // Else the link is invalid
            return indexOfCloseBracket;
        }
        
        private static int MatchClosingMonospaced(StringSlice line)
        {
            // Skip {{
            char pc = '\0';
            for (int i = line.Start + 2; i <= line.End; i++)
            {
                var c = line[i];
                if (pc != '\\' && c == '}' && line.PeekCharAbsolute(i + 1) == '}')
                {
                    var ac = line.PeekCharAbsolute(i + 2);

                    // Match only a closing monospace (followed by a space or punctuation, but not with a previous space)
                    var kind = CheckOpenCloseDelimiter(pc, ac);
                    if (kind == TextPartFlags.Close)
                    {
                        return i + 1;
                    }
                }
                pc = c;
            }

            return -1;
        }

        private static TextPartFlags CheckOpenCloseDelimiter(char pc, char nc)
        {
            pc.CheckUnicodeCategory(out bool prevIsWhiteSpace, out bool prevIsPunctuation);
            nc.CheckUnicodeCategory(out bool nextIsWhiteSpace, out bool nextIsPunctuation);

            var kind = TextPartFlags.Default;

            if ((prevIsWhiteSpace || prevIsPunctuation) && !nextIsWhiteSpace) kind |= TextPartFlags.Open;
            if ((nextIsWhiteSpace || nextIsPunctuation) && !prevIsWhiteSpace) kind |= TextPartFlags.Close;
            return kind;
        }

        private enum TextEffectKind
        {
            Default,
            Strong,
            Emphasis,
            Citation,
            Deleted,
            Inserted,
            Superscript,
            Subscript,
            Monospaced,
            Link,
            LinkExternal,
            LinkExternalWithTitle,
            LinkAnchor,
            LinkAttachment,
            LinkUser,
            Attachment,
            Hardline,
            Escape,
            MaybeEscape,
        }

        private struct TextPart
        {
            public TextPart(int start, int end, TextEffectKind effectKind, TextPartFlags flags)
            {
                Start = start;
                End = end;
                EffectKind = effectKind;
                Flags = flags;
                OtherTextPartIndex = -1;
            }


            public int Start;

            public int End;

            public TextEffectKind EffectKind;

            public TextPartFlags Flags;

            public int OtherTextPartIndex;

            public StringSlice ToSlice(string text)
            {
                return new (text, Start, End);
            }

            public override string ToString()
            {
                return $"{nameof(Start)}: {Start}, {nameof(End)}: {End}, {nameof(EffectKind)}: {EffectKind}, {nameof(Flags)}: {Flags}";
            }
        }

        [Flags]
        public enum TextPartFlags
        {
            Default = 0,
            Open = 1 << 0,
            Close = 1 << 1,
        }

        private enum WikiBlockKind
        {
            Default = 0,
            Code = 1,
            CodeWithArgs = -1,
            Panel = 2,
            PanelWithArgs = -2,
            NoFormat = 3,
            Quote = 4,
        }

        private readonly struct EmphasisCount
        {
            public EmphasisCount(char character, int count)
            {
                Character = character;
                Count = count;
            }

            public readonly char Character;
            public readonly int Count;
        }

        private struct WikiBlock
        {
            public WikiBlock(uint lineIndex, int start, int end, WikiBlockKind kind, TextPartFlags flags)
            {
                LineIndex = lineIndex;
                Start = start;
                End = end;
                Kind = kind;
                Flags = flags;
                OtherBlockIndex = -1;
            }

            public readonly uint LineIndex;
            public readonly int Start;
            public readonly int End;
            public WikiBlockKind Kind;
            public TextPartFlags Flags;

            /// <summary>
            /// - If <see cref="Flags"/> is <see cref="TextPartFlags.Open"/>, then this field gives the index to the closing block
            /// - If <see cref="Flags"/> is <see cref="TextPartFlags.Close"/>, then this field gives the index to the opening block
            /// - Otherwise it is == -1
            /// </summary>
            public int OtherBlockIndex;

            public StringSlice ToSlice(string text)
            {
                return new StringSlice(text, Start, End);
            }

            public override string ToString()
            {
                return $"{nameof(LineIndex)}: {LineIndex}, {nameof(Start)}: {Start}, {nameof(End)}: {End}, {nameof(Kind)}: {Kind}";
            }
        }

        private readonly struct Line
        {
            public Line(int start, int end, NewLine newLine)
            {
                Start = start;
                End = end;
                NewLine = newLine;
            }

            public readonly int Start;
            public readonly int End;
            public readonly NewLine NewLine;

            public bool IsEmpty => End == 0;

            public StringSlice ToSlice(string text)
            {
                return new(text, Start, End, NewLine);
            }

            public override string ToString() => $"{nameof(Start)}: {Start} {nameof(End)}: {End} {nameof(NewLine)}: {NewLine}";
        }

        private readonly struct BraceIndex
        {
            public BraceIndex(uint lineIndex, int charIndex)
            {
                LineIndex = lineIndex;
                CharIndex = charIndex;
            }

            public readonly uint LineIndex;

            public readonly int CharIndex;

            public override string ToString() => $"LineIndex: {LineIndex} CharIndex: {CharIndex}";
        }

        /// <summary>
        /// A line reader from a <see cref="TextReader"/> that can provide precise source position
        /// </summary>
        private struct InternalLineReader
        {
            private readonly string _text;

            public InternalLineReader(string text)
            {
                _text = text;
                SourcePosition = 0;
            }

            /// <summary>
            /// Gets the char position of the line. Valid for the next line before calling <see cref="ReadLine"/>.
            /// </summary>
            public int SourcePosition { get; private set; }

            public Line ReadLine(out int firstBraceIndex)
            {
                string text = _text;
                int sourcePosition = SourcePosition;
                firstBraceIndex = -1;

                for (int i = sourcePosition; i < text.Length; i++)
                {
                    char c = text[i];
                    if (c == '\r')
                    {
                        int length = 1;
                        var newLine = NewLine.CarriageReturn;
                        if (c == '\r' && (uint)(i + 1) < (uint)text.Length && text[i + 1] == '\n')
                        {
                            i++;
                            length = 2;
                            newLine = NewLine.CarriageReturnLineFeed;
                        }

                        var slice = new Line(sourcePosition, i - length, newLine);
                        SourcePosition = i + 1;
                        return slice;
                    }

                    if (c == '\n')
                    {
                        var slice = new Line(sourcePosition, i - 1, NewLine.LineFeed);
                        SourcePosition = i + 1;
                        return slice;
                    }

                    if (c == '{' && firstBraceIndex < 0) firstBraceIndex = i;
                }

                if (sourcePosition >= text.Length)
                    return default;

                SourcePosition = int.MaxValue;
                return new Line(sourcePosition, text.Length - 1, NewLine.None);
            }
        }
    }
}
