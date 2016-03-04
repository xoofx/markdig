// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Tables
{
    public class PiprTableDelimiterInline : DelimiterInline
    {
        public PiprTableDelimiterInline(InlineParser parser) : base(parser)
        {
        }

        public int LineIndex { get; set; }

        public override string ToLiteral()
        {
            return "|";
        }
    }
}