// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Renderers;

namespace Markdig.Extensions.Tables
{
    /// <summary>
    /// Extension that allows to use grid tables.
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class GridTableExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            if (!pipeline.BlockParsers.Contains<GridTableParser>())
            {
                pipeline.BlockParsers.Insert(0, new GridTableParser());
            }

            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<HtmlTableRenderer>())
            {
                htmlRenderer.ObjectRenderers.Add(new HtmlTableRenderer());
            }
        }
    }
}