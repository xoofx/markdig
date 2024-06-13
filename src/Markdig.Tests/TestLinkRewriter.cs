using Markdig.Parsers;
using Markdig.Renderers;

namespace Markdig.Tests;

public class TestLinkRewriter
{
    [Test]
    public void ReplacesRelativeLinks()
    {
        TestSpec(s => "abc" + s, "Link: [hello](/relative.jpg)", "abc/relative.jpg");
        TestSpec(s => s + "xyz", "Link: [hello](relative.jpg)", "relative.jpgxyz");
        TestSpec(null, "Link: [hello](relative.jpg)", "relative.jpg");
        TestSpec(null, "Link: [hello](/relative.jpg)", "/relative.jpg");
    }

    [Test]
    public void ReplacesRelativeImageSources()
    {
        TestSpec(s => "abc" + s, "Image: ![alt text](/image.jpg)", "abc/image.jpg");
        TestSpec(s => "abc" + s, "Image: ![alt text](image.jpg \"title\")", "abcimage.jpg");
        TestSpec(null, "Image: ![alt text](/image.jpg)", "/image.jpg");
    }

    public static void TestSpec(Func<string,string> linkRewriter, string markdown, string expectedLink)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .ConfigureHtmlRenderer(r => r.UseLinkRewriter(linkRewriter))
            .Build();

        var document = MarkdownParser.Parse(markdown, pipeline);
        var html = Markdown.ToHtml(document, pipeline);

        Assert.That(html, Contains.Substring("=\"" + expectedLink + "\""));
    }
}