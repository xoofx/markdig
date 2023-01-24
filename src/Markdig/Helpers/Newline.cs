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
    None = 0,
    CarriageReturn = 4 | 1,
    LineFeed = 8 | 1,
    CarriageReturnLineFeed = 16 | 2
}

public static class NewLineExtensions
{
    public static string AsString(this NewLine newLine) => newLine switch
    {
        NewLine.CarriageReturnLineFeed => "\r\n",
        NewLine.LineFeed => "\n",
        NewLine.CarriageReturn => "\r",
        _ => string.Empty,
    };

    public static int Length(this NewLine newLine) => (int)newLine & 3;
}

