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
}

#endif