// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Parsers;

/// <summary>
/// Block parser for a <see cref="HtmlBlock"/>.
/// </summary>
/// <seealso cref="BlockParser" />
public class HtmlBlockParser : BlockParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlBlockParser"/> class.
    /// </summary>
    public HtmlBlockParser()
    {
        OpeningCharacters = ['<'];
    }
       
    public override BlockState TryOpen(BlockProcessor processor)
    {
        var result = MatchStart(processor);

        // An end-tag can occur on the same line, so we try to parse it here
        if (result == BlockState.Continue)
        {
            result = MatchEnd(processor, (HtmlBlock) processor.NewBlocks.Peek());
        }
        return result;
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
        var htmlBlock = (HtmlBlock) block;
        return MatchEnd(processor, htmlBlock);
    }

    private BlockState MatchStart(BlockProcessor state)
    {
        if (state.IsCodeIndent)
        {
            return BlockState.None;
        }

        var line = state.Line;
        var startPosition = line.Start;
        line.SkipChar();
        var result = TryParseTagType16(state, line, state.ColumnBeforeIndent, startPosition);

        // HTML blocks of type 7 cannot interrupt a paragraph:
        if (result == BlockState.None && !(state.CurrentBlock is ParagraphBlock))
        {
            result = TryParseTagType7(state, line, state.ColumnBeforeIndent, startPosition);
        }
        return result;
    }

    private BlockState TryParseTagType7(BlockProcessor state, StringSlice line, int startColumn, int startPosition)
    {
        var builder = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);
        var c = line.CurrentChar;
        var result = BlockState.None;
        if ((c == '/' && HtmlHelper.TryParseHtmlCloseTag(ref line, ref builder)) || HtmlHelper.TryParseHtmlTagOpenTag(ref line, ref builder))
        {
            // Must be followed by whitespace only
            bool hasOnlySpaces = true;
            c = line.CurrentChar;
            while (true)
            {
                if (c == '\0')
                {
                    break;
                }
                if (!c.IsWhitespace())
                {
                    hasOnlySpaces = false;
                    break;
                }
                c = line.NextChar();
            }

            if (hasOnlySpaces)
            {
                result = CreateHtmlBlock(state, HtmlBlockType.NonInterruptingBlock, startColumn, startPosition);
            }
        }

        builder.Dispose();
        return result;
    }

    private BlockState TryParseTagType16(BlockProcessor state, StringSlice line, int startColumn, int startPosition)
    {
        char c;
        c = line.CurrentChar;
        if (c == '!')
        {
            c = line.NextChar();
            if (c == '-' && line.PeekChar() == '-')
            {
                return CreateHtmlBlock(state, HtmlBlockType.Comment, startColumn, startPosition); // group 2
            }
            if (c.IsAlphaUpper())
            {
                return CreateHtmlBlock(state, HtmlBlockType.DocumentType, startColumn, startPosition); // group 4
            }
            if (c == '[' && line.Match("CDATA[", 1))
            {
                return CreateHtmlBlock(state, HtmlBlockType.CData, startColumn, startPosition); // group 5
            }

            return BlockState.None;
        }

        if (c == '?')
        {
            return CreateHtmlBlock(state, HtmlBlockType.ProcessingInstruction, startColumn, startPosition); // group 3
        }

        var hasLeadingClose = c == '/';
        if (hasLeadingClose)
        {
            c = line.NextChar();
        }

        Span<char> tag = stackalloc char[10];
        var count = 0;
        for (; count < tag.Length; count++)
        {
            if (!c.IsAlphaNumeric())
            {
                break;
            }
            tag[count] = char.ToLowerInvariant(c);
            c = line.NextChar();
        }

        if (!(c == '>' || (!hasLeadingClose && c == '/' && line.PeekChar() == '>') || c.IsWhiteSpaceOrZero()))
        {
            return BlockState.None;
        }

        if (count == 0)
        {
            return BlockState.None;
        }

        if (!HtmlTags.TryMatchExact(tag.Slice(0, count), out var match))
        {
            return BlockState.None;
        }

        int tagIndex = match.Value;

        // Cannot start with </script </pre or </style or </textArea
        if ((tagIndex == 49 || tagIndex == 50 || tagIndex == 53 || tagIndex == 56))
        {
            if (c == '/' || hasLeadingClose)
            {
                return BlockState.None;
            }
            return CreateHtmlBlock(state, HtmlBlockType.ScriptPreOrStyle, startColumn, startPosition);
        }

        return CreateHtmlBlock(state, HtmlBlockType.InterruptingBlock, startColumn, startPosition);
    }

    private const string EndOfComment = "-->";
    private const string EndOfCDATA = "]]>";
    private const string EndOfProcessingInstruction = "?>";

    private BlockState MatchEnd(BlockProcessor state, HtmlBlock htmlBlock)
    {
        state.GoToColumn(state.ColumnBeforeIndent);

        // Early exit if it is not starting by an HTML tag
        var line = state.Line;
        var result = BlockState.Continue;
        int index;
        switch (htmlBlock.Type)
        {
            case HtmlBlockType.Comment:
                index = line.IndexOf(EndOfComment);
                if (index >= 0)
                {
                    htmlBlock.UpdateSpanEnd(index + EndOfComment.Length);
                    result = BlockState.Break;
                }
                break;
            case HtmlBlockType.CData:
                index = line.IndexOf(EndOfCDATA);
                if (index >= 0)
                {
                    htmlBlock.UpdateSpanEnd(index + EndOfCDATA.Length);
                    result = BlockState.Break;
                }
                break;
            case HtmlBlockType.ProcessingInstruction:
                index = line.IndexOf(EndOfProcessingInstruction);
                if (index >= 0)
                {
                    htmlBlock.UpdateSpanEnd(index + EndOfProcessingInstruction.Length);
                    result = BlockState.Break;
                }
                break;
            case HtmlBlockType.DocumentType:
                index = line.IndexOf('>');
                if (index >= 0)
                {
                    htmlBlock.UpdateSpanEnd(index + 1);
                    result = BlockState.Break;
                }
                break;
            case HtmlBlockType.ScriptPreOrStyle:
                index = line.IndexOf("</script>", 0, true);
                if (index >= 0)
                {
                    htmlBlock.UpdateSpanEnd(index + "</script>".Length);
                    result = BlockState.Break;
                }
                else
                {
                    index = line.IndexOf("</pre>", 0, true);
                    if (index >= 0)
                    {
                        htmlBlock.UpdateSpanEnd(index + "</pre>".Length);
                        result = BlockState.Break;
                    }
                    else
                    {
                        index = line.IndexOf("</style>", 0, true);
                        if (index >= 0)
                        {
                            htmlBlock.UpdateSpanEnd(index + "</style>".Length);
                            result = BlockState.Break;
                        }
                        else
                        {
                            index = line.IndexOf("</textarea>", 0, true);
                            if (index >= 0)
                            {
                                htmlBlock.UpdateSpanEnd(index + "</textarea>".Length);
                                result = BlockState.Break;
                            }
                        }
                    }
                }
                break;
            case HtmlBlockType.InterruptingBlock:
                if (state.IsBlankLine)
                {
                    result = BlockState.BreakDiscard;
                }
                break;
            case HtmlBlockType.NonInterruptingBlock:
                if (state.IsBlankLine)
                {
                    result = BlockState.BreakDiscard;
                }
                break;
        }

        // Update only if we don't have a break discard
        if (result != BlockState.BreakDiscard)
        {
            htmlBlock.Span.End = line.End;
            htmlBlock.NewLine = state.Line.NewLine;
        }

        return result;
    }

    private BlockState CreateHtmlBlock(BlockProcessor state, HtmlBlockType type, int startColumn, int startPosition)
    {
        var htmlBlock = new HtmlBlock(this)
        {
            Column = startColumn,
            Type = type,
            // By default, setup to the end of line
            Span = new SourceSpan(startPosition, startPosition + state.Line.End),
            //BeforeWhitespace = state.PopBeforeWhitespace(startPosition - 1),
        };

        if (state.TrackTrivia)
        {
            htmlBlock.LinesBefore = state.UseLinesBefore();
            htmlBlock.NewLine = state.Line.NewLine;
        }

        state.NewBlocks.Push(htmlBlock);
        return BlockState.Continue;
    }

    private static readonly CompactPrefixTree<int> HtmlTags = new(67, 96, 86)
    {
        { "address", 0 },
        { "article", 1 },
        { "aside", 2 },
        { "base", 3 },
        { "basefont", 4 },
        { "blockquote", 5 },
        { "body", 6 },
        { "caption", 7 },
        { "center", 8 },
        { "col", 9 },
        { "colgroup", 10 },
        { "dd", 11 },
        { "details", 12 },
        { "dialog", 13 },
        { "dir", 14 },
        { "div", 15 },
        { "dl", 16 },
        { "dt", 17 },
        { "fieldset", 18 },
        { "figcaption", 19 },
        { "figure", 20 },
        { "footer", 21 },
        { "form", 22 },
        { "frame", 23 },
        { "frameset", 24 },
        { "h1", 25 },
        { "h2", 26 },
        { "h3", 27 },
        { "h4", 28 },
        { "h5", 29 },
        { "h6", 30 },
        { "head", 31 },
        { "header", 32 },
        { "hr", 33 },
        { "html", 34 },
        { "iframe", 35 },
        { "legend", 36 },
        { "li", 37 },
        { "link", 38 },
        { "main", 39 },
        { "menu", 40 },
        { "menuitem", 41 },
        { "nav", 42 },
        { "noframes", 43 },
        { "ol", 44 },
        { "optgroup", 45 },
        { "option", 46 },
        { "p", 47 },
        { "param", 48 },
        { "pre", 49 },      // <=== special group 1
        { "script", 50 },   // <=== special group 1
        { "section", 51 },
        { "source", 52 },
        { "style", 53 },    // <=== special group 1
        { "summary", 54 },
        { "table", 55 },
        { "textarea", 56 }, // <=== special group 1
        { "tbody", 57 },
        { "td", 58 },
        { "tfoot", 59 },
        { "th", 60 },
        { "thead", 61 },
        { "title", 62 },
        { "tr", 63 },
        { "track", 64 },
        { "ul", 65 },
        { "search", 66 },
    };
}