using NUnit.Framework;
using static Markdig.Tests.RoundtripSpecs.TestHelper;

namespace Markdig.Tests.RoundtripSpecs.Inlines
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="https://spec.commonmark.org/0.29/#entity-and-numeric-character-references"/>
    [TestFixture]
    public class TestHtmlEntities
    {
        [TestCase("&gt;")]
        [TestCase("&lt;")]
        [TestCase("&nbsp;")]
        [TestCase("&heartsuit;")]
        public void Test(string value)
        {
            RoundTrip(value);
        }
    }
}
