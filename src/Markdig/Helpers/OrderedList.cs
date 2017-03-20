// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;

namespace Markdig.Helpers
{
    /// <summary>
    /// A List that provides methods for inserting/finding before/after. See remarks.
    /// </summary>
    /// <typeparam name="T">Type of the list item</typeparam>
    /// <seealso cref="System.Collections.Generic.List{T}" />
    /// <remarks>We use a typed list and don't use extension methods because it would pollute all list implemts and the top level namespace.</remarks>
    public class OrderedList<T> : List<T>
    {
        public OrderedList()
        {
        }

        public OrderedList(IEnumerable<T> collection) : base(collection)
        {
        }

        public bool InsertBefore<TElement>(T element) where TElement : T
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            for (int i = 0; i < Count; i++)
            {
                if (this[i] is TElement)
                {
                    Insert(i, element);
                    return true;
                }
            }
            return false;
        }

        public TElement Find<TElement>() where TElement : T
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] is TElement)
                {
                    return (TElement)this[i];
                }
            }
            return default(TElement);
        }

        public TElement FindExact<TElement>() where TElement : T
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].GetType() == typeof(TElement))
                {
                    return (TElement)this[i];
                }
            }
            return default(TElement);
        }

        public void AddIfNotAlready<TElement>() where TElement : class, T, new()
        {
            if (!Contains<TElement>())
            {
                Add(new TElement());
            }
        }

        public void AddIfNotAlready<TElement>(TElement telement) where TElement : T
        {
            if (!Contains<TElement>())
            {
                Add(telement);
            }
        }

        public bool InsertAfter<TElement>(T element) where TElement : T
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            for (int i = 0; i < Count; i++)
            {
                if (this[i] is TElement)
                {
                    Insert(i + 1, element);
                    return true;
                }
            }
            return false;
        }

        public bool Contains<TElement>() where TElement : T
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] is TElement)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Replaces <typeparamref name="TElement"/> with <paramref name="replacement"/>.
        /// </summary>
        /// <typeparam name="TElement">Element type to find in the list</typeparam>
        /// <param name="replacement">Object to replace this element with</param>
        /// <returns><c>true</c> if a replacement was made; otherwise <c>false</c>.</returns>
        public bool Replace<TElement>(T replacement) where TElement : T
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i] is TElement)
                {
                    RemoveAt(i);
                    Insert(i, replacement);
                    return true;
                }
            }
            return false;
        }
    }
}