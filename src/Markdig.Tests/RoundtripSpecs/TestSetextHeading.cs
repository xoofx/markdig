using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs
{
    [TestFixture]
    public class TestSetextHeading
    {
        [TestCase("h1===\n")]
        [TestCase("h2---\n")]
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
