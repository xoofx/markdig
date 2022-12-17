// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using System;

namespace Markdig.Extensions.Footnotes
{
    /// <summary>
    /// Extension to allow footnotes.
    /// </summary>
    /// <seealso cref="IMarkdownExtension" />
    public class FootnoteExtension : IMarkdownExtension
    {
        public FootnoteExtension()
        {
            Options = new FootnoteOptions();
        }

        public FootnoteExtension(FootnoteOptions options)
        {
            Options = options;
        }

        public FootnoteOptions Options { get; private set; }

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
            if (renderer is HtmlRenderer htmlRenderer)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready(new HtmlFootnoteGroupRenderer(Options));
                htmlRenderer.ObjectRenderers.AddIfNotAlready(new HtmlFootnoteLinkRenderer(Options));
            }
        }
    }
}
