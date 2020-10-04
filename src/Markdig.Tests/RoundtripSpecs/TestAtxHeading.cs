using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs
{
    [TestFixture]
    public class TestAtxHeading
    {
        [TestCase("# h")]
        [TestCase("# h ")]
        [TestCase("# h\n#h")]
        [TestCase("# h\n #h")]
        [TestCase("# h\n # h")]
        [TestCase("# h\n # h ")]
        [TestCase(" #  h   \n    #     h      ")]
        public void Test(string value)
        {
            RoundTrip(value);
        }

        [TestCase("\n# h\n\np")]
        [TestCase("\n# h\n\np\n")]
        [TestCase("\n# h\n\np\n\n")]
        [TestCase("\n\n# h\n\np\n\n")]
        [TestCase("\n\n# h\np\n\n")]
        [TestCase("\n\n# h\np\n\n")]
        public void TestParagrph(string value)
        {
            RoundTrip(value);
        }
    }
}
