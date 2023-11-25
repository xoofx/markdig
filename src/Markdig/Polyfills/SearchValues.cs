// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

#if !NET8_0_OR_GREATER

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Buffers;

internal static class SearchValues
{
    public static SearchValues<char> Create(string values) =>
        Create(values.AsSpan());

    public static SearchValues<char> Create(ReadOnlySpan<char> values) =>
        new PreNet8CompatSearchValues(values);

    public static int IndexOfAny(this ReadOnlySpan<char> span, SearchValues<char> values) =>
        values.IndexOfAny(span);

    public static int IndexOfAnyExcept(this ReadOnlySpan<char> span, SearchValues<char> values) =>
        values.IndexOfAnyExcept(span);
}

internal abstract class SearchValues<T>
{
    public abstract int IndexOfAny(ReadOnlySpan<char> span);

    public abstract int IndexOfAnyExcept(ReadOnlySpan<char> span);
}

internal sealed class PreNet8CompatSearchValues : SearchValues<char>
{
    private readonly BoolVector128 _ascii;
    private readonly HashSet<char>? _nonAscii;

    public PreNet8CompatSearchValues(ReadOnlySpan<char> values)
    {
        foreach (char c in values)
        {
            if (c < 128)
            {
                _ascii.Set(c);
            }
            else
            {
                _nonAscii ??= new HashSet<char>();
                _nonAscii.Add(c);
            }
        }
    }

    public override int IndexOfAny(ReadOnlySpan<char> span)
    {
        if (_nonAscii is null)
        {
            for (int i = 0; i < span.Length; i++)
            {
                char c = span[i];

                if (c < 128 && _ascii[c])
                {
                    return i;
                }
            }
        }
        else
        {
            for (int i = 0; i < span.Length; i++)
            {
                char c = span[i];

                if (c < 128 ? _ascii[c] : _nonAscii.Contains(c))
                {
                    return i;
                }
            }
        }

        return -1;
    }

    public override int IndexOfAnyExcept(ReadOnlySpan<char> span)
    {
        if (_nonAscii is null)
        {
            for (int i = 0; i < span.Length; i++)
            {
                char c = span[i];

                if (c >= 128 || !_ascii[c])
                {
                    return i;
                }
            }
        }
        else
        {
            for (int i = 0; i < span.Length; i++)
            {
                char c = span[i];

                if (c < 128 ? !_ascii[c] : !_nonAscii.Contains(c))
                {
                    return i;
                }
            }
        }

        return -1;
    }

    private unsafe struct BoolVector128
    {
        private fixed bool _values[128];

        public void Set(char c)
        {
            Debug.Assert(c < 128);
            _values[c] = true;
        }

        public readonly bool this[uint c]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Debug.Assert(c < 128);
                return _values[c];
            }
        }
    }
}

#endif