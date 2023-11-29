// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

#if !NET8_0_OR_GREATER

namespace System;

internal static class IndexOfHelpers
{
    public static bool ContainsAnyExcept(this ReadOnlySpan<char> span, char value0, char value1, char value2)
    {
        for (int i = 0; i < span.Length; i++)
        {
            char c = span[i];
            if (c != value0 && c != value1 && c != value2)
            {
                return true;
            }
        }

        return false;
    }

#if !NETSTANDARD2_1_OR_GREATER
    public static int IndexOfAny(this ReadOnlySpan<char> span, string values)
    {
        for (int i = 0; i < span.Length; i++)
        {
            char c = span[i];

            foreach (char v in values)
            {
                if (c == v)
                {
                    return i;
                }
            }
        }

        return -1;
    }
#endif

#if !NET6_0_OR_GREATER
    public static bool Contains<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>
    {
        return span.IndexOf(value) >= 0;
    }
#endif
}

#endif