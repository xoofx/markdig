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

        private readonly List<BlockParser> blockParsers;
        private readonly List<BlockState> blockStack;
        private readonly Document document;
        private readonly Stack<BlockState> cachedBlockStates;

        public MarkdownParser(TextReader reader)
        {
            document = new Document();
            Reader = reader;
            blockParsers = new List<BlockParser>();
            blockStack = new List<BlockState>();
            cachedBlockStates = new Stack<BlockState>();
            blockParsers = new List<BlockParser>()
            {
                QuoteBlock.Parser,
                ListBlock.Parser,

                HeadingBlock.Parser,
                BreakBlock.Parser,
                CodeBlock.Parser, 
                FencedCodeBlock.Parser,
                ParagraphBlock.Parser,
            };

            blockStack.Add(new BlockState() { Block = document});
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

                // Close blocks that are no longer opened
                CloseBlocks();
            }

            // Close opened blocks

            //ProcessPendingBlocks(true);
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
            var lineState = new MatchLineState {Liner = liner};

            // Process any current block potentially opened
            for (int i = 1; i < blockStack.Count; i++)
            {
                var blockState = blockStack[i];

                // Else tries to match the Parser with the current line
                var parser = blockState.Parser;
                lineState.Block = blockState.Block;
                if (lineState.Block is ParagraphBlock)
                {
                    break;
                }

                var saveLiner = liner.Save();

                // If we have a discard, we can remove it from the current state
                lineState.LastBlock = LastBlock;
                var result = parser.Match(ref lineState);
                if (result == MatchLineResult.None)
                {
                    // Restore the liner where it was
                    liner.Restore(ref saveLiner);
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
                        blockStack.RemoveAt(j);
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
                        leaf.Append(liner);
                    }
                    processLiner = false;
                    break;
                }

                if (result == MatchLineResult.LastDiscard || liner.IsEol)
                {
                    processLiner = false;
                    break;
                }
            }

            return processLiner;
        }

        private bool ParseNewBlocks(bool continueProcessLiner)
        {

            var state = new MatchLineState
            {
                Liner = liner,
            };

            for (int j = 0; j < blockParsers.Count; j++)
            {
                var blockParser = blockParsers[j];
                if (liner.IsEol)
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
                state.Block = isParsingParagraph ? previousParagraph : null;

                var saveLiner = liner.Save();
                var result = blockParser.Match(ref state);
                if (result == MatchLineResult.None)
                {
                    // If we have reached a blank line after trying to parse a paragraph
                    // we can ignore it
                    if (isParsingParagraph && liner.IsBlankLine())
                    {
                        continueProcessLiner = false;
                        break;
                    }

                    liner.Restore(ref saveLiner);
                    continue;
                }

                var block = state.Block;

                // We have a MatchLineResult.Break
                var leaf = block as LeafBlock;
                if (leaf != null)
                {
                    continueProcessLiner = false;
                    leaf.Append(liner);

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
                blockState.IsOpen = true;
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

        private void CloseBlocks()
        {
            // Close any previous blocks not opened
            for (int i = blockStack.Count - 1; i >= 1; i--)
            {
                var blockState = blockStack[i];

                // Stop on the first open block
                if (blockState.IsOpen)
                {
                    break;
                }

                ReleaseBlockState(blockState);
                blockStack.RemoveAt(i);
            }
        }

        private void ReadLine()
        {
            liner = new StringLiner {Text = new StringBuilder()};
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