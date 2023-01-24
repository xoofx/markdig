using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

[TestFixture]
public class TestNewLine
{
    [TestCase("a  \nb", "<p>a<br />\nb</p>\n")]
    [TestCase("a\\\nb", "<p>a<br />\nb</p>\n")]
    [TestCase("a `b\nc`", "<p>a <code>b c</code></p>\n")]
    [TestCase("# Text A\nText B\n\n## Text C", "<h1>Text A</h1>\n<p>Text B</p>\n<h2>Text C</h2>\n")]
    public void Test(string value, string expectedHtml)
    {
        Assert.AreEqual(expectedHtml, Markdown.ToHtml(value));
        Assert.AreEqual(expectedHtml, Markdown.ToHtml(value.Replace("\n", "\r\n")));
    }

    [Test()]
    public void TestEscapeLineBreak()
    {
        var input = "test\\\r\ntest1\r\n";
        var doc = Markdown.Parse(input);
        var inlines = doc.Descendants<LineBreakInline>().ToList();
        Assert.AreEqual(1, inlines.Count, "Invalid number of LineBreakInline");
        Assert.True(inlines[0].IsBackslash);
    }
}
