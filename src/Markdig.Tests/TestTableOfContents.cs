// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Extensions.TableOfContents;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;

namespace Markdig.Tests;

public class TestTableOfContents
{
    private const string FooTableOfContents = "<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>Foo</li>\n</ul>\n</nav>\n";

    private static readonly MarkdownPipeline s_pipeline = new MarkdownPipelineBuilder().UseTableOfContents().Build();
    private static readonly MarkdownPipeline s_autoIdentifierPipeline = new MarkdownPipelineBuilder().UseAutoIdentifiers().UseTableOfContents().Build();

    [Test]
    public void BuildsNestedEntries()
    {
        MarkdownDocument document = Markdown.Parse("[TOC]\n\n# A\n## B\n### C\n## D\n# E\n", s_pipeline);
        var tableOfContents = (TableOfContentsBlock)document[0];

        Assert.AreEqual("A(B(C) D) E", Describe(tableOfContents.Entries));
        Assert.AreSame(document[1], tableOfContents.Entries[0].Heading);
    }

    [Test]
    public void ReplacesParagraph()
    {
        MarkdownDocument document = Markdown.Parse("[TOC]\n\n# Foo\n", s_pipeline);

        Assert.IsInstanceOf<TableOfContentsBlock>(document[0]);
        Assert.AreSame(document, document[0].Parent);
        Assert.IsFalse(document[0].IsOpen);
        Assert.That(document.Descendants<ParagraphBlock>(), Is.Empty);
    }

    [TestCase("[TOC]")]
    [TestCase("Text\n\n  [TOC]  \n")]
    [TestCase("> - [TOC]\n")]
    public void KeepsPositionOfParagraph(string markdown)
    {
        ParagraphBlock paragraph = Markdown.Parse(markdown).Descendants<ParagraphBlock>().Last();
        TableOfContentsBlock tableOfContents = Markdown.Parse(markdown, s_pipeline).Descendants<TableOfContentsBlock>().Single();

        Assert.AreEqual(paragraph.Span, tableOfContents.Span);
        Assert.AreEqual(paragraph.Line, tableOfContents.Line);
        Assert.AreEqual(paragraph.Column, tableOfContents.Column);
    }

    [TestCase("   [TOC]")]
    [TestCase("[TOC]   ")]
    [TestCase(" [TOC]\t")]
    public void IgnoresSpacesAroundMarker(string marker)
    {
        Assert.AreEqual(FooTableOfContents + "<h1>Foo</h1>\n", Markdown.ToHtml(marker + "\n\n# Foo\n", s_pipeline));
    }

    [Test]
    public void RendersOrderedList()
    {
        var pipeline = new MarkdownPipelineBuilder().UseTableOfContents(new TableOfContentsOptions { ListKind = TableOfContentsListKind.Ordered }).Build();

        Assert.AreEqual("<nav class=\"toc\" role=\"doc-toc\">\n<ol>\n<li>Foo\n<ol>\n<li>Bar</li>\n</ol>\n</li>\n</ol>\n</nav>\n<h1>Foo</h1>\n<h2>Bar</h2>\n", Markdown.ToHtml("[TOC]\n\n# Foo\n## Bar\n", pipeline));
    }

    [Test]
    public void LinksOnlyEntriesOfHeadingsWithId()
    {
        var pipeline = new MarkdownPipelineBuilder().UseTableOfContents().UseGenericAttributes().Build();

        Assert.AreEqual("<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li><a href=\"#foo\">Foo</a></li>\n<li>Bar</li>\n</ul>\n</nav>\n<h1 id=\"foo\">Foo</h1>\n<h1>Bar</h1>\n", Markdown.ToHtml("[TOC]\n\n# Foo {#foo}\n# Bar\n", pipeline));
    }

    [Test]
    public void LinksToCustomHeadingId()
    {
        var pipeline = new MarkdownPipelineBuilder().UseAutoIdentifiers().UseTableOfContents().UseGenericAttributes().Build();

        Assert.AreEqual("<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li><a href=\"#custom\">Foo</a></li>\n<li><a href=\"#bar\">Bar</a></li>\n</ul>\n</nav>\n<h1 id=\"custom\">Foo</h1>\n<h1 id=\"bar\">Bar</h1>\n", Markdown.ToHtml("[TOC]\n\n# Foo {#custom}\n# Bar\n", pipeline));
    }

