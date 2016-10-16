// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Extensions.Footers;
using Markdig.Renderers;

namespace Markdig.Extensions.Figures
{
    /// <summary>
    /// Extension to allow usage of figures and figure captions.
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class FigureExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Contains<FigureBlockParser>())
            {
                // The Figure extension must come before the Footer extension
                if (pipeline.BlockParsers.Contains<FooterBlockParser>())
                {
                    pipeline.BlockParsers.InsertBefore<FooterBlockParser>(new FigureBlockParser());
                }
                else
                {
                    pipeline.BlockParsers.Insert(0, new FigureBlockParser());
                }
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready<HtmlFigureRenderer>();
                htmlRenderer.ObjectRenderers.AddIfNotAlready<HtmlFigureCaptionRenderer>();
            }
        }
    }
}