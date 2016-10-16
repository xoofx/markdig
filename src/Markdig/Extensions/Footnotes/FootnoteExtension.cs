// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Renderers;

namespace Markdig.Extensions.Footnotes
{
    /// <summary>
    /// Extension to allow footnotes.
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class FootnoteExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Contains<FootnoteParser>())
            {
                // Insert the parser before any other parsers
                pipeline.BlockParsers.Insert(0, new FootnoteParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready(new HtmlFootnoteGroupRenderer());
                htmlRenderer.ObjectRenderers.AddIfNotAlready(new HtmlFootnoteLinkRenderer());
            }
        }
    }
}