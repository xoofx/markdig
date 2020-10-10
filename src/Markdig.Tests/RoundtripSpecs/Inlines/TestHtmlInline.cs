using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    [TestFixture]
    public class TestHtmlInline
    {
        [TestCase(" &#xcab; ")]
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
