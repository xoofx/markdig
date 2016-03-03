using Textamina.Markdig.Renderers;

namespace Textamina.Markdig.Extensions.Footnotes
{
    public class FootnoteExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            if (!pipeline.BlockParsers.Contains<FootnoteParser>())
            {
                // Insert the parser before any other parsers
                pipeline.BlockParsers.Insert(0, new FootnoteParser());
            }

            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready(new HtmlFootnoteGroupRenderer());
                htmlRenderer.ObjectRenderers.AddIfNotAlready(new HtmlFootnoteLinkRenderer());
            }
        }
    }
}