// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Linq;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

[TestFixture]
public class TestGfmPipeTables
{
    private static MarkdownPipeline CreatePipeline(bool trackTrivia = false, bool inferWidths = false)
    {
        var builder = new MarkdownPipelineBuilder().UsePipeTables(new PipeTableOptions
        {
            UseGfmRules = true,
            RequireHeaderSeparator = false, // GFM rules take precedence over both legacy switches.
            UseHeaderForColumnCount = false,
            InferColumnWidthsFromSeparator = inferWidths
        });
        if (trackTrivia) builder.EnableTrackTrivia();
        return builder.Build();
    }

    [TestCase("| --- | |")]
    [TestCase("| | --- |")]
    [TestCase("| --- ||")]
    [TestCase("| --- | : |")]
    [TestCase("| --- | :: |")]
    [TestCase("| --- | - - |")]
    [TestCase("| --- | : - |")]
    [TestCase("| --- | - : |")]
    [TestCase("| --- | x |")]
    [TestCase("| --- |")]
    [TestCase("| --- | --- | --- |")]
    [TestCase("| | |")]
    [TestCase("| --- | \u00a0--- |")]
    public void RejectInvalidDelimiterRows(string separator)
    {
        var markdown = "| a | b |\n" + separator + "\n| x | y |";
        foreach (bool trivia in new[] { false, true })
        {
            Assert.That(Markdown.Parse(markdown, CreatePipeline(trivia)).Descendants<Table>(), Is.Empty);
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public void RequiresDelimiterAndUsesHeaderWidth(bool trivia)
    {
        var pipeline = CreatePipeline(trivia);
        Assert.That(Markdown.Parse("a | b\nx | y", pipeline).Descendants<Table>(), Is.Empty);
        var table = (Table)Markdown.Parse("a | b\n- | -\nx\ny | z | ignored\n|", pipeline)[0];
        Assert.That(table.Count, Is.EqualTo(4));
        Assert.That(table.Cast<TableRow>().Select(row => row.Count), Is.All.EqualTo(2));
        Assert.That(Markdown.ToHtml("a | b\n- | -\nx\ny | z | ignored", pipeline), Does.Not.Contain("ignored"));
    }

    [Test]
    public void DefaultsRemainPermissive()
    {
        Assert.That(new PipeTableOptions().UseGfmRules, Is.False);
        var pipeline = new MarkdownPipelineBuilder().UsePipeTables().Build();
        var table = (Table)Markdown.Parse("| a | b |\n| --- | |\n| x | y | z |", pipeline)[0];
        Assert.That(((TableRow)table[0]).Count, Is.EqualTo(3));
        Assert.That(Markdown.Parse("a | b\n--- |\nx | y", pipeline)[0], Is.TypeOf<Table>());
    }

    [TestCase("a\n---", "<h2>a</h2>\n")]
    [TestCase("|a|\n---", "<h2>|a|</h2>\n")]
    public void DoesNotStealSetextHeadings(string markdown, string expected)
    {
        Assert.That(Markdown.ToHtml(markdown, CreatePipeline()), Is.EqualTo(expected));
    }

    [TestCase("a\n:-\nx")]
    [TestCase("a\n-|\nx")]
    [TestCase("|a|\n:-:\nx")]
    public void SingleColumnTablesMayOmitOuterPipes(string markdown)
    {
        var table = (Table)Markdown.Parse(markdown, CreatePipeline())[0];
        Assert.That(table.Count, Is.EqualTo(2));
        Assert.That(table.ColumnDefinitions.Count, Is.EqualTo(1));
    }

    [TestCase("\n")]
    [TestCase("\r\n")]
    [TestCase("\r")]
    public void SupportsLineEndings(string newline)
    {
        const string markdown = "| a | b |\n| - | - |\n| x | y |\nz\n\nafter";
        foreach (bool trivia in new[] { false, true })
        {
            var pipeline = CreatePipeline(trivia);
            Assert.That(Markdown.ToHtml(markdown.Replace("\n", newline), pipeline),
                Is.EqualTo(Markdown.ToHtml(markdown, pipeline)));
        }
    }

    [Test]
    public void OnlyOddBackslashRunsEscapePipes()
    {
        const string markdown = """
            a | b
            - | -
            \\|x
            \\\|y
            """;
        var html = Markdown.ToHtml(markdown, CreatePipeline());
        Assert.That(html, Does.Contain("<td>\\</td>\n<td>x</td>"));
        Assert.That(html, Does.Contain("<td>\\|y</td>\n<td></td>"));
    }

    [Test]
    public void ExtensionConfigurationSelectsGfmRules()
    {
        var configured = new MarkdownPipelineBuilder().Configure("gfm-pipetables").Build();
        Assert.That(configured.Extensions.Find<PipeTableExtension>().Options.UseGfmRules, Is.True);
        const string markdown = "a | b\n- | -\nx";
        Assert.That(Markdown.ToHtml(markdown, configured), Is.EqualTo(Markdown.ToHtml(markdown, CreatePipeline())));
        var advanced = new MarkdownPipelineBuilder().UsePipeTables(new PipeTableOptions { UseGfmRules = true }).UseAdvancedExtensions().Build();
        Assert.That(Markdown.ToHtml(markdown, advanced), Is.EqualTo(Markdown.ToHtml(markdown, configured)));
    }

    [TestCase("`a|b`", "`a", "b`")]
    [TestCase("[a|b](url)", "[a", "b](url)")]
    [TestCase("**a|b**", "**a", "b**")]
    [TestCase("<i title='a|b'>", "&lt;i title='a", "b'&gt;")]
    public void PipesTakePrecedenceOverAllInlines(string body, string first, string second)
    {
        var html = Markdown.ToHtml("a | b\n- | -\n| " + body + " |", CreatePipeline());
        Assert.That(html, Does.Contain("<td>" + first + "</td>\n<td>" + second + "</td>"));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void EscapesAndReferencesRemainLocalToCells(bool trivia)
    {
        const string markdown = """
            | [a][ref] | b |
            | --- | --- |
            | `a\|b` | **\|** |
            | `\_` | \_ |
            | `\\` | \\ |
            | <i title="\|"> | [a\|b](url) |

            [ref]: /target

            `\|`
            """;
        var html = Markdown.ToHtml(markdown, CreatePipeline(trivia));
        Assert.That(html, Does.Contain("<th><a href=\"/target\">a</a></th>"));
        Assert.That(html, Does.Contain("<code>a|b</code>"));
        Assert.That(html, Does.Contain("<strong>|</strong>"));
        Assert.That(html, Does.Contain("<code>\\_</code>"));
        Assert.That(html, Does.Contain("<code>\\\\</code>"));
        Assert.That(html, Does.Contain("<i title=\"|\">"));
        Assert.That(html, Does.Contain("<a href=\"url\">a|b</a>"));
        Assert.That(html, Does.EndWith("<p><code>\\|</code></p>\n"));
    }

    [TestCase("# heading", "<h1>heading</h1>")]
    [TestCase("> quote", "<blockquote>")]
    [TestCase("- item", "<ul>")]
    [TestCase("1. item", "<ol>")]
    [TestCase("2. item", "<ol start=\"2\">")]
    [TestCase("---", "<hr />")]
    [TestCase("```\ncode\n```", "<pre><code>code")]
    [TestCase("<div>\nhtml\n</div>", "<div>")]
    public void OtherBlocksTerminateTables(string next, string expected)
    {
        var html = Markdown.ToHtml("a | b\n- | -\nx | y\n" + next, CreatePipeline());
        Assert.That(html, Does.Contain("</table>\n" + expected));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void TablesCanInterruptParagraphsAndNestInContainers(bool trivia)
    {
        const string table = "a | b\n- | -\nx | y";
        var pipeline = CreatePipeline(trivia);
        var html = Markdown.ToHtml(table, pipeline);
        Assert.That(Markdown.ToHtml("first\nsecond\n" + table, pipeline), Is.EqualTo("<p>first\nsecond</p>\n" + html));
        Assert.That(Markdown.ToHtml("> " + table.Replace("\n", "\n> ") + "\noutside", pipeline),
            Is.EqualTo("<blockquote>\n" + html + "</blockquote>\n<p>outside</p>\n"));
        Assert.That(Markdown.ToHtml("- " + table.Replace("\n", "\n  "), pipeline),
            Is.EqualTo("<ul>\n<li>\n" + html + "</li>\n</ul>\n"));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void PreservesSourceLocationsAndInferredWidths(bool trivia)
    {
        const string markdown = "before\n\n| a | b |\n| :- | ---: |\n| `c\\|d` | e |";
        var pipeline = CreatePipeline(trivia, inferWidths: true);
        var table = (Table)Markdown.Parse(markdown, pipeline)[1];
        Assert.That(table.Line, Is.EqualTo(2));
        Assert.That(table.Span.Start, Is.EqualTo(markdown.IndexOf('|')));
        Assert.That(table.Span.End, Is.EqualTo(markdown.Length - 1));
        Assert.That(table.ColumnDefinitions.Select(column => column.Width), Is.EqualTo(new[] { 25f, 75f }));
        var code = table.Descendants<CodeInline>().Single();
        Assert.That(markdown.Substring(code.Span.Start, code.Span.Length), Is.EqualTo("`c\\|d`"));
        Assert.That(code.Line, Is.EqualTo(4));
        Assert.That(code.Column, Is.EqualTo(2));
        var normalized = Markdown.Normalize(markdown, pipeline: pipeline);
        Assert.That(Markdown.ToHtml(normalized, pipeline), Is.EqualTo(Markdown.ToHtml(markdown, pipeline)));
    }

    [Test]
    public void NormalizationPreservesEscapedPipes()
    {
        const string markdown = """
            a | b
            - | -
            `a\|b` | **\|**
            <i title="\|"> | [a\|b](url)
            """;
        var pipeline = CreatePipeline();
        var normalized = Markdown.Normalize(markdown, pipeline: pipeline);
        Assert.That(Markdown.ToHtml(normalized, pipeline), Is.EqualTo(Markdown.ToHtml(markdown, pipeline)));
        Assert.That(Markdown.Normalize(normalized, pipeline: pipeline), Is.EqualTo(normalized));
    }
}
