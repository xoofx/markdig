using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    public class ParagraphRenderer : HtmlObjectRenderer<ParagraphBlock>
    {
        protected override void Write(HtmlMarkdownRenderer renderer, ParagraphBlock obj)
        {
            renderer.EnsureLine();
            if (!renderer.ImplicitParagraph)
            {
                renderer.Write("<p>");
            }
            renderer.WriteLeafInline(obj);
            if (!renderer.ImplicitParagraph)
            {
                renderer.WriteLine("</p>");
            }
        }
    }
}