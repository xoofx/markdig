using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs;

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

    [TestCase("h1\r===")]
    [TestCase("h1\n===")]
    [TestCase("h1\r\n===")]

    [TestCase("h1\r===\r")]
    [TestCase("h1\n===\r")]
    [TestCase("h1\r\n===\r")]

    [TestCase("h1\r===\n")]
    [TestCase("h1\n===\n")]
    [TestCase("h1\r\n===\n")]

    [TestCase("h1\r===\r\n")]
    [TestCase("h1\n===\r\n")]
    [TestCase("h1\r\n===\r\n")]

    [TestCase("h1\n===\n\n\nh2---\n\n")]
    [TestCase("h1\r===\r\r\rh2---\r\r")]
    [TestCase("h1\r\n===\r\n\r\n\r\nh2---\r\n\r\n")]
    public void TestNewline(string value)
    {
        RoundTrip(value);
    }
}
