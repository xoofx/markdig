using Markdig.Extensions.Tables;
using Markdig.Syntax;

namespace Markdig.Tests;

[TestFixture]
public sealed class TestPipeTable
{
    [TestCase("| S | T |\r\n|---|---| \r\n| G | H |")]
    [TestCase("| S | T |\r\n|---|---|\t\r\n| G | H |")]
    [TestCase("| S | T |\r\n|---|---|\f\r\n| G | H |")]
    [TestCase("| S | \r\n|---|\r\n| G |\r\n\r\n| D | D |\r\n| ---| ---| \r\n| V | V |", 2)]
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
}
