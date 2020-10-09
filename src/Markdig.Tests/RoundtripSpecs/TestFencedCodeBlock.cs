using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs
{
    [TestFixture]
    public class TestFencedCodeBlock
    {
        [TestCase("```\nc\n```")]
        [TestCase("```\nc\n```\n")]
        [TestCase("\n```\nc\n```")]
        [TestCase("\n\n```\nc\n```")]
        [TestCase("```\nc\n```\n")]
        [TestCase("```\nc\n```\n\n")]
        [TestCase("\n```\nc\n```\n")]
        [TestCase("\n```\nc\n```\n\n")]
        [TestCase("\n\n```\nc\n```\n")]
        [TestCase("\n\n```\nc\n```\n\n")]

        [TestCase(" ```\nc\n````")]
        [TestCase("```\nc\n````")]
        [TestCase("p\n\n```\nc\n```")]
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
