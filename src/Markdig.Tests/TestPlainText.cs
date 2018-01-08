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
    }
}
