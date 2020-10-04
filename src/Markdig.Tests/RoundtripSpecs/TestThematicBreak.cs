using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs
{
    [TestFixture]
    public class TestThematicBreak
    {
        [TestCase("---")]
        [TestCase(" ---")]
        [TestCase("  ---")]
        [TestCase("   ---")]
        [TestCase("--- ")]
        [TestCase(" --- ")]
        [TestCase("  --- ")]
        [TestCase("   --- ")]
        [TestCase("- - -")]
        [TestCase(" - - -")]
        [TestCase(" - - - ")]
        [TestCase("-- -")]
        [TestCase("---\n")]
        [TestCase("---\n\n")]
        [TestCase("---\np")]
        [TestCase("---\n\np")]
        [TestCase("---\n# h")]
        //[TestCase("p\n\n---")]
        /// Note: "p\n---" is parsed as setext heading
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
