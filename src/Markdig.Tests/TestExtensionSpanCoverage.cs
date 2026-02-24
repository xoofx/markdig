using Markdig.Extensions.Abbreviations;
using Markdig.Extensions.Alerts;
using Markdig.Extensions.DefinitionLists;
using Markdig.Extensions.Emoji;
using Markdig.Extensions.Figures;
using Markdig.Extensions.Footers;
using Markdig.Extensions.Footnotes;
using Markdig.Extensions.JiraLinks;
using Markdig.Extensions.Mathematics;
using Markdig.Extensions.SmartyPants;
using Markdig.Extensions.Tables;
using Markdig.Extensions.TaskLists;
using Markdig.Extensions.Yaml;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

[TestFixture]
public sealed class TestExtensionSpanCoverage
{
    public sealed class ExtensionSpanCase(
        string name,
        Action<MarkdownPipelineBuilder> configurePipeline,
        string markdown,
        Action<MarkdownDocument> validate,
        bool validateSpanTree = true)
    {
        public string Name { get; } = name;
        public Action<MarkdownPipelineBuilder> ConfigurePipeline { get; } = configurePipeline;
        public string Markdown { get; } = markdown;
        public Action<MarkdownDocument> Validate { get; } = validate;
        public bool ValidateSpanTree { get; } = validateSpanTree;
    }

    [TestCaseSource(nameof(GetExtensionSpanCases))]
    public void ExtensionSpanTreeIsValid(ExtensionSpanCase testCase)
    {
        var builder = new MarkdownPipelineBuilder
        {
            PreciseSourceLocation = true
        };
        testCase.ConfigurePipeline(builder);

        var document = Markdown.Parse(testCase.Markdown.ReplaceLineEndings("\n"), builder.Build());

        if (testCase.ValidateSpanTree)
        {
            Assert.That(document.HasValidSpan(recursive: true), Is.True, $"{testCase.Name} has invalid container spans");
        }

        testCase.Validate?.Invoke(document);
    }

    [Test]
    public void JiraLinkSpanMatchesToken()
    {
        var pipeline = new MarkdownPipelineBuilder
        {
            PreciseSourceLocation = true
        }.UseJiraLinks(new JiraLinkOptions("https://jira.example.com")).Build();

        var document = Markdown.Parse("ABC-123", pipeline);
        var jiraLink = document.Descendants<JiraLink>().FirstOrDefault();
        Assert.That(jiraLink, Is.Not.Null);
        Assert.That(jiraLink!.Span, Is.EqualTo(new SourceSpan(0, 6)));

        var literal = jiraLink.FirstChild as LiteralInline;
        Assert.That(literal, Is.Not.Null);
        Assert.That(literal!.Span, Is.EqualTo(new SourceSpan(0, 6)));
    }

    [Test]
    public void TaskListSpanMatchesCheckboxToken()
    {
        var pipeline = new MarkdownPipelineBuilder
        {
            PreciseSourceLocation = true
        }.UseTaskLists().Build();

        var document = Markdown.Parse("- [x] done", pipeline);
        var task = document.Descendants<TaskList>().FirstOrDefault();
        Assert.That(task, Is.Not.Null);
        Assert.That(task!.Span, Is.EqualTo(new SourceSpan(2, 4)));
    }

    [Test]
    public void AlertBlockSpanCoversSourceQuote()
    {
        var pipeline = new MarkdownPipelineBuilder
        {
            PreciseSourceLocation = true
        }.UseAlertBlocks().Build();

        var document = Markdown.Parse("> [!NOTE]\n> body", pipeline);
        var alert = document.Descendants<AlertBlock>().FirstOrDefault();
        Assert.That(alert, Is.Not.Null);
        Assert.That(alert!.Span.Start, Is.EqualTo(0));
        Assert.That(alert.Span.End, Is.GreaterThan(0));

        var paragraph = alert.Descendants<ParagraphBlock>().FirstOrDefault();
        Assert.That(paragraph, Is.Not.Null);
        Assert.That(paragraph!.Span.Start, Is.GreaterThanOrEqualTo(alert.Span.Start));
        Assert.That(paragraph.Span.End, Is.LessThanOrEqualTo(alert.Span.End));
    }

    [Test]
    public void YamlFrontMatterSpanCoversFrontMatterContent()
    {
        var pipeline = new MarkdownPipelineBuilder
        {
            PreciseSourceLocation = true
        }.UseYamlFrontMatter().Build();

        var document = Markdown.Parse("---\na: 1\n---\ntext", pipeline);
        var yaml = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
        Assert.That(yaml, Is.Not.Null);
        Assert.That(yaml!.Span.Start, Is.EqualTo(0));
        Assert.That(yaml.Span.End, Is.GreaterThanOrEqualTo(7));
    }

