// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Helpers;

namespace Textamina.Markdig.Extensions.Tables
{
    /// <summary>
    /// Helper methods for parsing tables.
    /// </summary>
    public static class TableHelper
    {
        /// <summary>
        /// Parses a column header equivalent to the regexp: <code>\s*:\s*[delimiterChar]+\s*:\s*</code>
        /// </summary>
        /// <param name="slice">The text slice.</param>
        /// <param name="delimiterChar">The delimiter character (either `-` or `=`).</param>
        /// <param name="align">The alignment of the column.</param>
        /// <returns>
        ///   <c>true</c> if parsing was successfull
        /// </returns>
        public static bool ParseColumnHeader(ref StringSlice slice, char delimiterChar, out TableColumnAlign align)
        {
            align = TableColumnAlign.Left;

            // Work on a copy of the slice
            slice.TrimStart();
            var c = slice.CurrentChar;
            bool hasLeft = false;
            bool hasRight = false;
            if (c == ':')
            {
                hasLeft = true;
                c = slice.NextChar();
            }

            int count = 0;
            while (c == delimiterChar)
            {
                c = slice.NextChar();
                count++;
            }

            if (count == 0)
            {
                return false;
            }

            if (c == ':')
            {
                hasRight = true;
                slice.NextChar();
            }
            slice.TrimStart();

            align = hasLeft && hasRight
                ? TableColumnAlign.Center
                : hasRight ? TableColumnAlign.Right : TableColumnAlign.Left;

            return true;
        }

    }
}