using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public class MarkdownParser
    {
        private StringLine line;
        private bool isEof;

        private readonly List<BlockParser> blockParsers;
        private readonly List<InlineParser> inlineParsers;
        private readonly List<InlineParser> regularInlineParsers;
        private readonly Dictionary<char, InlineParser> inlineWithFirstCharParsers;
        private readonly Document document;
        private readonly StringBuilder tempBuilder;
        private readonly BlockParserState blockParserState;
        private readonly InlineParserState inlineState;
        private readonly StringBuilderCache stringBuilderCache;

        public MarkdownParser(TextReader reader)
        {
            document = new Document();
            Reader = reader;
            blockParsers = new List<BlockParser>();
            inlineParsers = new List<InlineParser>();
            inlineWithFirstCharParsers = new Dictionary<char, InlineParser>();
            regularInlineParsers = new List<InlineParser>();
            tempBuilder = new StringBuilder();
            stringBuilderCache  = new StringBuilderCache();
            inlineState = new InlineParserState(stringBuilderCache, document);
            blockParserState = new BlockParserState(stringBuilderCache, document);
            blockParsers = new List<BlockParser>()
            {
                BreakBlock.Parser,
                HeadingBlock.Parser,
                QuoteBlock.Parser,
                ListBlock.Parser,

                HtmlBlock.Parser,
                CodeBlock.Parser, 
                FencedCodeBlock.Parser,
                ParagraphBlock.Parser,
            };

            inlineParsers = new List<InlineParser>()
            {
                LinkInline.Parser,
                EmphasisInline.Parser,
                EscapeInline.Parser,
                CodeInline.Parser,
                AutolinkInline.Parser,
                HardlineBreakInline.Parser,
                LiteralInline.Parser,
            };
            InitializeInlineParsers();
        }

        private void InitializeInlineParsers()
        {
            foreach (var inlineParser in inlineParsers)
            {
                if (inlineParser.FirstChars != null && inlineParser.FirstChars.Length > 0)
                {
                    foreach (var firstChar in inlineParser.FirstChars)
                    {
                        inlineWithFirstCharParsers[firstChar] = inlineParser;
                    }
                }
                else
                {
                    regularInlineParsers.Add(inlineParser);
                }
            }
        }

        public TextReader Reader { get; }

        private Block LastBlock
        {
            get
            {
                var count = blockParserState.Count;
                return count > 0 ? blockParserState[count - 1].Block : null;
            }
        }

        private ContainerBlock LastContainer
        {
            get
            {
                for (int i = blockParserState.Count - 1; i >= 0; i--)
                {
                    var container = blockParserState[i].Block as ContainerBlock;
                    if (container != null)
                    {
                        return container;
                    }
                }
                return null;
            }
        }

        public Document Parse()
        {
            ParseLines();
            ProcessInlines(document);
            return document;
        }

        private void ParseLines()
        {
            while (!isEof)
            {
                ReadLine();

                // If this is the end of file and the last line is empty
                if (isEof && line.IsEol)
                {
                    break;
                }

                bool continueProcessLiner = ProcessPendingBlocks();

                // If we have already reached eol and the last block was a paragraph
                // we close it
                if (blockParserState.Count > 0 && line.IsEol)
                {
                    int index = blockParserState.Count - 1;
                    var lastBlock = blockParserState[index];
                    if (lastBlock.Block is ParagraphBlock)
                    {
                        blockParserState.Close(index);
                        continue;
                    }
                }


                // If the line was not entirely processed by pending blocks, try to process it with any new block
                while (continueProcessLiner)
                {
                    continueProcessLiner = ParseNewBlocks(continueProcessLiner);
                }

                // Close blocks that are no longer opened
                blockParserState.CloseAll(false);
            }

            blockParserState.CloseAll(true);
            // Close opened blocks
            //ProcessPendingBlocks(true);
        }

        private void ProcessInlines(Block block)
        {
            // Avoid making a recursive call here
            if (block is ContainerBlock)
            {
                foreach (var child in ((ContainerBlock) block).Children)
                {
                    ProcessInlines(child);
                }
            }
            else
            {
                var leafBlock = (LeafBlock)block;
                if (!leafBlock.NoInline)
                {
                    ProcessInlines(leafBlock);
                }
            }
        }

        private bool ProcessPendingBlocks()
        {
            bool processLiner = true;

            // Set all blocks non opened. 
            // They will be marked as open in the following loop
            for (int i = 1; i < blockParserState.Count; i++)
            {
                blockParserState[i].IsOpen = false;
            }

            // Create the line state that will be used by all parser
            blockParserState.Reset(line);

            // Process any current block potentially opened
            for (int i = 1; i < blockParserState.Count; i++)
            {
                var blockState = blockParserState[i];

                // Else tries to match the Parser with the current line
                var parser = blockState.Parser;
                blockParserState.Pending = blockState.Block;

                // If a parser is used by multiple pending object
                // Only try to use it on the last pending block
                if ((i + 1) < blockParserState.Count && blockParserState[i+1].Parser == parser)
                {
                    continue;
                }

                // If we have a paragraph block, we want to try to match over blocks before trying the Paragraph
                if (blockParserState.Pending is ParagraphBlock)
                {
                    break;
                }

                var saveLiner = line.Save();

                // If we have a discard, we can remove it from the current state
                blockParserState.CurrentContainer = LastContainer;
                blockParserState.LastBlock = LastBlock;
                var result = parser.Match(blockParserState);
                if (result == MatchLineResult.None)
                {
                    // Restore the Line where it was
                    line.Restore(ref saveLiner);
                    break;
                }

                // In case the BlockParser has modified the blockParserState we are iterating on
                if (i >= blockParserState.Count)
                {
                    i = blockParserState.Count - 1;
                }

                // If a parser is adding a block, it must be the last of the list
                if ((i + 1) < blockParserState.Count && blockParserState.NewBlocks.Count > 0)
                {
                    throw new InvalidOperationException("A pending parser cannot add a new block when it is not the last pending block");
                }

                // A block is open only if it has a Continue state.
                // otherwise it is a Last state, and we don't keep it opened
                blockState.IsOpen = result == MatchLineResult.Continue;

                // If it is a leaf content, we need to grab the content
                var leaf = blockParserState.Pending as LeafBlock;
                if (leaf != null)
                {
                    // If the match doesn't require to add this line to the Inline content, we can discard it
                    if (result != MatchLineResult.LastDiscard)
                    {
                        leaf.Append(line);
                    }
                    processLiner = false;
                    break;
                }

                if (result == MatchLineResult.LastDiscard)
                {
                    processLiner = false;
                    break;
                }

                bool isLast = i == blockParserState.Count - 1;
                processLiner = ProcessNewBlocks(processLiner, parser, result, ref leaf, false);
                if (isLast || !processLiner)
                {
                    break;
                }
            }

            return processLiner;
        }

        private bool ParseNewBlocks(bool continueProcessLiner)
        {
            blockParserState.Reset(line);

            for (int j = 0; j < blockParsers.Count; j++)
            {
                var blockParser = blockParsers[j];
                if (line.IsEol)
                {
                    continueProcessLiner = false;
                    break;
                }

                // If a block parser cannot interrupt a paragraph, and the last block is a paragraph
                // we can skip this parser
                var previousParagraph = LastBlock as ParagraphBlock;
                if (previousParagraph != null && !blockParser.CanInterruptParagraph)
                {
                    continue;
                }

                bool isParsingParagraph = blockParser == ParagraphBlock.Parser;
                blockParserState.Pending = isParsingParagraph ? previousParagraph : null;
                blockParserState.CurrentContainer = LastContainer;
                blockParserState.LastBlock = LastBlock;

                var saveLiner = line.Save();
                var result = blockParser.Match(blockParserState);
                if (result == MatchLineResult.None)
                {
                    // If we have reached a blank line after trying to parse a paragraph
                    // we can ignore it
                    if (isParsingParagraph && line.IsBlankLine())
                    {
                        continueProcessLiner = false;
                        break;
                    }

                    line.Restore(ref saveLiner);
                    continue;
                }

                // Special case for paragraph
                if (blockParserState.Pending == previousParagraph && previousParagraph != null)
                {
                    Debug.Assert(blockParserState.NewBlocks.Count == 0);

                    continueProcessLiner = false;
                    previousParagraph.Append(line);

                    // We have just found a lazy continuation for a paragraph, early exit
                    // Mark all block opened after a lazy continuation
                    for (int i = 0; i < blockParserState.Count; i++)
                    {
                        blockParserState[i].IsOpen = true;
                    }
                    break;
                }

                // Nothing found but the BlockParser may instruct to break, so early exit
                if (blockParserState.NewBlocks.Count == 0 && result == MatchLineResult.LastDiscard)
                {
                    continueProcessLiner = false;
                    break;
                }

                LeafBlock leaf = null;
                continueProcessLiner = ProcessNewBlocks(continueProcessLiner, blockParser, result, ref leaf, true);

                // If we have a container, we can retry to match against all types of block.
                if (leaf == null)
                {
                    // rewind to the first parser
                    j = -1;
                }
                else
                {
                    // We have a leaf node, we can stop
                    break;
                }
            }

            return continueProcessLiner;
        }

        private bool ProcessNewBlocks(bool continueProcessLiner, BlockParser blockParser, MatchLineResult result, ref LeafBlock leaf, bool allowClosing)
        {
            while (continueProcessLiner && blockParserState.NewBlocks.Count > 0)
            {
                var block = blockParserState.NewBlocks.Pop();

                // If we have a leaf block
                leaf = block as LeafBlock;
                if (leaf != null)
                {
                    continueProcessLiner = false;
                    leaf.Append(line);

                    if (blockParserState.NewBlocks.Count > 0)
                    {
                        throw new InvalidOperationException(
                            "The NewBlocks is not empty. This is happening if a LeafBlock is not the last to be pushed");
                    }
                }

                if (allowClosing)
                {
                    // Close any previous blocks not opened
                    blockParserState.CloseAll(false);
                }

                // If previous block is a container, add the new block as a children of the previous block
                var container = LastContainer;
                if (container != null)
                {
                    AddToParent(block, container);
                }

                // Add a block blockParserState to the stack (and leave it opened)
                blockParserState.Open(blockParser, block, result == MatchLineResult.Continue);
            }
            return continueProcessLiner;
        }


        private void AddToParent(Block block, ContainerBlock parent)
        {
            if (block == null)
            {
                return;
            }

            while (block.Parent != null)
            {
                block = block.Parent;
            }

            if (!(block is Document))
            {
                parent.Children.Add(block);
                block.Parent = parent;
            }
        }

        private void ReadLine()
        {
            tempBuilder.Clear();
            while (true)
            {
                var nextChar = Reader.Read();
                if (nextChar < 0)
                {
                    isEof = true;
                    break;
                }
                var c = (char) nextChar;

                // 2.3 Insecure characters
                c = c.EscapeInsecure();

                // Go to next char, expecting most likely a \n, otherwise skip it
                // TODO: Should we treat it as an error in no \n is following?
                if (c == '\r')
                {
                    continue;
                }

                if (c == '\n')
                {
                    break;
                }

                tempBuilder.Append(c);
            }
            line = new StringLine(tempBuilder.ToString());
        }

        private void ProcessInlines(LeafBlock leafBlock)
        {
            var lines = leafBlock.Lines;

            leafBlock.Inline = new ContainerInline() {IsClosed = false};
            inlineState.Lines = lines;
            inlineState.Inline = leafBlock.Inline;
            inlineState.Block = leafBlock;

            while (!lines.IsEndOfLines)
            {
                var saveLines = lines.Save();

                var currentChar = lines.CurrentChar;

                InlineParser inlineParser = null;
                if (!inlineWithFirstCharParsers.TryGetValue(lines.CurrentChar, out inlineParser) || !inlineParser.Match(inlineState))
                {
                    for (int i = 0; i < regularInlineParsers.Count; i++)
                    {
                        lines.Restore(ref saveLines);

                        inlineParser = regularInlineParsers[i];

                        if (inlineParser.Match(inlineState))
                        {
                            break;
                        }

                        inlineParser = null;
                    }

                    if (inlineParser == null)
                    {
                        lines.Restore(ref saveLines);
                    }
                }

                var nextInline = inlineState.Inline;

                if (nextInline != null)
                {
                    if (nextInline.Parent == null)
                    {
                        // Get deepest container
                        var container = (ContainerInline)leafBlock.Inline;
                        while (true)
                        {
                            var nextContainer = container.LastChild as ContainerInline;
                            if (nextContainer != null && !nextContainer.IsClosed)
                            {
                                container = nextContainer;
                            }
                            else
                            {
                                break;
                            }
                        }

                        container.AppendChild(nextInline);
                    }

                    if (!nextInline.IsClosed)
                    {
                        inlineState.OpenedInlines.Add(nextInline);
                    }
                }
                else
                {
                    // Get deepest container
                    var container = (ContainerInline)leafBlock.Inline;
                    while (true)
                    {
                        var nextContainer = container.LastChild as ContainerInline;
                        if (nextContainer != null && !nextContainer.IsClosed)
                        {
                            container = nextContainer;
                        }
                        else
                        {
                            break;
                        }
                    }

                    inlineState.Inline = container.LastChild is LeafInline ? container.LastChild : container;
                }

                Log.WriteLine($"** Dump: char '{currentChar}");
                leafBlock.Inline.DumpTo(Log);
            }

            // Close all inlines not closed
            inlineState.Inline = null;
            foreach (var inline in inlineState.OpenedInlines)
            {
                inline.CloseInternal(inlineState);
            }
            inlineState.OpenedInlines.Clear();

            if (Log != null)
            {
                Log.WriteLine("** Dump before Emphasis:");
                leafBlock.Inline.DumpTo(Log);
                EmphasisInline.ProcessEmphasis(leafBlock.Inline);

                Log.WriteLine();
                Log.WriteLine("** Dump after Emphasis:");
                leafBlock.Inline.DumpTo(Log);
            }
            // TODO: Close opened inlines

            // Close last inline
            //while (inlineStack.Count > 0)
            //{
            //    var inlineState = inlineStack.Pop();
            //    inlineState.Parser.Close(state, inlineState.Inline);
            //}
        }

        public static TextWriter Log;

        private void CloseInline(InlineParserState state, Inline inline)
        {
            state.OpenedInlines.Remove(inline);
            inline.CloseInternal(inlineState);
        }

        private class InlineState
        {
            public InlineState(InlineParser parser, Inline inline)
            {
                Parser = parser;
                Inline = inline;
            }

            public InlineParser Parser;

            public Inline Inline;
        }
    }
}