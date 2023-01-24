using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs;

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
    [TestCase("p\n\n---")]
    // Note: "p\n---" is parsed as setext heading
    public void Test(string value)
    {
        RoundTrip(value);
    }

    [TestCase("\n---")]
    [TestCase("\r---")]
    [TestCase("\r\n---")]

    [TestCase("\n---\n")]
    [TestCase("\r---\n")]
    [TestCase("\r\n---\n")]

    [TestCase("\n---\r")]
    [TestCase("\r---\r")]
    [TestCase("\r\n---\r")]

    [TestCase("\n---\r\n")]
    [TestCase("\r---\r\n")]
    [TestCase("\r\n---\r\n")]
    public void TestNewline(string value)
    {
        RoundTrip(value);
    }
}
