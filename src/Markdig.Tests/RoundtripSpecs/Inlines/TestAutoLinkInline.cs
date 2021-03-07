using NUnit.Framework;
using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    [TestFixture]
    public class TestAutoLinkInline
    {
        [TestCase("<http://a>")]
        [TestCase(" <http://a>")]
        [TestCase("<http://a> ")]
        [TestCase(" <http://a> ")]
        [TestCase("<example@example.com>")]
        [TestCase(" <example@example.com>")]
        [TestCase("<example@example.com> ")]
        [TestCase(" <example@example.com> ")]
        [TestCase("p http://a p")]
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
