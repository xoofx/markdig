// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html.Inlines;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Medias
{
    /// <summary>
    /// Extension for extending image Markdown links in case a video or an audio file is linked and output proper link.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.IMarkdownExtension" />
    public class MediaExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
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
                if (Uri.TryCreate(linkInline.Url, UriKind.RelativeOrAbsolute, out uri))
                {
                    if (uri.Host.StartsWith("www.youtube.com", StringComparison.OrdinalIgnoreCase))
                    {
                        var query = SplitQuery(uri);
                        if (query.Length > 0 && query[0].StartsWith("v="))
                        {
                            renderer.Write(
                                $"<iframe width=\"420\" height=\"315\" src=\"https://www.youtube.com/embed/{query[0].Substring(2)}\" frameborder=\"0\" allowfullscreen></iframe>");
                            return true;
                        }
                    }
                    else if (uri.Host.StartsWith("vimeo.com", StringComparison.OrdinalIgnoreCase))
                    {
                        var path = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
                        var items = path.Split('/');
                        if (items.Length > 0)
                        {
                            renderer.Write(
                                $"<iframe width=\"500\" height=\"281\" src=\"https://player.vimeo.com/video/{items[items.Length - 1]}\" frameborder=\"0\" allowfullscreen></iframe>");
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static readonly string[] splitAnd = {"&"};
        private static string[] SplitQuery(Uri uri)
        {
            var query = uri.Query.Substring(uri.Query.IndexOf('?') + 1);
            return query.Split(splitAnd, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}