    [Test]
    public void LinksNoEntryForEmptyHeadingId()
    {
        MarkdownDocument document = Markdown.Parse("[TOC]\n\n# Foo\n", s_pipeline);
        document.Descendants<HeadingBlock>().Single().GetAttributes().Id = "";

        Assert.AreEqual(FooTableOfContents + "<h1 id=\"\">Foo</h1>\n", document.ToHtml(s_pipeline));
    }

    [Test]
    public void ResolvesLinksAgainstBaseUrl()
    {
        string html = Render("[TOC]\n\n# Foo\n\n[Foo](#foo)\n", renderer => renderer.BaseUrl = new Uri("https://example.com/docs/page.html"));

        Assert.AreEqual("<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li><a href=\"https://example.com/docs/page.html#foo\">Foo</a></li>\n</ul>\n</nav>\n<h1 id=\"foo\">Foo</h1>\n<p><a href=\"https://example.com/docs/page.html#foo\">Foo</a></p>\n", html);
    }

    [Test]
    public void PassesLinksToLinkRewriter()
    {
        string html = Render("[TOC]\n\n# Foo\n", renderer => renderer.LinkRewriter = url => "page.html" + url);

        Assert.AreEqual("<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li><a href=\"page.html#foo\">Foo</a></li>\n</ul>\n</nav>\n<h1 id=\"foo\">Foo</h1>\n", html);
    }

    // TODO: Remove the Ignore once HtmlFootnoteLinkRenderer writes nothing when EnableHtmlForInline is false.
    [Test, Ignore("HtmlFootnoteLinkRenderer ignores EnableHtmlForInline, so the entry still contains the footnote link")]
    public void LeavesFootnoteReferencesOutOfEntries()
    {
        var pipeline = new MarkdownPipelineBuilder().UseAutoIdentifiers().UseFootnotes().UseTableOfContents().Build();

        string html = Markdown.ToHtml("[TOC]\n\n# Intro[^1]\n\n[^1]: Note\n", pipeline);

        Assert.That(html, Does.StartWith("<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li><a href=\"#intro\">Intro</a></li>\n</ul>\n</nav>\n"));
    }

    [Test]
    public void RendersPlainText()
    {
        Assert.AreEqual("Use the <T> & co\nBar\nUse the <T> & co\nBar\n", Markdown.ToPlainText("[TOC]\n\n# Use *the* `<T>` & co\n## Bar\n", s_pipeline));
    }

    [TestCase("[TOC]\n\n# Foo\n")]
    [TestCase("Text\n\n[TOC]\n\nText\n")]
    [TestCase("- [TOC]\n- Foo\n")]
    public void NormalizesToMarker(string markdown)
    {
        Assert.That(Markdown.Parse(markdown, s_pipeline).Descendants<TableOfContentsBlock>(), Is.Not.Empty);
        Assert.AreEqual(Markdown.Normalize(markdown), Markdown.Normalize(markdown, pipeline: s_pipeline));
    }

    [Test]
    public void LeavesMarkerAsParagraphWhenTrackingTrivia()
    {
        const string markdown = "  [TOC]  \n\n# Foo\n";
        var pipeline = new MarkdownPipelineBuilder().UseTableOfContents().EnableTrackTrivia().Build();
        MarkdownDocument document = Markdown.Parse(markdown, pipeline);

        var writer = new StringWriter();
        var renderer = new RoundtripRenderer(writer);
        pipeline.Setup(renderer);
        renderer.Write(document);

        Assert.That(document.Descendants<TableOfContentsBlock>(), Is.Empty);
        Assert.AreEqual(markdown, writer.ToString());
        Assert.AreEqual("<p>[TOC]</p>\n<h1>Foo</h1>\n", Markdown.ToHtml("[TOC]\n\n# Foo\n", pipeline));
    }

    [Test]
    public void LeavesTermOfDefinitionList()
    {
        var pipeline = new MarkdownPipelineBuilder().UseDefinitionLists().UseTableOfContents().Build();

        Assert.AreEqual("<dl>\n<dt>[TOC]</dt>\n<dd>Definition</dd>\n</dl>\n<h1>Foo</h1>\n", Markdown.ToHtml("[TOC]\n:   Definition\n\n# Foo\n", pipeline));
    }

