using System.IO;
using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;
using NUnit.Framework;
using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    [TestFixture]
    public class TestAutoLinkInline
    {
        [TestCase("<http://a>")]
        [TestCase(" <http://a>")]
        [TestCase("<http://a> ")]
        [TestCase(" <http://a> ")]
        [TestCase("<example@example.com>")]
        [TestCase(" <example@example.com>")]
        [TestCase("<example@example.com> ")]
        [TestCase(" <example@example.com> ")]
        [TestCase("p http://a p")]
        public void Test(string value)
        {
            RoundTrip(value);
        }

        [TestCase("http://example.com/", "[http://example.com/](http://example.com/)")]
        [TestCase("www.example.com", "[www.example.com](http://www.example.com)")]
        [TestCase("mailto:user@example.com", "[user@example.com](mailto:user@example.com)")]
        public void AutoLinksKeepUrlWhenRoundTripped(string markdown, string expected)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .DisableHtml()
                .UseAutoLinks()
                .EnableTrackTrivia()
                .Build();
            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);
            var sw = new StringWriter();
            var rr = new RoundtripRenderer(sw);

            rr.Write(markdownDocument);

            Assert.AreEqual(expected, sw.ToString());
        }
    }
}
