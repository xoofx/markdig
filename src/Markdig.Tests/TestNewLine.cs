using NUnit.Framework;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestNewLine
    {
        [TestCase("a  \nb", "<p>a<br />\nb</p>\n")]
        [TestCase("a\\\nb", "<p>a<br />\nb</p>\n")]
        [TestCase("a `b\nc`", "<p>a <code>b c</code></p>\n")]
        public void Test(string value, string expectedHtml)
        {
            Assert.AreEqual(expectedHtml, Markdown.ToHtml(value));
            Assert.AreEqual(expectedHtml, Markdown.ToHtml(value.Replace("\n", "\r\n")));
        }
    }
}
