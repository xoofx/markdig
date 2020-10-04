using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    [TestFixture]
    public class TestLinkInline
    {
        [TestCase("[a]")] // TODO: this is not a link but a paragraph
        [TestCase("[a]()")]

        [TestCase("[](b)")]
        [TestCase(" [](b)")]
        [TestCase("[](b) ")]
        [TestCase(" [](b) ")]

        [TestCase("[a](b)")]
        [TestCase(" [a](b)")]
        [TestCase("[a](b) ")]
        [TestCase(" [a](b) ")]

        [TestCase("[ a](b)")]
        [TestCase(" [ a](b)")]
        [TestCase("[ a](b) ")]
        [TestCase(" [ a](b) ")]

        [TestCase("[a ](b)")]
        [TestCase(" [a ](b)")]
        [TestCase("[a ](b) ")]
        [TestCase(" [a ](b) ")]

        [TestCase("[ a ](b)")]
        [TestCase(" [ a ](b)")]
        [TestCase("[ a ](b) ")]
        [TestCase(" [ a ](b) ")]

        // below cases are required for a full CST but not have low prio for impl
        //[TestCase("[]( b)")]
        //[TestCase(" []( b)")]
        //[TestCase("[]( b) ")]
        //[TestCase(" []( b) ")]

        //[TestCase("[a]( b)")]
        //[TestCase(" [a]( b)")]
        //[TestCase("[a]( b) ")]
        //[TestCase(" [a]( b) ")]

        //[TestCase("[ a]( b)")]
        //[TestCase(" [ a]( b)")]
        //[TestCase("[ a]( b) ")]
        //[TestCase(" [ a]( b) ")]

        //[TestCase("[a ]( b)")]
        //[TestCase(" [a ]( b)")]
        //[TestCase("[a ]( b) ")]
        //[TestCase(" [a ]( b) ")]

        //[TestCase("[ a ]( b)")]
        //[TestCase(" [ a ]( b)")]
        //[TestCase("[ a ]( b) ")]
        //[TestCase(" [ a ]( b) ")]

        //[TestCase("[](b )")]
        //[TestCase(" [](b )")]
        //[TestCase("[](b ) ")]
        //[TestCase(" [](b ) ")]

        //[TestCase("[a](b )")]
        //[TestCase(" [a](b )")]
        //[TestCase("[a](b ) ")]
        //[TestCase(" [a](b ) ")]

        //[TestCase("[ a](b )")]
        //[TestCase(" [ a](b )")]
        //[TestCase("[ a](b ) ")]
        //[TestCase(" [ a](b ) ")]

        //[TestCase("[a ](b )")]
        //[TestCase(" [a ](b )")]
        //[TestCase("[a ](b ) ")]
        //[TestCase(" [a ](b ) ")]

        //[TestCase("[ a ](b )")]
        //[TestCase(" [ a ](b )")]
        //[TestCase("[ a ](b ) ")]
        //[TestCase(" [ a ](b ) ")]

        //[TestCase("[]( b )")]
        //[TestCase(" []( b )")]
        //[TestCase("[]( b ) ")]
        //[TestCase(" []( b ) ")]

        //[TestCase("[a]( b )")]
        //[TestCase(" [a]( b )")]
        //[TestCase("[a]( b ) ")]
        //[TestCase(" [a]( b ) ")]

        //[TestCase("[ a]( b )")]
        //[TestCase(" [ a]( b )")]
        //[TestCase("[ a]( b ) ")]
        //[TestCase(" [ a]( b ) ")]

        //[TestCase("[a ]( b )")]
        //[TestCase(" [a ]( b )")]
        //[TestCase("[a ]( b ) ")]
        //[TestCase(" [a ]( b ) ")]

        //[TestCase("[ a ]( b )")]
        //[TestCase(" [ a ]( b )")]
        //[TestCase("[ a ]( b ) ")]
        //[TestCase(" [ a ]( b ) ")]
        public void Test_InlineLink(string value)
        {
            RoundTrip(value);
        }
    }
}
