using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs;

[TestFixture]
public class TestLinkReferenceDefinition
{
    [TestCase(@"[a]: /r")]
    [TestCase(@" [a]: /r")]
    [TestCase(@"  [a]: /r")]
    [TestCase(@"   [a]: /r")]

    [TestCase(@"[a]:  /r")]
    [TestCase(@" [a]:  /r")]
    [TestCase(@"  [a]:  /r")]
    [TestCase(@"   [a]:  /r")]

    [TestCase(@"[a]:  /r ")]
    [TestCase(@" [a]:  /r ")]
    [TestCase(@"  [a]:  /r ")]
    [TestCase(@"   [a]:  /r ")]

    [TestCase(@"[a]: /r ""l""")]
    [TestCase(@"[a]:  /r ""l""")]
    [TestCase(@"[a]: /r  ""l""")]
    [TestCase(@"[a]: /r ""l"" ")]
    [TestCase(@"[a]:  /r  ""l""")]
    [TestCase(@"[a]:  /r  ""l"" ")]

    [TestCase(@" [a]: /r ""l""")]
    [TestCase(@" [a]:  /r ""l""")]
    [TestCase(@" [a]: /r  ""l""")]
    [TestCase(@" [a]: /r ""l"" ")]
    [TestCase(@" [a]:  /r  ""l""")]
    [TestCase(@" [a]:  /r  ""l"" ")]

    [TestCase(@"  [a]: /r ""l""")]
    [TestCase(@"  [a]:  /r ""l""")]
    [TestCase(@"  [a]: /r  ""l""")]
    [TestCase(@"  [a]: /r ""l"" ")]
    [TestCase(@"  [a]:  /r  ""l""")]
    [TestCase(@"  [a]:  /r  ""l"" ")]

    [TestCase(@"   [a]: /r ""l""")]
    [TestCase(@"   [a]:  /r ""l""")]
    [TestCase(@"   [a]: /r  ""l""")]
    [TestCase(@"   [a]: /r ""l"" ")]
    [TestCase(@"   [a]:  /r  ""l""")]
    [TestCase(@"   [a]:  /r  ""l"" ")]

    [TestCase("[a]:\t/r")]
    [TestCase("[a]:\t/r\t")]
    [TestCase("[a]:\t/r\t\"l\"")]
    [TestCase("[a]:\t/r\t\"l\"\t")]

    [TestCase("[a]: \t/r")]
    [TestCase("[a]: \t/r\t")]
    [TestCase("[a]: \t/r\t\"l\"")]
    [TestCase("[a]: \t/r\t\"l\"\t")]

    [TestCase("[a]:\t /r")]
    [TestCase("[a]:\t /r\t")]
    [TestCase("[a]:\t /r\t\"l\"")]
    [TestCase("[a]:\t /r\t\"l\"\t")]

    [TestCase("[a]: \t /r")]
    [TestCase("[a]: \t /r\t")]
    [TestCase("[a]: \t /r\t\"l\"")]
    [TestCase("[a]: \t /r\t\"l\"\t")]

    [TestCase("[a]:\t/r \t")]
    [TestCase("[a]:\t/r \t\"l\"")]
    [TestCase("[a]:\t/r \t\"l\"\t")]

    [TestCase("[a]: \t/r")]
    [TestCase("[a]: \t/r \t")]
    [TestCase("[a]: \t/r \t\"l\"")]
    [TestCase("[a]: \t/r \t\"l\"\t")]

    [TestCase("[a]:\t /r")]
    [TestCase("[a]:\t /r \t")]
    [TestCase("[a]:\t /r \t\"l\"")]
    [TestCase("[a]:\t /r \t\"l\"\t")]

    [TestCase("[a]: \t /r")]
    [TestCase("[a]: \t /r \t")]
    [TestCase("[a]: \t /r \t\"l\"")]
    [TestCase("[a]: \t /r \t\"l\"\t")]

    [TestCase("[a]:\t/r\t ")]
    [TestCase("[a]:\t/r\t \"l\"")]
    [TestCase("[a]:\t/r\t \"l\"\t")]

    [TestCase("[a]: \t/r")]
    [TestCase("[a]: \t/r\t ")]
    [TestCase("[a]: \t/r\t \"l\"")]
    [TestCase("[a]: \t/r\t \"l\"\t")]

