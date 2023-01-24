using Markdig.Extensions.EmphasisExtras;

namespace Markdig.Tests;

[TestFixture]
public class TestEmphasisExtraOptions
{
    [Test]
    public void OnlyStrikethrough_Single()
    {
        TestParser.TestSpec("~foo~", "<p>~foo~</p>", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Strikethrough).Build());
    }

    [Test]
    public void OnlyStrikethrough_Double()
    {
        TestParser.TestSpec("~~foo~~", "<p><del>foo</del></p>", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Strikethrough).Build());
    }

    [Test]
    public void OnlySubscript_Single()
    {
        TestParser.TestSpec("~foo~", "<p><sub>foo</sub></p>", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Subscript).Build());
    }

    [Test]
    public void OnlySubscript_Double()
    {
        TestParser.TestSpec("~~foo~~", "<p><sub><sub>foo</sub></sub></p>", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Subscript).Build());
    }

    [Test]
    public void SubscriptAndStrikethrough_Single()
    {
        TestParser.TestSpec("~foo~", "<p><sub>foo</sub></p>", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Strikethrough | EmphasisExtraOptions.Subscript).Build());
    }

    [Test]
    public void SubscriptAndStrikethrough_Double()
    {
        TestParser.TestSpec("~~foo~~", "<p><del>foo</del></p>", new MarkdownPipelineBuilder().UseEmphasisExtras(EmphasisExtraOptions.Strikethrough | EmphasisExtraOptions.Subscript).Build());
    }
}