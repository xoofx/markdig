// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;

namespace Markdig.Syntax;

/// <summary>
/// Helpers for the <see cref="ICharIterator"/> class.
/// </summary>
public static class CharIteratorHelper
{
    public static bool TrimStartAndCountNewLines<T>(ref T iterator, out int countNewLines) where T : ICharIterator
    {
        return TrimStartAndCountNewLines(ref iterator, out countNewLines, out _);
    }

    public static bool TrimStartAndCountNewLines<T>(ref T iterator, out int countNewLines, out NewLine lastLine) where T : ICharIterator
    {
        countNewLines = 0;
        var c = iterator.CurrentChar;
        bool hasWhitespaces = false;
        lastLine = NewLine.None;
        while (c.IsWhitespace())
        {
            if (c == '\n' || c == '\r')
            {
                if (c == '\r' && iterator.PeekChar() == '\n')
                {
                    lastLine = NewLine.CarriageReturnLineFeed;
                    iterator.SkipChar(); // skip \n
                }
                else if (c == '\n')
                {
                    lastLine = NewLine.LineFeed;
                }
                else if (c == '\r')
                {
                    lastLine = NewLine.CarriageReturn;
                }
                countNewLines++;
            }
            else
            {
                // reset last line if if have a whitespace after
                lastLine = NewLine.None;
            }
            hasWhitespaces = true;
            c = iterator.NextChar();
        }
        return hasWhitespaces;
    }
}