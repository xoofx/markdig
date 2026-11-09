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
}
