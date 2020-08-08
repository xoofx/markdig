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
    public class TestQuoteBlock
    {
        [Test]
        public void Test()
        {
            var markdown = "   >  This is a quote with whitespace before, between and after.   ";

            var pipelineBuilder = new MarkdownPipelineBuilder();
            MarkdownPipeline pipeline = pipelineBuilder.Build();

            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);
            var paragraphBlock = markdownDocument[0] as QuoteBlock;

            var sw = new StringWriter();
            var nr = new NormalizeRenderer(sw);
            nr.Write(markdownDocument);

            Assert.AreEqual(markdown, sw.ToString());
        }
    }
}
