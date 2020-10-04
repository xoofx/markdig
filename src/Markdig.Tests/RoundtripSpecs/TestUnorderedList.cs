using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs
{
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
        public void Test_UnorderedList(string value)
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

        [TestCase("- i1\n    - i1.1\n    ```\n    code\n    ```")]
        public void TestCodeBlock(string value)
        {
            RoundTrip(value);
        }
    }
}
