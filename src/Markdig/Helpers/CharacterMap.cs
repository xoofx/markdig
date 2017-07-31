// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
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
        private readonly bool[] isOpeningCharacter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMap{T}"/> class.
        /// </summary>
        /// <param name="maps">The states.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public CharacterMap(IEnumerable<KeyValuePair<char, T>> maps)
        {
            if (maps == null) throw new ArgumentNullException(nameof(maps));
            var charCounter = new Dictionary<char, int>();
            int maxChar = 0;

            foreach (var map in maps)
            {
                var openingChar = map.Key;

                if (!charCounter.ContainsKey(openingChar))
                {
                    charCounter[openingChar] = 0;
                }
                charCounter[openingChar]++;

                if (openingChar < 127 && openingChar > maxChar)
                {
                    maxChar = openingChar;
                }
                else if (openingChar >= 127 && nonAsciiMap == null)
                {
                    // Initialize only if with have an actual non-ASCII opening character
                    nonAsciiMap = new Dictionary<char, T>();
                }
            }
            OpeningCharacters = charCounter.Keys.ToArray();
            Array.Sort(OpeningCharacters);

            asciiMap = new T[maxChar + 1];
            isOpeningCharacter = new bool[maxChar + 1];

            foreach (var state in maps)
            {
                var openingChar = state.Key;
                T stateByChar;
                if (openingChar < 127)
                {
                    stateByChar = asciiMap[openingChar];

                    if (stateByChar == null)
                    {
                        asciiMap[openingChar] = state.Value;
                    }
                    isOpeningCharacter[openingChar] = true;
                }
                else
                {
                    if (!nonAsciiMap.TryGetValue(openingChar, out stateByChar))
                    {
                        nonAsciiMap[openingChar] = state.Value;
                    }
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
            var maxChar = isOpeningCharacter.Length;
#if SUPPORT_UNSAFE
            unsafe
#endif
            {
#if SUPPORT_FIXED_STRING
            fixed (char* pText = text)
#else
                var pText = text;
#endif
#if SUPPORT_UNSAFE
                fixed (bool* openingChars = isOpeningCharacter)
#else
                var openingChars = isOpeningCharacter;
#endif
                {
                    if (nonAsciiMap == null)
                    {
                        for (int i = start; i <= end; i++)
                        {
                            var c = pText[i];
                            if (c < maxChar && openingChars[c])
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
                            if ((c < maxChar && openingChars[c]) || nonAsciiMap.ContainsKey(c))
                            {
                                return i;
                            }
                        }
                    }
                }
            }
            return -1;
        }
    }
}