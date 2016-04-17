// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig.Extensions.Abbreviations
{
    /// <summary>
    /// Extension to allow abbreviations.
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class AbbreviationExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            pipeline.BlockParsers.AddIfNotAlready<AbbreviationParser>();

            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<HtmlAbbreviationRenderer>())
                {
                    // Must be inserted before CodeBlockRenderer
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlAbbreviationRenderer());
                }
            }
        }
    }
}