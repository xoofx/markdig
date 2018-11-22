using NUnit.Framework;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestConfigureNewLine
    {
        [Test]
        [TestCase(/* newLineForWriting: */ "\n",   /* markdownText: */ "*1*\n*2*\n",     /* expected: */ "<p><em>1</em>\n<em>2</em></p>\n")]
        [TestCase(/* newLineForWriting: */ "\n",   /* markdownText: */ "*1*\r\n*2*\r\n", /* expected: */ "<p><em>1</em>\n<em>2</em></p>\n")]
        [TestCase(/* newLineForWriting: */ "\r\n", /* markdownText: */ "*1*\n*2*\n",     /* expected: */ "<p><em>1</em>\r\n<em>2</em></p>\r\n")]
        [TestCase(/* newLineForWriting: */ "\r\n", /* markdownText: */ "*1*\r\n*2*\r\n", /* expected: */ "<p><em>1</em>\r\n<em>2</em></p>\r\n")]
        [TestCase(/* newLineForWriting: */ "!!!" , /* markdownText: */ "*1*\n*2*\n",     /* expected: */ "<p><em>1</em>!!!<em>2</em></p>!!!")]
        [TestCase(/* newLineForWriting: */ "!!!" , /* markdownText: */ "*1*\r\n*2*\r\n", /* expected: */ "<p><em>1</em>!!!<em>2</em></p>!!!")]
        public void TestHtmlOutputWhenConfiguringNewLine(string newLineForWriting, string markdownText, string expected)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .ConfigureNewLine(newLineForWriting)
                .Build();

            var actual = Markdown.ToHtml(markdownText, pipeline);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(/* newLineForWriting: */ "\n",   /* markdownText: */ "*1*\n*2*\n",     /* expected: */ "1\n2\n")]
        [TestCase(/* newLineForWriting: */ "\n",   /* markdownText: */ "*1*\r\n*2*\r\n", /* expected: */ "1\n2\n")]
        [TestCase(/* newLineForWriting: */ "\r\n", /* markdownText: */ "*1*\n*2*\n",     /* expected: */ "1\r\n2\r\n")]
        [TestCase(/* newLineForWriting: */ "\r\n", /* markdownText: */ "*1*\r\n*2*\r\n", /* expected: */ "1\r\n2\r\n")]
        [TestCase(/* newLineForWriting: */ "!!!", /* markdownText: */ "*1*\n*2*\n",     /* expected: */ "1!!!2!!!")]
        [TestCase(/* newLineForWriting: */ "!!!", /* markdownText: */ "*1*\r\n*2*\r\n", /* expected: */ "1!!!2!!!")]
        public void TestPlainOutputWhenConfiguringNewLine(string newLineForWriting, string markdownText, string expected)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .ConfigureNewLine(newLineForWriting)
                .Build();

            var actual = Markdown.ToPlainText(markdownText, pipeline);
            Assert.AreEqual(expected, actual);
        }
    }
}