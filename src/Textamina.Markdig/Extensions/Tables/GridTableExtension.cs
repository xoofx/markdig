using Textamina.Markdig.Parsers.Inlines;
using Textamina.Markdig.Renderers;

namespace Textamina.Markdig.Extensions.Tables
{
    /// <summary>
    /// Extension that allows to use grid tables.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.IMarkdownExtension" />
    public class GridTableExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            if (!pipeline.BlockParsers.Contains<GridTableBlockParser>())
            {
                pipeline.BlockParsers.Insert(0, new GridTableBlockParser());
            }

            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<HtmlTableRenderer>())
            {
                htmlRenderer.ObjectRenderers.Add(new HtmlTableRenderer());
            }
        }
    }
}