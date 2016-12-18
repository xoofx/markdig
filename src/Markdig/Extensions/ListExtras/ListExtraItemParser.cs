// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Extensions.ListExtras
{
    using System;

    /// <summary>
    /// Parser that adds supports for parsing alpha/roman list items (e.g: `a)` or `a.` or `ii.` or `II.`)
    /// </summary>
    /// <remarks>
    /// Note that we don't validate roman numbers.
    /// </remarks>
    /// <seealso cref="Markdig.Parsers.OrderedListItemParser" />
    public class ListExtraItemParser : OrderedListItemParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListExtraItemParser"/> class.
        /// </summary>
        public ListExtraItemParser()
        {
            OpeningCharacters = new char[('z' - 'a' + 1)*2];
            int index = 0;
            for (var c = 'A'; c <= 'Z'; c++)
            {
                OpeningCharacters[index++] = c;
                OpeningCharacters[index++] = (char)(c - 'A' + 'a');
            }
        }

        public override bool TryParse(BlockProcessor state, char pendingBulletType, out ListInfo result)
        {
            result = new ListInfo();

            var c = state.CurrentChar;

            var isRomanLow = CharHelper.IsRomanLetterLowerPartial(c);
            var isRomanUp = !isRomanLow && CharHelper.IsRomanLetterUpperPartial(c);

            // We allow to parse roman only if we start on a new list or the pending list is already a roman list)
            if ((isRomanLow || isRomanUp) && (pendingBulletType == '\0' || pendingBulletType == 'i' || pendingBulletType == 'I'))
            {
                int startChar = state.Start;
                int endChar = 0;
                // With a roman, we can have multiple characters
                // Note that we don't validate roman numbers
                while (isRomanLow ? CharHelper.IsRomanLetterLowerPartial(c) : CharHelper.IsRomanLetterUpperPartial(c))
                {
                    endChar = state.Start;
                    c = state.NextChar();
                }

                result.OrderedStart = CharHelper.RomanToArabic(state.Line.Text.Substring(startChar, endChar - startChar + 1)).ToString();
                result.BulletType = isRomanLow ? 'i' : 'I';
                result.DefaultOrderedStart = isRomanLow ? "i" : "I";
            }
            else
            {
                // otherwise we expect a regular alpha lettered list with a single character.
                var isUpper = c.IsAlphaUpper();
                result.OrderedStart = (Char.ToUpperInvariant(c) - 64).ToString();
                result.BulletType = isUpper ? 'A' : 'a';
                result.DefaultOrderedStart = isUpper ? "A" : "a";
                state.NextChar();
            }

            // Finally we expect to always have a delimiter '.' or ')'
            char orderedDelimiter;
            if (!TryParseDelimiter(state, out orderedDelimiter))
            {
                return false;
            }

            result.OrderedDelimiter = orderedDelimiter;
            return true;
        }
    }
}