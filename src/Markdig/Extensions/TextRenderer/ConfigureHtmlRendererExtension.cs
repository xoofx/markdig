// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Renderers;

namespace Markdig.Extensions.TextRenderer;

/// <summary>
/// Extension that calls an action with every <see cref="HtmlRenderer"/> set up by the pipeline,
/// including the renderers created by <see cref="Markdown.ToHtml(string, MarkdownPipeline?, MarkdownParserContext?)"/>.
/// </summary>
/// <seealso cref="IMarkdownExtension" />
public sealed class ConfigureHtmlRendererExtension : IMarkdownExtension
{
    private readonly Action<HtmlRenderer> _configure;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureHtmlRendererExtension"/> class.
    /// </summary>
    /// <param name="configure">The action called with each <see cref="HtmlRenderer"/> when the pipeline sets it up.</param>
    /// <exception cref="ArgumentNullException"><paramref name="configure"/> is <c>null</c>.</exception>
    public ConfigureHtmlRendererExtension(Action<HtmlRenderer> configure)
    {
        if (configure is null) ThrowHelper.ArgumentNullException(nameof(configure));

        _configure = configure;
    }

    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
    }

    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer)
        {
            _configure(htmlRenderer);
        }
    }
}
