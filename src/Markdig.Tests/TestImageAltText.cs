using System.Text.RegularExpressions;

namespace Markdig.Tests;

[TestFixture]
public class TestImageAltText
{
    [Test]
    [TestCase("![](image.jpg)", "")]
    [TestCase("![foo](image.jpg)", "foo")]
    [TestCase("![][1]\n\n[1]: image.jpg", "")]
    [TestCase("![bar][1]\n\n[1]: image.jpg", "bar")]
    [TestCase("![](image.jpg 'title')", "")]
    [TestCase("![foo](image.jpg 'title')", "foo")]
    [TestCase("![][1]\n\n[1]: image.jpg 'title'", "")]
    [TestCase("![bar][1]\n\n[1]: image.jpg 'title'", "bar")]
    public void TestImageHtmlAltText(string markdown, string expectedAltText)
    {
        string html = Markdown.ToHtml(markdown);
        string actualAltText = Regex.Match(html, "alt=\"(.*?)\"").Groups[1].Value;
        Assert.AreEqual(expectedAltText, actualAltText);
    }
}
