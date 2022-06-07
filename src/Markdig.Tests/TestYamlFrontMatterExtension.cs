using System;
using System.Collections.Generic;
using System.Linq;
using Markdig.Extensions.Yaml;
using Markdig.Renderers;
using Markdig.Syntax;
using NUnit.Framework;

namespace Markdig.Tests
{
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

            public event Action<IMarkdownRenderer, MarkdownObject> ObjectWriteBefore;
            public event Action<IMarkdownRenderer, MarkdownObject> ObjectWriteAfter;
            public ObjectRendererCollection ObjectRenderers { get; }
            public object Render(MarkdownObject markdownObject)
            {
                return null;
            }
        }
    }
}
