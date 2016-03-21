// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Extensions.ListExtra
{
    public class RomanListItemParser : OrderedListItemParser
    {
        private readonly bool isUpper;

        public RomanListItemParser(bool isUpper)
        {
            this.isUpper = isUpper;
            OpeningCharacters = isUpper ? new[] { 'I', 'V', 'X' } : new[] {'i', 'v', 'x'};
            DefaultOrderedStart = isUpper ? "I" : "i";
        }

        public override bool TryParse(BlockProcessor state, out char bulletType, out string orderedStart, out char orderedDelimiter)
        {
            // NOTE: We don't try to validate roman numbers
            bulletType = (char)0;
            orderedDelimiter = (char)0;
            var c = state.CurrentChar;
            orderedStart = null;

            int startChar = state.Start;
            int endChar = 0;
            while (isUpper ? CharHelper.IsRomanLetterUpperPartial(c) : CharHelper.IsRomanLetterLowerPartial(c))
            {
                endChar = state.Start;
                c = state.NextChar();
            }

            if (!ParseDelimiter(c))
            {
                return false;
            }

            orderedStart = state.Line.Text.Substring(startChar, endChar - startChar + 1);
            orderedDelimiter = c;
            bulletType = isUpper ? 'I' : 'i';
            return true;
        }
    }
}