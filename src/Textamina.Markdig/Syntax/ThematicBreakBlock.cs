// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a thematic break.
    /// </summary>
    public class ThematicBreakBlock : LeafBlock
    {
        public ThematicBreakBlock(BlockParser parser) : base(parser)
        {
        }
    }
}