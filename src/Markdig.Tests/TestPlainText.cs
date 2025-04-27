namespace Markdig.Tests;

[TestFixture]
public class TestPlainText
{
    [Test]
    [TestCase(/* markdownText: */ "foo bar", /* expected: */ "foo bar\n")]
    [TestCase(/* markdownText: */ "foo\nbar", /* expected: */ "foo\nbar\n")]
    [TestCase(/* markdownText: */ "*foo\nbar*", /* expected: */ "foo\nbar\n")]
    [TestCase(/* markdownText: */ "[foo\nbar](http://example.com)", /* expected: */ "foo\nbar\n")]
    [TestCase(/* markdownText: */ "<http://foo.bar.baz>", /* expected: */ "http://foo.bar.baz\n")]
    [TestCase(/* markdownText: */ "# foo bar", /* expected: */ "foo bar\n")]
    [TestCase(/* markdownText: */ "# foo\nbar", /* expected: */ "foo\nbar\n")]
    [TestCase(/* markdownText: */ "> foo", /* expected: */ "foo\n")]
    [TestCase(/* markdownText: */ "> foo\nbar\n> baz", /* expected: */ "foo\nbar\nbaz\n")]
    [TestCase(/* markdownText: */ "`foo`", /* expected: */ "foo\n")]
    [TestCase(/* markdownText: */ "`foo\nbar`", /* expected: */ "foo bar\n")] // new line within codespan is treated as whitespace (Example317)
    [TestCase(/* markdownText: */ "```\nfoo bar\n```", /* expected: */ "foo bar\n")]
    [TestCase(/* markdownText: */ "- foo\n- bar\n- baz", /* expected: */ "foo\nbar\nbaz\n")]
    [TestCase(/* markdownText: */ "- foo<baz", /* expected: */ "foo<baz\n")]
    [TestCase(/* markdownText: */ "- foo&lt;baz", /* expected: */ "foo<baz\n")]
    [TestCase(/* markdownText: */ "## foo `bar::baz >`", /* expected: */ "foo bar::baz >\n")]
    public void TestPlainEnsureNewLine(string markdownText, string expected)
    {
        var actual = Markdown.ToPlainText(markdownText);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCase(/* markdownText: */ "```\nConsole.WriteLine(\"Hello, World!\");\n```", /* expected: */ "Console.WriteLine(\"Hello, World!\");\n")]
    public void TestPlainCodeBlock(string markdownText, string expected)
    {
        var actual = Markdown.ToPlainText(markdownText);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCase(/* markdownText: */ ":::\nfoo\n:::", /* expected: */ "foo\n", /*extensions*/ "customcontainers|advanced")]
    [TestCase(/* markdownText: */ ":::bar\nfoo\n:::", /* expected: */ "foo\n", /*extensions*/ "customcontainers+attributes|advanced")]
    [TestCase(/* markdownText: */ "| Header1 | Header2 | Header3 |\n|--|--|--|\nt**es**t|value2|value3", /* expected: */ "Header1 Header2 Header3 test value2 value3","pipetables")]
    public void TestPlainWithExtensions(string markdownText, string expected, string extensions)
    {
        TestParser.TestSpec(markdownText, expected, extensions, plainText: true);
    }

    public static void TestSpec(string markdownText, string expected, string extensions, string context = null)
    {
        TestParser.TestSpec(markdownText, expected, extensions, plainText: true, context: context);
    }
}
