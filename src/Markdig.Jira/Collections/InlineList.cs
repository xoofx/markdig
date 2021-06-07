using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Markdig.Jira.Collections
{
    /// <summary>
    /// Lightweight struct list with optimized behavior over <see cref="List{T}"/>:
    /// - by ref on this[int index]
    /// - AddByRef(in T item)
    /// - RemoveAt returning remove item T
    /// - Settable Count
    /// - Underlying array accessible (pinnable...etc.)
    /// - Push/Pop methods
    /// - Implements <see cref="IListAdd{T}"/>  
    /// </summary>
    /// <typeparam name="T">Type of an item</typeparam>
    [DebuggerTypeProxy(typeof(DebugListView<>)), DebuggerDisplay("Count = {Count}")]
    internal struct InlineList<T> : IEnumerable<T> //, IListAdd<T>
    {
        private const int DefaultCapacity = 4;
        
        private static readonly InlineList<T> Empty = new InlineList<T>(0);

        public uint Count;

        public T[] Items;

        public InlineList(uint capacity)
        {
            Count = 0;
            Items = capacity == 0 ? Array.Empty<T>() : new T[capacity];
        }

        public uint Capacity
        {
            get => (uint)(Items?.Length ?? 0);
            set
            {
                Ensure();
                if (value <= Items.Length) return;
                EnsureCapacity(value);
            }
        }

        public static InlineList<T> Create()
        {
            return Empty;
        }

        public static InlineList<T> Create(uint capacity)
        {
            return new InlineList<T>(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Ensure()
        {
            if (Items == null) Items = Array.Empty<T>();
        }

        public bool IsReadOnly => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            if (Count > 0)
            {
                Array.Clear(Items, 0, (int)Count);
                Count = 0;
            }
        }

        public InlineList<T> Clone()
        {
            var items = (T[])Items?.Clone();
            return new InlineList<T>() { Count = Count, Items = items };
        }

        public bool Contains(T item)
        {
            return Count > 0 && IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (Count > 0)
            {
                System.Array.Copy(Items, 0, array, arrayIndex, Count);
            }
        }

        public void Reset()
        {
            Clear();
            Count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T child)
        {
            if (Count == Items.Length)
            {
                EnsureCapacity(Count + 1);
            }
            Items[Count++] = child;
        }

        public ref T GetOrCreate(uint index)
        {
            if (index >= Count)
            {
                var newCount = index + 1;
                EnsureCapacity(newCount);
                Count = newCount;
            }
            return ref Items[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddByRef(in T child)
        {
            if (Count == Items.Length)
            {
                EnsureCapacity(Count + 1);
            }
            Items[Count++] = child;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T AddByRefCurrent()
        {
            if (Count == Items.Length)
            {
                EnsureCapacity(Count + 1);
            }
            return ref Items[Count++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, T item)
        {
            if (Count == Items.Length)
            {
                EnsureCapacity(Count + 1);
            }
            if (index < Count)
            {
                Array.Copy(Items, index, Items, index + 1, Count - index);
            }
            Items[index] = item;
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T InsertReturnRef(int index, T item)
        {
            if (Count == Items.Length)
            {
                EnsureCapacity(Count + 1);
            }
            if (index < Count)
            {
                Array.Copy(Items, index, Items, index + 1, Count - index);
            }

            ref var refItem = ref Items[index];
            refItem = item;
            Count++;
            return ref refItem;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InsertByRef(int index, in T item)
        {
            if (Count == Items.Length)
            {
                EnsureCapacity(Count + 1);
            }
            if (index < Count)
            {
                Array.Copy(Items, index, Items, index + 1, Count - index);
            }
            Items[index] = item;
            Count++;
        }

        public bool Remove(T element)
        {
            var index = IndexOf(element);
            if (index >= 0)
            {
                RemoveAt((uint)index);
                return true;
            }
            return false;
        }

        public int IndexOf(T element)
        {
            return Array.IndexOf(Items, element, 0, (int)Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T RemoveAt(uint index)
        {
            if (index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
            Count--;
            // previous children
            var item = Items[index];
            if (index < Count)
            {
                Array.Copy(Items, index + 1, Items, index, Count - index);
            }
            Items[Count] = default(T);
            return item;
        }

        public T RemoveLast()
        {
            if (Count > 0)
            {
                ref var removed = ref Items[Count - 1];
                var copy = removed;
                removed = default;
                Count--;
                return copy;
            }
            return default;
        }

        public ref T this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if DEBUG
                ThrowHelper.CheckOutOfRange(index, Count);
#endif
                return ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(Items), (IntPtr)index);
            }
        }

        public void Push(T element)
        {
            Add(element);
        }

        public ref T Peek()
        {
            return ref Items[Count - 1];
        }
        
        public T Pop()
        {
            return RemoveAt(Count - 1);
        }

        private void EnsureCapacity(uint min)
        {
            if (Items.Length < min)
            {
                uint num = (Items.Length == 0) ? DefaultCapacity : ((uint)Items.Length) << 1;
                if (num < min)
                {
                    num = min;
                }
                var destinationArray = new T[num]; // ArrayPool<T>.Shared.Rent(num);
                if (Count > 0)
                {
                    Array.Copy(Items, 0, destinationArray, 0, Count);
                }
                if (Items.Length > 0)
                {
                    //ArrayPool<T>.Shared.Return(Items, true);
                }
                Items = destinationArray;
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly InlineList<T> list;
            private uint index;
            private T current;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(InlineList<T> list)
            {
                this.list = list;
                index = 0;
                current = default(T);
            }

            public T Current => current;

            object IEnumerator.Current => Current;


            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (index < list.Count)
                {
                    current = list[index];
                    index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                index = list.Count + 1;
                current = default(T);
                return false;
            }

            void IEnumerator.Reset()
            {
                index = 0;
                current = default(T);
            }
        }
    }

    internal class DebugListView<T>
    {
        private readonly InlineList<T> collection;

        public DebugListView(InlineList<T> collection)
        {
            this.collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                var array = new T[this.collection.Count];
                for (uint i = 0; i < array.Length; i++)
                {
                    array[i] = collection[i];
                }
                return array;
            }
        }
    }
}