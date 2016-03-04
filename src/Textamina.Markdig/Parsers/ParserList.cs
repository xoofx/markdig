// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Textamina.Markdig.Helpers;

namespace Textamina.Markdig.Parsers
{
    public abstract class ParserList<T, TState> : OrderedList<T> where T : ParserBase<TState>
    {
        private T[][] parsersWithOpeningCharacters;
        private T[] globalParsers;
        private bool[] isOpeningCharacter;

        public T[] GlobalParsers => globalParsers;

        public T[] GetParsersForOpeningCharacter(char openingChar)
        {
            return openingChar < parsersWithOpeningCharacters.Length ? parsersWithOpeningCharacters[openingChar] : null;
        }

        public char[] OpeningCharacters { get; private set; }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public int IndexOfOpeningCharacter(string text, int start, int end)
        {
            var maxChar = isOpeningCharacter.Length;
            for (int i = start; i <= end; i++)
            {
                var c = text[i];
                if (c < maxChar && isOpeningCharacter[c])
                {
                    return i;
                }
            }
            return -1;
        }

        public virtual void Initialize(TState initState)
        {
            var charCounter = new Dictionary<char, int>();
            int globalCounter = 0;
            int maxChar = 0;
            for (int i = 0; i < this.Count; i++)
            {
                var parser = this[i];
                parser.Initialize(initState);
                parser.Index = i;
                if (parser.OpeningCharacters != null && parser.OpeningCharacters.Length != 0)
                {
                    foreach (var openingChar in parser.OpeningCharacters)
                    {
                        if (openingChar >= 127)
                        {
                            throw new InvalidOperationException(
                                $"Invalid non-ascii character `{openingChar}` used by the parser [{parser.GetType().Name}]. Only ASCII < 127 are allowed");
                        }

                        if (!charCounter.ContainsKey(openingChar))
                        {
                            charCounter[openingChar] = 0;
                        }
                        charCounter[openingChar]++;
                        if (openingChar > maxChar)
                        {
                            maxChar = openingChar;
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
                        if (parsersWithOpeningCharacters[openingChar] == null)
                        {
                            parsersWithOpeningCharacters[openingChar] = new T[charCounter[openingChar]];
                        }
                        var list = parsersWithOpeningCharacters[openingChar];
                        var index = list.Length - charCounter[openingChar];
                        list[index] = parser;
                        charCounter[openingChar]--;
                        isOpeningCharacter[openingChar] = true;
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