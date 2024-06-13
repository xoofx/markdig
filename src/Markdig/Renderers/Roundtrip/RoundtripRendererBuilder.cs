using System.IO;

namespace Markdig.Renderers.Roundtrip;

/// <summary>
/// This class is used with <see cref="MarkdownExtensions.ConfigureRoundtripRenderer"/>
/// to set up a pipeline with a markdown renderer.
/// </summary>
/// <remarks>
/// This builder has no options since <see cref="RoundtripRenderer"/> has none.
/// It is solely in use internally in <see cref="MarkdownExtensions.ConfigureRoundtripRenderer"/>
/// because <see cref="MarkdownPipelineBuilder"/> can only use a renderer builder, not a renderer.
/// </remarks>
class RoundtripRendererBuilder : IMarkdownRendererBuilder
{
    public RoundtripRenderer Build(TextWriter writer) => new(writer);

    TextRendererBase IMarkdownRendererBuilder.Build(TextWriter writer) => Build(writer);
}