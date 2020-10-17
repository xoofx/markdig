using NUnit.Framework;
using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="https://spec.commonmark.org/0.29/#entity-and-numeric-character-references"/>
    [TestFixture]
    public class TestHtmlEntityInline
    {
        [TestCase("&gt;")]
        [TestCase("&lt;")]
        [TestCase("&nbsp;")]
        [TestCase("&heartsuit;")]
        [TestCase("&#42;")]
        [TestCase("&#0;")]
        [TestCase("&#1234;")]
        [TestCase("&#xcab;")]

        [TestCase(" &gt;")]
        [TestCase(" &lt;")]
        [TestCase(" &nbsp;")]
        [TestCase(" &heartsuit;")]
        [TestCase(" &#42;")]
        [TestCase(" &#0;")]
        [TestCase(" &#1234;")]
        [TestCase(" &#xcab;")]

        [TestCase("&gt; ")]
        [TestCase("&lt; ")]
        [TestCase("&nbsp; ")]
        [TestCase("&heartsuit; ")]
        [TestCase("&#42; ")]
        [TestCase("&#0; ")]
        [TestCase("&#1234; ")]
        [TestCase("&#xcab; ")]

        [TestCase(" &gt; ")]
        [TestCase(" &lt; ")]
        [TestCase(" &nbsp; ")]
        [TestCase(" &heartsuit; ")]
        [TestCase(" &#42; ")]
        [TestCase(" &#0; ")]
        [TestCase(" &#1234; ")]
        [TestCase(" &#xcab; ")]
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
