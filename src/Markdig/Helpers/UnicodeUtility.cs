// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text;

// Based on https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Text/UnicodeUtility.cs
internal static class UnicodeUtility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBmpCodePoint(uint value) => value <= 0xFFFFu;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidUnicodeScalar(uint value)
    {
        return ((value - 0x110000u) ^ 0xD800u) >= 0xFFEF0800u;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetUtf16SurrogatesFromSupplementaryPlaneScalar(uint value, out char highSurrogateCodePoint, out char lowSurrogateCodePoint)
    {
        Debug.Assert(IsValidUnicodeScalar(value) && IsBmpCodePoint(value));

        highSurrogateCodePoint = (char)((value + ((0xD800u - 0x40u) << 10)) >> 10);
        lowSurrogateCodePoint = (char)((value & 0x3FFu) + 0xDC00u);
    }
}
