namespace Markdig.Tests;

public class TestUnicodeWhitespaceTrimming
{
    // Paragraph and heading contents are trimmed of spaces and tabs only,
    // other Unicode whitespace such as U+00A0 or U+3000 is part of the text.
    // Compared exactly, as TestParser.TestSpec normalizes whitespace around list items and applies NFKD.
    [Test]
    [TestCase("a ", "<p>a </p>\n")]
    [TestCase(" a", "<p> a</p>\n")]
    [TestCase("a　", "<p>a　</p>\n")]
    [TestCase("a \n b", "<p>a \n b</p>\n")]
    [TestCase("a \t", "<p>a</p>\n")]
    [TestCase("# h ", "<h1>h </h1>\n")]
    [TestCase("h \n===", "<h1>h </h1>\n")]
    [TestCase("- a ", "<ul>\n<li>a </li>\n</ul>\n")]
    [TestCase("> a ", "<blockquote>\n<p>a </p>\n</blockquote>\n")]
    public void KeepsUnicodeWhitespaceThatIsNotASpaceOrTab(string markdown, string expected)
    {
        Assert.That(Markdown.ToHtml(markdown), Is.EqualTo(expected));
    }
}
