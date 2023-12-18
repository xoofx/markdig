// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Syntax;

/// <summary>
/// A base class for container blocks.
/// </summary>
/// <seealso cref="Block" />
[DebuggerDisplay("{GetType().Name} Count = {Count}")]
public abstract class ContainerBlock : Block, IList<Block>, IReadOnlyList<Block>
{
    private BlockWrapper[] _children;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBlock"/> class.
    /// </summary>
    /// <param name="parser">The parser used to create this block.</param>
    protected ContainerBlock(BlockParser? parser) : base(parser)
    {
        _children = [];
        SetTypeKind(isInline: false, isContainer: true);
    }

    /// <summary>
    /// Gets the last child.
    /// </summary>
    public Block? LastChild
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            BlockWrapper[] children = _children;
            int index = Count - 1;
            if ((uint)index < (uint)children.Length)
            {
                return children[index].Block;
            }
            else
            {
                return null;
            }
        }
    }

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
        if (item is null)
            ThrowHelper.ArgumentNullException_item();

        if (item.Parent != null)
        {
            ThrowHelper.ArgumentException("Cannot add this block as it as already attached to another container (block.Parent != null)");
        }

        if (Count == _children.Length)
        {
            Grow();
        }
        _children[Count] = new BlockWrapper(item);
        Count++;
        item.Parent = this;

        UpdateSpanEnd(item.Span.End);
    }

    private void Grow()
    {
        if (_children.Length == 0)
        {
            _children = new BlockWrapper[4];
        }
        else
        {
            Debug.Assert(_children[_children.Length - 1].Block is not null);

            var newArray = new BlockWrapper[_children.Length * 2];
            Array.Copy(_children, 0, newArray, 0, Count);
            _children = newArray;
        }
    }

    public void Clear()
    {
        BlockWrapper[] children = _children;
        for (int i = 0; i < Count && i < children.Length; i++)
        {
            children[i].Block.Parent = null;
            children[i] = default;
        }
        Count = 0;
    }

    public bool Contains(Block item)
    {
        return IndexOf(item) >= 0;
    }

    public void CopyTo(Block[] array, int arrayIndex)
    {
        BlockWrapper[] children = _children;
        for (int i = 0; i < Count && i < children.Length; i++)
        {
            array[arrayIndex + i] = children[i].Block;
        }
    }

    public bool Remove(Block item)
    {
        int index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false;
    }

    public int Count { get; private set; }

    public bool IsReadOnly => false;

    public int IndexOf(Block item)
    {
        if (item is null)
            ThrowHelper.ArgumentNullException_item();

        BlockWrapper[] children = _children;
        for (int i = 0; i < Count && i < children.Length; i++)
        {
            if (ReferenceEquals(children[i].Block, item))
            {
                return i;
            }
        }
        return -1;
    }

    public void Insert(int index, Block item)
    {
        if (item is null)
            ThrowHelper.ArgumentNullException_item();

        if (item.Parent != null)
        {
            ThrowHelper.ArgumentException("Cannot add this block as it as already attached to another container (block.Parent != null)");
        }
        if ((uint)index > (uint)Count)
        {
            ThrowHelper.ArgumentOutOfRangeException_index();
        }
        if (Count == _children.Length)
        {
            Grow();
        }
        if (index < Count)
        {
            Array.Copy(_children, index, _children, index + 1, Count - index);
        }
        _children[index] = new BlockWrapper(item);
        Count++;
        item.Parent = this;
    }

    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)Count)
            ThrowHelper.ArgumentOutOfRangeException_index();

        Count--;
        // previous children
        var item = _children[index].Block;
        item.Parent = null;
        if (index < Count)
        {
            Array.Copy(_children, index + 1, _children, index, Count - index);
        }
        _children[Count] = default;
    }

    public Block this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var array = _children;
            if ((uint)index >= (uint)array.Length || index >= Count)
            {
                ThrowHelper.ThrowIndexOutOfRangeException();
                return null;
            }
            return array[index].Block;
        }
        set
        {
            if ((uint)index >= (uint)Count) ThrowHelper.ThrowIndexOutOfRangeException();

            if (value is null)
                ThrowHelper.ArgumentNullException_item();

            if (value.Parent != null)
                ThrowHelper.ArgumentException("Cannot add this block as it as already attached to another container (block.Parent != null)");

            var existingChild = _children[index].Block;
            if (existingChild != null)
                existingChild.Parent = null;

            value.Parent = this;
            _children[index] = new BlockWrapper(value);
        }
    }

    public void Sort(IComparer<Block> comparer)
    {
        if (comparer is null) ThrowHelper.ArgumentNullException(nameof(comparer));
        Array.Sort(_children, 0, Count, new BlockComparerWrapper(comparer));
    }

    public void Sort(Comparison<Block> comparison)
    {
        if (comparison is null) ThrowHelper.ArgumentNullException(nameof(comparison));
        Array.Sort(_children, 0, Count, new BlockComparisonWrapper(comparison));
    }

    #region Nested type: Enumerator

    [StructLayout(LayoutKind.Sequential)]
    public struct Enumerator : IEnumerator<Block>
    {
        private readonly ContainerBlock block;
        private int index;
        private Block? current;

        internal Enumerator(ContainerBlock block)
        {
            this.block = block;
            index = 0;
            current = null;
        }

        public Block Current => current!;

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

    private sealed class BlockComparisonWrapper(Comparison<Block> comparison) : IComparer<BlockWrapper>
    {
        private readonly Comparison<Block> _comparison = comparison;

        public int Compare(BlockWrapper x, BlockWrapper y)
        {
            return _comparison(x.Block, y.Block);
        }
    }

    private sealed class BlockComparerWrapper(IComparer<Block> comparer) : IComparer<BlockWrapper>
    {
        private readonly IComparer<Block> _comparer = comparer;

        public int Compare(BlockWrapper x, BlockWrapper y)
        {
            return _comparer.Compare(x.Block, y.Block);
        }
    }
}