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
    public void TestPrecheckSkippingKeepsSyntaxTree(string markdown)
    {
        foreach (var trackTrivia in new[] { false, true })
        {
            var builder = new MarkdownPipelineBuilder().UsePreciseSourceLocation().UseAdvancedExtensions();
            builder.TrackTrivia = trackTrivia;
            var pipeline = builder.Build();

            // A global parser is tried at every opening character, which disables the skipping
            var referenceBuilder = new MarkdownPipelineBuilder().UsePreciseSourceLocation().UseAdvancedExtensions();
            referenceBuilder.TrackTrivia = trackTrivia;
            referenceBuilder.InlineParsers.Add(new NeverMatchingInlineParser());
            var referencePipeline = referenceBuilder.Build();

            var document = Markdown.Parse(markdown, pipeline);
            var referenceDocument = Markdown.Parse(markdown, referencePipeline);

            Assert.That(Dump(document), Is.EqualTo(Dump(referenceDocument)), $"TrackTrivia: {trackTrivia}");
            Assert.That(Markdown.ToHtml(document, pipeline), Is.EqualTo(Markdown.ToHtml(referenceDocument, referencePipeline)), $"TrackTrivia: {trackTrivia}");
        }
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
        public override bool Match(InlineProcessor processor, ref StringSlice slice) => false;
    }
}
