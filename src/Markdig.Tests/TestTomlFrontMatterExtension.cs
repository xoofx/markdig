using Markdig.Extensions.Yaml;
using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;

using System.IO;

namespace Markdig.Tests;

public class TestTomlFrontMatterExtension
{
    private const string Document = "+++\ntitle = \"Hello\"\n+++\n\n# Content\n";

    [Test]
    public void ParsesTomlFrontMatterAtStartOfDocument()
    {
        var pipeline = new MarkdownPipelineBuilder().UseTomlFrontMatter().Build();

        var document = Markdown.Parse(Document, pipeline);
        var block = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

        Assert.That(block, Is.Not.Null);
        Assert.That(block!.Lines.ToString(), Does.Contain("title"));
    }

    [Test]
    public void NoClosingFenceIsNotAFrontMatter()
    {
        var pipeline = new MarkdownPipelineBuilder().UseTomlFrontMatter().Build();

        var document = Markdown.Parse("+++\ntitle = \"Hello\"\n\n# Content\n", pipeline);

        Assert.That(document.Descendants<YamlFrontMatterBlock>(), Is.Empty);
    }

    [Test]
    public void HtmlRendererDropsTomlFrontMatter()
    {
        var pipeline = new MarkdownPipelineBuilder().UseTomlFrontMatter().Build();

        var html = Markdown.ToHtml(Document, pipeline);

        Assert.That(html, Is.EqualTo("<h1>Content</h1>\n"));
    }

    [Test]
    public void RoundtripKeepsTomlDelimiters()
    {
        var pipeline = new MarkdownPipelineBuilder().UseTomlFrontMatter().Build();
        var document = Markdown.Parse(Document, pipeline);

        using var writer = new StringWriter();
        var renderer = new RoundtripRenderer(writer);
        pipeline.Setup(renderer);
        renderer.Render(document);

        Assert.That(writer.ToString(), Does.Contain("+++"));
    }

    [Test]
    public void YamlAndTomlFrontMatterCanCoexist()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseYamlFrontMatter()
            .UseTomlFrontMatter()
            .Build();

        var document = Markdown.Parse(Document, pipeline);
        var yamlDocument = Markdown.Parse("---\ntitle: Hello\n---\n\n# Content\n", pipeline);

        Assert.That(document.Descendants<YamlFrontMatterBlock>(), Is.Not.Empty);
        Assert.That(yamlDocument.Descendants<YamlFrontMatterBlock>(), Is.Not.Empty);
    }

    [Test]
    public void YamlParserIsAddedWhenTomlIsRegisteredFirst()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseTomlFrontMatter()
            .UseYamlFrontMatter()
            .Build();

        var yamlHtml = Markdown.ToHtml("---\ntitle: Hello\n---\n\n# Content\n", pipeline);
        var tomlHtml = Markdown.ToHtml(Document, pipeline);

        Assert.That(yamlHtml, Is.EqualTo("<h1>Content</h1>\n"));
        Assert.That(tomlHtml, Is.EqualTo("<h1>Content</h1>\n"));
    }
}