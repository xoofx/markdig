// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
#if NETCOREAPP3_1_OR_GREATER
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif

namespace Markdig.Helpers;

/// <summary>
/// Allows to associate characters to a data structures and query efficiently for them.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class CharacterMap<T> where T : class
{
#if NETCOREAPP3_1_OR_GREATER
    private readonly Vector128<byte> _asciiBitmap;
#endif

    private readonly T[] asciiMap;
    private readonly Dictionary<uint, T>? nonAsciiMap;
    private readonly BoolVector128 isOpeningCharacter;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterMap{T}"/> class.
    /// </summary>
    /// <param name="maps">The states.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public CharacterMap(IEnumerable<KeyValuePair<char, T>> maps)
    {
        if (maps is null) ThrowHelper.ArgumentNullException(nameof(maps));
        var charSet = new HashSet<char>();
        int maxChar = 0;

        foreach (var map in maps)
        {
            var openingChar = map.Key;
            charSet.Add(openingChar);

            if (openingChar < 128)
            {
                maxChar = Math.Max(maxChar, openingChar);

                if (openingChar == 0)
                {
                    ThrowHelper.ArgumentOutOfRangeException("Null is not a valid opening character.", nameof(maps));
                }
            }
            else
            {
                nonAsciiMap ??= new Dictionary<uint, T>();
            }
        }

        OpeningCharacters = charSet.ToArray();
        Array.Sort(OpeningCharacters);

        asciiMap = new T[maxChar + 1];

        foreach (var state in maps)
        {
            char openingChar = state.Key;
            if (openingChar < 128)
            {
                asciiMap[openingChar] ??= state.Value;
                isOpeningCharacter.Set(openingChar);
            }
            else if (!nonAsciiMap!.ContainsKey(openingChar))
            {
                nonAsciiMap[openingChar] = state.Value;
            }
        }

#if NETCOREAPP3_1_OR_GREATER
        if (nonAsciiMap is null)
        {
            long bitmap_0_3 = 0;
            long bitmap_4_7 = 0;

            foreach (char openingChar in OpeningCharacters)
            {
                int position = (openingChar >> 4) | ((openingChar & 0x0F) << 3);
                if (position < 64) bitmap_0_3 |= 1L << position;
                else bitmap_4_7 |= 1L << (position - 64);
            }

            _asciiBitmap = Vector128.Create(bitmap_0_3, bitmap_4_7).AsByte();
        }
#endif
    }

    /// <summary>
    /// Gets all the opening characters defined.
    /// </summary>
    public char[] OpeningCharacters { get; }

    /// <summary>
    /// Gets the list of parsers valid for the specified opening character.
    /// </summary>
    /// <param name="openingChar">The opening character.</param>
    /// <returns>A list of parsers valid for the specified opening character or null if no parsers registered.</returns>
    public T? this[uint openingChar]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            T[] asciiMap = this.asciiMap;
            if (openingChar < (uint)asciiMap.Length)
            {
                return asciiMap[openingChar];
            }
            else
            {
                T? map = null;
                nonAsciiMap?.TryGetValue(openingChar, out map);
                return map;
            }
        }
    }


    /// <summary>
    /// Searches for an opening character from a registered parser in the specified string.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <returns>Index position within the string of the first opening character found in the specified text; if not found, returns -1</returns>
    public int IndexOfOpeningCharacter(string text, int start, int end)
    {
        Debug.Assert(text is not null);
        Debug.Assert(start >= 0 && end >= 0);
        Debug.Assert(end - start + 1 >= 0);
        Debug.Assert(end - start + 1 <= text.Length);

        if (nonAsciiMap is null)
        {
#if NETCOREAPP3_1_OR_GREATER
            if (Ssse3.IsSupported && BitConverter.IsLittleEndian)
            {
                // Based on http://0x80.pl/articles/simd-byte-lookup.html#universal-algorithm
                // Optimized for sets in the [1, 127] range

                int lengthMinusOne = end - start;
                int charsToProcessVectorized = lengthMinusOne & ~(2 * Vector128<short>.Count - 1);
                int finalStart = start + charsToProcessVectorized;

                if (start < finalStart)
                {
                    ref char textStartRef = ref Unsafe.Add(ref Unsafe.AsRef(in text.GetPinnableReference()), start);
                    Vector128<byte> bitmap = _asciiBitmap;
                    do
                    {
                        // Load 32 bytes (16 chars) into two Vector128<short>s (chars)
                        // Drop the high byte of each char
                        // Pack the remaining bytes into a single Vector128<byte>
                        Vector128<byte> input = Sse2.PackUnsignedSaturate(
                            Unsafe.ReadUnaligned<Vector128<short>>(ref Unsafe.As<char, byte>(ref textStartRef)),
                            Unsafe.ReadUnaligned<Vector128<short>>(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref textStartRef, Vector128<short>.Count))));

                        // Extract the higher nibble of each character ((input >> 4) & 0xF)
                        Vector128<byte> higherNibbles = Sse2.And(Sse2.ShiftRightLogical(input.AsUInt16(), 4).AsByte(), Vector128.Create((byte)0xF));

                        // Lookup the matching higher nibble for each character based on the lower nibble
                        // PSHUFB will set the result to 0 for any non-ASCII (> 127) character
                        Vector128<byte> bitsets = Ssse3.Shuffle(bitmap, input);

                        // Calculate a bitmask (1 << (higherNibble % 8)) for each character
                        Vector128<byte> bitmask = Ssse3.Shuffle(Vector128.Create(0x8040201008040201).AsByte(), higherNibbles);

                        // Check which characters are present in the set
                        // We are relying on bitsets being zero for non-ASCII characters
                        Vector128<byte> result = Sse2.And(bitsets, bitmask);

                        if (!result.Equals(Vector128<byte>.Zero))
                        {
                            int resultMask = ~Sse2.MoveMask(Sse2.CompareEqual(result, Vector128<byte>.Zero));
                            return start + BitOperations.TrailingZeroCount((uint)resultMask);
                        }

                        start += 2 * Vector128<short>.Count;
                        textStartRef = ref Unsafe.Add(ref textStartRef, 2 * Vector128<short>.Count);
                    }
                    while (start != finalStart);
                }
            }

            ref char textRef = ref Unsafe.AsRef(in text.GetPinnableReference());
            for (; start <= end; start++)
            {
                if (IntPtr.Size == 4)
                {
                    uint c = Unsafe.Add(ref textRef, start);
                    if (c < 128 && isOpeningCharacter[c])
                    {
                        return start;
                    }
                }
                else
                {
                    ulong c = Unsafe.Add(ref textRef, start);
                    if (c < 128 && isOpeningCharacter[c])
                    {
                        return start;
                    }
                }
            }
