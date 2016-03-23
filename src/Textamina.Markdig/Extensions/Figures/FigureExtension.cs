// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Renderers;

namespace Textamina.Markdig.Extensions.Figures
{
    /// <summary>
    /// Extension to allow usage of figures and figure captions.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.IMarkdownExtension" />
    public class FigureExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            pipeline.BlockParsers.AddIfNotAlready<FigureBlockParser>();
            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready<HtmlFigureRenderer>();
                htmlRenderer.ObjectRenderers.AddIfNotAlready<HtmlFigureCaptionRenderer>();
            }
        }
    }
}