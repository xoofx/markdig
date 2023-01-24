using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs;

[TestFixture]
public class TestHtmlBlock
{
    [TestCase("<br>")]
    [TestCase("<br>\n")]
    [TestCase("<br>\n\n")]
    [TestCase("<div></div>\n\n# h")]
    [TestCase("p\n\n<div></div>\n")]
    [TestCase("<div></div>\n\n# h")]
    public void Test(string value)
    {
        RoundTrip(value);
    }
}