#else
            unsafe
            {
                fixed (char* pText = text)
                {
                    for (int i = start; i <= end; i++)
                    {
                        char c = pText[i];
                        if (c < 128 && isOpeningCharacter[c])
                        {
                            return i;
                        }
                    }
                }
            }
#endif
            return -1;
        }
        else
        {
            return IndexOfOpeningCharacterNonAscii(text, start, end);
        }
    }

    private int IndexOfOpeningCharacterNonAscii(string text, int start, int end)
    {
#if NETCOREAPP3_1_OR_GREATER
        ref char textRef = ref Unsafe.AsRef(in text.GetPinnableReference());
        for (int i = start; i <= end; i++)
        {
            char c = Unsafe.Add(ref textRef, i);
            if (c < 128 ? isOpeningCharacter[c] : nonAsciiMap!.ContainsKey(c))
            {
                return i;
            }
        }
#else
        unsafe
        {
            fixed (char* pText = text)
            {
                for (int i = start; i <= end; i++)
                {
                    char c = pText[i];
                    if (c < 128 ? isOpeningCharacter[c] : nonAsciiMap!.ContainsKey(c))
                    {
                        return i;
                    }
                }
            }
        }
#endif
        return -1;
    }
}

internal unsafe struct BoolVector128
{
    private fixed bool values[128];

    public void Set(char c)
    {
        Debug.Assert(c < 128);
        values[c] = true;
    }

    public readonly bool this[uint c]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Debug.Assert(c < 128);
            return values[c];
        }
    }
    public readonly bool this[ulong c]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Debug.Assert(c < 128 && IntPtr.Size == 8);
            return values[c];
        }
    }
}