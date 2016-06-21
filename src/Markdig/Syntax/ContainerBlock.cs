// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// A base class for container blocks.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.Block" />
    [DebuggerDisplay("{GetType().Name} Count = {Count}")]
    public abstract class ContainerBlock : Block, IList<Block>
    {
        private Block[] children;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        protected ContainerBlock(BlockParser parser) : base(parser)
        {
            children = ArrayHelper<Block>.Empty;
        }

        /// <summary>
        /// Gets the last child.
        /// </summary>
        public Block LastChild => Count > 0 ? children[Count - 1] : null;

        /// <summary>
        /// Specialize enumerator.
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<Block> IEnumerable<Block>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Block item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.Parent != null)
            {
                throw new ArgumentException("Cannot add this block as it as already attached to another container (block.Parent != null)");
            }

            if (Count == children.Length)
            {
                EnsureCapacity(Count + 1);
            }
            children[Count++] = item;
            item.Parent = this;

            UpdateSpanEnd(item.Span.End);
        }

        private void EnsureCapacity(int min)
        {
            if (children.Length < min)
            {
                int num = (children.Length == 0) ? 4 : (children.Length * 2);
                if (num < min)
                {
                    num = min;
                }

                var destinationArray = new Block[num];
                if (Count > 0)
                {
                    Array.Copy(children, 0, destinationArray, 0, Count);
                }
                children = destinationArray;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                children[i].Parent = null;
                children[i] = null;
            }
        }

        public bool Contains(Block item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            for (int i = 0; i < Count; i++)
            {
                if (children[i] == item)
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(Block[] array, int arrayIndex)
        {
            Array.Copy(children, 0, array, arrayIndex, Count);
        }

        public bool Remove(Block item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            for (int i = Count - 1; i >= 0; i--)
            {
                if (children[i] == item)
                {
                    RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public int Count { get; private set; }

        public bool IsReadOnly => false;

        public int IndexOf(Block item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            for (int i = 0; i < Count; i++)
            {
                if (children[i] == item)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, Block item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.Parent != null)
            {
                throw new ArgumentException("Cannot add this block as it as already attached to another container (block.Parent != null)");
            }
            if (Count == children.Length)
            {
                EnsureCapacity(Count + 1);
            }
            if (index < Count)
            {
                Array.Copy(children, index, children, index + 1, Count - index);
            }
            children[index] = item;
            Count++;
            item.Parent = this;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
            Count--;
            // previous children
            var item = children[index];
            item.Parent = null;
            if (index < Count)
            {
                Array.Copy(children, index + 1, children, index, Count - index);
            }
            children[Count] = null;
        }

        public Block this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
                return children[index];
            }
            set
            {
                if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
                children[index] = value;
            }
        }

        public void Sort(IComparer<Block> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            Array.Sort(children, 0, Count, comparer);
        }

        public void Sort(Comparison<Block> comparison)
        {
            if (comparison == null) throw new ArgumentNullException(nameof(comparison));
            Array.Sort(children, 0, Count, new BlockComparer(comparison));
        }

        #region Nested type: Enumerator

        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<Block>
        {
            private readonly ContainerBlock block;
            private int index;
            private Block current;

            internal Enumerator(ContainerBlock block)
            {
                this.block = block;
                index = 0;
                current = null;
            }

            public Block Current => current;

            object IEnumerator.Current => Current;


            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (index < block.Count)
                {
                    current = block[index];
                    index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                index = block.Count + 1;
                current = null;
                return false;
            }

            void IEnumerator.Reset()
            {
                index = 0;
                current = null;
            }
        }

        #endregion

        private sealed class BlockComparer : IComparer<Block>
        {
            private readonly Comparison<Block> comparison;

            public BlockComparer(Comparison<Block> comparison)
            {
                this.comparison = comparison;
            }

            public int Compare(Block x, Block y)
            {
                return comparison(x, y);
            }
        }
    }
}