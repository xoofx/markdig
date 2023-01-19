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
        var pipeline = new MarkdownPipelineBuilder().Build();

        var writer = new StringWriter();
        var renderer = new HtmlRenderer(writer);
        if (baseUrl != null)
            renderer.BaseUrl = new Uri(baseUrl);
        pipeline.Setup(renderer);

        var document = MarkdownParser.Parse(markdown, pipeline);
        renderer.Render(document);
        writer.Flush();

        Assert.That(writer.ToString(), Contains.Substring("=\"" + expectedLink + "\""));
    }
}