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

        private class KnownProvider
        {
            public string HostPrefix { get; set; }
            public Func<Uri, string> Delegate { get; set; }
            public bool AllowFullScreen { get; set; } = true; //Should be false for audio embedding
        }

        private static readonly List<KnownProvider> KnownHosts = new List<KnownProvider>()
        {
            new KnownProvider {HostPrefix = "www.youtube.com", Delegate = YouTube},
            new KnownProvider {HostPrefix = "vimeo.com", Delegate = Vimeo},
            new KnownProvider {HostPrefix = "music.yandex.ru", Delegate = Yandex, AllowFullScreen = false},
            new KnownProvider {HostPrefix = "ok.ru", Delegate = Odnoklassniki},
        };


        private bool TryRenderIframeFromKnownProviders(Uri uri, HtmlRenderer renderer, LinkInline linkInline)
        {
            var foundProvider = 
                KnownHosts
                    .Where(pair => uri.Host.StartsWith(pair.HostPrefix, StringComparison.OrdinalIgnoreCase))  // when host is match
                    .Select(provider =>
                        new
                        {
                            provider.AllowFullScreen,
                            Result = provider.Delegate(uri) // try to call delegate to get iframeUrl
                        }
                        )
                    .FirstOrDefault(provider => provider.Result != null);                                   // use first success

            if (foundProvider == null)
            {
                return false;
            }

            var htmlAttributes = GetHtmlAttributes(linkInline);
            renderer.Write($"<iframe src=\"{foundProvider.Result}\"");

            if(!string.IsNullOrEmpty(Options.Width))
                htmlAttributes.AddPropertyIfNotExist("width", Options.Width);

            if (!string.IsNullOrEmpty(Options.Height))
                htmlAttributes.AddPropertyIfNotExist("height", Options.Height);

            if (!string.IsNullOrEmpty(Options.Class))
                htmlAttributes.AddPropertyIfNotExist("class", Options.Class);

            htmlAttributes.AddPropertyIfNotExist("frameborder", "0");
            if (foundProvider.AllowFullScreen)
            {
                htmlAttributes.AddPropertyIfNotExist("allowfullscreen", null);
            }
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

        private static string Odnoklassniki(Uri uri)
        {
            var items = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).Split('/');
            return items.Length > 0 ? $"https://ok.ru/videoembed/{items[items.Length - 1]}" : null;
        }

        private static string Yandex(Uri uri)
        {
            var items = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).Split('/');
            var albumKeyword
                = items.Skip(0).FirstOrDefault();
            var albumId
                = items.Skip(1).FirstOrDefault();
            var trackKeyword
                = items.Skip(2).FirstOrDefault();
            var trackId
                = items.Skip(3).FirstOrDefault();

            if (albumKeyword != "album" || albumId == null || trackKeyword != "track" || trackId == null)
            {
                return null;
            }

            return $"https://music.yandex.ru/iframe/#track/{trackId}/{albumId}/";
        }
        #endregion
    }
}