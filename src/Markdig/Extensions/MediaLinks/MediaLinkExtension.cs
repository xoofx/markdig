// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.MediaLinks;

/// <summary>
/// Extension for extending image Markdown links in case a video or an audio file is linked and output proper link.
/// </summary>
/// <seealso cref="IMarkdownExtension" />
public class MediaLinkExtension : IMarkdownExtension
{
    public MediaLinkExtension() : this(new MediaOptions())
    {
    }

    public MediaLinkExtension(MediaOptions? options)
    {
        Options = options ?? new MediaOptions();
    }

    public MediaOptions Options { get; }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer)
        {
            var inlineRenderer = htmlRenderer.ObjectRenderers.FindExact<LinkInlineRenderer>();
            if (inlineRenderer != null)
            {
                inlineRenderer.TryWriters.Remove(TryLinkInlineRenderer);
                inlineRenderer.TryWriters.Add(TryLinkInlineRenderer);
            }
        }
    }

    private bool TryLinkInlineRenderer(HtmlRenderer renderer, LinkInline linkInline)
    {
        if (!linkInline.IsImage || linkInline.Url is null)
        {
            return false;
        }

        var url = linkInline.Url;
        bool isSchemaRelative = false;

        // force // schema to an absolute url
        if (url.StartsWith("//", StringComparison.Ordinal))
        {
            url = "https:" + url;
            isSchemaRelative = true;
        }

        // Make sure we have a valid absolute/relative url
        if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri? uri)) // || !uri.IsAbsoluteUri)
        {
            return false;
        }
        
        // iFrame has to be absolute path
        if (uri.IsAbsoluteUri)
        {
            if (TryRenderIframeFromKnownProviders(uri, isSchemaRelative, renderer, linkInline))
            {
                return true;
            }
        }

        // audio/video has can have relative path
        if (TryGuessAudioVideoFile(uri, isSchemaRelative, renderer, linkInline))
        {
            return true;
        }

        return false;
    }

    private static HtmlAttributes GetHtmlAttributes(LinkInline linkInline)
    {
        var htmlAttributes = new HtmlAttributes();
        var fromAttributes = linkInline.TryGetAttributes();
        if (fromAttributes != null)
        {
            fromAttributes.CopyTo(htmlAttributes, false, false);
        }

        return htmlAttributes;
    }

    private bool TryGuessAudioVideoFile(Uri uri, bool isSchemaRelative, HtmlRenderer renderer, LinkInline linkInline)
    {
        string path = uri.IsAbsoluteUri
            ? uri.GetComponents(UriComponents.Path, UriFormat.Unescaped)
            : uri.ToString();
        
        // Otherwise try to detect if we have an audio/video from the file extension
        var lastDot = path.LastIndexOf('.');
        if (lastDot >= 0 &&
            Options.ExtensionToMimeType.TryGetValue(path.Substring(lastDot), out string? mimeType))
        {
            var htmlAttributes = GetHtmlAttributes(linkInline);
            var isAudio = mimeType.StartsWith("audio", StringComparison.Ordinal);
            var tagType = isAudio ? "audio" : "video";

            renderer.Write($"<{tagType}");
            htmlAttributes.AddPropertyIfNotExist("width", Options.Width);
            if (!isAudio)
            {
                htmlAttributes.AddPropertyIfNotExist("height", Options.Height);
            }
            if (Options.AddControlsProperty)
            {
                htmlAttributes.AddPropertyIfNotExist("controls", null);
            }

            if (!string.IsNullOrEmpty(Options.Class))
                htmlAttributes.AddClass(Options.Class);

            renderer.WriteAttributes(htmlAttributes);

            renderer.Write($"><source type=\"{mimeType}\" src=\"{linkInline.Url}\"></source></{tagType}>");

            return true;
        }
        return false;
    }

    private bool TryRenderIframeFromKnownProviders(Uri uri, bool isSchemaRelative, HtmlRenderer renderer, LinkInline linkInline)
    {
        IHostProvider? foundProvider = null;
        string? iframeUrl = null;
        foreach (var provider in Options.Hosts)
        {
            if (!provider.TryHandle(uri, isSchemaRelative, out iframeUrl))
                continue;
            foundProvider = provider;
            break;
        }

        if (foundProvider is null)
        {
            return false;
        }

        var htmlAttributes = GetHtmlAttributes(linkInline);
        renderer.Write("<iframe src=\"");
        renderer.WriteEscapeUrl(iframeUrl);
        renderer.Write('"');

        if (!string.IsNullOrEmpty(Options.Width))
            htmlAttributes.AddPropertyIfNotExist("width", Options.Width);

        if (!string.IsNullOrEmpty(Options.Height))
            htmlAttributes.AddPropertyIfNotExist("height", Options.Height);

        if (!string.IsNullOrEmpty(Options.Class))
            htmlAttributes.AddClass(Options.Class);

        if (foundProvider.Class is { Length: > 0 } className)
            htmlAttributes.AddClass(className);

        htmlAttributes.AddPropertyIfNotExist("frameborder", "0");
        if (foundProvider.AllowFullScreen)
        {
            htmlAttributes.AddPropertyIfNotExist("allowfullscreen", null);
        }
        renderer.WriteAttributes(htmlAttributes);
        renderer.Write("></iframe>");

        return true;
    }
}
