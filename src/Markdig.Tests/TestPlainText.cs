using NUnit.Framework;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestPlainText
    {
        [Test]
        public void TestPlain()
        {
            var markdownText = "*Hello*, [world](http://example.com)!";
            var expected = "Hello, world!";
            var actual = Markdown.ToPlainText(markdownText);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestPlain2()
        {
            var markdownText = "*Hello*,\r\n [world](http://example.com)!";
            var expected = "Hello,\n world!";
            var actual = Markdown.ToPlainText(markdownText);
            Assert.AreEqual(expected, actual);
        }
    }
}
