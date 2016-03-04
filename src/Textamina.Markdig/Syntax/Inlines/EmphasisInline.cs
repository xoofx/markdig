// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Parsers.Inlines;

namespace Textamina.Markdig.Syntax.Inlines
{
    public class EmphasisInline : ContainerInline
    {
        public char DelimiterChar { get; set; }

        public bool Strong { get; set; }

        public override string ToString()
        {
            return Strong ? "<strong>" : "<em>";
        }
    }
}