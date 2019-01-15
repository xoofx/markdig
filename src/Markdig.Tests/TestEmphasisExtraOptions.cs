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
            TestParser.TestSpec("~~test~~", "<del>test</del>", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Strikethrough).Build());
        }

        [Test]
        public void DisableSubscript2()
        {
            TestParser.TestSpec("~test~", "~test~", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Strikethrough).Build());
        }

        [Test]
        public void DisableStrikethrough()
        {
            TestParser.TestSpec("~~test~~", "<sup>~test~</sup>", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Subscript).Build());
        }
    }
}