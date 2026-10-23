// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Renderers;

namespace Markdig.Extensions.RelativeFileLinks;

/// <summary>
/// Extension that rewrites relative links to Markdown files (e.g. `usage.md#preview`) into links to the generated HTML pages (e.g. `usage.html#preview`).
/// </summary>
/// <remarks>
/// The extension sets <see cref="HtmlRenderer.LinkRewriter"/> and calls any rewriter assigned before. Links with a scheme and protocol-relative links are left unchanged.
/// When <see cref="HtmlRenderer.BaseUrl"/> is set, relative links are already resolved against it, so absolute links with the same scheme, host and port are rewritten too.
/// </remarks>
public sealed class RelativeFileLinkExtension : IMarkdownExtension
{
    private readonly string[] _sourceExtensions;
    private readonly string _targetExtension;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelativeFileLinkExtension"/> class with the default options.
    /// </summary>
    public RelativeFileLinkExtension() : this(new RelativeFileLinkOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelativeFileLinkExtension"/> class. The options are copied.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">The target extension is <c>null</c>, or the source extensions are empty or contain an extension that does not start with `.`.</exception>
    public RelativeFileLinkExtension(RelativeFileLinkOptions options)
    {
        if (options is null) ThrowHelper.ArgumentNullException(nameof(options));

        if (options.TargetExtension is null)
        {
            ThrowHelper.ArgumentException("The target extension must not be null.", nameof(options));
        }

        if (options.SourceExtensions.Count == 0)
        {
            ThrowHelper.ArgumentException("At least one source extension is required.", nameof(options));
        }

        foreach (string extension in options.SourceExtensions)
        {
            if (string.IsNullOrEmpty(extension) || extension[0] != '.')
            {
                ThrowHelper.ArgumentException($"Invalid source extension `{extension}`: a source extension must start with `.`.", nameof(options));
            }
        }

        _sourceExtensions = options.SourceExtensions.ToArray();
        _targetExtension = options.TargetExtension;
    }

    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
    }

    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is not HtmlRenderer htmlRenderer)
        {
            return;
        }

        // Markdown.Convert sets up a caller-owned renderer again on every call, so don't wrap the rewriter twice
        if (htmlRenderer.LinkRewriter?.Target is ChainedRewriter chained && chained.Extension == this)
        {
            return;
        }

        htmlRenderer.LinkRewriter = new ChainedRewriter(this, htmlRenderer, htmlRenderer.LinkRewriter).Rewrite;
    }

    private string Rewrite(string url, Uri? baseUrl)
    {
        int pathLength = url.AsSpan().IndexOfAny('?', '#');
        if (pathLength < 0)
        {
            pathLength = url.Length;
        }

        int extensionLength = GetSourceExtensionLength(url.AsSpan(0, pathLength));
        if (extensionLength == 0 || !IsInternal(url, baseUrl))
        {
            return url;
        }

        var builder = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);
        builder.Append(url.AsSpan(0, pathLength - extensionLength));
        builder.Append(_targetExtension);
        builder.Append(url.AsSpan(pathLength));
        return builder.ToString();
    }

    private int GetSourceExtensionLength(ReadOnlySpan<char> path)
    {
        foreach (string extension in _sourceExtensions)
        {
            if (path.EndsWith(extension.AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return extension.Length;
            }
        }

        return 0;
    }

    private static bool IsInternal(string url, Uri? baseUrl)
    {
        if (url.StartsWith("//", StringComparison.Ordinal))
        {
            return false;
        }

        if (!HasScheme(url))
        {
            return true;
        }

        // BaseUrl resolves relative links before the rewriter runs, so an absolute link with the same origin is internal
        return baseUrl is { IsAbsoluteUri: true }
            && Uri.TryCreate(url, UriKind.Absolute, out Uri? uri)
            && Uri.Compare(uri, baseUrl, UriComponents.SchemeAndServer, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0;
    }

    private static bool HasScheme(string url)
    {
        // RFC 3986: scheme = ALPHA *( ALPHA / DIGIT / "+" / "-" / "." ) ":"
        if (url.Length == 0 || !url[0].IsAlpha())
        {
            return false;
        }

        for (int i = 1; i < url.Length; i++)
        {
            char c = url[i];
            if (c == ':')
            {
                return true;
            }

            if (!c.IsAlphaNumeric() && c is not ('+' or '-' or '.'))
            {
                return false;
            }
        }

        return false;
    }

    private sealed class ChainedRewriter(RelativeFileLinkExtension extension, HtmlRenderer renderer, Func<string, string>? next)
    {
        public RelativeFileLinkExtension Extension => extension;

        public string Rewrite(string url)
        {
            url = extension.Rewrite(url, renderer.BaseUrl);
            return next is null ? url : next(url);
        }
    }
}
