using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs
{
    [TestFixture]
    public class TestCodeBlock
    {
        // A codeblock is indented with 4 spaces. After the 4th space, whitespace is interpreted as content.
        // l = line
        [TestCase("    l")]
        [TestCase("     l")]
        [TestCase("\tl")]
        [TestCase("\tl1\n    l1")]
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
