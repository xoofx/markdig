


using System;
using System.Collections.Generic;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public class BlockParserState : List<BlockState>
    {
        private readonly Stack<BlockState> cachedBlockStates;

        public BlockParserState(StringBuilderCache stringBuilders, Document root)
        {
            if (stringBuilders == null) throw new ArgumentNullException(nameof(stringBuilders));
            if (root == null) throw new ArgumentNullException(nameof(root));
            cachedBlockStates = new Stack<BlockState>();
            StringBuilders = stringBuilders;
            Root = root;
            NewBlocks = new Stack<Block>();
            Add(new BlockState() {Block = root});
        }

        public StringLine Line;

        public Block Pending { get; set; }

        public readonly Stack<Block> NewBlocks;

        public ContainerBlock CurrentContainer;

        public Block LastBlock;

        public readonly Document Root;

        public StringBuilderCache StringBuilders { get; }

        public BlockState Open(BlockParser parser, Block block, bool isOpen)
        {
            var blockState = cachedBlockStates.Count > 0 ? cachedBlockStates.Pop() : new BlockState();
            blockState.Reset();
            blockState.Parser = parser;
            blockState.Block = block;
            blockState.IsOpen = isOpen;
            Add(blockState);
            return blockState;
        }

        public void Close(Block block)
        {
            // If we close a block, we close all blocks above
            for (int i = Count - 1; i >= 1; i--)
            {
                if (this[i].Block == block)
                {
                    for (int j = Count - 1; j >= i; j--)
                    {
                        Close(j);
                    }
                    break;
                }
            }
        }

        public void Close(int index)
        {
            var blockState = this[index];

            var saveBlock = Pending;

            var previousBlock = blockState.Block;
            Pending = blockState.Block;
            blockState.Parser.Close(this);

            // If the pending object is removed, we need to remove it from the parent container
            if (Pending == null)
            {
                var parent = blockState.Block.Parent as ContainerBlock;
                if (parent != null)
                {
                    parent.Children.Remove(previousBlock);
                }
            }

            blockState.Reset();
            cachedBlockStates.Push(blockState);
            RemoveAt(index);
            Pending = saveBlock;
        }

        public void CloseAll(bool force)
        {
            // Close any previous blocks not opened
            for (int i = Count - 1; i >= 1; i--)
            {
                var blockState = this[i];

                // Stop on the first open block
                if (!force && blockState.IsOpen)
                {
                    break;
                }
                Close(i);
            }
        }

        internal void Reset(StringLine line)
        {
            Line = line;
            Pending = null;
        }
    }
}