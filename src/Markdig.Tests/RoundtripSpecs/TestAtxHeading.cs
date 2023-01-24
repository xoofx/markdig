using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs;

[TestFixture]
public class TestAtxHeading
{
    [TestCase("# h")]
    [TestCase("# h ")]
    [TestCase("# h\n#h")]
    [TestCase("# h\n #h")]
    [TestCase("# h\n # h")]
    [TestCase("# h\n # h ")]
    [TestCase(" #  h   \n    #     h      ")]
    public void Test(string value)
    {
        RoundTrip(value);
    }

    [TestCase("\n# h\n\np")]
    [TestCase("\n# h\n\np\n")]
    [TestCase("\n# h\n\np\n\n")]
    [TestCase("\n\n# h\n\np\n\n")]
    [TestCase("\n\n# h\np\n\n")]
    [TestCase("\n\n# h\np\n\n")]
    public void TestParagraph(string value)
    {
        RoundTrip(value);
    }

    [TestCase("\n# h")]
    [TestCase("\n# h\n")]
    [TestCase("\n# h\r")]
    [TestCase("\n# h\r\n")]

    [TestCase("\r# h")]
    [TestCase("\r# h\n")]
    [TestCase("\r# h\r")]
    [TestCase("\r# h\r\n")]

    [TestCase("\r\n# h")]
    [TestCase("\r\n# h\n")]
    [TestCase("\r\n# h\r")]
    [TestCase("\r\n# h\r\n")]

    [TestCase("# h\n\n ")]
    [TestCase("# h\n\n  ")]
    [TestCase("# h\n\n   ")]
    public void TestNewline(string value)
    {
        RoundTrip(value);
    }
}
