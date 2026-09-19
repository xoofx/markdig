using Markdig.Extensions.AutoLinks;

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
}
