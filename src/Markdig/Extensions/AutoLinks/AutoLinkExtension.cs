// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.AutoLinks
{
    /// <summary>
    /// Extension to automatically create <see cref="LinkInline"/> when a link url http: or mailto: is found.
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class AutoLinkExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<AutoLinkParser>())
            {
                // Insert the parser before any other parsers
                pipeline.InlineParsers.Insert(0, new AutoLinkParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }
    }
}