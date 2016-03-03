using Textamina.Markdig.Extensions.Tables;
using Textamina.Markdig.Parsers.Inlines;
using Textamina.Markdig.Renderers;

namespace Textamina.Markdig.Extensions.Footnotes
{
    public class FootnoteExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            if (!pipeline.BlockParsers.Contains<FootnoteBlockParser>())
            {
                // Insert the parser before any other parsers
                pipeline.BlockParsers.Insert(0, new FootnoteBlockParser());
            }

            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ClosingObjectRenderers.Contains<HtmlFootnoteRenderer>())
            {
                htmlRenderer.ClosingObjectRenderers.Add(new HtmlFootnoteRenderer());
            }
        }
    }
}