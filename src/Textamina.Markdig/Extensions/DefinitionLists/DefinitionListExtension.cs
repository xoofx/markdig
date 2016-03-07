// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Renderers;

namespace Textamina.Markdig.Extensions.DefinitionLists
{
    /// <summary>
    /// Extension to allow definition lists
    /// </summary>
    /// <seealso cref="Textamina.Markdig.IMarkdownExtension" />
    public class DefinitionListExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            if (!pipeline.BlockParsers.Contains<DefinitionListParser>())
            {
                // Insert the parser before any other parsers
                pipeline.BlockParsers.Insert(0, new DefinitionListParser());
            }

            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
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