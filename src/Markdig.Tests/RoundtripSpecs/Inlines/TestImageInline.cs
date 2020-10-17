using NUnit.Framework;
using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    [TestFixture]
    public class TestImageInline
    {
        [TestCase("![](a)")]
        [TestCase(" ![](a)")]
        [TestCase("![](a) ")]
        [TestCase(" ![](a) ")]
        [TestCase("   ![description](http://example.com)")]
        public void Test(string value)
        {
            RoundTrip(value);
        }

        [TestCase("paragraph   ![description](http://example.com)")]
        public void TestParagraph(string value)
        {
            RoundTrip(value);
        }
    }
}
