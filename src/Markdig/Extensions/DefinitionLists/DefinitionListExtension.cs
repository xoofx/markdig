// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Renderers;

namespace Markdig.Extensions.DefinitionLists
{
    /// <summary>
    /// Extension to allow definition lists
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class DefinitionListExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Contains<DefinitionListParser>())
            {
                // Insert the parser before any other parsers
                pipeline.BlockParsers.Insert(0, new DefinitionListParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<HtmlDefinitionListRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlDefinitionListRenderer());
                }
            }
        }
    }
}