// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

#nullable enable

using Markdig.Helpers;

namespace Markdig.Syntax
{
    /// <summary>
    /// Helpers for the <see cref="ICharIterator"/> class.
    /// </summary>
    public static class CharIteratorHelper
    {
        public static bool TrimStartAndCountNewLines<T>(ref T iterator, out int countNewLines) where T : ICharIterator
        {
            countNewLines = 0;
            var c = iterator.CurrentChar;
            bool hasWhitespaces = false;
            while (c.IsWhitespace())
            {
                if (c == '\n')
                {
                    countNewLines++;
                }
                hasWhitespaces = true;
                c = iterator.NextChar();
            }
            return hasWhitespaces;
        }

        public static bool TrimStartAndCountNewLines<T>(ref T iterator, out int countNewLines, out NewLine firstNewline) where T : ICharIterator
        {
            countNewLines = 0;
            var c = iterator.CurrentChar;
            bool hasWhitespaces = false;
            firstNewline = NewLine.None;
            while (c.IsWhitespace())
            {
                if (c == '\n' || c == '\r')
                {
                    if (c == '\r' && iterator.PeekChar() == '\n' && firstNewline != NewLine.None)
                    {
                        firstNewline = NewLine.CarriageReturnLineFeed;
                    }
                    else if (c == '\n' && firstNewline != NewLine.None)
                    {
                        firstNewline = NewLine.LineFeed;
                    }
                    else if (c == '\r' && firstNewline != NewLine.None)
                    {
                        firstNewline = NewLine.CarriageReturn;
                    }
                    countNewLines++;
                }
                hasWhitespaces = true;
                c = iterator.NextChar();
            }
            return hasWhitespaces;
        }
    }
}