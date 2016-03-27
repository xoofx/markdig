// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html.Inlines;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Medias
{
    public class MediaOptions
    {
        public MediaOptions()
        {
            Width = 500;
            Height = 281;
            ExtensionToMimeType = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {".3gp", "video/3gpp"},
                {".3g2", "video/3gpp2"},
                {".avi", "video/x-msvideo"},
                {".uvh", "video/vnd.dece.hd"},
                {".uvm", "video/vnd.dece.mobile"},
                {".uvu", "video/vnd.uvvu.mp4"},
                {".uvp", "video/vnd.dece.pd"},
                {".uvs", "video/vnd.dece.sd"},
                {".uvv", "video/vnd.dece.video"},
                {".fvt", "video/vnd.fvt"},
                {".f4v", "video/x-f4v"},
                {".flv", "video/x-flv"},
                {".fli", "video/x-fli"},
                {".h261", "video/h261"},
                {".h263", "video/h263"},
                {".h264", "video/h264"},
                {".jpm", "video/jpm"},
                {".jpgv", "video/jpeg"},
                {".m4v", "video/x-m4v"},
                {".asf", "video/x-ms-asf"},
                {".pyv", "video/vnd.ms-playready.media.pyv"},
                {".wm", "video/x-ms-wm"},
                {".wmx", "video/x-ms-wmx"},
                {".wmv", "video/x-ms-wmv"},
                {".wvx", "video/x-ms-wvx"},
                {".mj2", "video/mj2"},
                {".mxu", "video/vnd.mpegurl"},
                {".mpeg", "video/mpeg"},
                {".mp4", "video/mp4"},
                {".ogv", "video/ogg"},
                {".webm", "video/webm"},
                {".qt", "video/quicktime"},
                {".movie", "video/x-sgi-movie"},
                {".viv", "video/vnd.vivo"},

                {".adp", "audio/adpcm"},
                {".aac", "audio/x-aac"},
                {".aif", "audio/x-aiff"},
                {".uva", "audio/vnd.dece.audio"},
                {".eol", "audio/vnd.digital-winds"},
                {".dra", "audio/vnd.dra"},
                {".dts", "audio/vnd.dts"},
                {".dtshd", "audio/vnd.dts.hd"},
                {".rip", "audio/vnd.rip"},
                {".lvp", "audio/vnd.lucent.voice"},
                {".m3u", "audio/x-mpegurl"},
                {".pya", "audio/vnd.ms-playready.media.pya"},
                {".wma", "audio/x-ms-wma"},
                {".wax", "audio/x-ms-wax"},
                {".mid", "audio/midi"},
                {".mpga", "audio/mpeg"},
                {".mp4a", "audio/mp4"},
                {".ecelp4800", "audio/vnd.nuera.ecelp4800"},
                {".ecelp7470", "audio/vnd.nuera.ecelp7470"},
                {".ecelp9600", "audio/vnd.nuera.ecelp9600"},
                {".oga", "audio/ogg"},
                {".weba", "audio/webm"},
                {".ram", "audio/x-pn-realaudio"},
                {".rmp", "audio/x-pn-realaudio-plugin"},
                {".au", "audio/basic"},
                {".wav", "audio/x-wav"},
            };
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public Dictionary<string, string> ExtensionToMimeType { get; }
    }



    /// <summary>
    /// Extension for extending image Markdown links in case a video or an audio file is linked and output proper link.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.IMarkdownExtension" />
    public class MediaExtension : IMarkdownExtension
    {
        public MediaExtension(MediaOptions options = null)
        {
            Options = options;
        }

        public MediaOptions Options { get; }

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
                    // TODO: change code for extensions with Options
                    var path = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);

                    var isMp3 = path.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase);
                    var isOgg = path.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase);
                    var isMp4 = path.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase);
                    var isWebM = path.EndsWith(".webm", StringComparison.OrdinalIgnoreCase);

                    if (uri.Host.StartsWith("www.youtube.com", StringComparison.OrdinalIgnoreCase))
                    {
                        var query = SplitQuery(uri);
                        if (query.Length > 0 && query[0].StartsWith("v="))
                        {
                            renderer.Write(
                                $"<iframe width=\"{Options.Width}\" height=\"{Options.Height}\" src=\"https://www.youtube.com/embed/{query[0].Substring(2)}\" frameborder=\"0\" allowfullscreen></iframe>");
                            return true;
                        }
                    }
                    else if (uri.Host.StartsWith("vimeo.com", StringComparison.OrdinalIgnoreCase))
                    {
                        var items = path.Split('/');
                        if (items.Length > 0)
                        {
                            renderer.Write(
                                $"<iframe width=\"{Options.Width}\" height=\"{Options.Height}\" src=\"https://player.vimeo.com/video/{items[items.Length - 1]}\" frameborder=\"0\" allowfullscreen></iframe>");
                            return true;
                        }
                    }
                    else if (isMp3 || isOgg)
                    {
                        var audioType = isMp3 ? "mp3" : "ogg";
                        renderer.Write($"<audio width=\"{Options.Width}\" controls class=\"audioplayer\"><source type=\"audio/{audioType}\" src=\"{linkInline.Url}\"></source></audio>");
                        return true;
                    }
                    else if (isMp4 || isWebM)
                    {
                        var videoType = isMp4 ? "mp4" : "webm";
                        renderer.Write($"<video width=\"{Options.Width}\" height=\"{Options.Height}\" controls class=\"videoplayer\"><source type=\"video/{videoType}\" src=\"{linkInline.Url}\"></source></video>");
                        return true;
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