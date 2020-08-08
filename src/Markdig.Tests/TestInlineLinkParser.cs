using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using NUnit.Framework;
using System.IO;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestInlineLinkParser
    {
        [Test]
        public void Test()
        {
            string markdown = " ![  description   ](    http://example.com     )";
            var pipelineBuilder = new MarkdownPipelineBuilder();
            MarkdownPipeline pipeline = pipelineBuilder.Build();

            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);
            var paragraphBlock = markdownDocument[0] as ParagraphBlock;
            var containerInline = paragraphBlock.Inline as ContainerInline;
            var linkInline = containerInline.FirstChild as LinkInline;
            var description = linkInline.FirstChild as LiteralInline;

            var sw = new StringWriter();
            var nr = new NormalizeRenderer(sw);
            nr.Write(markdownDocument);

            Assert.AreEqual(markdown, sw.ToString());
        }

        [Test]
        public void Test1()
        {
            string markdown = " ![description](http://example.com)";
            var pipelineBuilder = new MarkdownPipelineBuilder();
            MarkdownPipeline pipeline = pipelineBuilder.Build();

            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);
            var paragraphBlock = markdownDocument[0] as ParagraphBlock;
            var containerInline = paragraphBlock.Inline as ContainerInline;
            var linkInline = containerInline.FirstChild as LinkInline;
            var description = linkInline.FirstChild as LiteralInline;

            var sw = new StringWriter();
            var nr = new NormalizeRenderer(sw);
            nr.Write(markdownDocument);

            Assert.AreEqual(markdown, sw.ToString());
        }
    }
}
