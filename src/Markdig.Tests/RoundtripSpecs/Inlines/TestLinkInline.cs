using NUnit.Framework;
using static Markdig.Tests.TestRoundtrip;

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
        [TestCase("[]( b)")]
        [TestCase(" []( b)")]
        [TestCase("[]( b) ")]
        [TestCase(" []( b) ")]

        [TestCase("[a]( b)")]
        [TestCase(" [a]( b)")]
        [TestCase("[a]( b) ")]
        [TestCase(" [a]( b) ")]

        [TestCase("[ a]( b)")]
        [TestCase(" [ a]( b)")]
        [TestCase("[ a]( b) ")]
        [TestCase(" [ a]( b) ")]

        [TestCase("[a ]( b)")]
        [TestCase(" [a ]( b)")]
        [TestCase("[a ]( b) ")]
        [TestCase(" [a ]( b) ")]

        [TestCase("[ a ]( b)")]
        [TestCase(" [ a ]( b)")]
        [TestCase("[ a ]( b) ")]
        [TestCase(" [ a ]( b) ")]

        [TestCase("[](b )")]
        [TestCase(" [](b )")]
        [TestCase("[](b ) ")]
        [TestCase(" [](b ) ")]

        [TestCase("[a](b )")]
        [TestCase(" [a](b )")]
        [TestCase("[a](b ) ")]
        [TestCase(" [a](b ) ")]

        [TestCase("[ a](b )")]
        [TestCase(" [ a](b )")]
        [TestCase("[ a](b ) ")]
        [TestCase(" [ a](b ) ")]

        [TestCase("[a ](b )")]
        [TestCase(" [a ](b )")]
        [TestCase("[a ](b ) ")]
        [TestCase(" [a ](b ) ")]

        [TestCase("[ a ](b )")]
        [TestCase(" [ a ](b )")]
        [TestCase("[ a ](b ) ")]
        [TestCase(" [ a ](b ) ")]

        [TestCase("[]( b )")]
        [TestCase(" []( b )")]
        [TestCase("[]( b ) ")]
        [TestCase(" []( b ) ")]

        [TestCase("[a]( b )")]
        [TestCase(" [a]( b )")]
        [TestCase("[a]( b ) ")]
        [TestCase(" [a]( b ) ")]

        [TestCase("[ a]( b )")]
        [TestCase(" [ a]( b )")]
        [TestCase("[ a]( b ) ")]
        [TestCase(" [ a]( b ) ")]

        [TestCase("[a ]( b )")]
        [TestCase(" [a ]( b )")]
        [TestCase("[a ]( b ) ")]
        [TestCase(" [a ]( b ) ")]

        [TestCase("[ a ]( b )")]
        [TestCase(" [ a ]( b )")]
        [TestCase("[ a ]( b ) ")]
        [TestCase(" [ a ]( b ) ")]
        public void Test(string value)
        {
            RoundTrip(value);
        }

        [TestCase("[a](b \"t\") ")]
        [TestCase("[a](b \" t\") ")]
        [TestCase("[a](b \"t \") ")]
        [TestCase("[a](b \" t \") ")]

        [TestCase("[a](b  \"t\") ")]
        [TestCase("[a](b  \" t\") ")]
        [TestCase("[a](b  \"t \") ")]
        [TestCase("[a](b  \" t \") ")]

        [TestCase("[a](b \"t\" ) ")]
        [TestCase("[a](b \" t\" ) ")]
        [TestCase("[a](b \"t \" ) ")]
        [TestCase("[a](b \" t \" ) ")]

        [TestCase("[a](b  \"t\" ) ")]
        [TestCase("[a](b  \" t\" ) ")]
        [TestCase("[a](b  \"t \" ) ")]
        [TestCase("[a](b  \" t \" ) ")]

        [TestCase("[a](b 't') ")]
        [TestCase("[a](b ' t') ")]
        [TestCase("[a](b 't ') ")]
        [TestCase("[a](b ' t ') ")]

        [TestCase("[a](b  't') ")]
        [TestCase("[a](b  ' t') ")]
        [TestCase("[a](b  't ') ")]
        [TestCase("[a](b  ' t ') ")]

        [TestCase("[a](b 't' ) ")]
        [TestCase("[a](b ' t' ) ")]
        [TestCase("[a](b 't ' ) ")]
        [TestCase("[a](b ' t ' ) ")]

        [TestCase("[a](b  't' ) ")]
        [TestCase("[a](b  ' t' ) ")]
        [TestCase("[a](b  't ' ) ")]
        [TestCase("[a](b  ' t ' ) ")]

        [TestCase("[a](b (t)) ")]
        [TestCase("[a](b ( t)) ")]
        [TestCase("[a](b (t )) ")]
        [TestCase("[a](b ( t )) ")]

        [TestCase("[a](b  (t)) ")]
        [TestCase("[a](b  ( t)) ")]
        [TestCase("[a](b  (t )) ")]
        [TestCase("[a](b  ( t )) ")]

        [TestCase("[a](b (t) ) ")]
        [TestCase("[a](b ( t) ) ")]
        [TestCase("[a](b (t ) ) ")]
        [TestCase("[a](b ( t ) ) ")]

        [TestCase("[a](b  (t) ) ")]
        [TestCase("[a](b  ( t) ) ")]
        [TestCase("[a](b  (t ) ) ")]
        [TestCase("[a](b  ( t ) ) ")]
        public void Test_Title(string value)
        {
            RoundTrip(value);
        }

        [TestCase("[a](<>)")]
        [TestCase("[a]( <>)")]
        [TestCase("[a](<> )")]
        [TestCase("[a]( <> )")]

        [TestCase("[a](< >)")]
        [TestCase("[a]( < >)")]
        [TestCase("[a](< > )")]
        [TestCase("[a]( < > )")]

        [TestCase("[a](<b>)")]
        [TestCase("[a](<b >)")]
        [TestCase("[a](< b>)")]
        [TestCase("[a](< b >)")]

        [TestCase("[a](<b b>)")]
        [TestCase("[a](<b b >)")]
        [TestCase("[a](< b b >)")]
        public void Test_PointyBrackets(string value)
        {
            RoundTrip(value);
        }

        // | [ a ]( b " t " ) |
        [TestCase(" [ a ]( b \" t \" ) ")]
        [TestCase("\v[\va\v](\vb\v\"\vt\v\"\v)\v")]
        [TestCase("\f[\fa\f](\fb\f\"\ft\f\"\f)\f")]
        [TestCase("\t[\ta\t](\tb\t\"\tt\t\"\t)\t")]
        public void Test_UncommonWhitespace(string value)
        {
            RoundTrip(value);
        }
    }
}
