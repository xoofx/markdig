// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Extensions.Figures;
using Textamina.Markdig.Renderers;

namespace Textamina.Markdig.Extensions.Footers
{
    /// <summary>
    /// Extension that provides footer.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.IMarkdownExtension" />
    public class FooterExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
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

            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready(new HtmlFooterBlockRenderer());
            }
        }
    }
}