// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

using Markdig.Helpers;

namespace Markdig.Syntax;

/// <summary>
/// Base implementation for a the Markdown syntax tree.
/// </summary>
public abstract class MarkdownObject : IMarkdownObject
{
    private const uint ValueBitMask = (1u << 30) - 1;
    private const uint FirstBitMask = 1u << 31;
    private const uint SecondBitMask = 1u << 30;

    private const uint IsInlineMask = FirstBitMask;
    private const uint IsContainerMask = SecondBitMask;
    private const uint TypeKindMask = IsInlineMask | IsContainerMask;

    // Limit the value to 30 bits and repurpose the last two bits for commonly used flags
    private uint _lineBits;     // Also stores TypeKindMask (IsInline and IsContainer)
    private uint _columnBits;   // Also stores IsClosedInternal and InternalSpareBit

    internal bool IsContainerInline => (_lineBits & TypeKindMask) == (IsContainerMask | IsInlineMask);

    internal bool IsContainerBlock => (_lineBits & TypeKindMask) == IsContainerMask;

    internal bool IsContainer => (_lineBits & IsContainerMask) != 0;

    internal bool IsInline => (_lineBits & IsInlineMask) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private protected void SetTypeKind(bool isInline, bool isContainer)
    {
        Debug.Assert(_lineBits == 0);
        _lineBits = (isInline ? IsInlineMask : 0) | (isContainer ? IsContainerMask : 0);
    }

    private protected bool IsClosedInternal
    {
        get => (_columnBits & FirstBitMask) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (value) _columnBits |= FirstBitMask;
            else _columnBits &= ~FirstBitMask;
        }
    }

    private protected bool InternalSpareBit
    {
        get => (_columnBits & SecondBitMask) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (value) _columnBits |= SecondBitMask;
            else _columnBits &= ~SecondBitMask;
        }
    }

    protected MarkdownObject()
    {
        Span = SourceSpan.Empty;
    }

    /// <summary>
    /// The attached datas. Use internally a simple array instead of a Dictionary{Object,Object}
    /// as we expect less than 5~10 entries, usually typically 1 (HtmlAttributes)
    /// so it will gives faster access than a Dictionary, and lower memory occupation
    /// </summary>
    private DataEntriesAndTrivia? _attachedDatas;

    /// <summary>
    /// Gets or sets the text column this instance was declared (zero-based).
    /// </summary>
    public int Column
    {
        get => (int)(_columnBits & ValueBitMask);
        set => _columnBits = (_columnBits & ~ValueBitMask) | ((uint)value & ValueBitMask);
    }

    /// <summary>
    /// Gets or sets the text line this instance was declared (zero-based).
    /// </summary>
    public int Line
    {
        get => (int)(_lineBits & ValueBitMask);
        set => _lineBits = (_lineBits & ~ValueBitMask) | ((uint)value & ValueBitMask);
    }

    /// <summary>
    /// The source span
    /// </summary>
    public SourceSpan Span;

    /// <summary>
    /// Gets a string of the location in the text.
    /// </summary>
    /// <returns></returns>
    public string ToPositionText()
    {
        return $"${Line}, {Column}, {Span.Start}-{Span.End}";
    }

    /// <summary>
    /// Stores a key/value pair for this instance.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentNullException">if key is null</exception>
    public void SetData(object key, object value) => (_attachedDatas ??= new DataEntriesAndTrivia()).SetData(key, value);

    /// <summary>
    /// Determines whether this instance contains the specified key data.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns><c>true</c> if a data with the key is stored</returns>
    /// <exception cref="ArgumentNullException">if key is null</exception>
    public bool ContainsData(object key) => _attachedDatas?.ContainsData(key) ?? false;

    /// <summary>
    /// Gets the associated data for the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The associated data or null if none</returns>
    /// <exception cref="ArgumentNullException">if key is null</exception>
    public object? GetData(object key) => _attachedDatas?.GetData(key);

    /// <summary>
    /// Removes the associated data for the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns><c>true</c> if the data was removed; <c>false</c> otherwise</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool RemoveData(object key) => _attachedDatas?.RemoveData(key) ?? false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private protected T? GetTrivia<T>() where T : class
    {
        object? trivia = _attachedDatas?.Trivia;
        return Unsafe.As<T>(trivia);
    }

    private protected T GetOrSetTrivia<T>() where T : class, new()
    {
        var storage = _attachedDatas ??= new DataEntriesAndTrivia();
        storage.Trivia ??= new T();
        return Unsafe.As<T>(storage.Trivia);
    }

    private sealed class DataEntriesAndTrivia
    {
        private struct DataEntry(object key, object value)
        {
            public readonly object Key = key;
            public object Value = value;
        }

        private DataEntry[]? _entries;
        private int _count;

        public object? Trivia;

        public void SetData(object key, object value)
        {
            if (key is null) ThrowHelper.ArgumentNullException_key();

            DataEntry[]? entries = _entries;
            int count = _count;

            if (entries is null)
            {
                _entries = new DataEntry[2];
            }
            else
            {
                for (int i = 0; i < entries.Length && i < count; i++)
                {
                    ref DataEntry entry = ref entries[i];
                    if (entry.Key == key)
                    {
                        entry.Value = value;
                        return;
                    }
                }

                if (count == entries.Length)
                {
                    Array.Resize(ref _entries, count + 2);
                }
            }

            _entries![count] = new DataEntry(key, value);
            _count++;
        }

        public object? GetData(object key)
        {
            if (key is null) ThrowHelper.ArgumentNullException_key();

            DataEntry[]? entries = _entries;
            if (entries is null)
            {
                return null;
            }

            int count = _count;

            for (int i = 0; i < entries.Length && i < count; i++)
            {
                ref DataEntry entry = ref entries[i];
                if (entry.Key == key)
                {
                    return entry.Value;
                }
            }

            return null;
        }

        public bool ContainsData(object key)
        {
            if (key is null) ThrowHelper.ArgumentNullException_key();

            DataEntry[]? entries = _entries;
            if (entries is null)
            {
                return false;
            }

            int count = _count;

            for (int i = 0; i < entries.Length && i < count; i++)
            {
                if (entries[i].Key == key)
                {
                    return true;
                }
            }

            return false;
        }

        public bool RemoveData(object key)
        {
            if (key is null) ThrowHelper.ArgumentNullException_key();

            DataEntry[]? entries = _entries;
            if (entries is null)
            {
                return false;
            }

            int count = _count;

            for (int i = 0; i < entries.Length && i < count; i++)
            {
                if (entries[i].Key == key)
                {
                    if (i < count - 1)
                    {
                        Array.Copy(entries, i + 1, entries, i, count - i - 1);
                    }
                    count--;
                    entries[count] = default;
                    _count = count;
                    return true;
                }
            }

            return false;
        }
    }
}