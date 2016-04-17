// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Parsers
{
    /// <summary>
    /// Defines the result of parsing a line for a <see cref="BlockParser"/>.
    /// </summary>
    public enum BlockState
    {
        /// <summary>
        /// A line is not accepted by this parser.
        /// </summary>
        None,

        /// <summary>
        /// The parser is skipped.
        /// </summary>
        Skip,

        /// <summary>
        /// The parser accepts a line and instruct to continue.
        /// </summary>
        Continue,

        /// <summary>
        /// The parser accepts a line, instruct to continue but discard the line (not stored on the block)
        /// </summary>
        ContinueDiscard,

        /// <summary>
        /// The parser is ending a block, instruct to stop and keep the line being processed.
        /// </summary>
        Break,

        /// <summary>
        /// The parser is ending a block, instruct to stop and discard the line being processed.
        /// </summary>
        BreakDiscard
    }
}