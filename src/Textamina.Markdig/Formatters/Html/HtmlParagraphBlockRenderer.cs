using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Formatters.Html
{
    public class HtmlParagraphBlockRenderer : HtmlRendererBase<ParagraphBlock>
    {
        protected override void Write(HtmlFormatter visitor, HtmlWriter writer, ParagraphBlock obj)
        {
            writer.EnsureLine();
            if (!writer.ImplicitParagraph)
            {
                writer.WriteLine("<p>");
            }
            WriteLeafInline(visitor, writer, obj);
            if (!writer.ImplicitParagraph)
            {
                writer.WriteLine("</p>");
            }
        }
    }
}