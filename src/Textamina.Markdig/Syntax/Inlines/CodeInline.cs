// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Parsers.Inlines;

namespace Textamina.Markdig.Syntax.Inlines
{
    public class CodeInline : LeafInline
    {
        public string Content { get; set; }
    }
}