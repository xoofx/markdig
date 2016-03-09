// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Textamina.Markdig.Helpers;

namespace Textamina.Markdig.Parsers
{
    /// <summary>
    /// Base class for a list of parsers.
    /// </summary>
    /// <typeparam name="T">Type of the parser</typeparam>
    /// <typeparam name="TState">The type of the parser state.</typeparam>
    /// <seealso cref="Textamina.Markdig.Helpers.OrderedList{T}" />
    public abstract class ParserList<T, TState> : OrderedList<T> where T : ParserBase<TState>
    {
        private T[][] parsersWithOpeningCharacters;
        private Dictionary<char, T[]> parsersWithOpeningCharactersFallback;
        private T[] globalParsers;
        private bool[] isOpeningCharacter;

        protected ParserList()
        {
        }

        /// <summary>
        /// Gets the list of global parsers (that don't have any opening characters defined)
        /// </summary>
        public T[] GlobalParsers => globalParsers;

        /// <summary>
        /// Gets all the opening characters defined.
        /// </summary>
        public char[] OpeningCharacters { get; private set; }

        /// <summary>
        /// Gets the list of parsers valid for the specified opening character.
        /// </summary>
        /// <param name="openingChar">The opening character.</param>
        /// <returns>A list of parsers valid for the specified opening character or null if no parsers registered.</returns>
        public T[] GetParsersForOpeningCharacter(char openingChar)
        {
            T[] parsers = null;
            if (openingChar < parsersWithOpeningCharacters.Length)
            {
                parsers = parsersWithOpeningCharacters[openingChar];
            }
            else if (parsersWithOpeningCharactersFallback != null)
            {
                parsersWithOpeningCharactersFallback.TryGetValue(openingChar, out parsers);
            }
            return parsers;
        }

        /// <summary>
        /// Searches for an opening character from a registered parser in the specified string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Index position within the string of the first opening character found in the specified text; if not found, returns -1</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public unsafe int IndexOfOpeningCharacter(string text, int start, int end)
        {
            var maxChar = isOpeningCharacter.Length;
            fixed (char* pText = text)
            fixed (bool* openingChars = isOpeningCharacter)
            {
                for (int i = start; i <= end; i++)
                {
                    var c = pText[i];
                    if ((c < maxChar && openingChars[c]) || (parsersWithOpeningCharactersFallback != null && parsersWithOpeningCharactersFallback.ContainsKey(c)))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Initializes this instance with specified parser state.
        /// </summary>
        /// <param name="initState">State of the initialize.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Unexpected null parser found
        /// or
        /// </exception>
        public virtual void Initialize(TState initState)
        {
            var charCounter = new Dictionary<char, int>();
            int globalCounter = 0;
            int maxChar = 0;

            for (int i = 0; i < Count; i++)
            {
                var parser = this[i];
                if (parser == null)
                {
                    throw new InvalidOperationException("Unexpected null parser found");
                }

                parser.Initialize(initState);
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

                        if (openingChar < 127 && openingChar > maxChar)
                        {
                            maxChar = openingChar;
                        }
                        else if (openingChar >= 127 && parsersWithOpeningCharactersFallback == null)
                        {
                            parsersWithOpeningCharactersFallback = new Dictionary<char, T[]>();
                        }
                    }
                }
                else
                {
                    globalCounter++;
                }
            }
            OpeningCharacters = charCounter.Keys.ToArray();
            Array.Sort(OpeningCharacters);

            if (globalCounter > 0)
            {
                globalParsers = new T[globalCounter];
            }
            parsersWithOpeningCharacters = new T[maxChar + 1][];
            isOpeningCharacter = new bool[maxChar + 1];

            foreach (var parser in this)
            {
                if (parser.OpeningCharacters != null && parser.OpeningCharacters.Length != 0)
                {
                    foreach (var openingChar in parser.OpeningCharacters)
                    {
                        T[] parsersByChar;
                        if (openingChar < 127)
                        {
                            parsersByChar = parsersWithOpeningCharacters[openingChar];

                            if (parsersByChar == null)
                            {
                                parsersWithOpeningCharacters[openingChar] = parsersByChar = new T[charCounter[openingChar]];
                            }
                            isOpeningCharacter[openingChar] = true;
                        }
                        else
                        {
                            if (!parsersWithOpeningCharactersFallback.TryGetValue(openingChar, out parsersByChar))
                            {
                                parsersByChar = new T[charCounter[openingChar]];
                            }
                        }

                        var index = parsersByChar.Length - charCounter[openingChar];
                        parsersByChar[index] = parser;
                        charCounter[openingChar]--;
                    }
                }
                else
                {
                    globalParsers[globalParsers.Length - globalCounter] = parser;
                    globalCounter--;
                }
            }
        }
    }
}