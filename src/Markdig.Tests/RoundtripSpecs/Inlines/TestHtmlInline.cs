using NUnit.Framework;
using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    [TestFixture]
    public class TestHtmlInline
    {
        [TestCase("<em>f</em>")]
        [TestCase("<em> f</em>")]
        [TestCase("<em>f </em>")]
        [TestCase("<em> f </em>")]
        [TestCase("<b>p</b>")]
        [TestCase("<b></b>")]
        [TestCase("<b> </b>")]
        [TestCase("<b>  </b>")]
        [TestCase("<b>   </b>")]
        [TestCase("<b>\t</b>")]
        [TestCase("<b> \t</b>")]
        [TestCase("<b>\t </b>")]
        [TestCase("<b> \t </b>")]
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
