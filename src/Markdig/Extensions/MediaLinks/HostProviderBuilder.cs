// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Markdig.Extensions.MediaLinks
{
    public class HostProviderBuilder
    {
        private sealed class DelegateProvider : IHostProvider
        {
            public string HostPrefix { get; set; }
            public Func<Uri, string> Delegate { get; set; }
            public bool AllowFullScreen { get; set; } = true;
            public string Class { get; set; }

            public bool TryHandle(Uri mediaUri, bool isSchemaRelative, out string iframeUrl)
            {
                if (!mediaUri.Host.StartsWith(HostPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    iframeUrl = null;
                    return false;
                }
                iframeUrl = Delegate(mediaUri);
                return !string.IsNullOrEmpty(iframeUrl);
            }
        }

        /// <summary>
        /// Create a <see cref="IHostProvider"/> with delegate handler.
        /// </summary>
        /// <param name="hostPrefix">Prefix of host that can be handled.</param>
        /// <param name="handler">Handler that generate iframe url, if uri cannot be handled, it can return <see langword="null"/>.</param>
        /// <param name="allowFullScreen">Should the generated iframe has allowfullscreen attribute.</param>
        /// <param name="iframeClass">"class" attribute of generated iframe.</param>
        /// <returns>A <see cref="IHostProvider"/> with delegate handler.</returns>
        public static IHostProvider Create(string hostPrefix, Func<Uri, string> handler, bool allowFullScreen = true, string iframeClass = null)
        {
            if (string.IsNullOrEmpty(hostPrefix))
                ThrowHelper.ArgumentException("hostPrefix is null or empty.", nameof(hostPrefix));
            if (handler == null)
                ThrowHelper.ArgumentNullException(nameof(handler));

            return new DelegateProvider { HostPrefix = hostPrefix, Delegate = handler, AllowFullScreen = allowFullScreen, Class = iframeClass };
        }

        internal static Dictionary<string, IHostProvider> KnownHosts { get; }
            = new Dictionary<string, IHostProvider>(StringComparer.OrdinalIgnoreCase)
            {
                ["YouTube"] = Create("www.youtube.com", YouTube, iframeClass: "youtube"),
                ["YouTubeShortened"] = Create("youtu.be", YouTubeShortened, iframeClass: "youtube"),
                ["Vimeo"] = Create("vimeo.com", Vimeo, iframeClass: "vimeo"),
                ["Yandex"] = Create("music.yandex.ru", Yandex, allowFullScreen: false, iframeClass: "yandex"),
                ["Odnoklassniki"] = Create("ok.ru", Odnoklassniki, iframeClass: "odnoklassniki"),
            };

        #region Known providers

        private static readonly string[] SplitAnd = { "&" };
        private static string[] SplitQuery(Uri uri)
        {
            var query = uri.Query.Substring(uri.Query.IndexOf('?') + 1);
            return query.Split(SplitAnd, StringSplitOptions.RemoveEmptyEntries);
        }

        private static string YouTube(Uri uri)
        {
            string uriPath = uri.AbsolutePath;
            if (string.Equals(uriPath, "/embed", StringComparison.OrdinalIgnoreCase) || uriPath.StartsWith("/embed/", StringComparison.OrdinalIgnoreCase))
            {
                return uri.ToString();
            }
            if (!string.Equals(uriPath, "/watch", StringComparison.OrdinalIgnoreCase) && !uriPath.StartsWith("/watch/", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            var queryParams = SplitQuery(uri);
            return BuildYouTubeIframeUrl(
                queryParams.FirstOrDefault(p => p.StartsWith("v="))?.Substring(2),
                queryParams.FirstOrDefault(p => p.StartsWith("t="))?.Substring(2)
            );
        }

        private static string YouTubeShortened(Uri uri)
        {
            return BuildYouTubeIframeUrl(
                uri.AbsolutePath.Substring(1),
                SplitQuery(uri).FirstOrDefault(p => p.StartsWith("t="))?.Substring(2)
            );
        }

        private static string BuildYouTubeIframeUrl(string videoId, string startTime)
        {
            if (string.IsNullOrEmpty(videoId))
            {
                return null;
            }
            string url = $"https://www.youtube.com/embed/{videoId}";
            return string.IsNullOrEmpty(startTime) ? url : $"{url}?start={startTime}";
        }

        private static string Vimeo(Uri uri)
        {
            var items = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).Split('/');
            return items.Length > 0 ? $"https://player.vimeo.com/video/{ items[items.Length - 1] }" : null;
        }

        private static string Odnoklassniki(Uri uri)
        {
            var items = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).Split('/');
            return items.Length > 0 ? $"https://ok.ru/videoembed/{ items[items.Length - 1] }" : null;
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
