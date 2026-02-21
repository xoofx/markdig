// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace Markdig.Helpers;

/// <summary>
/// A List that provides methods for inserting/finding before/after. See remarks.
/// </summary>
/// <typeparam name="T">Type of the list item</typeparam>
/// <seealso cref="List{T}" />
/// <remarks>We use a typed list and don't use extension methods because it would pollute all list implements and the top level namespace.</remarks>
public class OrderedList<T> : List<T> where T: notnull
{
    /// <summary>
    /// Performs the ordered list operation.
    /// </summary>
    public OrderedList()
    {
    }

    /// <summary>
    /// Performs the ordered list operation.
    /// </summary>
    public OrderedList(IEnumerable<T> collection) : base(collection)
    {
    }

    /// <summary>
    /// Performs the insert before t item operation.
    /// </summary>
    public bool InsertBefore<TItem>(T item) where TItem : T
    {
        if (item is null) ThrowHelper.ArgumentNullException_item();
        for (int i = 0; i < Count; i++)
        {
            if (this[i] is TItem)
            {
                Insert(i, item);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Performs the find t item operation.
    /// </summary>
    public TItem? Find<TItem>() where TItem : T
    {
        for (int i = 0; i < Count; i++)
        {
            if (this[i] is TItem)
            {
                return (TItem)this[i];
            }
        }
        return default;
    }

    /// <summary>
    /// Attempts to find t item.
    /// </summary>
    public bool TryFind<TItem>([NotNullWhen(true)] out TItem? item) where TItem : T
    {
        item = Find<TItem>();
        return item != null;
    }

    /// <summary>
    /// Performs the find exact t item operation.
    /// </summary>
    public TItem? FindExact<TItem>() where TItem : T
    {
        for (int i = 0; i < Count; i++)
        {
            if (this[i].GetType() == typeof(TItem))
            {
                return (TItem)this[i];
            }
        }
        return default;
    }

    /// <summary>
    /// Adds if not already t item.
    /// </summary>
    public void AddIfNotAlready<TItem>() where TItem : class, T, new()
    {
        if (!Contains<TItem>())
        {
            Add(new TItem());
        }
    }

    /// <summary>
    /// Adds if not already t item.
    /// </summary>
    public void AddIfNotAlready<TItem>(TItem item) where TItem : T
    {
        if (!Contains<TItem>())
        {
            Add(item);
        }
    }

    /// <summary>
    /// Performs the insert after t item operation.
    /// </summary>
    public bool InsertAfter<TItem>(T item) where TItem : T
    {
        if (item is null) ThrowHelper.ArgumentNullException_item();
        for (int i = 0; i < Count; i++)
        {
            if (this[i] is TItem)
            {
                Insert(i + 1, item);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Performs the contains t item operation.
    /// </summary>
    public bool Contains<TItem>() where TItem : T
    {
        for (int i = 0; i < Count; i++)
        {
            if (this[i] is TItem)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Replaces <typeparamref name="TItem"/> with <paramref name="replacement"/>.
    /// </summary>
    /// <typeparam name="TItem">Item type to find in the list</typeparam>
    /// <param name="replacement">Object to replace this item with</param>
    /// <returns><c>true</c> if a replacement was made; otherwise <c>false</c>.</returns>
    public bool Replace<TItem>(T replacement) where TItem : T
    {
        for (var i = 0; i < Count; i++)
        {
            if (this[i] is TItem)
            {
                RemoveAt(i);
                Insert(i, replacement);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Replaces <typeparamref name="TItem"/> with <paramref name="newItem"/> or adds <paramref name="newItem"/>.
    /// </summary>
    /// <typeparam name="TItem">Item type to find in the list</typeparam>
    /// <param name="newItem">Object to add/replace the found item with</param>
    /// <returns><c>true</c> if a replacement was made; otherwise <c>false</c>.</returns>
    public bool ReplaceOrAdd<TItem>(T newItem) where TItem : T
    {
        if (Replace<TItem>(newItem))
            return true;

        Add(newItem);
        return false;
    }

    /// <summary>
    /// Removes the first occurrence of <typeparamref name="TItem"/>
    /// </summary>
    public bool TryRemove<TItem>() where TItem : T
    {
        for (int i = 0; i < Count; i++)
        {
            if (this[i] is TItem)
            {
                RemoveAt(i);
                return true;
            }
        }
        return false;
    }
}
