// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig.Tests;

public class TestConfigureHtmlRenderer
{
    [Test]
    public void AppliesToRendererUsedByToHtml()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .ConfigureHtmlRenderer(renderer => renderer.ImplicitParagraph = true)
            .Build();

        Assert.AreEqual("some *text*", Markdown.ToHtml("some \\*text\\*", pipeline));
        Assert.AreEqual("<em>again</em>", Markdown.ToHtml("*again*", pipeline));
    }

    [Test]
    public void AppliesToRendererSetUpByPipeline()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .ConfigureHtmlRenderer(renderer => renderer.BaseUrl = new Uri("https://example.com/docs/"))
            .Build();

        var writer = new StringWriter();
        var renderer = new HtmlRenderer(writer);
        pipeline.Setup(renderer);
        renderer.Render(Markdown.Parse("[a](usage.html)", pipeline));
        writer.Flush();

        Assert.AreEqual("<p><a href=\"https://example.com/docs/usage.html\">a</a></p>\n", writer.ToString());
    }

    [Test]
    public void RunsEachConfigurationInOrder()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .ConfigureHtmlRenderer(renderer => renderer.LinkRewriter = url => url + "?a")
            .ConfigureHtmlRenderer(renderer =>
            {
                Func<string, string> previous = renderer.LinkRewriter!;
                renderer.LinkRewriter = url => previous(url) + "&b";
            })
            .Build();

        Assert.AreEqual("<p><a href=\"usage.html?a&amp;b\">a</a></p>\n", Markdown.ToHtml("[a](usage.html)", pipeline));
    }

    [Test]
    public void ThrowsWhenConfigureIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new MarkdownPipelineBuilder().ConfigureHtmlRenderer(null!));
    }

    [TestCase(true, "> [a](fail.html)", "one")] // the quote renderer sets ImplicitParagraph to false while it renders
    [TestCase(false, "- [a](fail.html)", "<p>one</p>\n")] // the list renderer sets ImplicitParagraph to true for a tight list
    public void FailedRenderingDoesNotChangeTheNextConversion(bool implicitParagraph, string failingMarkdown, string expected)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .ConfigureHtmlRenderer(renderer =>
            {
                renderer.ImplicitParagraph = implicitParagraph;
                renderer.LinkRewriter = url => url == "fail.html" ? throw new InvalidOperationException("Rewrite failed") : url;
            })
            .Build();

        Assert.Throws<InvalidOperationException>(() => Markdown.ToHtml(failingMarkdown, pipeline));
        Assert.AreEqual(expected, Markdown.ToHtml("one", pipeline));

        Assert.Throws<InvalidOperationException>(() => Markdown.ToHtml(failingMarkdown, new StringWriter(), pipeline));
        var writer = new StringWriter();
        Markdown.ToHtml("one", writer, pipeline);
        Assert.AreEqual(expected, writer.ToString());
    }

    [Test]
    public void ReplacingTheWriterOfToHtmlThrows()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .ConfigureHtmlRenderer(renderer => renderer.Writer = new StringWriter())
            .Build();

        Assert.Throws<InvalidOperationException>(() => Markdown.ToHtml("one", pipeline));
        Assert.Throws<InvalidOperationException>(() => Markdown.ToHtml("one", new StringWriter(), pipeline));
    }
}
