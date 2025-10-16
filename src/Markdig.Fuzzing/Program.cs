using Markdig;
using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;
using SharpFuzz;
using System.Diagnostics;
using System.Text;

ReadOnlySpanAction fuzzTarget = ParseRenderFuzzer.FuzzTarget;

if (args.Length > 0)
{
    // Run the target on existing inputs
    string[] files = Directory.Exists(args[0])
        ? Directory.GetFiles(args[0])
        : [args[0]];

    Debugger.Launch();

    foreach (string inputFile in files)
    {
        fuzzTarget(File.ReadAllBytes(inputFile));
    }
}
else
{
    Fuzzer.LibFuzzer.Run(fuzzTarget);
}

sealed class ParseRenderFuzzer
{
    private static readonly MarkdownPipeline s_advancedPipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    private static readonly ResettableRoundtripRenderer _roundtripRenderer = new();

    public static void FuzzTarget(ReadOnlySpan<byte> bytes)
    {
        string text = Encoding.UTF8.GetString(bytes);

        try
        {
            MarkdownDocument document = Markdown.Parse(text);
            _ = document.ToHtml();

            document = Markdown.Parse(text, s_advancedPipeline);
            _ = document.ToHtml(s_advancedPipeline);

            document = Markdown.Parse(text, trackTrivia: true);
            _ = document.ToHtml();
            _roundtripRenderer.Reset();
            _roundtripRenderer.Render(document);

            _ = Markdown.Normalize(text);
            _ = Markdown.ToPlainText(text);
        }
        catch (Exception ex) when (IsIgnorableException(ex)) { }
    }

    private static bool IsIgnorableException(Exception exception)
    {
        return exception.Message.Contains("Markdown elements in the input are too deeply nested", StringComparison.Ordinal);
    }

    private sealed class ResettableRoundtripRenderer : RoundtripRenderer
    {
        public ResettableRoundtripRenderer() : base(new StringWriter(new StringBuilder(1024 * 1024))) { }

        public new void Reset() => base.Reset();
    }
}