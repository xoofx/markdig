// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Markdig.Helpers;

namespace Markdig.Extensions.MediaLinks;

public class HostProviderBuilder
{
    private sealed class DelegateProvider(
        string hostPrefix,
        Func<Uri, string?> handler,
        bool allowFullscreen = true,
        string? className = null) : IHostProvider
    {
        public string HostPrefix { get; } = hostPrefix;

        public Func<Uri, string?> Delegate { get; } = handler;

        public bool AllowFullScreen { get; } = allowFullscreen;

        public string? Class { get; } = className;

        public bool TryHandle(Uri mediaUri, bool isSchemaRelative, [NotNullWhen(true)] out string? iframeUrl)
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
    public static IHostProvider Create(string hostPrefix, Func<Uri, string?> handler, bool allowFullScreen = true, string? iframeClass = null)
    {
        if (string.IsNullOrEmpty(hostPrefix))
            ThrowHelper.ArgumentException("hostPrefix is null or empty.", nameof(hostPrefix));
        if (handler is null)
            ThrowHelper.ArgumentNullException(nameof(handler));

        return new DelegateProvider(hostPrefix, handler, allowFullScreen, iframeClass);
    }

    internal static readonly IHostProvider[] KnownHosts =
    [
        Create("www.youtube.com", YouTubeShort, iframeClass: "youtubeshort"),
        Create("www.youtube.com", YouTube, iframeClass: "youtube"),
        Create("youtu.be", YouTubeShortened, iframeClass: "youtube"),
        Create("vimeo.com", Vimeo, iframeClass: "vimeo"),
        Create("music.yandex.ru", Yandex, allowFullScreen: false, iframeClass: "yandex"),
        Create("ok.ru", Odnoklassniki, iframeClass: "odnoklassniki"),
    ];

    #region Known providers

    private static readonly string[] SplitAnd = ["&"];
    private static string[] SplitQuery(Uri uri)
    {
        var query = uri.Query.Substring(uri.Query.IndexOf('?') + 1);
        return query.Split(SplitAnd, StringSplitOptions.RemoveEmptyEntries);
    }

    private static string? YouTube(Uri uri)
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
            queryParams.FirstOrDefault(p => p.StartsWith("v=", StringComparison.Ordinal))?.Substring(2),
            queryParams.FirstOrDefault(p => p.StartsWith("t=", StringComparison.Ordinal))?.Substring(2)
        );
    }

    private static string? YouTubeShort(Uri uri)
    {
        string uriPath = uri.AbsolutePath;
        bool isYouTubeShort = uriPath.StartsWith("/shorts/", StringComparison.OrdinalIgnoreCase);
        if (!isYouTubeShort)
        {
            return null;
        }

        var shortId = uriPath.Substring("/shorts/".Length).Split('?').FirstOrDefault(); // the format might be "/shorts/6BUptHVuvyI?feature=share"
        return BuildYouTubeIframeUrl(shortId, null);
    }

    private static string? YouTubeShortened(Uri uri)
    {
        return BuildYouTubeIframeUrl(
            uri.AbsolutePath.Substring(1),
            SplitQuery(uri).FirstOrDefault(p => p.StartsWith("t=", StringComparison.Ordinal))?.Substring(2)
        );
    }

    private static string? BuildYouTubeIframeUrl(string? videoId, string? startTime)
    {
        if (string.IsNullOrEmpty(videoId))
        {
            return null;
        }
        string url = $"https://www.youtube.com/embed/{videoId}";
        return string.IsNullOrEmpty(startTime) ? url : $"{url}?start={startTime}";
    }

    private static string? Vimeo(Uri uri)
    {
        var items = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).Split('/');
        return items.Length > 0 ? $"https://player.vimeo.com/video/{ items[items.Length - 1] }" : null;
    }

    private static string? Odnoklassniki(Uri uri)
    {
        var items = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).Split('/');
        return items.Length > 0 ? $"https://ok.ru/videoembed/{ items[items.Length - 1] }" : null;
    }

    private static string? Yandex(Uri uri)
    {
        string[] items = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).Split('/');
        string? albumKeyword = items.Skip(0).FirstOrDefault();
        string? albumId = items.Skip(1).FirstOrDefault();
        string? trackKeyword = items.Skip(2).FirstOrDefault();
        string? trackId = items.Skip(3).FirstOrDefault();

        if (albumKeyword != "album" || albumId is null || trackKeyword != "track" || trackId is null)
        {
            return null;
        }

        return $"https://music.yandex.ru/iframe/#track/{trackId}/{albumId}/";
    }
    #endregion
}
