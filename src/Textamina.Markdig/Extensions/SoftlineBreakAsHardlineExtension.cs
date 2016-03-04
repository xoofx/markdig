// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using Textamina.Markdig.Extensions.Footnotes;
using Textamina.Markdig.Parsers.Inlines;

namespace Textamina.Markdig.Extensions
{
    public class SoftlineBreakAsHardlineExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            var parser = pipeline.InlineParsers.Find<LineBreakInlineParser>();
            if (parser != null)
            {
                parser.EnableSoftAsHard = true;
            }
        }
    }
}