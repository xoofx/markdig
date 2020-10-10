using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs
{
    [TestFixture]
    public class TestSetextHeading
    {
        [TestCase("h1\n===")] //3
        [TestCase("h1\n ===")] //3
        [TestCase("h1\n  ===")] //3
        [TestCase("h1\n   ===")] //3
        [TestCase("h1\n=== ")] //3
        [TestCase("h1 \n===")] //3
        [TestCase("h1\\\n===")] //3
        [TestCase("h1\n === ")] //3
        [TestCase("h1\nh1 l2\n===")] //3
        [TestCase("h1\n====")] // 4
        [TestCase("h1\n ====")] // 4
        [TestCase("h1\n==== ")] // 4
        [TestCase("h1\n ==== ")] // 4
        [TestCase("h1\n===\nh1\n===")] //3
        [TestCase("\\>h1\n===")] //3
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
