using static Markdig.Tests.TestRoundtrip;

namespace Markdig.Tests.RoundtripSpecs;

[TestFixture]
public class TestIndentedCodeBlock
{
    // A codeblock is indented with 4 spaces. After the 4th space, whitespace is interpreted as content.
    // l = line
    [TestCase("    l")]
    [TestCase("     l")]
    [TestCase("\tl")]
    [TestCase("\t\tl")]
    [TestCase("\tl1\n    l1")]

    [TestCase("\n    l")]
    [TestCase("\n\n    l")]
    [TestCase("\n    l\n")]
    [TestCase("\n    l\n\n")]
    [TestCase("\n\n    l\n")]
    [TestCase("\n\n    l\n\n")]

    [TestCase("    l\n    l")]
    [TestCase("    l\n    l\n    l")]


    // two newlines are needed for indented codeblock start after paragraph
    [TestCase("p\n\n    l")]
    [TestCase("p\n\n    l\n")]
    [TestCase("p\n\n    l\n\n")]

    [TestCase("p\n\n    l\n    l")]
    [TestCase("p\n\n    l\n     l")]

    [TestCase("    l\n\np\n\n    l")]
    [TestCase("    l    l\n\np\n\n    l    l")]
    public void Test(string value)
    {
        RoundTrip(value);
    }

    [TestCase("    l\n")]
    [TestCase("    l\r")]
    [TestCase("    l\r\n")]

    [TestCase("    l\n    l")]
    [TestCase("    l\n    l\n")]
    [TestCase("    l\n    l\r")]
    [TestCase("    l\n    l\r\n")]

    [TestCase("    l\r    l")]
    [TestCase("    l\r    l\n")]
    [TestCase("    l\r    l\r")]
    [TestCase("    l\r    l\r\n")]

    [TestCase("    l\r\n    l")]
    [TestCase("    l\r\n    l\n")]
    [TestCase("    l\r\n    l\r")]
    [TestCase("    l\r\n    l\r\n")]
    public void TestNewline(string value)
    {
        RoundTrip(value);
    }

    [TestCase("    l\n\n    l\n")]
    [TestCase("    l\n\n\n    l\n")]
    public void TestNewlinesInBetweenResultInOneCodeBlock(string value)
    {
        var pipelineBuilder = new MarkdownPipelineBuilder();
        pipelineBuilder.EnableTrackTrivia();
        MarkdownPipeline pipeline = pipelineBuilder.Build();
        var markdownDocument = Markdown.Parse(value, pipeline);

        Assert.AreEqual(1, markdownDocument.Count);
    }

    [TestCase("    l\n\np")]
    [TestCase("    l\n\n\np")]
    [TestCase("    l\n\n\n\np")]
    public void TestParagraph(string value)
    {
        RoundTrip(value);
    }
}
