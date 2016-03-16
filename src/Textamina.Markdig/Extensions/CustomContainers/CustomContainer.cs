// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.CustomContainers
{
    public class CustomContainer : ContainerBlock, IFencedBlock
    {
        public CustomContainer(BlockParser parser) : base(parser)
        {
        }

        public string Info { get; set; }

        public string Arguments { get; set; }

        public int FencedCharCount { get; set; }

        public char FencedChar { get; set; }
    }
}