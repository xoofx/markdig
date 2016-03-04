// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Footnotes
{
    public class FootnoteLink : Inline
    {
        public bool IsBackLink { get; set; }

        public int Index { get; set; }

        public Footnote Footnote { get; set; }
    }
}