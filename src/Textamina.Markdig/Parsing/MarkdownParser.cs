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

        private readonly List<BlockBuilder> builders;
        private readonly List<BlockState> states;
        private readonly Document document;
        private readonly Stack<BlockState> cacheStates;

        public MarkdownParser(TextReader reader)
        {
            document = new Document();
            Reader = reader;
            builders = new List<BlockBuilder>();
            liner = new StringLiner() {Text = new StringBuilder()};
            states = new List<BlockState>();
            cacheStates = new Stack<BlockState>();
            builders = new List<BlockBuilder>()
            {
                BlockQuote.Builder,
                //Break.Builder,
                //CodeBlock.Builder, 
                //FencedCodeBlock.Builder,
                //Heading.Builder,
                Paragraph.Builder,
            };

            states.Add(new BlockState() { Block = document});
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
                    continueProcessLiner = ProcessBlockBuilders(continueProcessLiner);
                }
            }

            // Close opened blocks

            //ProcessPendingBlocks(true);
        }

        private bool ProcessBlockBuilders(bool continueProcessLiner)
        {
            var lastLeafBlock = LastBlock as BlockLeaf;

            for (int j = 0; j < builders.Count; j++)
            {
                var builder = builders[j];
                if (liner.IsEol)
                {
                    continueProcessLiner = false;
                    break;
                }

                Block block = builder == Paragraph.Builder ? lastLeafBlock : null;
                var saveLiner = liner;
                if (!builder.Match(ref liner, ref block))
                {
                    liner = saveLiner;
                    continue;
                }

                if (block is BlockLeaf)
                {
                    continueProcessLiner = false;
                }

                // We have a MatchLineState.Break
                var leaf = block as BlockLeaf;
                if (leaf != null)
                {
                    leaf.Append(liner);
                }

                // We have just found a lazy continuation for a paragraph, early exit
                if (leaf != null && leaf == lastLeafBlock)
                {
                    break;
                }

                // If previous blocks were not matched, close all non matched blocks
                for (int i = states.Count - 1; i >= 1; i--)
                {
                    var state = states[i];
                    if (state.IsOpen)
                    {
                        break;
                    }
                    else
                    {
                        ReleaseBlockState(state);
                        states.RemoveAt(i);
                    }
                }

                // If previous block is a container, add the new block as a children of the previous block
                var container = LastBlock as BlockContainer;
                if (container != null)
                {
                    container.Children.Add(block);
                    block.Parent = container;
                }

                var blockState = NewBlockState(builder, block);
                blockState.IsOpen = true;
                states.Add(blockState);

                // If we have a container, we can try to match again with all type of blocks.
                if (leaf == null)
                {
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

        private Block LastBlock
        {
            get
            {
                var count = states.Count;
                return count > 0 ? states[count - 1].Block : null;
            }
        }

        private bool ProcessPendingBlocks()
        {
            bool processLiner = true;

            // Reset all states to non open
            for (int i = 1; i < states.Count; i++)
            {
                states[i].IsOpen = false;
            }

            // Process any current block potentially opened
            for (int i = 1; i < states.Count; i++)
            {
                var state = states[i];

                // Else tries to match the builder with the current line
                var block = state.Block;
                var builder = state.Builder;
                var saveLiner = liner;
                // If we have a discard, we can remove it from the current state
                if (!builder.Match(ref liner, ref block))
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

        private BlockState NewBlockState(BlockBuilder builder, Block block)
        {
            var state = cacheStates.Count > 0 ? cacheStates.Pop() : new BlockState();
            state.Reset();
            state.Builder = builder;
            state.Block = block;
            return state;
        }

        private void ReleaseBlockState(BlockState state)
        {
            cacheStates.Push(state);
        }

        private class BlockState
        {
            public BlockBuilder Builder;

            public Block Block;

            public bool IsOpen;

            public void Reset()
            {
                Builder = null;
                Block = null;
                IsOpen = true;
            }
        }
    }
}