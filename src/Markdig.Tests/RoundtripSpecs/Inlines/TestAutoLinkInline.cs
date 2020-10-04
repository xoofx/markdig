using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    [TestFixture]
    public class TestAutoLinkInline
    {
        [TestCase("<http://a>")]
        [TestCase(" <http://a>")]
        [TestCase("<http://a> ")]
        [TestCase(" <http://a> ")]

        [TestCase("< http://a>")]
        [TestCase(" < http://a>")]
        [TestCase("< http://a> ")]
        [TestCase(" < http://a> ")]

        [TestCase("<http://a >")]
        [TestCase(" <http://a >")]
        [TestCase("<http://a > ")]
        [TestCase(" <http://a > ")]

        [TestCase("< http://a >")]
        [TestCase(" < http://a >")]
        [TestCase("< http://a > ")]
        [TestCase(" < http://a > ")]
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
