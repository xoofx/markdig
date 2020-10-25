// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;

namespace Markdig.Syntax
{
    /// <summary>
    /// Helpers for the <see cref="ICharIterator"/> class.
    /// </summary>
    public static class CharIteratorHelper
    {
        public static bool TrimStartAndCountNewLines<T>(ref T iterator, out int countNewLines, out Newline firstNewline) where T : ICharIterator
        {
            countNewLines = 0;
            var c = iterator.CurrentChar;
            bool hasWhitespaces = false;
            firstNewline = Newline.None;
            while (c.IsWhitespace())
            {
                // TODO: RTP: fix newline check here for \r\n
                if (c == '\n' || c == '\r')
                {
                    if (c == '\r' && iterator.PeekChar() == '\n' && firstNewline != Newline.None)
                    {
                        firstNewline = Newline.CarriageReturnLineFeed;
                    }
                    else if (c == '\n' && firstNewline != Newline.None)
                    {
                        firstNewline = Newline.LineFeed;
                    }
                    else if (c == '\r' && firstNewline != Newline.None)
                    {
                        firstNewline = Newline.CarriageReturn;
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