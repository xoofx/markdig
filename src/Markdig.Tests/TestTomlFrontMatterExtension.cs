// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Extensions.Toml;
using Markdig.Extensions.Yaml;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;

namespace Markdig.Tests;

public class TestTomlFrontMatterExtension
{
    private const string Document = "+++\ntitle = \"Hello\"\n+++\n\n# Content\n";

    [Test]
    public void ParsesTomlFrontMatterAtStartOfDocument()
    {
        var pipeline = new MarkdownPipelineBuilder().UseTomlFrontMatter().Build();

        var document = Markdown.Parse(Document, pipeline);
        var block = document.Descendants<TomlFrontMatterBlock>().SingleOrDefault();

        Assert.That(block, Is.Not.Null);
        Assert.That(block!.FenceChar, Is.EqualTo('+'));
        Assert.That(block.Lines.ToString(), Is.EqualTo("title = \"Hello\""));
    }

    [TestCase("+++\ntitle = \"Hello\"\n\n# Content\n")] // no closing fence
    [TestCase("+++\ntitle = \"Hello\"\n...\n\n# Content\n")] // `...` only closes YAML
    [TestCase("++++\ntitle = \"Hello\"\n+++\n")] // the opening fence must be exactly `+++`
    [TestCase("Text\n\n+++\ntitle = \"Hello\"\n+++\n")] // not at the start of the document
    public void NotATomlFrontMatter(string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder().UseTomlFrontMatter().Build();

        var document = Markdown.Parse(markdown, pipeline);

        Assert.That(document.Descendants<YamlFrontMatterBlock>(), Is.Empty);
    }

    [Test]
    public void DotsLineDoesNotCloseTomlFrontMatter()
    {
        var pipeline = new MarkdownPipelineBuilder().UseTomlFrontMatter().Build();

        var document = Markdown.Parse("+++\na = 1\n...\nb = 2\n+++\nText\n", pipeline);
        var block = document.Descendants<TomlFrontMatterBlock>().SingleOrDefault();

        Assert.That(block, Is.Not.Null);
        Assert.That(block!.Lines.ToString(), Is.EqualTo("a = 1\n...\nb = 2"));
        Assert.That(Markdown.ToHtml("+++\na = 1\n...\nb = 2\n+++\nText\n", pipeline), Is.EqualTo("<p>Text</p>\n"));
    }

    [Test]
    public void TomlFrontMatterIsOptIn()
    {
        var expected = "<p>+++\ntitle = &quot;Hello&quot;\n+++</p>\n<h1>Content</h1>\n";

        Assert.That(Markdown.ToHtml(Document), Is.EqualTo(expected));
        Assert.That(Markdown.ToHtml(Document, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build()), Is.EqualTo("<p>+++\ntitle = &quot;Hello&quot;\n+++</p>\n<h1 id=\"content\">Content</h1>\n"));
        Assert.That(Markdown.Parse(Document, new MarkdownPipelineBuilder().UseAdvancedExtensions().UseYamlFrontMatter().Build()).Descendants<YamlFrontMatterBlock>(), Is.Empty);
    }

    [Test]
    public void ConfigureEnablesTomlFrontMatter()
    {
        var pipeline = new MarkdownPipelineBuilder().Configure("toml").Build();

        Assert.That(Markdown.Parse(Document, pipeline).Descendants<TomlFrontMatterBlock>(), Is.Not.Empty);
        Assert.That(Markdown.ToHtml(Document, pipeline), Is.EqualTo("<h1>Content</h1>\n"));
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
        var pipeline = new MarkdownPipelineBuilder().EnableTrackTrivia().UseTomlFrontMatter().Build();
        var document = Markdown.Parse(Document, pipeline);

        using var writer = new StringWriter();
        var renderer = new RoundtripRenderer(writer);
        pipeline.Setup(renderer);
        renderer.Render(document);

        Assert.That(writer.ToString(), Is.EqualTo(Document));
    }

    [Test]
    public void YamlAndTomlFrontMatterCanCoexist()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseYamlFrontMatter()
            .UseTomlFrontMatter()
            .Build();

        var tomlBlock = Markdown.Parse(Document, pipeline).Descendants<YamlFrontMatterBlock>().SingleOrDefault();
        var yamlBlock = Markdown.Parse("---\ntitle: Hello\n---\n\n# Content\n", pipeline).Descendants<YamlFrontMatterBlock>().SingleOrDefault();