    [TestCase("[a]:\t /r")]
    [TestCase("[a]:\t /r\t ")]
    [TestCase("[a]:\t /r\t \"l\"")]
    [TestCase("[a]:\t /r\t \"l\"\t")]

    [TestCase("[a]: \t /r")]
    [TestCase("[a]: \t /r\t ")]
    [TestCase("[a]: \t /r\t \"l\"")]
    [TestCase("[a]: \t /r\t \"l\"\t")]

    [TestCase("[a]:\t/r \t ")]
    [TestCase("[a]:\t/r \t \"l\"")]
    [TestCase("[a]:\t/r \t \"l\"\t")]

    [TestCase("[a]: \t/r")]
    [TestCase("[a]: \t/r \t ")]
    [TestCase("[a]: \t/r \t \"l\"")]
    [TestCase("[a]: \t/r \t \"l\"\t")]

    [TestCase("[a]:\t /r")]
    [TestCase("[a]:\t /r \t ")]
    [TestCase("[a]:\t /r \t \"l\"")]
    [TestCase("[a]:\t /r \t \"l\"\t")]

    [TestCase("[a]: \t /r")]
    [TestCase("[a]: \t /r \t ")]
    [TestCase("[a]: \t /r \t \"l\"")]
    [TestCase("[a]: \t /r \t \"l\"\t")]
    public void Test(string value)
    {
        RoundTrip(value);
    }

    [TestCase("[a]: /r\n[b]: /r\n")]
    [TestCase("[a]: /r\n[b]: /r\n[c] /r\n")]
    public void TestMultiple(string value)
    {
        RoundTrip(value);
    }

    [TestCase("[a]:\f/r\f\"l\"")]
    [TestCase("[a]:\v/r\v\"l\"")]
    public void TestUncommonWhitespace(string value)
    {
        RoundTrip(value);
    }

    [TestCase("[a]:\n/r\n\"t\"")]
    [TestCase("[a]:\n/r\r\"t\"")]
    [TestCase("[a]:\n/r\r\n\"t\"")]

    [TestCase("[a]:\r/r\n\"t\"")]
    [TestCase("[a]:\r/r\r\"t\"")]
    [TestCase("[a]:\r/r\r\n\"t\"")]

    [TestCase("[a]:\r\n/r\n\"t\"")]
    [TestCase("[a]:\r\n/r\r\"t\"")]
    [TestCase("[a]:\r\n/r\r\n\"t\"")]

    [TestCase("[a]:\n/r\n\"t\nt\"")]
    [TestCase("[a]:\n/r\n\"t\rt\"")]
    [TestCase("[a]:\n/r\n\"t\r\nt\"")]

    [TestCase("[a]:\r\n  /r\t \n \t \"t\r\nt\"   ")]
    [TestCase("[a]:\n/r\n\n[a],")]
    [TestCase("[a]: /r\n[b]: /r\n\n[a],")]
    public void TestNewlines(string value)
    {
        RoundTrip(value);
    }

    [TestCase("[ a]: /r")]
    [TestCase("[a ]: /r")]
    [TestCase("[ a ]: /r")]
    [TestCase("[  a]: /r")]
    [TestCase("[  a ]: /r")]
    [TestCase("[a  ]: /r")]
    [TestCase("[ a  ]: /r")]
    [TestCase("[  a  ]: /r")]
    [TestCase("[a a]: /r")]
    [TestCase("[a\va]: /r")]
    [TestCase("[a\fa]: /r")]
    [TestCase("[a\ta]: /r")]
    [TestCase("[\va]: /r")]
    [TestCase("[\fa]: /r")]
    [TestCase("[\ta]: /r")]
    [TestCase(@"[\]]: /r")]
    public void TestLabel(string value)
    {
        RoundTrip(value);
    }

    [TestCase("[a]: /r ()")]
    [TestCase("[a]: /r (t)")]
    [TestCase("[a]: /r ( t)")]
    [TestCase("[a]: /r (t )")]
    [TestCase("[a]: /r ( t )")]

    [TestCase("[a]: /r ''")]
    [TestCase("[a]: /r 't'")]
    [TestCase("[a]: /r ' t'")]
    [TestCase("[a]: /r 't '")]
    [TestCase("[a]: /r ' t '")]
    public void Test_Title(string value)
    {
        RoundTrip(value);
    }

    [TestCase("[a]: /r\n===\n[a]")]
    public void TestSetextHeader(string value)
    {
        RoundTrip(value);
    }
}
