// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers.Inlines;
using Textamina.Markdig.Renderers;

namespace Textamina.Markdig.Extensions.Tables
{
    public class PipeTableExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            if (!pipeline.InlineParsers.Contains<PipeTableInlineParser>())
            {
                pipeline.InlineParsers.InsertBefore<EmphasisInlineParser>(new PipeTableInlineParser());
            }

            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<HtmlTableRenderer>())
            {
                htmlRenderer.ObjectRenderers.Add(new HtmlTableRenderer());
            }
        }
    }
}