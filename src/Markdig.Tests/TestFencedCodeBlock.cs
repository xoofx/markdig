using Markdig.Renderers.Normalize;
using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;
using NUnit.Framework;
using System.IO;

namespace Markdig.Tests
{
    // TODO: RTP: integrate with roundtrip synthetic tests
    [TestFixture]
    public partial class TestFencedCodeBlock
    {
        [Test]
        public void CstInfoParser_ParsesHappyFlow()
        {
            string markdown = $@"
```  derpy   args    
```
";
            var pipelineBuilder = new MarkdownPipelineBuilder();
            MarkdownPipeline pipeline = pipelineBuilder.Build();

            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);
            var fencedCodeBlock = markdownDocument[0] as FencedCodeBlock;

            Assert.AreEqual('`', fencedCodeBlock.FencedChar);
            Assert.AreEqual(3, fencedCodeBlock.OpeningFencedCharCount);
            Assert.AreEqual("  ", fencedCodeBlock.WhitespaceAfterFencedChar);
            Assert.AreEqual("derpy", fencedCodeBlock.Info);
            Assert.AreEqual("   ", fencedCodeBlock.WhitespaceAfterInfo);
            Assert.AreEqual("args", fencedCodeBlock.Arguments);
            Assert.AreEqual("    ", fencedCodeBlock.WhitespaceAfterArguments);
        }

        [Test]
        public void CstInfoParser_ParsesComplicatedArguments()
        {
            string markdown = "``` derpy args  more\t args  \t\t and even more args   \t  \r\n```";
            var pipelineBuilder = new MarkdownPipelineBuilder();
            MarkdownPipeline pipeline = pipelineBuilder.Build();

            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);
            var fencedCodeBlock = markdownDocument[0] as FencedCodeBlock;

            Assert.AreEqual('`', fencedCodeBlock.FencedChar);
            Assert.AreEqual(3, fencedCodeBlock.OpeningFencedCharCount);
            Assert.AreEqual(" ", fencedCodeBlock.WhitespaceAfterFencedChar);
            Assert.AreEqual("derpy", fencedCodeBlock.Info);
            Assert.AreEqual(" ", fencedCodeBlock.WhitespaceAfterInfo);
            Assert.AreEqual("args  more\t args  \t\t and even more args", fencedCodeBlock.Arguments);
            Assert.AreEqual("   \t  ", fencedCodeBlock.WhitespaceAfterArguments);
        }

        [Test]
        public void CstInfoParser_RoundTrip()
        {
            string markdown = "``` derpy args  more\t args  \t\t and even more args   \t  \n```";
            var pipelineBuilder = new MarkdownPipelineBuilder();
            MarkdownPipeline pipeline = pipelineBuilder.Build();

            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);
            var sw = new StringWriter();
            var nr = new RoundtripRenderer(sw);
            nr.Write(markdownDocument);

            Assert.AreEqual(markdown, sw.ToString());
        }
    }
}
