using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

[TestFixture]
public sealed class TestPipeTable
{
    [TestCase("| S | T |\r\n|---|---| \r\n| G | H |")]
    [TestCase("| S | T |\r\n|---|---|\t\r\n| G | H |")]
    [TestCase("| S | T |\r\n|---|---|\f\r\n| G | H |")]
    [TestCase("| S | \r\n|---|\r\n| G |\r\n\r\n| D | D |\r\n| ---| ---| \r\n| V | V |", 2)]
    [TestCase("a\r| S | T |\r|---|---|")]
    [TestCase("a\n| S | T |\r|---|---|")]
    public void TestTableBug(string markdown, int tableCount = 1)
    {
        MarkdownDocument document =
            Markdown.Parse(markdown, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

        Table[] tables = document.Descendants().OfType<Table>().ToArray();

        Assert.AreEqual(tableCount, tables.Length);
    }

    [TestCase("A | B\r\n---|---", new[] {50.0f, 50.0f})]
    [TestCase("A | B\r\n-|---", new[] {25.0f, 75.0f})]
    [TestCase("A | B\r\n-|---\r\nA | B\r\n---|---", new[] {25.0f, 75.0f})]
    [TestCase("A | B\r\n---|---|---", new[] {33.33f, 33.33f, 33.33f})]
    [TestCase("A | B\r\n---|---|---|", new[] {33.33f, 33.33f, 33.33f})]
    public void TestColumnWidthByHeaderLines(string markdown, float[] expectedWidth)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UsePipeTables(new PipeTableOptions() {InferColumnWidthsFromSeparator = true})
            .Build();
        var document = Markdown.Parse(markdown, pipeline);
        var table = document.Descendants().OfType<Table>().FirstOrDefault();
        Assert.IsNotNull(table);
        var actualWidths = table.ColumnDefinitions.Select(x => x.Width).ToList();
        Assert.AreEqual(actualWidths.Count, expectedWidth.Length);
        for (int i = 0; i < expectedWidth.Length; i++)
        {
            Assert.AreEqual(actualWidths[i], expectedWidth[i], 0.01);
        }
    }

    [Test]
    public void TestColumnWidthIsNotSetWithoutConfigurationFlag()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UsePipeTables(new PipeTableOptions() {InferColumnWidthsFromSeparator = false})
            .Build();
        var document = Markdown.Parse("| A | B | C |\r\n|---|---|---|", pipeline);
        var table = document.Descendants().OfType<Table>().FirstOrDefault();
        Assert.IsNotNull(table);
        foreach (var column in table.ColumnDefinitions)
        {
            Assert.AreEqual(0, column.Width);
        }
    }

    [Test]
    public void TableWithUnbalancedCodeSpanParsesWithoutDepthLimitError()
    {
        const string markdown = """
| Count | A | B | C | D | E |
|-------|---|---|---|---|---|
|     0 | B | C | D | E | F |
|     1 | B | `C | D | E | F |
|     2 | B | `C | D | E | F |
|     3 | B | C | D | E | F |
|     4 | B | C | D | E | F |
|     5 | B | C | D | E | F |
|     6 | B | C | D | E | F |
|     7 | B | C | D | E | F |
|     8 | B | C | D | E | F |
|     9 | B | C | D | E | F |
|    10 | B | C | D | E | F |
|    11 | B | C | D | E | F |
|    12 | B | C | D | E | F |
|    13 | B | C | D | E | F |
|    14 | B | C | D | E | F |
|    15 | B | C | D | E | F |
|    16 | B | C | D | E | F |
|    17 | B | C | D | E | F |
|    18 | B | C | D | E | F |
|    19 | B | C | D | E | F |
""";

        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        MarkdownDocument document = null!;
        Assert.DoesNotThrow(() => document = Markdown.Parse(markdown, pipeline));

        var tables = document.Descendants().OfType<Table>().ToArray();
        Assert.That(tables, Has.Length.EqualTo(1));

        string html = string.Empty;
        Assert.DoesNotThrow(() => html = Markdown.ToHtml(markdown, pipeline));
        Assert.That(html, Does.Contain("<table"));
        Assert.That(html, Does.Contain("<td>`C</td>"));
    }

    [Test]
    public void CodeInlineWithPipeDelimitersRemainsCodeInline()
    {
        const string markdown = "`|| hidden text ||`";

        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        var document = Markdown.Parse(markdown, pipeline);

        var codeInline = document.Descendants().OfType<CodeInline>().SingleOrDefault();
        Assert.IsNotNull(codeInline);
        Assert.That(codeInline!.Content, Is.EqualTo("|| hidden text ||"));
    }

    [Test]
    public void SingleLineCodeInlineWithPipeDelimitersRendersAsCode()
    {
        const string markdown = "`|| hidden text ||`";

        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        var html = Markdown.ToHtml(markdown, pipeline);

        Assert.That(html, Is.EqualTo("<p><code>|| hidden text ||</code></p>\n"));
    }

    [Test]
    public void MultiLineCodeInlineWithPipeDelimitersRendersAsCode()
    {
        const string markdown = """
`
|| hidden text ||
`
""";

        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        var html = Markdown.ToHtml(markdown, pipeline);

        Assert.That(html, Is.EqualTo("<p><code>|| hidden text ||</code></p>\n"));
    }

    [Test]
    public void TableCellWithCodeInlineRendersCorrectly()
    {
        const string markdown = """
| Count | A | B | C | D | E |
|-------|---|---|---|---|---|
|     0 | B | C | D | E | F |
|     1 | B | `Code block` | D | E | F |
|     2 | B | C | D | E | F |
""";

        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        var html = Markdown.ToHtml(markdown, pipeline);

        Assert.That(html, Does.Contain("<td><code>Code block</code></td>"));
    }
}
