using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs
{
    [TestFixture]
    public class TestParagraph
    {
        [TestCase("p")]
        [TestCase(" p")]
        [TestCase("p ")]
        [TestCase(" p ")]

        [TestCase("p\np")]
        [TestCase(" p\np")]
        [TestCase("p \np")]
        [TestCase(" p \np")]

        [TestCase("p\n p")]
        [TestCase(" p\n p")]
        [TestCase("p \n p")]
        [TestCase(" p \n p")]

        [TestCase("p\np ")]
        [TestCase(" p\np ")]
        [TestCase("p \np ")]
        [TestCase(" p \np ")]

        [TestCase("p\n\n p ")]
        [TestCase(" p\n\n p ")]
        [TestCase("p \n\n p ")]
        [TestCase(" p \n\n p ")]

        [TestCase("p\n\np")]
        [TestCase(" p\n\np")]
        [TestCase("p \n\np")]
        [TestCase(" p \n\np")]

        [TestCase("p\n\n p")]
        [TestCase(" p\n\n p")]
        [TestCase("p \n\n p")]
        [TestCase(" p \n\n p")]

        [TestCase("p\n\np ")]
        [TestCase(" p\n\np ")]
        [TestCase("p \n\np ")]
        [TestCase(" p \n\np ")]

        [TestCase("p\n\n p ")]
        [TestCase(" p\n\n p ")]
        [TestCase("p \n\n p ")]
        [TestCase(" p \n\n p ")]

        [TestCase("\np")]
        [TestCase("\n p")]
        [TestCase("\np ")]
        [TestCase("\n p ")]

        [TestCase("\np\np")]
        [TestCase("\n p\np")]
        [TestCase("\np \np")]
        [TestCase("\n p \np")]

        [TestCase("\np\n p")]
        [TestCase("\n p\n p")]
        [TestCase("\np \n p")]
        [TestCase("\n p \n p")]

        [TestCase("\np\np ")]
        [TestCase("\n p\np ")]
        [TestCase("\np \np ")]
        [TestCase("\n p \np ")]

        [TestCase("\np\n\n p ")]
        [TestCase("\n p\n\n p ")]
        [TestCase("\np \n\n p ")]
        [TestCase("\n p \n\n p ")]

        [TestCase("\np\n\np")]
        [TestCase("\n p\n\np")]
        [TestCase("\np \n\np")]
        [TestCase("\n p \n\np")]

        [TestCase("\np\n\n p")]
        [TestCase("\n p\n\n p")]
        [TestCase("\np \n\n p")]
        [TestCase("\n p \n\n p")]

        [TestCase("\np\n\np ")]
        [TestCase("\n p\n\np ")]
        [TestCase("\np \n\np ")]
        [TestCase("\n p \n\np ")]

        [TestCase("\np\n\n p ")]
        [TestCase("\n p\n\n p ")]
        [TestCase("\np \n\n p ")]
        [TestCase("\n p \n\n p ")]

        // special cases
        [TestCase(" p \n\n\n\n p \n\n")]
        [TestCase("\n\np")]
        [TestCase("p\n")]
        [TestCase("p\n\n")]
        [TestCase("p\np\n p")]
        [TestCase("p\np\n p\n p")]
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
