using Markdig.Parsers;
using Markdig.Renderers;

namespace Markdig.Tests;

public class TestRelativeUrlReplacement
{
    [Test]
    public void ReplacesRelativeLinks()
    {
        TestSpec("https://example.com", "Link: [hello](/relative.jpg)", "https://example.com/relative.jpg");
        TestSpec("https://example.com", "Link: [hello](relative.jpg)", "https://example.com/relative.jpg");
        TestSpec("https://example.com/", "Link: [hello](/relative.jpg?a=b)", "https://example.com/relative.jpg?a=b");
        TestSpec("https://example.com/", "Link: [hello](relative.jpg#x)", "https://example.com/relative.jpg#x");
        TestSpec(null, "Link: [hello](relative.jpg)", "relative.jpg");
        TestSpec(null, "Link: [hello](/relative.jpg)", "/relative.jpg");
        TestSpec("https://example.com", "Link: [hello](/relative.jpg)", "https://example.com/relative.jpg");
    }

    [Test]
    public void ReplacesRelativeImageSources()
    {
        TestSpec("https://example.com", "Image: ![alt text](/image.jpg)", "https://example.com/image.jpg");
        TestSpec("https://example.com", "Image: ![alt text](image.jpg \"title\")", "https://example.com/image.jpg");
        TestSpec(null, "Image: ![alt text](/image.jpg)", "/image.jpg");
    }

    public static void TestSpec(string baseUrl, string markdown, string expectedLink)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .ConfigureHtmlRenderer((r) =>
            {
                if (baseUrl != null)
                    r.BaseUrl = new Uri(baseUrl);
            })
            .Build();

        var document = MarkdownParser.Parse(markdown, pipeline);
        var html = Markdown.ToHtml(document, pipeline);

        Assert.That(html, Contains.Substring("=\"" + expectedLink + "\""));
    }
}