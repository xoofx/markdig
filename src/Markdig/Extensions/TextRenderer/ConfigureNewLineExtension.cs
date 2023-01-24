// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig.Extensions.TextRenderer;

/// <summary>
/// Extension that allows setting line-endings for any IMarkdownRenderer
/// that inherits from <see cref="TextRendererBase"/>
/// </summary>
/// <seealso cref="IMarkdownExtension" />
public class ConfigureNewLineExtension : IMarkdownExtension
{
    private readonly string newLine;

    public ConfigureNewLineExtension(string newLine)
    {
        this.newLine = newLine;
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is TextRendererBase textRenderer)
        {
            textRenderer.Writer.NewLine = newLine;
        }
    }
}
