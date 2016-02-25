


using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public class BlockParserState : List<Block>
    {
        public BlockParserState(StringBuilderCache stringBuilders, Document root)
        {
            if (stringBuilders == null) throw new ArgumentNullException(nameof(stringBuilders));
            if (root == null) throw new ArgumentNullException(nameof(root));
            StringBuilders = stringBuilders;
            Root = root;
            NewBlocks = new Stack<Block>();
            Add(root);
        }


        public StringSlice Line;

        public int LineIndex;

        public bool IsBlankLine => CurrentChar == '\0';

        public bool IsEndOfLine => Line.IsEndOfSlice;

        public char CurrentChar => Line.CurrentChar;

        public char NextChar() => Line.NextChar();

        public char CharAt(int index) => Line[index];

        public int Start => Line.Start;

        public int EndOffset => Line.End;

        public int Indent => Column - ColumnBegin;

        public bool IsCodeIndent => Indent >= 4;

        public int ColumnBegin { get; private set; }

        public int Column { get; set; }

        public Block Pending { get; set; }

        public int PendingIndex { get; internal set; }

        public readonly Stack<Block> NewBlocks;

        public ContainerBlock CurrentContainer;

        public Block LastBlock;

        public readonly Document Root;

        public StringBuilderCache StringBuilders { get; }

        public char PeekChar(int offset)
        {
            return Line.PeekChar(offset);
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public void SetCurrentLine(ref StringSlice line)
        {
            Line = line;
            EatSpaces();
        }

        public void EatSpaces()
        {
            var c = CurrentChar;
            ColumnBegin = Column;
            while (c !='\0')
            {
                if (c == ' ')
                {
                    Column++;
                }
                else if (c == '\t')
                {
                    Column = ((Column + 3) >> 2) << 2;
                }
                else
                {
                    break;
                }
                c = NextChar();
            }
        }

        public Block NextPending
        {
            get { return PendingIndex + 1 < Count ? this[PendingIndex + 1] : null; }
        }

        public void Close(Block block)
        {
            // If we close a block, we close all blocks above
            for (int i = Count - 1; i >= 1; i--)
            {
                if (this[i] == block)
                {
                    for (int j = Count - 1; j >= i; j--)
                    {
                        Close(j);
                    }
                    break;
                }
            }
        }

        public void Discard(Block block)
        {
            for (int i = Count - 1; i >= 1; i--)
            {
                if (this[i] == block)
                {
                    if (Pending == block)
                    {
                        Pending = null;
                    }
                    block.Parent.Children.Remove(block);
                    RemoveAt(i);
                    break;
                }
            }
        }

        public void Close(int index)
        {
            var block = this[index];

            var saveBlock = Pending;

            Pending = block;
            block.Parser.Close(this);

            // If the pending object is removed, we need to remove it from the parent container
            if (Pending == null)
            {
                var parent = block.Parent as ContainerBlock;
                if (parent != null)
                {
                    parent.Children.Remove(block);
                }
            }

            RemoveAt(index);
            Pending = saveBlock;
        }

        public void CloseAll(bool force)
        {
            // Close any previous blocks not opened
            for (int i = Count - 1; i >= 1; i--)
            {
                var block = this[i];

                // Stop on the first open block
                if (!force && block.IsOpen)
                {
                    break;
                }
                Close(i);
            }
        }
    }
}