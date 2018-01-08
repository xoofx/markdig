// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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
            if (!linkInline.IsImage || linkInline.Url == null)
            {
                return false;
            }

            Uri uri;
            // Only process absolute Uri
            if (!Uri.TryCreate(linkInline.Url, UriKind.RelativeOrAbsolute, out uri) || !uri.IsAbsoluteUri)
            {
                return false;
            }

            if (TryRenderIframeFromKnownProviders(uri, renderer, linkInline))
            {
                return true;
            }

            if (TryGuessAudioVideoFile(uri, renderer, linkInline))
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

        private bool TryGuessAudioVideoFile(Uri uri, HtmlRenderer renderer, LinkInline linkInline)
        {
            var path = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
            // Otherwise try to detect if we have an audio/video from the file extension
            string mimeType;
            var lastDot = path.LastIndexOf('.');
            if (lastDot >= 0 &&
                Options.ExtensionToMimeType.TryGetValue(path.Substring(lastDot), out mimeType))
            {
                var htmlAttributes = GetHtmlAttributes(linkInline);
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
            return false;
        }

        #region Known providers
        private static readonly Dictionary<string, Func<Uri, string>> KnownHosts = new Dictionary<string, Func<Uri, string>>()
        {
            {"www.youtube.com", YouTube},
            {"vimeo.com", Vimeo },
        };


        private bool TryRenderIframeFromKnownProviders(Uri uri, HtmlRenderer renderer, LinkInline linkInline)
        {
            var iFrameUrl = 
                KnownHosts
                    .Where(pair => uri.Host.StartsWith(pair.Key, StringComparison.OrdinalIgnoreCase))  // when host is match
                    .Select(pair => pair.Value(uri))                                                   // try to call delegate to get iframeUrl
                    .FirstOrDefault(iframeSrc => iframeSrc != null);                                   // use first success

            if (iFrameUrl == null)
            {
                return false;
            }

            var htmlAttributes = GetHtmlAttributes(linkInline);
            renderer.Write($"<iframe src=\"{iFrameUrl}\"");
            htmlAttributes.AddPropertyIfNotExist("width", Options.Width);
            htmlAttributes.AddPropertyIfNotExist("height", Options.Height);
            htmlAttributes.AddPropertyIfNotExist("frameborder", "0");
            htmlAttributes.AddPropertyIfNotExist("allowfullscreen", null);
            renderer.WriteAttributes(htmlAttributes);
            renderer.Write("></iframe>");

            return true;
        }

        private static readonly string[] SplitAnd = {"&"};
        private static string[] SplitQuery(Uri uri)
        {
            var query = uri.Query.Substring(uri.Query.IndexOf('?') + 1);
            return query.Split(SplitAnd, StringSplitOptions.RemoveEmptyEntries);
        }

        private static string YouTube(Uri uri)
        {
            var query = SplitQuery(uri);
            return query.Length > 0 && query[0].StartsWith("v=")
                ? $"https://www.youtube.com/embed/{query[0].Substring(2)}"
                : null;
        }

        private static string Vimeo(Uri uri)
        {
            var items = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).Split('/');
            return items.Length > 0 ? $"https://player.vimeo.com/video/{items[items.Length - 1]}" : null;
        }
        #endregion
    }
}