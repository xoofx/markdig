// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Markdig.Helpers
{
    /// <summary>
    /// Allows to associate characters to a data structures and query efficiently for them.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CharacterMap<T> where T : class
    {
        private readonly T[] asciiMap;
        private readonly Dictionary<uint, T> nonAsciiMap;
        private readonly BoolVector128 isOpeningCharacter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMap{T}"/> class.
        /// </summary>
        /// <param name="maps">The states.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CharacterMap(IEnumerable<KeyValuePair<char, T>> maps)
        {
            if (maps == null) ThrowHelper.ArgumentNullException(nameof(maps));
            var charSet = new HashSet<char>();
            int maxChar = 0;

            foreach (var map in maps)
            {
                var openingChar = map.Key;
                charSet.Add(openingChar);

                if (openingChar < 128)
                {
                    maxChar = Math.Max(maxChar, openingChar);
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
                else if (!nonAsciiMap.ContainsKey(openingChar))
                {
                    nonAsciiMap[openingChar] = state.Value;
                }
            }
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
        public T this[uint openingChar]
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
                    T map = null;
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
            if (nonAsciiMap is null)
            {
#if NETCOREAPP3_1
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
#if NETCOREAPP3_1
            ref char textRef = ref Unsafe.AsRef(in text.GetPinnableReference());
            for (int i = start; i <= end; i++)
            {
                char c = Unsafe.Add(ref textRef, i);
                if (c < 128 ? isOpeningCharacter[c] : nonAsciiMap.ContainsKey(c))
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
                        if (c < 128 ? isOpeningCharacter[c] : nonAsciiMap.ContainsKey(c))
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
}