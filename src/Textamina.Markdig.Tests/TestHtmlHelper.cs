


using NUnit.Framework;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Tests
{
    [TestFixture]
    public class TestHtmlHelper
    {
        [Test]
        public void TestParseHtmlTagSimple()
        {
            var inputTag = "<a>";
            var text = new StringSlice(inputTag);
            string outputTag;
            Assert.True(HtmlHelper.TryParseHtmlTag(text, out outputTag));
            Assert.AreEqual(inputTag, outputTag);
        }

        [Test]
        public void TestParseHtmlTagSimpleWithAttribute()
        {
            var inputTag = "<a href='http://google.com'>";
            var text = new StringSlice(inputTag);
            string outputTag;
            Assert.True(HtmlHelper.TryParseHtmlTag(text, out outputTag));
            Assert.AreEqual(inputTag, outputTag);
        }
    }
}