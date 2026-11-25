using System.Text;

using Markdig.Extensions.AutoLinks;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

[TestFixture]
public class TestAutoLinks
{
    [Test]
    [TestCase("https://localhost", "<p><a href=\"https://localhost\">https://localhost</a></p>")]
    [TestCase("http://localhost", "<p><a href=\"http://localhost\">http://localhost</a></p>")]
    [TestCase("https://l", "<p><a href=\"https://l\">https://l</a></p>")]
    [TestCase("www.l", "<p><a href=\"http://www.l\">www.l</a></p>")]
    [TestCase("https://localhost:5000", "<p><a href=\"https://localhost:5000\">https://localhost:5000</a></p>")]
    [TestCase("www.l:5000", "<p><a href=\"http://www.l:5000\">www.l:5000</a></p>")]
    public void TestLinksWithAllowDomainWithoutPeriod(string markdown, string expected)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAutoLinks(new AutoLinkOptions { AllowDomainWithoutPeriod = true })
            .Build();
        var html = Markdown.ToHtml(markdown, pipeline);

        Assert.That(html, Is.EqualTo(expected).IgnoreWhiteSpace);
    }

    // https://github.com/xoofx/markdig/issues/668
    // A heading's implicit reference must not resolve inside another still-open
    // link bracket, which would break the outer link.
    [Test]
    public void TestAutoIdentifierHeadingLinkDoesNotHijackNestedLinkLabel()
    {
        var markdown = "# Testing Markdown\n\n" +
                        "### Header\n\n" +
                        "[Testing [Header]](https://www.bing.com)\n\n" +
                        "[Testing [test]](https://www.google.com)\n";
        var expected = "<h1 id=\"testing-markdown\">Testing Markdown</h1>\n" +
                        "<h3 id=\"header\">Header</h3>\n" +
                        "<p><a href=\"https://www.bing.com\">Testing [Header]</a></p>\n" +
                        "<p><a href=\"https://www.google.com\">Testing [test]</a></p>";

        var pipeline = new MarkdownPipelineBuilder()
            .UseAutoLinks()
            .UseAutoIdentifiers()
            .Build();
        var html = Markdown.ToHtml(markdown, pipeline);

        Assert.That(html, Is.EqualTo(expected).IgnoreWhiteSpace);
    }

    [TestCase("[Header]", "<a href=\"#header\">Header</a>")]
    [TestCase("[Header][]", "<a href=\"#header\">Header</a>")]
    [TestCase("[label][Header]", "<a href=\"#header\">label</a>")]
    [TestCase("[Testing [Header]](/target)", "<a href=\"/target\">Testing [Header]</a>")]
    [TestCase("[Testing [Header][]](/target)", "<a href=\"/target\">Testing [Header][]</a>")]
    [TestCase("[Testing [label][Header]](/target)", "<a href=\"/target\">Testing [label][Header]</a>")]
    [TestCase("![Testing [Header]](/image.png)", "<img src=\"/image.png\" alt=\"Testing [Header]\" />")]
    public void TestAutoIdentifierReferenceResolution(string markdown, string expected)
    {
        foreach (var trackTrivia in new[] { false, true })
        {
            var builder = new MarkdownPipelineBuilder().UseAutoLinks().UseAutoIdentifiers();
            if (trackTrivia)
            {
                builder.EnableTrackTrivia();
            }

            var html = Markdown.ToHtml("# Header\n\n" + markdown, builder.Build());

            Assert.That(html, Is.EqualTo("<h1 id=\"header\">Header</h1>\n<p>" + expected + "</p>").IgnoreWhiteSpace, $"TrackTrivia: {trackTrivia}");
        }
    }

    [Test]
    public void TestExplicitReferenceStillResolvesInsideOpenLink()
    {
        var pipeline = new MarkdownPipelineBuilder().UseAutoLinks().UseAutoIdentifiers().Build();
        var html = Markdown.ToHtml("[Header]: /explicit\n\n# Header\n\n[Testing [Header]](/target)", pipeline);

        Assert.That(html, Is.EqualTo("<h1 id=\"header\">Header</h1>\n<p>[Testing <a href=\"/explicit\">Header</a>](/target)</p>").IgnoreWhiteSpace);
    }

    [TestCase("the thin wall with http://a.com", "<p>the thin wall with <a href=\"http://a.com\">http://a.com</a></p>")]
    [TestCase("other nhttp://a.com", "<p>other nhttp://a.com</p>")]
    [TestCase("then tel:+123 and fax", "<p>then <a href=\"tel:+123\">+123</a> and fax</p>")]
    [TestCase("mostly ftp://a.com from mailto:a@b.com", "<p>mostly <a href=\"ftp://a.com\">ftp://a.com</a> from <a href=\"mailto:a@b.com\">a@b.com</a></p>")]
    [TestCase("we want www.a.com", "<p>we want <a href=\"http://www.a.com\">www.a.com</a></p>")]
    [TestCase("see (http://a.com) and ~http://b.com", "<p>see (<a href=\"http://a.com\">http://a.com</a>) and ~<a href=\"http://b.com\">http://b.com</a></p>")]
    [TestCase("see *http://a.com* and _http://b.com_", "<p>see <em><a href=\"http://a.com\">http://a.com</a></em> and <em><a href=\"http://b.com\">http://b.com</a></em></p>")]
    [TestCase("line one\nthat http://a.com", "<p>line one\nthat <a href=\"http://a.com\">http://a.com</a></p>")]
    [TestCase("ends with h", "<p>ends with h</p>")]
    public void TestLinksAfterSkippedOpeningCharacters(string markdown, string expected)
    {
        var html = Markdown.ToHtml(markdown, new MarkdownPipelineBuilder().UseAutoLinks().Build());

        Assert.That(html, Is.EqualTo(expected + "\n"));
    }

    [Test]
    public void TestCustomValidPreviousCharacters()
    {
        var pipeline = new MarkdownPipelineBuilder().UseAutoLinks(new AutoLinkOptions { ValidPreviousCharacters = "-" }).Build();

        Assert.That(Markdown.ToHtml("a-http://a.com b(http://b.com", pipeline), Is.EqualTo("<p>a-<a href=\"http://a.com\">http://a.com</a> b(http://b.com</p>\n"));
        Assert.That(Markdown.ToHtml("a-http://a.com", new MarkdownPipelineBuilder().UseAutoLinks().Build()), Is.EqualTo("<p>a-http://a.com</p>\n"));
    }

    [TestCase("the thin wall with http://a.com, then tel:+123 and www.b.com")]
    [TestCase("see (http://a.com) and ~http://b.com\nthat *http://c.com* or __www.d.com__ fine")]
    [TestCase("first line  \nwith hard break\tand tab then http://a.com  \nhttp://b.com last")]
    [TestCase("crlf line\r\nthat http://a.com\r\nwith text")]
    [TestCase("> quoted text with http://a.com\n> and more text\n\n- item with www.a.com\n  that continues here")]
    [TestCase("| a | b |\n|---|---|\n| with \\| pipe http://a.com | then text |\n")]
    [TestCase("other nhttp://a.com mailto:@a.com ftp:// tel: www. h t w")]
    [TestCase("*[HTML]: Hyper Text\n\nThe HTML with http://a.com and [link](http://b.com) and `code http://c.com`")]
    [TestCase("http:// and [http://a.com then www.b.com and (https://c.com) or www. and www.d.com")]
    [TestCase("www.a.com www.b.com `www.c.com` www.d.com <b>www.e.com</b> tel:1 mailto:a@b.com mailto:c@d.com")]
    [TestCase("awww.a.com wwww.b.com ww.c.com w www.d.com: http:/e.com https:// f https://g.com:80/h:i")]
    [TestCase("ahttp://a.com shttp://b.com httpss://c.com ttp://d.com ftps://e.com mailtos:f@g.com tel:")]
    public void TestPossibleMatchSearchKeepsSyntaxTree(string markdown)
    {
        foreach (var trackTrivia in new[] { false, true })
        {
            AssertSameSyntaxTreeAsWithoutSearch(markdown, builder =>
            {
                builder.UsePreciseSourceLocation().UseAdvancedExtensions();
                builder.TrackTrivia = trackTrivia;
            });
        }
    }

    // The scheme of a link contains opening characters of another parser, at which the literal parser stops.
    [TestCase("a mailto:a@b.com b", "<p>a <a href=\"mailto:a@b.com\">a@b.com</a> b</p>")]
    [TestCase("a http://a.com b https://b.com c", "<p>a <a href=\"http://a.com\">http://a.com</a> b <a href=\"https://b.com\">https://b.com</a> c</p>")]
    [TestCase("to ftp://a.com so tel:+1 pop", "<p>to <a href=\"ftp://a.com\">ftp://a.com</a> so <a href=\"tel:+1\">+1</a> pop</p>")]
    [TestCase("oops www.a.com stop", "<p>oops <a href=\"http://www.a.com\">www.a.com</a> stop</p>")]
    public void TestLinksWithOpeningCharactersOfAnotherParserInScheme(string markdown, string expected)
    {
        Action<MarkdownPipelineBuilder> configure = builder => builder.UseAutoLinks().InlineParsers.Add(
            new NeverMatchingInlineParser('o', 'p', 's', 't', 'l', ':', '/', '.'));

        var builder = new MarkdownPipelineBuilder();
        configure(builder);

        Assert.That(Markdown.ToHtml(markdown, builder.Build()), Is.EqualTo(expected + "\n"));
        AssertSameSyntaxTreeAsWithoutSearch(markdown, configure);
    }

    [Test]
    public void TestPossibleMatchSearchKeepsSyntaxTreeOfRandomDocuments()
    {
        string[] tokens =
        [
            "http://", "https://", "ftp://", "mailto:", "tel:", "www.", "a.com", "b@c.de", "+123", "http", "https", "ftp",
            "mailto", "tel", "www", "w", "h", "t", "s", "p", ":", "//", ".", " ", " ", " ", "\n", "\n\n", "*", "_", "~", "(", ")",
            "[", "]", "`", "<", ">", "|", "\\", "word", "\u00e9", "- ", "> ",
        ];
        var random = new Random(980);

        for (int i = 0; i < 2000; i++)
        {
            var markdown = new StringBuilder();
            for (int count = random.Next(1, 40); count > 0; count--)
            {
                markdown.Append(tokens[random.Next(tokens.Length)]);
            }

            string text = markdown.ToString();
            AssertSameSyntaxTreeAsWithoutSearch(text, builder => builder.UsePreciseSourceLocation().UseAutoLinks());
            AssertSameSyntaxTreeAsWithoutSearch(text, builder => builder.UsePreciseSourceLocation().UseAdvancedExtensions());
        }
    }

    private static void AssertSameSyntaxTreeAsWithoutSearch(string markdown, Action<MarkdownPipelineBuilder> configure)
    {
        var builder = new MarkdownPipelineBuilder();
        configure(builder);
        var pipeline = builder.Build();

        // A global parser is tried at every opening character, which disables the search for possible matches
        var referenceBuilder = new MarkdownPipelineBuilder();
        configure(referenceBuilder);
        referenceBuilder.InlineParsers.Add(new NeverMatchingInlineParser());
        var referencePipeline = referenceBuilder.Build();

        var document = Markdown.Parse(markdown, pipeline);
        var referenceDocument = Markdown.Parse(markdown, referencePipeline);

        var html = Markdown.ToHtml(document, pipeline);
        var referenceHtml = Markdown.ToHtml(referenceDocument, referencePipeline);

        Assert.That(Dump(document), Is.EqualTo(Dump(referenceDocument)), markdown);
        Assert.That(html, Is.EqualTo(referenceHtml), markdown);
    }

    [Test]
    public void TestPostMatchIsCalledAtEveryOpeningCharacter()
    {
        var pipeline = new MarkdownPipelineBuilder().UseAutoLinks().Build();
        var document = new MarkdownDocument();
        var processor = new InlineProcessor(document, pipeline.InlineParsers, false, null);
        var starts = new List<int>();
        processor.LiteralInlineParser.PostMatch = (InlineProcessor _, ref StringSlice slice) => starts.Add(slice.Start);

        var paragraph = new ParagraphBlock();
        var line = new StringSlice("a th");
        paragraph.AppendLine(ref line, 0, 0, 0, false);
        processor.ProcessInlineLeaf(paragraph);

        Assert.That(starts, Is.EqualTo(new[] { 2, 3, 4 }));
    }

    // A possible match found in one text must not be used for another text of the same length.
    [Test]
    public void TestPossibleMatchIsSearchedAgainForAnotherText()
    {
        var pipeline = new MarkdownPipelineBuilder().UseAutoLinks().Build();
        var processor = new InlineProcessor(new MarkdownDocument(), pipeline.InlineParsers, false, null);

        var slice = new StringSlice("a www.b.com");
        processor.LiteralInlineParser.Match(processor, ref slice);
        Assert.That(slice.Start, Is.EqualTo(2));

        slice = new StringSlice("a wwwxb.com");
        processor.LiteralInlineParser.Match(processor, ref slice);
        Assert.That(slice.Start, Is.EqualTo(11));
    }

    private static string Dump(MarkdownDocument document)
    {
        var builder = new StringBuilder();
        foreach (var node in document.Descendants())
        {
            builder.Append($"{node.GetType().Name} ({node.Line},{node.Column}) {node.Span.Start}-{node.Span.End}");
            if (node is LiteralInline literal)
            {
                builder.Append($" '{literal.Content}'");
            }
            else if (node is LinkInline link)
            {
                builder.Append($" {link.Url}");
            }
            builder.Append('\n');
        }
        return builder.ToString();
    }

    private sealed class NeverMatchingInlineParser : InlineParser
    {
        public NeverMatchingInlineParser(params char[] openingCharacters)
        {
            OpeningCharacters = openingCharacters;
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice) => false;
    }
}
