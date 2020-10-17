using NUnit.Framework;
using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    [TestFixture]
    public class TestNullCharacterInline
    {
        [TestCase("\0")]
        [TestCase("\0p")]
        [TestCase("p\0")]
        [TestCase("p\0p")]
        [TestCase("p\0\0p")] // I promise you, this was not intentional
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
