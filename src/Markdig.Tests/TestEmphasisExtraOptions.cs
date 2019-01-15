using NUnit.Framework;
using Markdig.Extensions.EmphasisExtras;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestEmphasisExtraOptions
    {
        [Test]
        public void DisableSubscript1()
        {
            TestParser.TestSpec("~~test~~", "<p><del>test</del></p>", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Strikethrough).Build());
        }

        [Test]
        public void DisableSubscript2()
        {
            TestParser.TestSpec("~test~", "<p>~test~</p>", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Strikethrough).Build());
        }

        [Test]
        public void DisableStrikethrough()
        {
            TestParser.TestSpec("~~test~~", "<p><sup>~test~</sup></p>", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Subscript).Build());
        }
    }
}