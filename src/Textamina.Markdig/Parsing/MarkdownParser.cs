using System.Collections.Generic;
using System.IO;
using System.Text;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public class MarkdownParser
    {
        private StringLiner liner;
        private bool isEof;

        private readonly List<BlockParser> parsers;
        private readonly List<BlockState> blockStack;
        private readonly Document document;
        private readonly Stack<BlockState> cacheStates;

        public MarkdownParser(TextReader reader)
        {
            document = new Document();
            Reader = reader;
            parsers = new List<BlockParser>();
            liner = new StringLiner() {Text = new StringBuilder()};
            blockStack = new List<BlockState>();
            cacheStates = new Stack<BlockState>();
            parsers = new List<BlockParser>()
            {
                BlockQuote.Parser,
                //Break.Parser,
                //CodeBlock.Parser, 
                //FencedCodeBlock.Parser,
                //Heading.Parser,
                Paragraph.Parser,
            };

            blockStack.Add(new BlockState() { Block = document});
        }

        public TextReader Reader { get; }

        public Document Parse()
        {
            ParseLines();
            return document;
        }

        private void ParseLines()
        {
            while (!isEof)
            {
                ReadLine();

                // If this is the end of file and the last line is empty
                if (isEof && liner.IsEol)
                {
                    break;
                }

                bool continueProcessLiner = ProcessPendingBlocks();

                // If the line was not entirely processed by pending blocks, try to process it with any new block
                while (continueProcessLiner)
                {
                    continueProcessLiner = ParseNewBlocks(continueProcessLiner);
                }
            }

            // Close opened blocks

            //ProcessPendingBlocks(true);
        }

        private bool ParseNewBlocks(bool continueProcessLiner)
        {
            var previousParagraph = LastBlock as Paragraph;

            for (int j = 0; j < parsers.Count; j++)
            {
                var builder = parsers[j];
                if (liner.IsEol)
                {
                    continueProcessLiner = false;
                    break;
                }

                bool isParsingParagraph = builder == Paragraph.Parser;
                Block block = isParsingParagraph ? previousParagraph : null;
                var saveLiner = liner;
                var result = builder.Match(ref liner, ref block);
                if (result == MatchLineResult.None)
                {
                    // If we have reached a blank line after trying to parse a paragraph
                    // we can ignore it
                    if (isParsingParagraph && liner.IsBlankLine())
                    {
                        continueProcessLiner = false;
                        break;
                    }

                    liner = saveLiner;
                    continue;
                }

                if (block is BlockLeaf)
                {
                    continueProcessLiner = false;
                }

                // We have a MatchLineResult.Break
                var leaf = block as BlockLeaf;
                if (leaf != null)
                {
                    leaf.Append(liner);

                    // We have just found a lazy continuation for a paragraph, early exit
                    if (leaf == previousParagraph)
                    {
                        break;
                    }
                }

                // Close any previous blocks not opened
                TerminateBlocksNotOpened();

                // If previous block is a container, add the new block as a children of the previous block
                var container = LastBlock as BlockContainer;
                if (container != null)
                {
                    container.Children.Add(block);
                    block.Parent = container;
                }

                // Add a block blockStack to the stack
                var state = NewBlockState(builder, block);
                state.IsOpen = true;
                blockStack.Add(state);

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

            TerminateBlocksNotOpened();

            return continueProcessLiner;
        }

        private void TerminateBlocksNotOpened()
        {
            // Close any previous blocks not opened
            for (int i = blockStack.Count - 1; i >= 1; i--)
            {
                var state = blockStack[i];

                // Stop on the first open block
                if (state.IsOpen)
                {
                    break;
                }

                ReleaseBlockState(state);
                blockStack.RemoveAt(i);
            }
        }

        private Block LastBlock
        {
            get
            {
                var count = blockStack.Count;
                return count > 0 ? blockStack[count - 1].Block : null;
            }
        }

        private bool ProcessPendingBlocks()
        {
            bool processLiner = true;

            // Reset all blockStack to non open
            for (int i = 1; i < blockStack.Count; i++)
            {
                blockStack[i].IsOpen = false;
            }

            // Process any current block potentially opened
            for (int i = 1; i < blockStack.Count; i++)
            {
                var state = blockStack[i];

                // Else tries to match the Parser with the current line
                var block = state.Block;
                var parser = state.Parser;
                var saveLiner = liner;
                // If we have a discard, we can remove it from the current state
                var result = parser.Match(ref liner, ref block);
                if (result == MatchLineResult.None)
                {
                    // Restore the liner where it was
                    liner = saveLiner;
                    break;
                }

                state.IsOpen = true;

                // If it is a leaf content, we need to grab the content
                var leaf = block as BlockLeaf;
                if (leaf != null)
                {
                    leaf.Append(liner);
                    processLiner = false;
                    break;
                }

                if (liner.IsEol)
                {
                    processLiner = false;
                }
            }

            return processLiner;
        }

        private void ReadLine()
        {
            liner.Text = new StringBuilder();
            var sb = liner.Text;
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
                c = Utility.EscapeInsecure(c);

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

                sb.Append(c);
            }

            liner.Initialize();
        }

        private BlockState NewBlockState(BlockParser parser, Block block)
        {
            var state = cacheStates.Count > 0 ? cacheStates.Pop() : new BlockState();
            state.Reset();
            state.Parser = parser;
            state.Block = block;
            return state;
        }

        private void ReleaseBlockState(BlockState state)
        {
            cacheStates.Push(state);
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
    }
}