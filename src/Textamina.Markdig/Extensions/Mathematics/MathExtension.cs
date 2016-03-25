// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers.Inlines;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html.Inlines;

namespace Textamina.Markdig.Extensions.Mathematics
{
    /// <summary>
    /// Extension for adding inline mathematics $...$
    /// </summary>
    /// <seealso cref="Textamina.Markdig.IMarkdownExtension" />
    public class MathExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            // Adds the inline parser
            if (!pipeline.InlineParsers.Contains<MathInlineParser>())
            {
                // Insert before EmphasisInlineParser to take precedence
                pipeline.InlineParsers.InsertBefore<EmphasisInlineParser>(new MathInlineParser());
            }
            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<HtmlMathInlineRenderer>())
                {
                    // Insert before EmphasisInlineRenderer to take precedence
                    htmlRenderer.ObjectRenderers.InsertBefore<EmphasisInlineRenderer>(new HtmlMathInlineRenderer());
                }
            }
        }
    }
}