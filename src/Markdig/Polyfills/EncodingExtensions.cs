// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

#if !NETCOREAPP2_1_OR_GREATER && !NETSTANDARD2_1_OR_GREATER

using System.Runtime.InteropServices;

namespace System.Text;

internal static class EncodingExtensions
{
    public static unsafe int GetBytes(this Encoding encoding, ReadOnlySpan<char> chars, Span<byte> bytes)
    {
        fixed (char* charsPtr = &MemoryMarshal.GetReference(chars))
        {
            fixed (byte* bytesPtr = &MemoryMarshal.GetReference(bytes))
            {
                return encoding.GetBytes(charsPtr, chars.Length, bytesPtr, bytes.Length);
            }
        }
    }
}
#endif
