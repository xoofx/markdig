// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// Repressents a thematic break (Section 4.1 CommonMark specs).
    /// </summary>
    public class ThematicBreakBlock : LeafBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThematicBreakBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public ThematicBreakBlock(BlockParser parser) : base(parser)
        {
        }
    }
}