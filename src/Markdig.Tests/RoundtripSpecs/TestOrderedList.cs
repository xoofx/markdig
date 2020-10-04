using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs
{
    [TestFixture]
    public class TestOrderedList
    {
        [TestCase("1. i")]
        [TestCase("1.  i")]
        [TestCase("1. i ")]
        [TestCase("1.  i ")]

        [TestCase("1. i1\n2. i2")]
        [TestCase("1. i1\n2. i2\n  a. i2.1")]
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