        Assert.That(tomlBlock, Is.TypeOf<TomlFrontMatterBlock>());
        Assert.That(tomlBlock!.FenceChar, Is.EqualTo('+'));
        Assert.That(yamlBlock, Is.TypeOf<YamlFrontMatterBlock>());
        Assert.That(yamlBlock!.FenceChar, Is.EqualTo('-'));
    }

    [Test]
    public void YamlParserIsAddedWhenTomlIsRegisteredFirst()
    {
        var builder = new MarkdownPipelineBuilder()
            .UseTomlFrontMatter()
            .UseYamlFrontMatter();
        var pipeline = builder.Build();

        var yamlHtml = Markdown.ToHtml("---\ntitle: Hello\n---\n\n# Content\n", pipeline);
        var tomlHtml = Markdown.ToHtml(Document, pipeline);

        Assert.That(yamlHtml, Is.EqualTo("<h1>Content</h1>\n"));
        Assert.That(tomlHtml, Is.EqualTo("<h1>Content</h1>\n"));
        Assert.That(builder.BlockParsers.Count(parser => parser is YamlFrontMatterParser), Is.EqualTo(1));
        Assert.That(builder.BlockParsers.Count(parser => parser is TomlFrontMatterParser), Is.EqualTo(1));
    }

    [Test]
    public void YamlFrontMatterBlockKeepsItsPublicConstructor()
    {
        // A second public constructor would make constructor selection ambiguous (e.g. for System.Text.Json)
        var constructors = typeof(YamlFrontMatterBlock).GetConstructors();

        Assert.That(constructors, Has.Length.EqualTo(1));
        Assert.That(constructors[0].GetParameters().Select(parameter => parameter.ParameterType), Is.EqualTo(new[] { typeof(BlockParser) }));
    }

    [Test]
    public void TomlParserIsNotAYamlParser()
    {
        var builder = new MarkdownPipelineBuilder().UseTomlFrontMatter();
        builder.Build();

        Assert.That(builder.BlockParsers.Contains<YamlFrontMatterParser>(), Is.False);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void YamlParserCanBeCustomizedWithTomlEnabled(bool tomlFirst)
    {
        var builder = new MarkdownPipelineBuilder();
        if (tomlFirst)
        {
            builder.UseTomlFrontMatter().UseYamlFrontMatter();
        }
        else
        {
            builder.UseYamlFrontMatter().UseTomlFrontMatter();
        }
        builder.Extensions.Add(new AllowYamlInMiddleOfDocumentExtension());
        var pipeline = builder.Build();

        Assert.That(builder.BlockParsers.Find<YamlFrontMatterParser>()!.FenceChar, Is.EqualTo('-'));
        Assert.That(Markdown.ToHtml("Text\n\n---\nkey: 1\n---\n", pipeline), Is.EqualTo("<p>Text</p>\n"));
        Assert.That(Markdown.Parse("Text\n\n+++\nkey = 1\n+++\n", pipeline).Descendants<TomlFrontMatterBlock>(), Is.Empty);
    }

    [Test]
    public void TomlParserIsAddedWithoutThematicBreakParser()
    {
        var builder = new MarkdownPipelineBuilder();
        builder.BlockParsers.TryRemove<ThematicBreakParser>();
        var pipeline = builder.UseTomlFrontMatter().Build();

        Assert.That(Markdown.ToHtml(Document, pipeline), Is.EqualTo("<h1>Content</h1>\n"));
    }

    [Test]
    public void YamlParserIsNotAddedTwiceWithDerivedYamlParser()
    {
        var builder = new MarkdownPipelineBuilder();
        builder.BlockParsers.Insert(0, new DerivedYamlFrontMatterParser());
        builder.UseYamlFrontMatter().Build();

        Assert.That(builder.BlockParsers.Count(parser => parser is YamlFrontMatterParser), Is.EqualTo(1));
    }

    private sealed class DerivedYamlFrontMatterParser : YamlFrontMatterParser
    {
    }

    private sealed class AllowYamlInMiddleOfDocumentExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline) => pipeline.BlockParsers.Find<YamlFrontMatterParser>()!.AllowInMiddleOfDocument = true;

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }
    }
}
