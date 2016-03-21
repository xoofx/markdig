// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Extensions.ListExtra
{
    public class AlphaListItemParser : OrderedListItemParser
    {
        private readonly char from;

        public AlphaListItemParser(bool isUpper, bool discardRoman)
        {
            from = isUpper ? 'A' : 'a';
            var to = isUpper ? 'Z' : 'z';

            OpeningCharacters = new char['z' - 'a' + (discardRoman ? -2 : 0)]; // full alphabet minus the i|v|x for roman letter
            int index = 0;
            for (char c = from; c <= to; c++)
            {
                // discard the i letter for roman list
                if (discardRoman && CharHelper.IsRomanLetterPartial(c))
                {
                    continue;
                }
                OpeningCharacters[index] = c;
                index++;
            }
            DefaultOrderedStart = from.ToString();
        }

        public override bool TryParse(BlockProcessor state, out char bulletType, out string orderedStart, out char orderedDelimiter)
        {
            bulletType = from;
            orderedStart = state.CurrentChar.ToString();
            orderedDelimiter = state.NextChar();
            if (!ParseDelimiter(orderedDelimiter))
            {
                return false;
            }
            return true;
        }
    }
}