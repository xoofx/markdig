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
}
