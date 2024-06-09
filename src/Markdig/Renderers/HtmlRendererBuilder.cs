// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.IO;

namespace Markdig.Renderers;

public class HtmlRendererBuilder : IMarkdownRendererBuilder
{
    public Uri? BaseUrl { get; private set; }

    public HtmlRenderer Build(TextWriter writer)
    {
        HtmlRenderer htmlRenderer = new HtmlRenderer(writer);

        if (BaseUrl != null) htmlRenderer.BaseUrl = BaseUrl;

        return htmlRenderer;
    }

    TextRendererBase IMarkdownRendererBuilder.Build(TextWriter writer) => Build(writer);

    public HtmlRendererBuilder UseBaseUrl(string baseUrl)
    {
        BaseUrl = baseUrl != null ? new Uri(baseUrl) : null;
        return this;
    }

    public HtmlRendererBuilder UseBaseUrl(Uri baseUrl)
    {
        BaseUrl = baseUrl;
        return this;
    }
}
