using NUnit.Framework;
using Markdig.Extensions.EmphasisExtras;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestEmphasisExtraOptions
    {
        [Test]
        public void StrikethroughThenSubscript()
        {
            TestParser.TestSpec("~~~test~~~", "<del><sup>test</sup></del>", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Strikethrough | EmphasisExtraOptions.Subscript).Build());
        }

        [Test]
        public void DisableSubscript()
        {
            TestParser.TestSpec("~~~test~~~", "<del>~test~</del>", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Strikethrough).Build());
        }

        [Test]
        public void DisableStrikethrough()
        {
            TestParser.TestSpec("~~~test~~~", "<sup>~~test~~</sup>", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Subscript).Build());
        }
    }
}