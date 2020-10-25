using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;
using NUnit.Framework;
using System.IO;

namespace Markdig.Tests
{
    internal static class TestRoundtrip
    {
        internal static void TestSpec(string markdownText, string expected, string extensions)
        {
            RoundTrip(markdownText);
        }

        internal static void RoundTrip(string markdown)
        {
            var pipelineBuilder = new MarkdownPipelineBuilder();
            MarkdownPipeline pipeline = pipelineBuilder.Build();
            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline, trackTrivia: true);
            var sw = new StringWriter();
            var nr = new RoundtripRenderer(sw);

            nr.Write(markdownDocument);

            Assert.AreEqual(markdown, sw.ToString());
        }
    }
}
