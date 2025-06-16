using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs;

[TestFixture]
public class TestUnorderedList
{
    // i = item
    [TestCase("- i1")]
    [TestCase("- i1 ")]
    [TestCase("- i1\n")]
    [TestCase("- i1\n\n")]
    [TestCase("- i1\n- i2")]
    [TestCase("- i1\n    - i2")]
    [TestCase("- i1\n    - i1.1\n    - i1.2")]
    [TestCase("- i1 \n- i2 \n")]
    [TestCase("- i1  \n- i2  \n")]
    [TestCase(" - i1")]
    [TestCase("  - i1")]
    [TestCase("   - i1")]
    [TestCase("- i1\n\n- i1")]
    [TestCase("- i1\n\n\n- i1")]
    [TestCase("- i1\n    - i1.1\n        - i1.1.1\n")]

    [TestCase("-\ti1")]
    [TestCase("-\ti1\n-\ti2")]
    [TestCase("-\ti1\n-  i2\n-\ti3")]
    [TestCase("- 1.\n- 2.")]
    public void Test(string value)
    {
        RoundTrip(value);
    }

    [TestCase("- > q")]
    [TestCase(" - > q")]
    [TestCase("  - > q")]
    [TestCase("   - > q")]
    [TestCase("-  > q")]
    [TestCase(" -  > q")]
    [TestCase("  -  > q")]
    [TestCase("   -  > q")]
    [TestCase("-   > q")]
    [TestCase(" -   > q")]
    [TestCase("  -   > q")]
    [TestCase("   -   > q")]
    [TestCase("-    > q")]
    [TestCase(" -    > q")]
    [TestCase("  -    > q")]
    [TestCase("   -    > q")]
    [TestCase("   -    > q1\n   -    > q2")]
    public void TestBlockQuote(string value)
    {
        RoundTrip(value);
    }

    [TestCase("-     i1\n\np\n")] // TODO: listblock should render newline, apparently last paragraph of last listitem dont have newline
    [TestCase("-     i1\n\n\np\n")]
    [TestCase("- i1\n\np")]
    [TestCase("- i1\n\np\n")]
    public void TestParagraph(string value)
    {
        RoundTrip(value);
    }

    [TestCase("- i1\n\n---\n")]
    [TestCase("- i1\n\n\n---\n")]
    public void TestThematicBreak(string value)
    {
        RoundTrip(value);
    }

    [TestCase("-     c")] // 5
    [TestCase("-     c\n      c")] // 5, 6
    [TestCase(" -     c\n      c")] // 5, 6
    [TestCase(" -     c\n       c")] // 5, 7
    [TestCase("-      c\n      c")] // 6, 6
    [TestCase(" -      c\n      c")] // 6, 6
    [TestCase(" -      c\n       c")] // 6, 7
    public void TestIndentedCodeBlock(string value)
    {
        RoundTrip(value);
    }

    [TestCase("- ```a```")]
    [TestCase("- ```\n  a\n```")]
    [TestCase("- i1\n    - i1.1\n    ```\n    c\n    ```")]
    [TestCase("- i1\n    - i1.1\n    ```\nc\n```")]
    [TestCase("- i1\n    - i1.1\n    ```\nc\n```\n")]
    public void TestFencedCodeBlock(string value)
    {
        RoundTrip(value);
    }

    [TestCase("\n- i")]
    [TestCase("\r- i")]
    [TestCase("\r\n- i")]

    [TestCase("\n- i\n")]
    [TestCase("\r- i\n")]
    [TestCase("\r\n- i\n")]

    [TestCase("\n- i\r")]
    [TestCase("\r- i\r")]
    [TestCase("\r\n- i\r")]

    [TestCase("\n- i\r\n")]
    [TestCase("\r- i\r\n")]
    [TestCase("\r\n- i\r\n")]

    [TestCase("- i\n- j")]
    [TestCase("- i\r- j")]
    [TestCase("- i\r\n- j")]

    [TestCase("\n- i\n- j")]
    [TestCase("\n- i\r- j")]
    [TestCase("\n- i\r\n- j")]

    [TestCase("\r- i\n- j")]
    [TestCase("\r- i\r- j")]
    [TestCase("\r- i\r\n- j")]

    [TestCase("\r\n- i\n- j")]
    [TestCase("\r\n- i\r- j")]
    [TestCase("\r\n- i\r\n- j")]

    [TestCase("- i\n- j\n")]
    [TestCase("- i\r- j\n")]
    [TestCase("- i\r\n- j\n")]

    [TestCase("\n- i\n- j\n")]
    [TestCase("\n- i\r- j\n")]
    [TestCase("\n- i\r\n- j\n")]

    [TestCase("\r- i\n- j\n")]
    [TestCase("\r- i\r- j\n")]
    [TestCase("\r- i\r\n- j\n")]

    [TestCase("\r\n- i\n- j\n")]
    [TestCase("\r\n- i\r- j\n")]
    [TestCase("\r\n- i\r\n- j\n")]

    [TestCase("- i\n- j\r")]
    [TestCase("- i\r- j\r")]
    [TestCase("- i\r\n- j\r")]

    [TestCase("\n- i\n- j\r")]
    [TestCase("\n- i\r- j\r")]
    [TestCase("\n- i\r\n- j\r")]

    [TestCase("\r- i\n- j\r")]
    [TestCase("\r- i\r- j\r")]
    [TestCase("\r- i\r\n- j\r")]

    [TestCase("\r\n- i\n- j\r")]
    [TestCase("\r\n- i\r- j\r")]
    [TestCase("\r\n- i\r\n- j\r")]

    [TestCase("- i\n- j\r\n")]
    [TestCase("- i\r- j\r\n")]
    [TestCase("- i\r\n- j\r\n")]

    [TestCase("\n- i\n- j\r\n")]
    [TestCase("\n- i\r- j\r\n")]
    [TestCase("\n- i\r\n- j\r\n")]

    [TestCase("\r- i\n- j\r\n")]
    [TestCase("\r- i\r- j\r\n")]
    [TestCase("\r- i\r\n- j\r\n")]

    [TestCase("\r\n- i\n- j\r\n")]
    [TestCase("\r\n- i\r- j\r\n")]
    [TestCase("\r\n- i\r\n- j\r\n")]

    [TestCase("- i\n")]
    [TestCase("- i\n\n")]
    [TestCase("- i\n\n\n")]
    [TestCase("- i\n\n\n\n")]
    public void TestNewline(string value)
    {
        RoundTrip(value);
    }
}
