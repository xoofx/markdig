using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;

namespace Markdig.Tests;

internal static class TestRoundtrip
{
    internal static void TestSpec(string markdownText, string expected, string extensions, string context = null)
    {
        RoundTrip(markdownText, context);
    }

    internal static void RoundTrip(string markdown, string context = null)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .EnableTrackTrivia()
            .UseYamlFrontMatter()
            .ConfigureRoundtripRenderer()
            .Build();
        MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);

        var result = Markdown.ToHtml(markdownDocument, pipeline);
        TestParser.PrintAssertExpected("", result, markdown, context);
    }
}
