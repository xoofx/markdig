using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs
{
    [TestFixture]
    public class TestQuoteBlock
    {
        [TestCase(">q")]
        [TestCase(" >q")]
        [TestCase("  >q")]
        [TestCase("   >q")]
        [TestCase("> q")]
        [TestCase(" > q")]
        [TestCase("  > q")]
        [TestCase("   > q")]
        [TestCase(">  q")]
        [TestCase(" >  q")]
        [TestCase("  >  q")]
        [TestCase("   >  q")]

        [TestCase(">q\n>q")]
        [TestCase(">q\n >q")]
        [TestCase(">q\n  >q")]
        [TestCase(">q\n   >q")]
        [TestCase(">q\n> q")]
        [TestCase(">q\n > q")]
        [TestCase(">q\n  > q")]
        [TestCase(">q\n   > q")]
        [TestCase(">q\n>  q")]
        [TestCase(">q\n >  q")]
        [TestCase(">q\n  >  q")]
        [TestCase(">q\n   >  q")]

        [TestCase(" >q\n>q")]
        [TestCase(" >q\n >q")]
        [TestCase(" >q\n  >q")]
        [TestCase(" >q\n   >q")]
        [TestCase(" >q\n> q")]
        [TestCase(" >q\n > q")]
        [TestCase(" >q\n  > q")]
        [TestCase(" >q\n   > q")]
        [TestCase(" >q\n>  q")]
        [TestCase(" >q\n >  q")]
        [TestCase(" >q\n  >  q")]
        [TestCase(" >q\n   >  q")]

        [TestCase("  >q\n>q")]
        [TestCase("  >q\n >q")]
        [TestCase("  >q\n  >q")]
        [TestCase("  >q\n   >q")]
        [TestCase("  >q\n> q")]
        [TestCase("  >q\n > q")]
        [TestCase("  >q\n  > q")]
        [TestCase("  >q\n   > q")]
        [TestCase("  >q\n>  q")]
        [TestCase("  >q\n >  q")]
        [TestCase("  >q\n  >  q")]
        [TestCase("  >q\n   >  q")]

        [TestCase("> q\n>q")]
        [TestCase("> q\n >q")]
        [TestCase("> q\n  >q")]
        [TestCase("> q\n   >q")]
        [TestCase("> q\n> q")]
        [TestCase("> q\n > q")]
        [TestCase("> q\n  > q")]
        [TestCase("> q\n   > q")]
        [TestCase("> q\n>  q")]
        [TestCase("> q\n >  q")]
        [TestCase("> q\n  >  q")]
        [TestCase("> q\n   >  q")]

        [TestCase(" > q\n>q")]
        [TestCase(" > q\n >q")]
        [TestCase(" > q\n  >q")]
        [TestCase(" > q\n   >q")]
        [TestCase(" > q\n> q")]
        [TestCase(" > q\n > q")]
        [TestCase(" > q\n  > q")]
        [TestCase(" > q\n   > q")]
        [TestCase(" > q\n>  q")]
        [TestCase(" > q\n >  q")]
        [TestCase(" > q\n  >  q")]
        [TestCase(" > q\n   >  q")]

        [TestCase("  > q\n>q")]
        [TestCase("  > q\n >q")]
        [TestCase("  > q\n  >q")]
        [TestCase("  > q\n   >q")]
        [TestCase("  > q\n> q")]
        [TestCase("  > q\n > q")]
        [TestCase("  > q\n  > q")]
        [TestCase("  > q\n   > q")]
        [TestCase("  > q\n>  q")]
        [TestCase("  > q\n >  q")]
        [TestCase("  > q\n  >  q")]
        [TestCase("  > q\n   >  q")]

        [TestCase("   > q\n>q")]
        [TestCase("   > q\n >q")]
        [TestCase("   > q\n  >q")]
        [TestCase("   > q\n   >q")]
        [TestCase("   > q\n> q")]
        [TestCase("   > q\n > q")]
        [TestCase("   > q\n  > q")]
        [TestCase("   > q\n   > q")]
        [TestCase("   > q\n>  q")]
        [TestCase("   > q\n >  q")]
        [TestCase("   > q\n  >  q")]
        [TestCase("   > q\n   >  q")]

        [TestCase(">  q\n>q")]
        [TestCase(">  q\n >q")]
        [TestCase(">  q\n  >q")]
        [TestCase(">  q\n   >q")]
        [TestCase(">  q\n> q")]
        [TestCase(">  q\n > q")]
        [TestCase(">  q\n  > q")]
        [TestCase(">  q\n   > q")]
        [TestCase(">  q\n>  q")]
        [TestCase(">  q\n >  q")]
        [TestCase(">  q\n  >  q")]
        [TestCase(">  q\n   >  q")]

        [TestCase(" >  q\n>q")]
        [TestCase(" >  q\n >q")]
        [TestCase(" >  q\n  >q")]
        [TestCase(" >  q\n   >q")]
        [TestCase(" >  q\n> q")]
        [TestCase(" >  q\n > q")]
        [TestCase(" >  q\n  > q")]
        [TestCase(" >  q\n   > q")]
        [TestCase(" >  q\n>  q")]
        [TestCase(" >  q\n >  q")]
        [TestCase(" >  q\n  >  q")]
        [TestCase(" >  q\n   >  q")]

        [TestCase("  >  q\n>q")]
        [TestCase("  >  q\n >q")]
        [TestCase("  >  q\n  >q")]
        [TestCase("  >  q\n   >q")]
        [TestCase("  >  q\n> q")]
        [TestCase("  >  q\n > q")]
        [TestCase("  >  q\n  > q")]
        [TestCase("  >  q\n   > q")]
        [TestCase("  >  q\n>  q")]
        [TestCase("  >  q\n >  q")]
        [TestCase("  >  q\n  >  q")]
        [TestCase("  >  q\n   >  q")]

        [TestCase("   >  q\n>q")]
        [TestCase("   >  q\n >q")]
        [TestCase("   >  q\n  >q")]
        [TestCase("   >  q\n   >q")]
        [TestCase("   >  q\n> q")]
        [TestCase("   >  q\n > q")]
        [TestCase("   >  q\n  > q")]
        [TestCase("   >  q\n   > q")]
        [TestCase("   >  q\n>  q")]
        [TestCase("   >  q\n >  q")]
        [TestCase("   >  q\n  >  q")]
        [TestCase("   >  q\n   >  q")]

        [TestCase(">q\n>q\n>q")]
        [TestCase(">q\n>\n>q")]
        [TestCase(">q\np\n>q")]
        [TestCase(">q\n>\n>\n>q")]
        [TestCase(">q\n>\n>\n>\n>q")]
        [TestCase(">q\n>\n>q\n>\n>q")]
        [TestCase("> **q**\n>p\n")]
        [TestCase("p\n\n> **q**\n>p\n")]

        [TestCase("> q\np\n> q")] // lazy
        [TestCase("> q\n> q\np")] // lazy

        [TestCase(">>q")]
        [TestCase(" >  >   q")]
        public void Test(string value)
        {
            RoundTrip(value);
        }

        [TestCase(" >     q")]
        [TestCase(" > \tq")]
        public void TestCodeBlock(string value)
        {
            RoundTrip(value);
        }

        [TestCase("\n> q")]
        [TestCase("\n> q\n")]
        [TestCase("\n> q\n\n")]
        [TestCase("> q\n\np")]
        [TestCase("p\n\n> q\n\n# h")]

        //https://github.com/lunet-io/markdig/issues/480
        //[TestCase(">\np")]
        //[TestCase(">**b**\n>\n>p\n>\np\n")]
        public void TestParagraph(string value)
        {
            RoundTrip(value);
        }

        [TestCase("> q\n\n# h\n")]
        public void TestAtxHeader(string value)
        {
            RoundTrip(value);
        }

        [TestCase(">- i1\n>- i2\n")]
        [TestCase("> **p** p\n>- i1\n>- i2\n")]
        public void TestUnorderedList(string value)
        {
            RoundTrip(value);
        }
    }
}
