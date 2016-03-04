// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax.Inlines
{
    public class LinkDelimiterInline : DelimiterInline
    {
        public LinkDelimiterInline(InlineParser parser) : base(parser)
        {
        }

        public bool IsImage;

        public string Label;

        public override string ToLiteral()
        {
            return IsImage ? "![" : "[";
        }
    }
}