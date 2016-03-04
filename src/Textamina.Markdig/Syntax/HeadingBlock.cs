// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Diagnostics;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a thematic break.
    /// </summary>
    [DebuggerDisplay("{GetType().Name} Line: {Line}, {Lines} Level: {Level}")]
    public class HeadingBlock : LeafBlock
    {
        public HeadingBlock(BlockParser parser) : base(parser)
        {
            ProcessInlines = true;
        }

        public int Level { get; set; }
    }
}