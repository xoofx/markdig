using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;
using NUnit.Framework;
using System.IO;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    [TestFixture]
    public class TestNullCharacterInline
    {
        [TestCase("\0", "\uFFFD")]
        [TestCase("\0p", "\uFFFDp")]
        [TestCase("p\0", "p\uFFFD")]
        [TestCase("p\0p", "p\uFFFDp")]
        [TestCase("p\0\0p", "p\uFFFD\uFFFDp")] // I promise you, this was not intentional
        public void Test(string value, string expected)
        {
            RoundTrip(value, expected);
        }

        // this method is copied intentionally to ensure all other tests
        // do not unintentionally use the expected parameter
        private static void RoundTrip(string markdown, string expected)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .EnableTrackTrivia()
                .ConfigureRoundtripRenderer()
                .Build();

            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);
            var result = Markdown.ToHtml(markdownDocument, pipeline);

            Assert.AreEqual(expected, result);
        }
    }
}
