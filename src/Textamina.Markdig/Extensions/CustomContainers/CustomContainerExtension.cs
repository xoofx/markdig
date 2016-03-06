// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Renderers;

namespace Textamina.Markdig.Extensions.CustomContainers
{
    /// <summary>
    /// Extension to allow custom containers.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.IMarkdownExtension" />
    public class CustomContainerExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            if (!pipeline.BlockParsers.Contains<CustomContainerParser>())
            {
                // Insert the parser before any other parsers
                pipeline.BlockParsers.Insert(0, new CustomContainerParser());
            }

            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<HtmlCustomContainerRenderer>())
                {
                    // Must be inserted before CodeBlockRenderer
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlCustomContainerRenderer());
                }
            }
        }
    }
}