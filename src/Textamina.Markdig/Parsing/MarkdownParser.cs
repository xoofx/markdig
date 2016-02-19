using System;
using System.Collections.Generic;
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
        private readonly List<BlockState> blockStack;
        private readonly Document document;
        private readonly Stack<BlockState> cachedBlockStates;
        private readonly StringBuilder tempBuilder;
        private readonly MatchLineState lineState;
        private readonly MatchInlineState inlineState;
        private readonly StringBuilderCache stringBuilderCache;

        public MarkdownParser(TextReader reader)
        {
            document = new Document();
            Reader = reader;
            blockParsers = new List<BlockParser>();
            inlineParsers = new List<InlineParser>();
            inlineWithFirstCharParsers = new Dictionary<char, InlineParser>();
            regularInlineParsers = new List<InlineParser>();
            blockStack = new List<BlockState>();
            cachedBlockStates = new Stack<BlockState>();
            tempBuilder = new StringBuilder();
            stringBuilderCache  = new StringBuilderCache();
            inlineState = new MatchInlineState(stringBuilderCache);
            lineState = new MatchLineState(stringBuilderCache, document);
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
                EscapeInline.Parser,
                CodeInline.Parser,
                LiteralInline.Parser,
            };
            InitializeInlineParsers();

            blockStack.Add(new BlockState() { Block = document});
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
                var count = blockStack.Count;
                return count > 0 ? blockStack[count - 1].Block : null;
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

                // If the line was not entirely processed by pending blocks, try to process it with any new block
                while (continueProcessLiner)
                {
                    continueProcessLiner = ParseNewBlocks(continueProcessLiner);
                }

                // Close blocks that are no longer opened
                CloseBlocks();
            }

            CloseBlocks(true);
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
            for (int i = 1; i < blockStack.Count; i++)
            {
                blockStack[i].IsOpen = false;
            }

            // Create the line state that will be used by all parser
            lineState.Reset(line);

            // Process any current block potentially opened
            for (int i = 1; i < blockStack.Count; i++)
            {
                var blockState = blockStack[i];

                // Else tries to match the Parser with the current line
                var parser = blockState.Parser;
                lineState.Block = blockState.Block;

                // If we have a paragraph block, we want to try to match over blocks before trying the Paragraph
                if (lineState.Block is ParagraphBlock)
                {
                    break;
                }

                var saveLiner = line.Save();

                // If we have a discard, we can remove it from the current state
                lineState.LastBlock = LastBlock;
                var result = parser.Match(lineState);
                if (result == MatchLineResult.None)
                {
                    // Restore the Line where it was
                    line.Restore(ref saveLiner);
                    break;
                }

                // The parser could modify the block, so we restore it here.
                var previousBlock = blockState.Block;
                blockState.Block = lineState.Block;

                // If we have a new block, we need to add it to the closest parent
                if (previousBlock != blockState.Block)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        var containerBlock = blockStack[j].Block as ContainerBlock;
                        if (containerBlock != null)
                        {
                            AddToParent(blockState.Block, containerBlock);
                            break;
                        }
                    }

                    // If there are any child blocks but we are the current block by another
                    // we need to close the next blocks
                    for (int j = blockStack.Count - 1; j >= i + 1; j--)
                    {
                        var blockStateToClose = blockStack[j];
                        CloseBlock(blockStateToClose, j);
                    }
                }

                // A block is open only if it has a Continue state.
                // otherwise it is a Last state, and we don't keep it opened
                blockState.IsOpen = result == MatchLineResult.Continue;

                // If it is a leaf content, we need to grab the content
                var leaf = lineState.Block as LeafBlock;
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

                if (result == MatchLineResult.LastDiscard || line.IsEol)
                {
                    processLiner = false;
                    break;
                }
            }

            return processLiner;
        }

        private bool ParseNewBlocks(bool continueProcessLiner)
        {
            lineState.Reset(line);

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
                lineState.Block = isParsingParagraph ? previousParagraph : null;

                var saveLiner = line.Save();
                var result = blockParser.Match(lineState);
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

                var block = lineState.Block;

                // We have a MatchLineResult.Break
                var leaf = block as LeafBlock;
                if (leaf != null)
                {
                    continueProcessLiner = false;
                    leaf.Append(line);

                    // We have just found a lazy continuation for a paragraph, early exit
                    if (leaf == previousParagraph)
                    {
                        // Mark all block opened after a lazy continuation
                        for (int i = 0; i < blockStack.Count; i++)
                        {
                            blockStack[i].IsOpen = true;
                        }
                        break;
                    }
                }

                // Close any previous blocks not opened
                CloseBlocks();

                // If previous block is a container, add the new block as a children of the previous block
                var container = LastBlock as ContainerBlock;
                if (container != null)
                {
                    AddToParent(block, container);
                }

                // Add a block blockStack to the stack (and leave it opened)
                var blockState = NewBlockState(blockParser, block);
                blockState.IsOpen = result == MatchLineResult.Continue;
                blockStack.Add(blockState);

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

        private void AddToParent(Block block, ContainerBlock parent)
        {
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

        private void CloseBlocks(bool force = false)
        {
            // Close any previous blocks not opened
            for (int i = blockStack.Count - 1; i >= 1; i--)
            {
                var blockState = blockStack[i];

                // Stop on the first open block
                if (!force && blockState.IsOpen)
                {
                    break;
                }
                CloseBlock(blockState, i);
            }
        }

        private void CloseBlock(BlockState blockState, int index)
        {
            blockState.Parser.Close(blockState.Block);
            ReleaseBlockState(blockState);
            blockStack.RemoveAt(index);
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
                c = CharHelper.EscapeInsecure(c);

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

        private BlockState NewBlockState(BlockParser parser, Block block)
        {
            var blockState = cachedBlockStates.Count > 0 ? cachedBlockStates.Pop() : new BlockState();
            blockState.Reset();
            blockState.Parser = parser;
            blockState.Block = block;
            return blockState;
        }

        private void ReleaseBlockState(BlockState state)
        {
            cachedBlockStates.Push(state);
        }

        private void ProcessInlines(LeafBlock leafBlock)
        {
            var lines = leafBlock.Lines;

            leafBlock.Inline = new ContainerInline();
            inlineState.Lines = lines;
            inlineState.Inline = leafBlock.Inline;

            var previousInline = leafBlock.Inline;
            while (!lines.IsEndOfLines)
            {
                var saveLines = lines.Save();

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

                if (previousInline != nextInline)
                {
                    if (previousInline is LeafInline)
                    {
                        previousInline.Close(inlineState);
                        previousInline.IsClosed = true;
                        if (nextInline.Parent == null)
                        {
                            previousInline.InsertAfter(nextInline);
                        }
                    }
                    else if (previousInline != null)
                    {
                        var container = (ContainerInline) previousInline;

                        if (container.IsClosed)
                        {
                            container.InsertAfter(nextInline);
                        }
                        else
                        {
                            container.AppendChild(nextInline);
                        }
                    }
                }

                previousInline = nextInline;
            }

            while (previousInline != null)
            {
                previousInline.Close(inlineState);
                previousInline = previousInline.Parent;
            }

            // TODO: Close opened inlines

            // Close last inline
            //while (inlineStack.Count > 0)
            //{
            //    var inlineState = inlineStack.Pop();
            //    inlineState.Parser.Close(state, inlineState.Inline);
            //}
        }

        private class BlockState
        {
            public BlockParser Parser;

            public Block Block;

            public bool IsOpen;

            public void Reset()
            {
                Parser = null;
                Block = null;
                IsOpen = true;
            }
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