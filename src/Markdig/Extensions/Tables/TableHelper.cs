// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;

namespace Markdig.Extensions.Tables
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
        public static bool ParseColumnHeader(ref StringSlice slice, char delimiterChar, out TableColumnAlign? align)
        {
            return ParseColumnHeaderDetect(ref slice, ref delimiterChar, out align);
        }

        /// <summary>
        /// Parses a column header equivalent to the regexp: <code>\s*:\s*[delimiterChar]+\s*:\s*</code>
        /// </summary>
        /// <param name="slice">The text slice.</param>
        /// <param name="delimiterChar">The delimiter character (either `-` or `=`).</param>
        /// <param name="align">The alignment of the column.</param>
        /// <returns>
        ///   <c>true</c> if parsing was successfull
        /// </returns>
        public static bool ParseColumnHeaderAuto(ref StringSlice slice, out char delimiterChar, out TableColumnAlign? align)
        {
            delimiterChar = '\0';
            return ParseColumnHeaderDetect(ref slice, ref delimiterChar, out align);
        }

        /// <summary>
        /// Parses a column header equivalent to the regexp: <code>\s*:\s*[delimiterChar]+\s*:\s*</code>
        /// </summary>
        /// <param name="slice">The text slice.</param>
        /// <param name="delimiterChar">The delimiter character (either `-` or `=`). If `\0`, it will detect the character (either `-` or `=`)</param>
        /// <param name="align">The alignment of the column.</param>
        /// <returns>
        ///   <c>true</c> if parsing was successfull
        /// </returns>
        public static bool ParseColumnHeaderDetect(ref StringSlice slice, ref char delimiterChar, out TableColumnAlign? align)
        {
            align = null;

            slice.TrimStart();
            var c = slice.CurrentChar;
            bool hasLeft = false;
            bool hasRight = false;
            if (c == ':')
            {
                hasLeft = true;
                slice.NextChar();
            }

            slice.TrimStart();
            c = slice.CurrentChar;

            // if we want to automatically detect
            if (delimiterChar == '\0')
            {
                if (c == '=' || c == '-')
                {
                    delimiterChar = c;
                }
                else
                {
                    return false;
                }
            }

            int count = 0;
            while (c == delimiterChar)
            {
                c = slice.NextChar();
                count++;
            }

            // We expect at least one `-` delimiter char
            if (count == 0)
            {
                return false;
            }

            slice.TrimStart();
            c = slice.CurrentChar;

            if (c == ':')
            {
                hasRight = true;
                slice.NextChar();
            }
            slice.TrimStart();

            align = hasLeft && hasRight
                ? TableColumnAlign.Center
                : hasRight ? TableColumnAlign.Right : hasLeft ? TableColumnAlign.Left : (TableColumnAlign?) null;

            return true;
        }

    }
}