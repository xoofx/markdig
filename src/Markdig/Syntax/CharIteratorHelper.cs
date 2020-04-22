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
    }
}