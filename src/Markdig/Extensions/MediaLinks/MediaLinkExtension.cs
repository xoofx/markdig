// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.MediaLinks
{
    /// <summary>
    /// Extension for extending image Markdown links in case a video or an audio file is linked and output proper link.
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class MediaLinkExtension : IMarkdownExtension
    {
        public MediaLinkExtension() : this(new MediaOptions())
        {
        }

        public MediaLinkExtension(MediaOptions options)
        {
            Options = options ?? new MediaOptions();
        }

        public MediaOptions Options { get; }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
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
            if (linkInline.IsImage && linkInline.Url != null)
            {
                Uri uri;
                // Only process absolute Uri
                if (Uri.TryCreate(linkInline.Url, UriKind.RelativeOrAbsolute, out uri) && uri.IsAbsoluteUri)
                {
                    var htmlAttributes = new HtmlAttributes();
                    var fromAttributes = linkInline.TryGetAttributes();
                    if (fromAttributes != null)
                    {
                        fromAttributes.CopyTo(htmlAttributes, false, false);
                    }

                    // TODO: this code is not pluggable, so for now, we handle only the following web providers:
                    // - youtube
                    // - vimeo
                    var path = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);

                    string iFrameUrl = null;
                    if (uri.Host.StartsWith("www.youtube.com", StringComparison.OrdinalIgnoreCase))
                    {
                        var query = SplitQuery(uri);
                        if (query.Length > 0 && query[0].StartsWith("v="))
                        {
                            iFrameUrl = $"https://www.youtube.com/embed/{query[0].Substring(2)}";
                        }
                    }
                    else if (uri.Host.StartsWith("vimeo.com", StringComparison.OrdinalIgnoreCase))
                    {
                        var items = path.Split('/');
                        if (items.Length > 0)
                        {
                            iFrameUrl = $"https://player.vimeo.com/video/{items[items.Length - 1]}";
                        }
                    }

                    if (iFrameUrl != null)
                    {
                        renderer.Write($"<iframe src=\"{iFrameUrl}\"");
                        htmlAttributes.AddPropertyIfNotExist("width", Options.Width);
                        htmlAttributes.AddPropertyIfNotExist("height", Options.Height);
                        htmlAttributes.AddPropertyIfNotExist("frameborder", "0");
                        htmlAttributes.AddPropertyIfNotExist("allowfullscreen", null);
                        renderer.WriteAttributes(htmlAttributes);
                        renderer.Write("></iframe>");
                        return true;
                    }
                    else
                    {
                        // Otherwise try to detect if we have an audio/video from the file extension
                        string mimeType;
                        var lastDot = path.LastIndexOf('.');
                        if (lastDot >= 0 &&
                            Options.ExtensionToMimeType.TryGetValue(path.Substring(lastDot), out mimeType))
                        {
                            var isAudio = mimeType.StartsWith("audio");
                            var tagType = isAudio ? "audio" : "video";

                            renderer.Write($"<{tagType}");
                            htmlAttributes.AddPropertyIfNotExist("width", Options.Width);
                            if (!isAudio)
                            {
                                htmlAttributes.AddPropertyIfNotExist("height", Options.Height);
                            }
                            htmlAttributes.AddPropertyIfNotExist("controls", null);
                            renderer.WriteAttributes(htmlAttributes);

                            renderer.Write($"><source type=\"{mimeType}\" src=\"{linkInline.Url}\"></source></{tagType}>");

                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static readonly string[] SplitAnd = {"&"};
        private static string[] SplitQuery(Uri uri)
        {
            var query = uri.Query.Substring(uri.Query.IndexOf('?') + 1);
            return query.Split(SplitAnd, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}