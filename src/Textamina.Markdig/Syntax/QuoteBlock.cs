// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    public class QuoteBlock : ContainerBlock
    {
        public QuoteBlock(BlockParser parser) : base(parser)
        {
        }

        public char QuoteChar { get; set; }
    }
}