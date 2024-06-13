// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.IO;

namespace Markdig.Renderers;

/// <summary>
/// This class is used with <see cref="MarkdownExtensions.ConfigureHtmlRenderer"/>
/// to set up a pipeline with a html renderer.
/// </summary>
public class HtmlRendererBuilder : IMarkdownRendererBuilder
{
    private Uri? baseUrl;
    private bool? enableHtmlEscape;
    private bool? enableHtmlForBlock;
    private bool? enableHtmlForInline;
    private bool? useNonAsciiNoEscape;

    public HtmlRenderer Build(TextWriter writer)
    {
        HtmlRenderer htmlRenderer = new HtmlRenderer(writer);

        if (baseUrl != null) htmlRenderer.BaseUrl = baseUrl;
        if (enableHtmlEscape != null) htmlRenderer.EnableHtmlEscape = enableHtmlEscape.Value;
        if (enableHtmlForBlock != null) htmlRenderer.EnableHtmlForBlock = enableHtmlForBlock.Value;
        if (enableHtmlForInline != null) htmlRenderer.EnableHtmlForInline = enableHtmlForInline.Value;
        if (useNonAsciiNoEscape != null) htmlRenderer.UseNonAsciiNoEscape = useNonAsciiNoEscape.Value;

        return htmlRenderer;
    }

    TextRendererBase IMarkdownRendererBuilder.Build(TextWriter writer) => Build(writer);

    /// <inheritdoc cref="HtmlRenderer.BaseUrl"/>
    public HtmlRendererBuilder UseBaseUrl(string baseUrl)
    {
        this.baseUrl = baseUrl != null ? new Uri(baseUrl) : null;
        return this;
    }

    /// <inheritdoc cref="HtmlRenderer.BaseUrl"/>
    public HtmlRendererBuilder UseBaseUrl(Uri baseUrl)
    {
        this.baseUrl = baseUrl;
        return this;
    }

    /// <inheritdoc cref="HtmlRenderer.EnableHtmlEscape"/>
    public HtmlRendererBuilder EnableHtmlEscape(bool enable)
    {
        enableHtmlEscape = enable;
        return this;
    }

    /// <inheritdoc cref="HtmlRenderer.EnableHtmlForBlock"/>
    public HtmlRendererBuilder EnableHtmlForBlock(bool enable)
    {
        enableHtmlForBlock = enable;
        return this;
    }

    /// <inheritdoc cref="HtmlRenderer.EnableHtmlForInline"/>
    public HtmlRendererBuilder EnableHtmlForInline(bool enable)
    {
        enableHtmlForInline = enable;
        return this;
    }

    /// <inheritdoc cref="HtmlRenderer.UseNonAsciiNoEscape"/>
    public HtmlRendererBuilder UseNonAsciiNoEscape(bool enable)
    {
        useNonAsciiNoEscape = enable;
        return this;
    }
}
