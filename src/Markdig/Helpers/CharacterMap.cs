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
        private readonly Dictionary<char, T> nonAsciiMap;
        private readonly BitVector128 isOpeningCharacter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMap{T}"/> class.
        /// </summary>
        /// <param name="maps">The states.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public CharacterMap(IEnumerable<KeyValuePair<char, T>> maps)
        {
            if (maps == null) throw new ArgumentNullException(nameof(maps));
            var charSet = new HashSet<char>();
            int maxChar = 0;

            foreach (var map in maps)
            {
                var openingChar = map.Key;

                charSet.Add(openingChar);

                if (openingChar < 128 && openingChar > maxChar)
                {
                    maxChar = openingChar;
                }
                else if (openingChar >= 128 && nonAsciiMap == null)
                {
                    // Initialize only if with have an actual non-ASCII opening character
                    nonAsciiMap = new Dictionary<char, T>();
                }
            }
            OpeningCharacters = charSet.ToArray();
            Array.Sort(OpeningCharacters);

            asciiMap = new T[maxChar + 1];
            var isOpeningCharacter = new BitVector128();

            foreach (var state in maps)
            {
                var openingChar = state.Key;
                T stateByChar;
                if (openingChar < 128)
                {
                    stateByChar = asciiMap[openingChar];

                    if (stateByChar == null)
                    {
                        asciiMap[openingChar] = state.Value;
                    }
                    isOpeningCharacter.Set(openingChar);
                }
                else
                {
                    if (!nonAsciiMap.TryGetValue(openingChar, out stateByChar))
                    {
                        nonAsciiMap[openingChar] = state.Value;
                    }
                }
            }

            this.isOpeningCharacter = isOpeningCharacter;
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
        public T this[char openingChar]
        {
            [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
            get
            {
                T map = null;
                if (openingChar < asciiMap.Length)
                {
                    map = asciiMap[openingChar];
                }
                else if (nonAsciiMap != null)
                {
                    nonAsciiMap.TryGetValue(openingChar, out map);
                }
                return map;
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
            var openingChars = isOpeningCharacter;

            unsafe
            {
                fixed (char* pText = text)
                {
                    if (nonAsciiMap == null)
                    {
                        for (int i = start; i <= end; i++)
                        {
                            var c = pText[i];
                            if (c < 128 && openingChars[c])
                            {
                                return i;
                            }
                        }
                    }
                    else
                    {
                        for (int i = start; i <= end; i++)
                        {
                            var c = pText[i];
                            if (c < 128 ? openingChars[c] : nonAsciiMap.ContainsKey(c))
                            {
                                return i;
                            }
                        }
                    }
                }
            }
            return -1;
        }

        internal unsafe struct BitVector128
        {
            fixed ulong values[2];

            public void Set(char c)
            {
                Debug.Assert(c < 128);
                values[c >> 6] |= (ulong)1 << c;
            }

            public readonly bool this[char c]
            {
                [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
                get
                {
                    Debug.Assert(c < 128);
                    return (values[c >> 6] & (ulong)1 << c) != 0;
                }
            }
        }
    }
}