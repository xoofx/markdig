// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Helpers;

namespace Markdig.Parsers
{
    /// <summary>
    /// The default parser for parsing numbered list item (e.g: 1) or 1.)
    /// </summary>
    /// <seealso cref="Markdig.Parsers.OrderedListItemParser" />
    public class NumberedListItemParser : OrderedListItemParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumberedListItemParser"/> class.
        /// </summary>
        public NumberedListItemParser()
        {
            OpeningCharacters = new char[10];
            for (int i = 0; i < 10; i++)
            {
                OpeningCharacters[i] = (char) ('0' + i);
            }
        }

        public override bool TryParse(BlockProcessor state, char pendingBulletType, out ListInfo result)
        {
            result = new ListInfo();
            var c = state.CurrentChar;

            int countDigit = 0;
            int startChar = -1;
            int endChar = 0;
            while (c.IsDigit())
            {
                endChar = state.Start;
                // Trim left 0
                if (startChar < 0 && c != '0')
                {
                    startChar = endChar;
                }
                c = state.NextChar();
                countDigit++;
            }
            if (startChar < 0)
            {
                startChar = endChar;
            }

            // Note that ordered list start numbers must be nine digits or less:
            char orderedDelimiter;
            if (countDigit > 9 || !TryParseDelimiter(state, out orderedDelimiter))
            {
                return false;
            }

            result.OrderedStart = state.Line.Text.Substring(startChar, endChar - startChar + 1);
            result.OrderedDelimiter = orderedDelimiter;
            result.BulletType = '1';
            result.DefaultOrderedStart = "1";
            return true;
        }
    }
}