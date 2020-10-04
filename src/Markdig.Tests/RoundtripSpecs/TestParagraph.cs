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

        // special cases
        [TestCase(" p \n\n\n\n p \n\n")]
        [TestCase("\np")]
        [TestCase("\n\np")]
        [TestCase("p\n")]
        [TestCase("p\n\n")]
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
