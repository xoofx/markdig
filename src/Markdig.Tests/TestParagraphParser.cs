using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestParagraphParser
    {
        [Test]
        public void TestWhitespaceBefore()
        {
            var markdown = " This is a paragraph.  ";

            var pipelineBuilder = new MarkdownPipelineBuilder();
            MarkdownPipeline pipeline = pipelineBuilder.Build();

            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);
            var paragraphBlock = markdownDocument[0] as ParagraphBlock;

            var sw = new StringWriter();
            var nr = new NormalizeRenderer(sw);
            nr.Write(markdownDocument);

            Assert.AreEqual(markdown, sw.ToString());
        }

        [Test]
        public void TestNewLinesBeforeAndAfter()
        {
            var markdown = " \n  \nLine2\n\nLine1\n\n";
            //var markdown = "\r\nLine2\r\n\r\nLine1\r\n\r\n";

            var pipelineBuilder = new MarkdownPipelineBuilder();
            MarkdownPipeline pipeline = pipelineBuilder.Build();

            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);
            var paragraphBlock = markdownDocument[0] as ParagraphBlock;

            var sw = new StringWriter();
            var nr = new NormalizeRenderer(sw);
            nr.Write(markdownDocument);

            Assert.AreEqual(markdown, sw.ToString());
        }

        private void RoundTrip(string markdown)
        {
            var pipelineBuilder = new MarkdownPipelineBuilder();
            MarkdownPipeline pipeline = pipelineBuilder.Build();
            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);
            var sw = new StringWriter();
            var nr = new NormalizeRenderer(sw);

            nr.Write(markdownDocument);

            Assert.AreEqual(markdown, sw.ToString());
        }

        [Test]
        public void TestNewLinesBeforeAndAfter2()
        {
            RoundTrip("\n# H1\n\nLine1");
            RoundTrip("\n# H1\n\nLine1\n");
            RoundTrip("\n# H1\n\nLine1\n\n");
            RoundTrip("\n\n# H1\n\nLine1\n\n");
            RoundTrip("\n\n# H1\nLine1\n\n");
        }
    }
}
