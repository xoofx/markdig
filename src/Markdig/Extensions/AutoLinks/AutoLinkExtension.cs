// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Normalize;
using Markdig.Renderers.Normalize.Inlines;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.AutoLinks
{
    /// <summary>
    /// Extension to automatically create <see cref="LinkInline"/> when a link url http: or mailto: is found.
    /// </summary>
    /// <seealso cref="IMarkdownExtension" />
    public class AutoLinkExtension : IMarkdownExtension
    {
        public readonly AutoLinkOptions Options;

        public AutoLinkExtension(AutoLinkOptions options)
        {
            Options = options ?? new AutoLinkOptions();
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<AutoLinkParser>())
            {
                // Insert the parser before any other parsers
                pipeline.InlineParsers.Insert(0, new AutoLinkParser(Options));
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is NormalizeRenderer normalizeRenderer && !normalizeRenderer.ObjectRenderers.Contains<NormalizeAutoLinkRenderer>())
            {
                normalizeRenderer.ObjectRenderers.InsertBefore<LinkInlineRenderer>(new NormalizeAutoLinkRenderer());
            }
        }
    }
}