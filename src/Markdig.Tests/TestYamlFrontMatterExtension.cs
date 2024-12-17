using Markdig.Extensions.Yaml;
using Markdig.Renderers;
using Markdig.Syntax;

namespace Markdig.Tests;

public class TestYamlFrontMatterExtension
{
    [TestCaseSource(nameof(TestCases))]
    public void ProperYamlFrontMatterRenderersAdded(IMarkdownObjectRenderer[] objectRenderers, bool hasYamlFrontMatterHtmlRenderer, bool hasYamlFrontMatterRoundtripRenderer)
    {
        var builder = new MarkdownPipelineBuilder();
        builder.Extensions.Add(new YamlFrontMatterExtension());
        var markdownRenderer = new DummyRenderer();
        markdownRenderer.ObjectRenderers.AddRange(objectRenderers);
        builder.Build().Setup(markdownRenderer);
        Assert.That(markdownRenderer.ObjectRenderers.Contains<YamlFrontMatterHtmlRenderer>(), Is.EqualTo(hasYamlFrontMatterHtmlRenderer));
        Assert.That(markdownRenderer.ObjectRenderers.Contains<YamlFrontMatterRoundtripRenderer>(), Is.EqualTo(hasYamlFrontMatterRoundtripRenderer));
    }

    [Test]
    public void AllowYamlFrontMatterInMiddleOfDocument()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .Use(new YamlFrontMatterExtension { AllowInMiddleOfDocument = true })
            .Build();

        TestParser.TestSpec(
            "This is a text1\n---\nthis: is a frontmatter\n---\nThis is a text2",
            "<p>This is a text1</p>\n<p>This is a text2</p>",
            pipeline);
    }

    private static IEnumerable<TestCaseData> TestCases()
    {
        yield return new TestCaseData(new IMarkdownObjectRenderer[]
        {
        }, false, false) {TestName = "No ObjectRenderers"};

        yield return new TestCaseData(new IMarkdownObjectRenderer[]
        {
            new Markdig.Renderers.Html.CodeBlockRenderer()
        }, true, false) {TestName = "Html CodeBlock"};

        yield return new TestCaseData(new IMarkdownObjectRenderer[]
        {
            new Markdig.Renderers.Roundtrip.CodeBlockRenderer()
        }, false, true) {TestName = "Roundtrip CodeBlock"};

        yield return new TestCaseData(new IMarkdownObjectRenderer[]
        {
            new Markdig.Renderers.Html.CodeBlockRenderer(),
            new Markdig.Renderers.Roundtrip.CodeBlockRenderer()
        }, true, true) {TestName = "Html/Roundtrip CodeBlock"};

        yield return new TestCaseData(new IMarkdownObjectRenderer[]
        {
            new Markdig.Renderers.Html.CodeBlockRenderer(),
            new Markdig.Renderers.Roundtrip.CodeBlockRenderer(),
            new YamlFrontMatterHtmlRenderer()
        }, true, true) {TestName = "Html/Roundtrip CodeBlock, Yaml Html"};

        yield return new TestCaseData(new IMarkdownObjectRenderer[]
        {
            new Markdig.Renderers.Html.CodeBlockRenderer(),
            new Markdig.Renderers.Roundtrip.CodeBlockRenderer(),
            new YamlFrontMatterRoundtripRenderer()
        }, true, true) { TestName = "Html/Roundtrip CodeBlock, Yaml Roundtrip" };
    }

    private class DummyRenderer : IMarkdownRenderer
    {
        public DummyRenderer()
        {
            ObjectRenderers = new ObjectRendererCollection();
        }

#pragma warning disable CS0067 // ObjectWriteBefore/ObjectWriteAfter is never used
        public event Action<IMarkdownRenderer, MarkdownObject> ObjectWriteBefore;
        public event Action<IMarkdownRenderer, MarkdownObject> ObjectWriteAfter;
#pragma warning restore CS0067

        public ObjectRendererCollection ObjectRenderers { get; }
        public object Render(MarkdownObject markdownObject)
        {
            return null;
        }
    }

    [TestCase("---\nkey1: value1\nkey2: value2\n---\n\n# Content\n")]
    [TestCase("---\nkey1: value1\nkey2: value2\nkey3: value3\nkey4: value4\nkey5: value5\nkey6: value6\nkey7: value7\nkey8: value8\n---\n\n# Content\n")]
    public void FrontMatterBlockLinesCharIterator(string value)
    {
        var builder = new MarkdownPipelineBuilder();
        builder.Extensions.Add(new YamlFrontMatterExtension());
        var markdownDocument = Markdown.Parse(value, builder.Build());

        var yamlBlocks = markdownDocument.Descendants<YamlFrontMatterBlock>();
        Assert.True(yamlBlocks.Any());

        foreach (var yamlBlock in yamlBlocks)
        {
            var iterator = yamlBlock.Lines.ToCharIterator();
            while(iterator.CurrentChar != '\0')
            {
                iterator.NextChar();
            }
        }

        // No exception parsing and iterating through YAML front matter block lines
    }

}
