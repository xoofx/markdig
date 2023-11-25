// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Runtime.CompilerServices;

namespace Markdig.Helpers;

// Based on https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/HexConverter.cs
internal static class HexConverter
{
    public enum Casing : uint
    {
        Upper = 0,
        Lower = 0x2020U,
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToCharsBuffer(byte value, Span<char> buffer, int startingIndex = 0, Casing casing = Casing.Upper)
    {
        uint difference = (((uint)value & 0xF0U) << 4) + ((uint)value & 0x0FU) - 0x8989U;
        uint packedResult = ((((uint)(-(int)difference) & 0x7070U) >> 4) + difference + 0xB9B9U) | (uint)casing;

        buffer[startingIndex + 1] = (char)(packedResult & 0xFF);
        buffer[startingIndex] = (char)(packedResult >> 8);
    }
}
