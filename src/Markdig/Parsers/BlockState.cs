// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Parsers;

/// <summary>
/// Defines the result of parsing a line for a <see cref="BlockParser"/>.
/// </summary>
public enum BlockState
{
    /// <summary>
    /// The parser does not accept the line for this block.
    /// No line content is consumed by this result.
    /// </summary>
    None,

    /// <summary>
    /// Skips this parser for the current line and continues with the next parser/candidate block.
    /// </summary>
    Skip,

    /// <summary>
    /// The parser accepts the line and keeps the block open.
    /// For leaf blocks, the current line is appended to the block.
    /// </summary>
    Continue,

    /// <summary>
    /// The parser accepts the line and keeps the block open, but the line is consumed and not appended.
    /// </summary>
    ContinueDiscard,

    /// <summary>
    /// The parser ends the block and keeps the current line available for further parsing.
    /// </summary>
    Break,

    /// <summary>
    /// The parser ends the block and consumes the current line.
    /// </summary>
    BreakDiscard
}
