// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;

namespace Markdig.Extensions.Tables;

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
    /// <param name="delimiterCount">The number of delimiters.</param>
    /// <returns>
    ///   <c>true</c> if parsing was successful
    /// </returns>
    public static bool ParseColumnHeader(ref StringSlice slice, char delimiterChar, out TableColumnAlign? align, out int delimiterCount)
    {
        return ParseColumnHeaderDetect(ref slice, ref delimiterChar, out align, out delimiterCount);
    }

    /// <summary>
    /// Parses a column header equivalent to the regexp: <code>\s*:\s*[delimiterChar]+\s*:\s*</code>
    /// </summary>
    /// <param name="slice">The text slice.</param>
    /// <param name="delimiterChar">The delimiter character (either `-` or `=`).</param>
    /// <param name="align">The alignment of the column.</param>
    /// <returns>
    ///   <c>true</c> if parsing was successful
    /// </returns>
    public static bool ParseColumnHeaderAuto(ref StringSlice slice, out char delimiterChar, out TableColumnAlign? align)
    {
        delimiterChar = '\0';
        return ParseColumnHeaderDetect(ref slice, ref delimiterChar, out align, out _);
    }

    /// <summary>
    /// Parses a column header equivalent to the regexp: <code>\s*:\s*[delimiterChar]+\s*:\s*</code>
    /// </summary>
    /// <param name="slice">The text slice.</param>
    /// <param name="delimiterChar">The delimiter character (either `-` or `=`). If `\0`, it will detect the character (either `-` or `=`)</param>
    /// <param name="align">The alignment of the column.</param>
    /// <returns>
    ///   <c>true</c> if parsing was successful
    /// </returns>
    public static bool ParseColumnHeaderDetect(ref StringSlice slice, ref char delimiterChar, out TableColumnAlign? align, out int delimiterCount)
    {
        align = null;
        delimiterCount = 0;
        slice.TrimStart();
        var c = slice.CurrentChar;
        bool hasLeft = false;
        bool hasRight = false;
        if (c == ':')
        {
            hasLeft = true;
            slice.SkipChar();
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

        // We expect at least one `-` delimiter char
        delimiterCount = slice.CountAndSkipChar(delimiterChar);
        if (delimiterCount == 0)
        {
            return false;
        }

        slice.TrimStart();
        c = slice.CurrentChar;

        if (c == ':')
        {
            hasRight = true;
            slice.SkipChar();
        }
        slice.TrimStart();

        align = hasLeft && hasRight
            ? TableColumnAlign.Center
            : hasRight ? TableColumnAlign.Right : hasLeft ? TableColumnAlign.Left : (TableColumnAlign?) null;

        return true;
    }

}