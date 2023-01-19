using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs;

[TestFixture]
public class TestParagraph
{
    [TestCase("p")]
    [TestCase(" p")]
    [TestCase("p ")]
    [TestCase(" p ")]

    [TestCase("p\np")]
    [TestCase(" p\np")]
    [TestCase("p \np")]
    [TestCase(" p \np")]

    [TestCase("p\n p")]
    [TestCase(" p\n p")]
    [TestCase("p \n p")]
    [TestCase(" p \n p")]

    [TestCase("p\np ")]
    [TestCase(" p\np ")]
    [TestCase("p \np ")]
    [TestCase(" p \np ")]

    [TestCase("p\n\n p ")]
    [TestCase(" p\n\n p ")]
    [TestCase("p \n\n p ")]
    [TestCase(" p \n\n p ")]

    [TestCase("p\n\np")]
    [TestCase(" p\n\np")]
    [TestCase("p \n\np")]
    [TestCase(" p \n\np")]

    [TestCase("p\n\n p")]
    [TestCase(" p\n\n p")]
    [TestCase("p \n\n p")]
    [TestCase(" p \n\n p")]

    [TestCase("p\n\np ")]
    [TestCase(" p\n\np ")]
    [TestCase("p \n\np ")]
    [TestCase(" p \n\np ")]

    [TestCase("p\n\n p ")]
    [TestCase(" p\n\n p ")]
    [TestCase("p \n\n p ")]
    [TestCase(" p \n\n p ")]

    [TestCase("\np")]
    [TestCase("\n p")]
    [TestCase("\np ")]
    [TestCase("\n p ")]

    [TestCase("\np\np")]
    [TestCase("\n p\np")]
    [TestCase("\np \np")]
    [TestCase("\n p \np")]

    [TestCase("\np\n p")]
    [TestCase("\n p\n p")]
    [TestCase("\np \n p")]
    [TestCase("\n p \n p")]

    [TestCase("\np\np ")]
    [TestCase("\n p\np ")]
    [TestCase("\np \np ")]
    [TestCase("\n p \np ")]

    [TestCase("\np\n\n p ")]
    [TestCase("\n p\n\n p ")]
    [TestCase("\np \n\n p ")]
    [TestCase("\n p \n\n p ")]

    [TestCase("\np\n\np")]
    [TestCase("\n p\n\np")]
    [TestCase("\np \n\np")]
    [TestCase("\n p \n\np")]

    [TestCase("\np\n\n p")]
    [TestCase("\n p\n\n p")]
    [TestCase("\np \n\n p")]
    [TestCase("\n p \n\n p")]

    [TestCase("\np\n\np ")]
    [TestCase("\n p\n\np ")]
    [TestCase("\np \n\np ")]
    [TestCase("\n p \n\np ")]

    [TestCase("\np\n\n p ")]
    [TestCase("\n p\n\n p ")]
    [TestCase("\np \n\n p ")]
    [TestCase("\n p \n\n p ")]

    [TestCase("p  p")]
    [TestCase("p\tp")]
    [TestCase("p \tp")]
    [TestCase("p \t p")]
    [TestCase("p \tp")]

    // special cases
    [TestCase(" p \n\n\n\n p \n\n")]
    [TestCase("\n\np")]
    [TestCase("p\n")]
    [TestCase("p\n\n")]
    [TestCase("p\np\n p")]
    [TestCase("p\np\n p\n p")]
    public void Test(string value)
    {
        RoundTrip(value);
    }


    [TestCase("\n")]
    [TestCase("\r\n")]
    [TestCase("\r")]

    [TestCase("p\n")]
    [TestCase("p\r")]
    [TestCase("p\r\n")]

    [TestCase("p\np")]
    [TestCase("p\rp")]
    [TestCase("p\r\np")]

    [TestCase("p\np\n")]
    [TestCase("p\rp\n")]
    [TestCase("p\r\np\n")]

    [TestCase("p\np\r")]
    [TestCase("p\rp\r")]
    [TestCase("p\r\np\r")]

    [TestCase("p\np\r\n")]
    [TestCase("p\rp\r\n")]
    [TestCase("p\r\np\r\n")]

    [TestCase("\np\n")]
    [TestCase("\np\r")]
    [TestCase("\np\r\n")]

    [TestCase("\np\np")]
    [TestCase("\np\rp")]
    [TestCase("\np\r\np")]

    [TestCase("\np\np\n")]
    [TestCase("\np\rp\n")]
    [TestCase("\np\r\np\n")]

    [TestCase("\np\np\r")]
    [TestCase("\np\rp\r")]
    [TestCase("\np\r\np\r")]

    [TestCase("\np\np\r\n")]
    [TestCase("\np\rp\r\n")]
    [TestCase("\np\r\np\r\n")]

    [TestCase("\rp\n")]
    [TestCase("\rp\r")]
    [TestCase("\rp\r\n")]

    [TestCase("\rp\np")]
    [TestCase("\rp\rp")]
    [TestCase("\rp\r\np")]

    [TestCase("\rp\np\n")]
    [TestCase("\rp\rp\n")]
    [TestCase("\rp\r\np\n")]

    [TestCase("\rp\np\r")]
    [TestCase("\rp\rp\r")]
    [TestCase("\rp\r\np\r")]

    [TestCase("\rp\np\r\n")]
    [TestCase("\rp\rp\r\n")]
    [TestCase("\rp\r\np\r\n")]

    [TestCase("\r\np\n")]
    [TestCase("\r\np\r")]
    [TestCase("\r\np\r\n")]

    [TestCase("\r\np\np")]
    [TestCase("\r\np\rp")]
    [TestCase("\r\np\r\np")]

    [TestCase("\r\np\np\n")]
    [TestCase("\r\np\rp\n")]
    [TestCase("\r\np\r\np\n")]

    [TestCase("\r\np\np\r")]
    [TestCase("\r\np\rp\r")]
    [TestCase("\r\np\r\np\r")]

    [TestCase("\r\np\np\r\n")]
    [TestCase("\r\np\rp\r\n")]
    [TestCase("\r\np\r\np\r\n")]

    [TestCase("p\n")]
    [TestCase("p\n\n")]
    [TestCase("p\n\n\n")]
    [TestCase("p\n\n\n\n")]
    public void TestNewline(string value)
    {
        RoundTrip(value);
    }

    [TestCase(" \n")]
    [TestCase(" \r")]
    [TestCase(" \r\n")]

    [TestCase(" \np")]
    [TestCase(" \rp")]
    [TestCase(" \r\np")]

    [TestCase("  \np")]
    [TestCase("  \rp")]
    [TestCase("  \r\np")]

    [TestCase("   \np")]
    [TestCase("   \rp")]
    [TestCase("   \r\np")]

    [TestCase(" \n ")]
    [TestCase(" \r ")]
    [TestCase(" \r\n ")]

    [TestCase(" \np ")]
    [TestCase(" \rp ")]
    [TestCase(" \r\np ")]

    [TestCase("  \np ")]
    [TestCase("  \rp ")]
    [TestCase("  \r\np ")]

    [TestCase("   \np ")]
    [TestCase("   \rp ")]
    [TestCase("   \r\np ")]
    public void Test_WhitespaceWithNewline(string value)
    {
        RoundTrip(value);
    }
}
