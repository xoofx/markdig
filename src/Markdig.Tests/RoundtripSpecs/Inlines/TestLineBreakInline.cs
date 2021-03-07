using NUnit.Framework;
using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    [TestFixture]
    public class TestLineBreakInline
    {
        [TestCase("p\n")]
        [TestCase("p\r\n")]
        [TestCase("p\r")]
        [TestCase("[]() ![]()  ``  ` `  `  `  ![]()   ![]()")]
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
