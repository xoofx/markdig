using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs;

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
    [TestCase("p\n\n> **q**\n>p\n")]

    [TestCase("> q\np\n> q")] // lazy
    [TestCase("> q\n> q\np")] // lazy

    [TestCase(">>q")]
    [TestCase(" >  >   q")]

    [TestCase("> **q**\n>p\n")]
    [TestCase("> **q**")]
    public void Test(string value)
    {
        RoundTrip(value);
    }

    [TestCase(">     q")] // 5
    [TestCase(">      q")] // 6
    [TestCase(" >     q")] //5
    [TestCase(" >      q")] //6
    [TestCase(" > \tq")]
    [TestCase(">     q\n>     q")] // 5, 5
    [TestCase(">     q\n>      q")] // 5, 6
    [TestCase(">      q\n>     q")] // 6, 5
    [TestCase(">      q\n>      q")] // 6, 6
    [TestCase(">     q\n\n>     5")] // 5, 5
    public void TestIndentedCodeBlock(string value)
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

    [TestCase(">- i")]
    [TestCase("> - i")]
    [TestCase(">- i\n>- i")]
    [TestCase(">- >p")]
    [TestCase("> - >p")]
    [TestCase(">- i1\n>- i2\n")]
    [TestCase("> **p** p\n>- i1\n>- i2\n")]
    public void TestUnorderedList(string value)
    {
        RoundTrip(value);
    }

    [TestCase("> *q*\n>p\n")]
    [TestCase("> *q*")]
    public void TestEmphasis(string value)
    {
        RoundTrip(value);
    }

    [TestCase("> **q**\n>p\n")]
    [TestCase("> **q**")]
    public void TestStrongEmphasis(string value)
    {
        RoundTrip(value);
    }

    [TestCase(">p\n")]
    [TestCase(">p\r")]
    [TestCase(">p\r\n")]

    [TestCase(">p\n>p")]
    [TestCase(">p\r>p")]
    [TestCase(">p\r\n>p")]

    [TestCase(">p\n>p\n")]
    [TestCase(">p\r>p\n")]
    [TestCase(">p\r\n>p\n")]

    [TestCase(">p\n>p\r")]
    [TestCase(">p\r>p\r")]
    [TestCase(">p\r\n>p\r")]

    [TestCase(">p\n>p\r\n")]
    [TestCase(">p\r>p\r\n")]
    [TestCase(">p\r\n>p\r\n")]
    public void TestNewline(string value)
    {
        RoundTrip(value);
    }

    [TestCase(">\n>q")]
    [TestCase(">\n>\n>q")]
    [TestCase(">q\n>\n>q")]
    [TestCase(">q\n>\n>\n>q")]
    [TestCase(">q\n> \n>q")]
    [TestCase(">q\n>  \n>q")]
    [TestCase(">q\n>   \n>q")]
    [TestCase(">q\n>\t\n>q")]
    [TestCase(">q\n>\v\n>q")]
    [TestCase(">q\n>\f\n>q")]
    public void TestEmptyLines(string value)
    {
        RoundTrip(value);
    }
}
