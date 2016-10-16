// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Extensions.Figures;
using Markdig.Renderers;

namespace Markdig.Extensions.Footers
{
    /// <summary>
    /// Extension that provides footer.
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class FooterExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Contains<FooterBlockParser>())
            {
                // The Figure extension must come before the Footer extension
                if (pipeline.BlockParsers.Contains<FigureBlockParser>())
                {
                    pipeline.BlockParsers.InsertAfter<FigureBlockParser>(new FooterBlockParser());
                }
                else
                {
                    pipeline.BlockParsers.Insert(0, new FooterBlockParser());
                }
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready(new HtmlFooterBlockRenderer());
            }
        }
    }
}