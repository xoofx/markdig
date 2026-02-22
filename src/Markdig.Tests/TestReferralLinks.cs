namespace Markdig.Tests;

[TestFixture]
public class TestReferralLinks
{
    [Test]
    [TestCase(new[] { "nofollow" }, "nofollow")]
    [TestCase(new[] { "noopener" }, "noopener")]
    [TestCase(new[] { "nofollow", "noopener"}, "nofollow noopener")]
    public void TestLinksWithCustomRel(string[] rels, string expected)
    {
        var markdown = "[world](http://example.com)";

        var pipeline = new MarkdownPipelineBuilder()
            .UseReferralLinks(rels)
            .Build();
        var html = Markdown.ToHtml(markdown, pipeline);

        Assert.That(html, Contains.Substring($"rel=\"{expected}\""));
    }

    [Test]
    [TestCase(new[] { "noopener" }, "noopener")]
    [TestCase(new[] { "nofollow" }, "nofollow")]
    [TestCase(new[] { "nofollow", "noopener" }, "nofollow noopener")]
    public void TestAutoLinksWithCustomRel(string[] rels, string expected)
    {
        var markdown = "http://example.com";

        var pipeline = new MarkdownPipelineBuilder()
            .UseAutoLinks()
            .UseReferralLinks(rels)
            .Build();
        var html = Markdown.ToHtml(markdown, pipeline);

        Assert.That(html, Contains.Substring($"rel=\"{expected}\""));
    }
}
