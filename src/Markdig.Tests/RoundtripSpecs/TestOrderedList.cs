using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs;

[TestFixture]
public class TestOrderedList
{
    [TestCase("1. i")]
    [TestCase("1.  i")]
    [TestCase("1. i ")]
    [TestCase("1.  i ")]
    [TestCase("1.  i  ")]

    [TestCase(" 1. i")]
    [TestCase(" 1.  i")]
    [TestCase(" 1. i ")]
    [TestCase(" 1.  i ")]
    [TestCase(" 1.  i  ")]

    [TestCase("  1. i")]
    [TestCase("  1.  i")]
    [TestCase("  1. i ")]
    [TestCase("  1.  i ")]
    [TestCase("  1.  i  ")]

    [TestCase("   1. i")]
    [TestCase("   1.  i")]
    [TestCase("   1. i ")]
    [TestCase("   1.  i ")]
    [TestCase("   1.  i  ")]

    [TestCase("1. i\n")]
    [TestCase("1.  i\n")]
    [TestCase("1. i \n")]
    [TestCase("1.  i \n")]
    [TestCase("1.  i  \n")]

    [TestCase(" 1. i\n")]
    [TestCase(" 1.  i\n")]
    [TestCase(" 1. i \n")]
    [TestCase(" 1.  i \n")]
    [TestCase(" 1.  i  \n")]

    [TestCase("  1. i\n")]
    [TestCase("  1.  i\n")]
    [TestCase("  1. i \n")]
    [TestCase("  1.  i \n")]
    [TestCase("  1.  i  \n")]

    [TestCase("   1. i\n")]
    [TestCase("   1.  i\n")]
    [TestCase("   1. i \n")]
    [TestCase("   1.  i \n")]
    [TestCase("   1.  i  \n")]

    [TestCase("1. i\n2. j")]
    [TestCase("1.  i\n2. j")]
    [TestCase("1. i \n2. j")]
    [TestCase("1.  i \n2. j")]
    [TestCase("1.  i  \n2. j")]

    [TestCase(" 1. i\n2. j")]
    [TestCase(" 1.  i\n2. j")]
    [TestCase(" 1. i \n2. j")]
    [TestCase(" 1.  i \n2. j")]
    [TestCase(" 1.  i  \n2. j")]

    [TestCase("  1. i\n2. j")]
    [TestCase("  1.  i\n2. j")]
    [TestCase("  1. i \n2. j")]
    [TestCase("  1.  i \n2. j")]
    [TestCase("  1.  i  \n2. j")]

    [TestCase("   1. i\n2. j")]
    [TestCase("   1.  i\n2. j")]
    [TestCase("   1. i \n2. j")]
    [TestCase("   1.  i \n2. j")]
    [TestCase("   1.  i  \n2. j")]

    [TestCase("1. i\n2. j\n")]
    [TestCase("1.  i\n2. j\n")]
    [TestCase("1. i \n2. j\n")]
    [TestCase("1.  i \n2. j\n")]
    [TestCase("1.  i  \n2. j\n")]

    [TestCase(" 1. i\n2. j\n")]
    [TestCase(" 1.  i\n2. j\n")]
    [TestCase(" 1. i \n2. j\n")]
    [TestCase(" 1.  i \n2. j\n")]
    [TestCase(" 1.  i  \n2. j\n")]

    [TestCase("  1. i\n2. j\n")]
    [TestCase("  1.  i\n2. j\n")]
    [TestCase("  1. i \n2. j\n")]
    [TestCase("  1.  i \n2. j\n")]
    [TestCase("  1.  i  \n2. j\n")]

    [TestCase("   1. i\n2. j\n")]
    [TestCase("   1.  i\n2. j\n")]
    [TestCase("   1. i \n2. j\n")]
    [TestCase("   1.  i \n2. j\n")]
    [TestCase("   1.  i  \n2. j\n")]

    [TestCase("1. i\n2. j\n3. k")]
    [TestCase("1. i\n2. j\n3. k\n")]
    public void Test(string value)
    {
        RoundTrip(value);
    }

    [TestCase("10. i")]
    [TestCase("11. i")]
    [TestCase("10. i\n12. i")]
    [TestCase("2. i\n3. i")]
    public void Test_MoreThenOneStart(string value)
    {
        RoundTrip(value);
    }


    [TestCase("\n1. i")]
    [TestCase("\r1. i")]
    [TestCase("\r\n1. i")]

    [TestCase("\n1. i\n")]
    [TestCase("\r1. i\n")]
    [TestCase("\r\n1. i\n")]

    [TestCase("\n1. i\r")]
    [TestCase("\r1. i\r")]
    [TestCase("\r\n1. i\r")]

    [TestCase("\n1. i\r\n")]
    [TestCase("\r1. i\r\n")]
    [TestCase("\r\n1. i\r\n")]

    [TestCase("1. i\n2. i")]
    [TestCase("\n1. i\n2. i")]
    [TestCase("\r1. i\n2. i")]
    [TestCase("\r\n1. i\n2. i")]

    [TestCase("1. i\r2. i")]
    [TestCase("\n1. i\r2. i")]
    [TestCase("\r1. i\r2. i")]
    [TestCase("\r\n1. i\r2. i")]

    [TestCase("1. i\r\n2. i")]
    [TestCase("\n1. i\r\n2. i")]
    [TestCase("\r1. i\r\n2. i")]
    [TestCase("\r\n1. i\r\n2. i")]

    [TestCase("1. i\n2. i\n")]
    [TestCase("\n1. i\n2. i\n")]
    [TestCase("\r1. i\n2. i\n")]
    [TestCase("\r\n1. i\n2. i\n")]

    [TestCase("1. i\r2. i\r")]
    [TestCase("\n1. i\r2. i\r")]
    [TestCase("\r1. i\r2. i\r")]
    [TestCase("\r\n1. i\r2. i\r")]

    [TestCase("1. i\r\n2. i\r\n")]
    [TestCase("\n1. i\r\n2. i\r\n")]
    [TestCase("\r1. i\r\n2. i\r\n")]
    [TestCase("\r\n1. i\r\n2. i\r\n")]
    public void TestNewline(string value)
    {
        RoundTrip(value);
    }

    [TestCase("1. i\n  1. i")]
    [TestCase("1. i\n  1. i\n")]
    [TestCase("1. i\n  1. i\n  2. i")]
    [TestCase("1. i\n  2. i\n  3. i")]

    [TestCase("1. i\n\t1. i")]
    [TestCase("1. i\n\t1. i\n2. i")]
    public void TestMultipleLevels(string value)
    {
        RoundTrip(value);
    }

    [TestCase("1.     c")]
    [TestCase("1.      c")]
    public void Test_IndentedCodeBlock(string value)
    {
        RoundTrip(value);
    }
}
