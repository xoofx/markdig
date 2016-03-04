// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    public class ListBlock : ContainerBlock
    {
        public ListBlock(BlockParser parser) : base(parser)
        {
        }

        public bool IsOrdered { get; set; }

        public char BulletChar { get; set; }

        public int OrderedStart { get; set; }

        public char OrderedDelimiter { get; set; }

        public bool IsLoose { get; set; }

        internal int CountAllBlankLines { get; set; }

        internal int CountBlankLinesReset { get; set; }
    }
}