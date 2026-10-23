// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Extensions.RelativeFileLinks;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

public class TestRelativeFileLinks
{
    private static readonly MarkdownPipeline s_pipeline = new MarkdownPipelineBuilder().UseRelativeFileLinks().Build();

    [TestCase("usage.md#preview", "https://example.com/docs/usage.html#preview")]
    [TestCase("../api/usage.md", "https://example.com/api/usage.html")]
    [TestCase("/root.md", "https://example.com/root.html")]
    [TestCase("https://example.com/other.md", "https://example.com/other.html")]
    [TestCase("//cdn.example.com/usage.md", "https://cdn.example.com/usage.md")]
    [TestCase("https://github.com/xoofx/markdig/blob/main/readme.md", "https://github.com/xoofx/markdig/blob/main/readme.md")]
    [TestCase("http://example.com/docs/usage.md", "http://example.com/docs/usage.md")]
    [TestCase("https://example.com:8443/docs/usage.md", "https://example.com:8443/docs/usage.md")]
    public void RewritesLinksWithTheOriginOfBaseUrl(string url, string expectedUrl)
    {
        string html = Render($"[a]({url})", renderer => renderer.BaseUrl = new Uri("https://example.com/docs/"));

        Assert.AreEqual($"<p><a href=\"{expectedUrl}\">a</a></p>\n", html);
    }

    [TestCase("//cdn.example.com/usage.md")]
    [TestCase("C:/docs/usage.md")]
    [TestCase("notes:usage.md")]
    [TestCase("usage.rmd")]
    [TestCase("usage.md.txt")]
    [TestCase("")]
    public void LeavesOtherLinksUnchanged(string url)
    {
        Assert.AreEqual($"<p><a href=\"{url}\">a</a></p>\n", Markdown.ToHtml($"[a]({url})", s_pipeline));
    }

    [Test]
    public void ReadsBaseUrlWhenRendering()
    {
        var writer = new StringWriter();
        var renderer = new HtmlRenderer(writer);
        s_pipeline.Setup(renderer);
        renderer.BaseUrl = new Uri("https://example.com/docs/");

        renderer.Render(Markdown.Parse("[a](usage.md)", s_pipeline));
        writer.Flush();

        Assert.AreEqual("<p><a href=\"https://example.com/docs/usage.html\">a</a></p>\n", writer.ToString());
    }

    [Test]
    public void RunsBeforeExistingLinkRewriter()
    {
        string html = Render("[a](usage.md)", renderer => renderer.LinkRewriter = url => "https://cdn.example.com/" + url);

        Assert.AreEqual("<p><a href=\"https://cdn.example.com/usage.html\">a</a></p>\n", html);
    }

    [Test]
    public void DoesNotWrapLinkRewriterTwice()
    {
        var renderer = new HtmlRenderer(new StringWriter());
        Markdown.Convert("[a](usage.md)", renderer, s_pipeline);
        Func<string, string> rewriter = renderer.LinkRewriter;

        Markdown.Convert("[b](usage.md)", renderer, s_pipeline);

        Assert.That(renderer.LinkRewriter, Is.SameAs(rewriter));
    }

    [TestCase("", "usage#preview")]
    [TestCase("/", "usage/#preview")]
    [TestCase(".xhtml", "usage.xhtml#preview")]
    public void ReplacesSourceExtensionWithTargetExtension(string targetExtension, string expectedUrl)
    {
        var pipeline = new MarkdownPipelineBuilder().UseRelativeFileLinks(new RelativeFileLinkOptions { TargetExtension = targetExtension }).Build();

        Assert.AreEqual($"<p><a href=\"{expectedUrl}\">a</a></p>\n", Markdown.ToHtml("[a](usage.md#preview)", pipeline));
    }

    [Test]
    public void RewritesOnlyConfiguredSourceExtensions()
    {
        var options = new RelativeFileLinkOptions();
        options.SourceExtensions.Clear();
        options.SourceExtensions.Add(".mdx");
        var pipeline = new MarkdownPipelineBuilder().UseRelativeFileLinks(options).Build();

        Assert.AreEqual("<p><a href=\"guide.html\">a</a> <a href=\"usage.md\">b</a></p>\n", Markdown.ToHtml("[a](guide.mdx) [b](usage.md)", pipeline));
    }

    [Test]
    public void CopiesOptions()
    {
        var options = new RelativeFileLinkOptions();
        var builder = new MarkdownPipelineBuilder().UseRelativeFileLinks(options);
        options.SourceExtensions.Clear();
        options.SourceExtensions.Add(".txt");
        options.TargetExtension = ".htm";

        Assert.AreEqual("<p><a href=\"usage.html\">a</a> <a href=\"notes.txt\">b</a></p>\n", Markdown.ToHtml("[a](usage.md) [b](notes.txt)", builder.Build()));
    }

    [Test]
    public void LastUseRelativeFileLinksCallWins()
    {
        var options = new RelativeFileLinkOptions();
        options.SourceExtensions.Clear();
        options.SourceExtensions.Add(".txt");
        var pipeline = new MarkdownPipelineBuilder().UseRelativeFileLinks(options).UseRelativeFileLinks().Build();

        Assert.AreEqual("<p><a href=\"usage.html\">a</a> <a href=\"notes.txt\">b</a></p>\n", Markdown.ToHtml("[a](usage.md) [b](notes.txt)", pipeline));
    }

    [Test]
    public void ThrowsForNullOptions()
    {
        Assert.Throws<ArgumentNullException>(() => new MarkdownPipelineBuilder().UseRelativeFileLinks(null));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("md")]
    public void ThrowsForInvalidSourceExtension(string extension)
    {
        var options = new RelativeFileLinkOptions();
        options.SourceExtensions.Add(extension);

        Assert.Throws<ArgumentException>(() => new MarkdownPipelineBuilder().UseRelativeFileLinks(options));
    }

    [Test]
    public void ThrowsForMissingExtensions()
    {
        var noSource = new RelativeFileLinkOptions();
        noSource.SourceExtensions.Clear();
        var noTarget = new RelativeFileLinkOptions { TargetExtension = null };

        Assert.Throws<ArgumentException>(() => new MarkdownPipelineBuilder().UseRelativeFileLinks(noSource));
        Assert.Throws<ArgumentException>(() => new MarkdownPipelineBuilder().UseRelativeFileLinks(noTarget));
    }

    [Test]
    public void LeavesDocumentUnchanged()
    {
        MarkdownDocument document = Markdown.Parse("[a](usage.md)", s_pipeline);

        Assert.AreEqual("<p><a href=\"usage.html\">a</a></p>\n", document.ToHtml(s_pipeline));
        Assert.AreEqual("usage.md", document.Descendants<LinkInline>().Single().Url);
        Assert.AreEqual("[a](usage.md)", Markdown.Normalize("[a](usage.md)", pipeline: s_pipeline));
    }

    private static string Render(string markdown, Action<HtmlRenderer> configure)
    {
        var writer = new StringWriter();
        var renderer = new HtmlRenderer(writer);
        configure(renderer);
        s_pipeline.Setup(renderer);

        renderer.Render(Markdown.Parse(markdown, s_pipeline));
        writer.Flush();
        return writer.ToString();
    }
}
