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
        [TestCase("1.  i  ")]

        [TestCase(" 1. i")]
        [TestCase(" 1.  i")]
        [TestCase(" 1. i ")]
        [TestCase(" 1.  i ")]
        [TestCase(" 1.  i  ")]

        [TestCase("  1. i")]
        [TestCase("  1.  i")]
        [TestCase("  1. i ")]
        [TestCase("  1.  i ")]
        [TestCase("  1.  i  ")]

        [TestCase("   1. i")]
        [TestCase("   1.  i")]
        [TestCase("   1. i ")]
        [TestCase("   1.  i ")]
        [TestCase("   1.  i  ")]

        [TestCase("1. i\n")]
        [TestCase("1.  i\n")]
        [TestCase("1. i \n")]
        [TestCase("1.  i \n")]
        [TestCase("1.  i  \n")]

        [TestCase(" 1. i\n")]
        [TestCase(" 1.  i\n")]
        [TestCase(" 1. i \n")]
        [TestCase(" 1.  i \n")]
        [TestCase(" 1.  i  \n")]

        [TestCase("  1. i\n")]
        [TestCase("  1.  i\n")]
        [TestCase("  1. i \n")]
        [TestCase("  1.  i \n")]
        [TestCase("  1.  i  \n")]

        [TestCase("   1. i\n")]
        [TestCase("   1.  i\n")]
        [TestCase("   1. i \n")]
        [TestCase("   1.  i \n")]
        [TestCase("   1.  i  \n")]

        [TestCase("1. i\n2. j")]
        [TestCase("1.  i\n2. j")]
        [TestCase("1. i \n2. j")]
        [TestCase("1.  i \n2. j")]
        [TestCase("1.  i  \n2. j")]

        [TestCase(" 1. i\n2. j")]
        [TestCase(" 1.  i\n2. j")]
        [TestCase(" 1. i \n2. j")]
        [TestCase(" 1.  i \n2. j")]
        [TestCase(" 1.  i  \n2. j")]

        [TestCase("  1. i\n2. j")]
        [TestCase("  1.  i\n2. j")]
        [TestCase("  1. i \n2. j")]
        [TestCase("  1.  i \n2. j")]
        [TestCase("  1.  i  \n2. j")]

        [TestCase("   1. i\n2. j")]
        [TestCase("   1.  i\n2. j")]
        [TestCase("   1. i \n2. j")]
        [TestCase("   1.  i \n2. j")]
        [TestCase("   1.  i  \n2. j")]

        [TestCase("1. i\n2. j\n")]
        [TestCase("1.  i\n2. j\n")]
        [TestCase("1. i \n2. j\n")]
        [TestCase("1.  i \n2. j\n")]
        [TestCase("1.  i  \n2. j\n")]

        [TestCase(" 1. i\n2. j\n")]
        [TestCase(" 1.  i\n2. j\n")]
        [TestCase(" 1. i \n2. j\n")]
        [TestCase(" 1.  i \n2. j\n")]
        [TestCase(" 1.  i  \n2. j\n")]

        [TestCase("  1. i\n2. j\n")]
        [TestCase("  1.  i\n2. j\n")]
        [TestCase("  1. i \n2. j\n")]
        [TestCase("  1.  i \n2. j\n")]
        [TestCase("  1.  i  \n2. j\n")]

        [TestCase("   1. i\n2. j\n")]
        [TestCase("   1.  i\n2. j\n")]
        [TestCase("   1. i \n2. j\n")]
        [TestCase("   1.  i \n2. j\n")]
        [TestCase("   1.  i  \n2. j\n")]

        [TestCase("1. i\n2. j\n3. k")]
        [TestCase("1. i\n2. j\n3. k\n")]

        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
