// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Extensions.ListExtra
{
    public class ListExtraItemParser : OrderedListItemParser
    {
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

        public override bool TryParse(BlockProcessor state, char previousBulletType, out ListInfo result)
        {
            result = new ListInfo();

            var c = state.CurrentChar;

            var isRomanLow = CharHelper.IsRomanLetterLowerPartial(c);
            var isRomanUp = !isRomanLow && CharHelper.IsRomanLetterUpperPartial(c);
            if ((isRomanLow || isRomanUp) && (previousBulletType == '\0' || previousBulletType == 'i' || previousBulletType == 'I'))
            {
                int startChar = state.Start;
                int endChar = 0;
                while (isRomanLow ? CharHelper.IsRomanLetterLowerPartial(c) : CharHelper.IsRomanLetterUpperPartial(c))
                {
                    endChar = state.Start;
                    c = state.NextChar();
                }

                result.OrderedStart = state.Line.Text.Substring(startChar, endChar - startChar + 1);
                result.BulletType = isRomanLow ? 'i' : 'I';
                result.DefaultOrderedStart = isRomanLow ? "i" : "I";
            }
            else
            {
                var isUpper = c.IsAlphaUpper();
                result.BulletType = isUpper ? 'A' : 'a';
                result.OrderedStart = state.CurrentChar.ToString();
                result.DefaultOrderedStart = isUpper ? "A" : "a";
                state.NextChar();
            }

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