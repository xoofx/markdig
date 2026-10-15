using Markdig.Syntax;

namespace Markdig.Tests;

public class TestHtmlCodeBlocks
{
    // Start condition: line begins with the string < or </ followed by one of the strings (case-insensitive)
    // {list of all tags}, followed by a space, a tab, the end of the line, the string >, or the string />.
    public static string[] KnownSimpleHtmlTags =>
    [
        "address", "article", "aside", "base", "basefont", "blockquote", "body", "caption", "center", "col", "colgroup", "dd", "details",
        "dialog", "dir", "div", "dl", "dt", "fieldset", "figcaption", "figure", "footer", "form", "frame", "frameset",
        "h1", "h2", "h3", "h4", "h5", "h6", "head", "header", "hr", "html", "iframe", "legend", "li", "link",
        "main", "menu", "menuitem", "nav", "noframes", "ol", "optgroup", "option", "p", "param",
        "search", "section", "summary", "table", "tbody", "td", "tfoot", "th", "thead", "title", "tr", "track", "ul",
    ];

    [Theory]
    [TestCaseSource(nameof(KnownSimpleHtmlTags))]
    public void TestKnownTags(string tag)
    {
        MarkdownDocument document = Markdown.Parse(
            $"""
            Hello
             <{tag} />
            World
            """.ReplaceLineEndings("\n"));

        HtmlBlock[] htmlBlocks = document.Descendants<HtmlBlock>().ToArray();

        Assert.AreEqual(1, htmlBlocks.Length);
        Assert.AreEqual(7, htmlBlocks[0].Span.Start);
        Assert.AreEqual(10 + tag.Length, htmlBlocks[0].Span.Length);
    }

    // A line containing only spaces or tabs is a blank line, which ends HTML blocks of type 6 and 7.
    // https://spec.commonmark.org/0.31.2/#blank-line
    [TestCase("<div>\n  \nfoo", "<div>\n<p>foo</p>\n")]
    [TestCase("<div>\n\t\nfoo", "<div>\n<p>foo</p>\n")]
    [TestCase("<del>\n \n*foo*", "<del>\n<p><em>foo</em></p>\n")]
    public void WhitespaceOnlyLineEndsHtmlBlock(string markdown, string expected)
    {
        TestParser.TestSpec(markdown, expected);
    }

    [TestCase("<div>\n  \nfoo")]
    [TestCase("<div>\n\t\nfoo")]
    [TestCase("<del>\n \t \n*foo*")]
    [TestCase("<div>\r\n  \r\nfoo")]
    [TestCase("<del>\r\n\t\r\n*foo*")]
    [TestCase("<div>\r \t\rfoo")]
    [TestCase("> <div>\n>  \n> foo")]
    [TestCase("> <div>\n>   \n> foo")]
    [TestCase("> > <div>\n> >  \n> > foo")]
    [TestCase("- > <div>\n  >  \n  > foo")]
    [TestCase("- <div>\n  \t\n  foo")]
    [TestCase("<div>\n  \n")]
    public void WhitespaceOnlyHtmlBlockTerminatorRoundtrips(string markdown)
    {
        TestRoundtrip.RoundTrip(markdown);
    }

    [TestCase("<script>\n \t\n*foo*\n</script>")]
    [TestCase("<!--\n \t\n*foo*\n-->")]
    [TestCase("<div>\n\u00a0\n*foo*")]
    [TestCase("<del>\n\u00a0\n*foo*")]
    public void NonTerminatingHtmlBlockLinesArePreserved(string markdown)
    {
        TestParser.TestSpec(markdown, markdown + "\n");
        TestRoundtrip.RoundTrip(markdown);
    }
}
