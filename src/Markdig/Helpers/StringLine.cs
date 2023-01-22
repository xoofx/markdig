// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Helpers;

/// <summary>
/// A struct representing a text line.
/// </summary>
public struct StringLine
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringLine"/> struct.
    /// </summary>
    /// <param name="slice">The slice.</param>
    public StringLine(ref StringSlice slice) : this()
    {
        Slice = slice;
        NewLine = slice.NewLine;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringLine"/> struct.
    /// </summary>
    /// <param name="slice">The slice.</param>
    /// <param name="line">The line.</param>
    /// <param name="column">The column.</param>
    /// <param name="position">The position.</param>
    /// <param name="newLine">The line separation.</param>
    public StringLine(StringSlice slice, int line, int column, int position, NewLine newLine)
    {
        Slice = slice;
        Line = line;
        Column = column;
        Position = position;
        NewLine = newLine;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringLine"/> struct.
    /// </summary>
    /// <param name="slice">The slice.</param>
    /// <param name="line">The line.</param>
    /// <param name="column">The column.</param>
    /// <param name="position">The position.</param>
    /// <param name="newLine">The line separation.</param>
    public StringLine(ref StringSlice slice, int line, int column, int position, NewLine newLine)
    {
        Slice = slice;
        Line = line;
        Column = column;
        Position = position;
        NewLine = newLine;
    }

    /// <summary>
    /// The slice used for this line.
    /// </summary>
    public StringSlice Slice;

    /// <summary>
    /// The line position.
    /// </summary>
    public int Line;

    /// <summary>
    /// The position of the start of this line within the original source code
    /// </summary>
    public int Position;

    /// <summary>
    /// The column position.
    /// </summary>
    public int Column;

    /// <summary>
    /// The newline.
    /// </summary>
    public NewLine NewLine;

    /// <summary>
    /// Performs an implicit conversion from <see cref="StringLine"/> to <see cref="StringSlice"/>.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator StringSlice(StringLine line)
    {
        return line.Slice;
    }

    public readonly override string ToString()
    {
        return Slice.ToString();
    }
}