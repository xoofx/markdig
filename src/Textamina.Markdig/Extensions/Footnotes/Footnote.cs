// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.Footnotes
{
    public class Footnote : ContainerBlock
    {
        public static readonly object DocumentKey = typeof (Footnote);

        public Footnote(BlockParser parser) : base(parser)
        {
            RemoveAfterProcessInlines = true;
            Links = new List<FootnoteLink>();
        }

        public string Label { get; set; }

        public int? Order { get; set; }

        public List<FootnoteLink> Links { get; private set; }

        internal bool IsLastLineEmpty { get; set; }
    }
}