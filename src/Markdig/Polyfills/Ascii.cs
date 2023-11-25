// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

#if !NET8_0_OR_GREATER

namespace System.Text;

internal static class Ascii
{
    public static bool IsValid(this string value)
    {
        return IsValid(value.AsSpan());
    }

    public static bool IsValid(this ReadOnlySpan<char> value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] > 127)
            {
                return false;
            }
        }

        return true;
    }
}

#endif