    private static IEnumerable<TestCaseData> GetExtensionSpanCases()
    {
        yield return Case(
            "AlertBlocks",
            builder => builder.UseAlertBlocks(),
            "> [!NOTE]\n> body",
            document => AssertNodesHaveNonEmptySpan<AlertBlock>(document));

        yield return Case(
            "AutoLinks",
            builder => builder.UseAutoLinks(),
            "http://example.com",
            document =>
            {
                var autoLink = document.Descendants<LinkInline>().FirstOrDefault(link => link.IsAutoLink);
                Assert.That(autoLink, Is.Not.Null);
                Assert.That(autoLink!.Span.IsEmpty, Is.False);
                Assert.That(autoLink.UrlSpan, Is.EqualTo(autoLink.Span));
            });

        yield return Case(
            "NonAsciiNoEscape",
            builder => builder.UseNonAsciiNoEscape(),
            "Café");

        yield return Case(
            "YamlFrontMatter",
            builder => builder.UseYamlFrontMatter(),
            "---\na: 1\n---\ntext",
            document => AssertNodesHaveNonEmptySpan<YamlFrontMatterBlock>(document));

        yield return Case(
            "SelfPipeline",
            builder => builder.UseSelfPipeline(),
            "<!--markdig:tasklists-->\n- [x] done",
            document => AssertNodesHaveNonEmptySpan<TaskList>(document),
            validateSpanTree: false);

        yield return Case(
            "PragmaLines",
            builder => builder.UsePragmaLines(),
            "# Heading\n\nParagraph");

        yield return Case(
            "Diagrams",
            builder => builder.UseDiagrams(),
            "```mermaid\ngraph TD;\n```");

        yield return Case(
            "TaskLists",
            builder => builder.UseTaskLists(),
            "- [x] done",
            document => AssertNodesHaveNonEmptySpan<TaskList>(document));

        yield return Case(
            "CustomContainers",
            builder => builder.UseCustomContainers(),
            ":::\nvalue\n:::",
            document => AssertNodesHaveNonEmptySpan<Markdig.Extensions.CustomContainers.CustomContainer>(document));

        yield return Case(
            "MediaLinks",
            builder => builder.UseMediaLinks(),
            "[video](https://www.youtube.com/watch?v=dQw4w9WgXcQ)",
            document => AssertNodesHaveNonEmptySpan<LinkInline>(document));

        yield return Case(
            "AutoIdentifiers",
            builder => builder.UseAutoIdentifiers(),
            "# Heading",
            document =>
            {
                var heading = document.Descendants<HeadingBlock>().FirstOrDefault();
                Assert.That(heading, Is.Not.Null);
                Assert.That(heading!.GetAttributes().Id, Is.Not.Null.And.Not.Empty);
            });

        yield return Case(
            "SmartyPants",
            builder => builder.UseSmartyPants(),
            "<<a>>",
            document => AssertNodesHaveNonEmptySpan<SmartyPant>(document));

        yield return Case(
            "Bootstrap",
            builder => builder.UseBootstrap(),
            "![alt](img.png)");

        yield return Case(
            "Mathematics",
            builder => builder.UseMathematics(),
            "$a$",
            document => AssertNodesHaveNonEmptySpan<MathInline>(document));

        yield return Case(
            "Figures",
            builder => builder.UseFigures(),
            "^^^\ntext\n^^^",
            document => AssertNodesHaveNonEmptySpan<Figure>(document));

        yield return Case(
            "Abbreviations",
            builder => builder.UseAbbreviations(),
            "*[HTML]: HyperText Markup Language\n\nHTML",
            document => AssertNodesHaveNonEmptySpan<AbbreviationInline>(document));

        yield return Case(
            "DefinitionLists",
            builder => builder.UseDefinitionLists(),
            "a0\n:   1234",
            document => AssertNodesHaveNonEmptySpan<DefinitionList>(document));

        yield return Case(
            "PipeTables",
            builder => builder.UsePipeTables(),
            "a|b\n-|-\n1|2",
            document => AssertNodesHaveNonEmptySpan<Table>(document));

        yield return Case(
            "GridTables",
            builder => builder.UseGridTables(),
            "+-+-+\n|a|b|\n+=+=+\n|c|d|\n+-+-+",
            document => AssertNodesHaveNonEmptySpan<Table>(document));

        yield return Case(
            "Citations",
            builder => builder.UseCitations(),
            "\"\"text\"\"",
            document =>
            {
                var citation = document.Descendants<EmphasisInline>().FirstOrDefault(x => x.DelimiterChar == '"' && x.DelimiterCount == 2);
                Assert.That(citation, Is.Not.Null);
                Assert.That(citation!.Span.IsEmpty, Is.False);
            });

        yield return Case(
            "Footers",
            builder => builder.UseFooters(),
            "^^ one\n^^ two",
            document => AssertNodesHaveNonEmptySpan<FooterBlock>(document));

        yield return Case(
            "Footnotes",
            builder => builder.UseFootnotes(),
            "[^1]: note\n\nref[^1]",
            document =>
            {
                AssertNodesHaveNonEmptySpan<FootnoteGroup>(document);
                AssertNodesHaveNonEmptySpan<Footnote>(document);
                var links = document.Descendants<FootnoteLink>().ToList();
                Assert.That(links.Count, Is.GreaterThan(0));
            });

        yield return Case(
            "Hardlines",
            builder => builder.UseSoftlineBreakAsHardlineBreak(),
            "a\nb",
            document =>
            {
                var lineBreak = document.Descendants<LineBreakInline>().FirstOrDefault();
                Assert.That(lineBreak, Is.Not.Null);
                Assert.That(lineBreak!.IsHard, Is.True);
            });

        yield return Case(
            "EmphasisExtras",
            builder => builder.UseEmphasisExtras(),
            "~~text~~",
            document =>
            {
                var emphasis = document.Descendants<EmphasisInline>().FirstOrDefault(x => x.DelimiterChar == '~' && x.DelimiterCount == 2);
                Assert.That(emphasis, Is.Not.Null);
                Assert.That(emphasis!.Span.IsEmpty, Is.False);
            });

        yield return Case(
            "ListExtras",
            builder => builder.UseListExtras(),
            "A. item",
            document =>
            {
                var list = document.Descendants<ListBlock>().FirstOrDefault();
                Assert.That(list, Is.Not.Null);
                Assert.That(list!.Span.IsEmpty, Is.False);
                Assert.That(list.IsOrdered, Is.True);
            });

        yield return Case(
            "GenericAttributes",
            builder => builder.UseGenericAttributes(),
            "text{#custom-id}",
            document =>
            {
                var paragraph = document.Descendants<ParagraphBlock>().FirstOrDefault();
                Assert.That(paragraph, Is.Not.Null);
                var attributes = paragraph!.TryGetAttributes();
                Assert.That(attributes, Is.Not.Null);
                Assert.That(attributes!.Span.IsEmpty, Is.False);
            });

        yield return Case(
            "EmojiAndSmiley",
            builder => builder.UseEmojiAndSmiley(),
            ":)",
            document => AssertNodesHaveNonEmptySpan<EmojiInline>(document));

        yield return Case(
            "NoFollowLinks",
            builder => builder.UseReferralLinks("nofollow"),
            "[link](https://example.com)",
            document => AssertNodesHaveNonEmptySpan<LinkInline>(document));

        yield return Case(
            "ReferralLinks",
            builder => builder.UseReferralLinks("nofollow", "noopener"),
            "[link](https://example.com)",
            document => AssertNodesHaveNonEmptySpan<LinkInline>(document));

        yield return Case(
            "JiraLinks",
            builder => builder.UseJiraLinks(new JiraLinkOptions("https://jira.example.com")),
            "ABC-123",
            document => AssertNodesHaveNonEmptySpan<JiraLink>(document));

        yield return Case(
            "Globalization",
            builder => builder.UseGlobalization(),
            "## Héllo");
    }

    private static TestCaseData Case(
        string name,
        Action<MarkdownPipelineBuilder> configurePipeline,
        string markdown,
        Action<MarkdownDocument> validate = null,
        bool validateSpanTree = true)
    {
        return new TestCaseData(new ExtensionSpanCase(name, configurePipeline, markdown, validate, validateSpanTree))
            .SetName($"Extension_{name}_MaintainsValidSpans");
    }

    private static void AssertNodesHaveNonEmptySpan<T>(MarkdownDocument document) where T : MarkdownObject
    {
        var nodes = document.Descendants<T>().ToList();
        Assert.That(nodes.Count, Is.GreaterThan(0), $"Expected at least one node of type `{typeof(T).Name}`");
        foreach (var node in nodes)
        {
            Assert.That(node.Span.IsEmpty, Is.False, $"Node `{typeof(T).Name}` has an empty span");
        }
    }
}
