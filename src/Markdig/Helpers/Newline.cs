// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

namespace Markdig.Helpers;

/// <summary>
/// Represents a character or set of characters that represent a separation
/// between two lines of text
/// </summary>
public enum NewLine : byte
{
    // Values have the length encoded in last 2 bits
    /// <summary>
    /// Specifies none.
    /// </summary>
    None = 0,
    /// <summary>
    /// Specifies carriage return.
    /// </summary>
    CarriageReturn = 4 | 1,
    /// <summary>
    /// Specifies line feed.
    /// </summary>
    LineFeed = 8 | 1,
    /// <summary>
    /// Gets or sets the carriage return line feed.
    /// </summary>
    CarriageReturnLineFeed = 16 | 2
}

/// <summary>
/// Represents the NewLineExtensions type.
/// </summary>
public static class NewLineExtensions
{
    /// <summary>
    /// Performs the as string operation.
    /// </summary>
    public static string AsString(this NewLine newLine) => newLine switch
    {
        NewLine.CarriageReturnLineFeed => "\r\n",
        NewLine.LineFeed => "\n",
        NewLine.CarriageReturn => "\r",
        _ => string.Empty,
    };

    /// <summary>
    /// Performs the length operation.
    /// </summary>
    public static int Length(this NewLine newLine) => (int)newLine & 3;
}

