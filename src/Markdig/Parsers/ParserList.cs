// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Markdig.Helpers;

namespace Markdig.Parsers
{
    /// <summary>
    /// Base class for a list of parsers.
    /// </summary>
    /// <typeparam name="T">Type of the parser</typeparam>
    /// <typeparam name="TState">The type of the parser state.</typeparam>
    /// <seealso cref="Markdig.Helpers.OrderedList{T}" />
    public abstract class ParserList<T, TState> : OrderedList<T> where T : ParserBase<TState>
    {
        private readonly CharacterMap<T[]> charMap;
        private readonly T[] globalParsers;

        protected ParserList(IEnumerable<T> parsersArg) : base(parsersArg)
        {
            var charCounter = new Dictionary<char, int>();
            int globalCounter = 0;

            for (int i = 0; i < Count; i++)
            {
                var parser = this[i];
                if (parser == null)
                {
                    throw new InvalidOperationException("Unexpected null parser found");
                }

                parser.Initialize();
                parser.Index = i;
                if (parser.OpeningCharacters != null && parser.OpeningCharacters.Length != 0)
                {
                    foreach (var openingChar in parser.OpeningCharacters)
                    {
                        if (!charCounter.ContainsKey(openingChar))
                        {
                            charCounter[openingChar] = 0;
                        }
                        charCounter[openingChar]++;
                    }
                }
                else
                {
                    globalCounter++;
                }
            }

            if (globalCounter > 0)
            {
                globalParsers = new T[globalCounter];
            }

            var tempCharMap = new Dictionary<char, T[]>();
            foreach (var parser in this)
            {
                if (parser.OpeningCharacters != null && parser.OpeningCharacters.Length != 0)
                {
                    foreach (var openingChar in parser.OpeningCharacters)
                    {
                        T[] parsers;
                        if (!tempCharMap.TryGetValue(openingChar, out parsers))
                        {
                            parsers = new T[charCounter[openingChar]];
                            tempCharMap[openingChar] = parsers;
                        }

                        var index = parsers.Length - charCounter[openingChar];
                        parsers[index] = parser;
                        charCounter[openingChar]--;
                    }
                }
                else
                {
                    globalParsers[globalParsers.Length - globalCounter] = parser;
                    globalCounter--;
                }
            }

            charMap = new CharacterMap<T[]>(tempCharMap);
        }

        /// <summary>
        /// Gets the list of global parsers (that don't have any opening characters defined)
        /// </summary>
        public T[] GlobalParsers => globalParsers;

        /// <summary>
        /// Gets all the opening characters defined.
        /// </summary>
        public char[] OpeningCharacters => charMap.OpeningCharacters;

        /// <summary>
        /// Gets the list of parsers valid for the specified opening character.
        /// </summary>
        /// <param name="openingChar">The opening character.</param>
        /// <returns>A list of parsers valid for the specified opening character or null if no parsers registered.</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public T[] GetParsersForOpeningCharacter(char openingChar)
        {
            return charMap[openingChar];
        }

        /// <summary>
        /// Searches for an opening character from a registered parser in the specified string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Index position within the string of the first opening character found in the specified text; if not found, returns -1</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public int IndexOfOpeningCharacter(string text, int start, int end)
        {
            return charMap.IndexOfOpeningCharacter(text, start, end);
        }

        /// <summary>
        /// Initializes this instance with specified parser state.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Unexpected null parser found
        /// or
        /// </exception>
        private void Initialize()
        {
        }
    }
}