    [Test]
    public void ReplacesMarkerBeforeBlankLineAndDefinition()
    {
        var pipeline = new MarkdownPipelineBuilder().UseDefinitionLists().UseTableOfContents().Build();

        Assert.AreEqual(FooTableOfContents + "<p>:   Definition</p>\n<h1>Foo</h1>\n", Markdown.ToHtml("[TOC]\n\n:   Definition\n\n# Foo\n", pipeline));
    }

    [TestCase("pipetables+tableofcontents")]
    [TestCase("gfm-pipetables+tableofcontents")]
    public void LeavesMarkerBeforePipeTable(string extensions)
    {
        var pipeline = new MarkdownPipelineBuilder().Configure(extensions).Build();

        Assert.AreEqual("<p>[TOC]</p>\n<table>\n<thead>\n<tr>\n<th>a</th>\n</tr>\n</thead>\n</table>\n<h1>Foo</h1>\n", Markdown.ToHtml("[TOC]\n| a |\n| - |\n\n# Foo\n", pipeline));
    }

    [Test]
    public void ListsHeadingsOfItsOwnDocumentOnly()
    {
        Markdown.ToHtml("[TOC]\n\n# Foo\n", s_pipeline);

        Assert.AreEqual("<p>Text</p>\n", Markdown.ToHtml("[TOC]\n\nText\n", s_pipeline));
    }

    [Test]
    public void ConfigureEnablesExtension()
    {
        var pipeline = new MarkdownPipelineBuilder().Configure("tableofcontents").Build();

        Assert.AreEqual(FooTableOfContents + "<h1>Foo</h1>\n", Markdown.ToHtml("[TOC]\n\n# Foo\n", pipeline));
    }

    [Test]
    public void CopiesOptions()
    {
        var options = new TableOfContentsOptions { ListKind = TableOfContentsListKind.Ordered };
        var builder = new MarkdownPipelineBuilder().UseTableOfContents(options);
        options.ListKind = TableOfContentsListKind.Unordered;

        Assert.AreEqual("<nav class=\"toc\" role=\"doc-toc\">\n<ol>\n<li>Foo</li>\n</ol>\n</nav>\n<h1>Foo</h1>\n", Markdown.ToHtml("[TOC]\n\n# Foo\n", builder.Build()));
    }

    [Test]
    public void LastUseTableOfContentsCallWins()
    {
        var ordered = new TableOfContentsOptions { ListKind = TableOfContentsListKind.Ordered };
        var pipeline = new MarkdownPipelineBuilder().UseTableOfContents(ordered).UseTableOfContents().Build();

        Assert.AreEqual(FooTableOfContents + "<h1>Foo</h1>\n", Markdown.ToHtml("[TOC]\n\n# Foo\n", pipeline));
    }

    [Test]
    public void ThrowsForNullOptions()
    {
        Assert.Throws<ArgumentNullException>(() => new MarkdownPipelineBuilder().UseTableOfContents(null));
    }

    [TestCase(-1)]
    [TestCase(2)]
    public void ThrowsForUndefinedListKind(int listKind)
    {
        var options = new TableOfContentsOptions { ListKind = (TableOfContentsListKind)listKind };

        Assert.Throws<ArgumentException>(() => new MarkdownPipelineBuilder().UseTableOfContents(options));
    }

    [Test]
    public void ThrowsForNullHeading()
    {
        Assert.Throws<ArgumentNullException>(() => new TableOfContentsEntry(null));
    }

    private static string Render(string markdown, Action<HtmlRenderer> configure)
    {
        var writer = new StringWriter();
        var renderer = new HtmlRenderer(writer);
        configure(renderer);
        s_autoIdentifierPipeline.Setup(renderer);

        renderer.Render(Markdown.Parse(markdown, s_autoIdentifierPipeline));
        writer.Flush();
        return writer.ToString();
    }

    private static string Describe(IReadOnlyList<TableOfContentsEntry> entries)
    {
        return string.Join(" ", entries.Select(entry =>
        {
            string text = entry.Heading.Inline!.FirstChild!.ToString();
            return entry.Children.Count == 0 ? text : $"{text}({Describe(entry.Children)})";
        }));
    }
}
