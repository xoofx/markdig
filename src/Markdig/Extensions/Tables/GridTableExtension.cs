// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig.Extensions.Tables;

/// <summary>
/// Extension that allows to use grid tables.
/// </summary>
/// <seealso cref="IMarkdownExtension" />
public class GridTableExtension : IMarkdownExtension
{
    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.BlockParsers.Contains<GridTableParser>())
        {
            pipeline.BlockParsers.Insert(0, new GridTableParser());
        }
    }

    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer && !htmlRenderer.ObjectRenderers.Contains<HtmlTableRenderer>())
        {
            htmlRenderer.ObjectRenderers.Add(new HtmlTableRenderer());
        }
    }
}
