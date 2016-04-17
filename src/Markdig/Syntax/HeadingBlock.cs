// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Diagnostics;
using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// Repressents a heading.
    /// </summary>
    [DebuggerDisplay("{GetType().Name} Line: {Line}, {Lines} Level: {Level}")]
    public class HeadingBlock : LeafBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeadingBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public HeadingBlock(BlockParser parser) : base(parser)
        {
            ProcessInlines = true;
        }

        /// <summary>
        /// Gets or sets the header character used to defines this heading (usually #)
        /// </summary>
        public char HeaderChar { get; set; }

        /// <summary>
        /// Gets or sets the level of heading (starting at 1 for the lowest level).
        /// </summary>
        public int Level { get; set; }
    }
}