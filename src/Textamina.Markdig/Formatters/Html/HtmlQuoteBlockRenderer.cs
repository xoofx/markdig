using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Formatters.Html
{
    public class HtmlQuoteBlockRenderer : HtmlRendererBase<QuoteBlock>
    {
        protected override void Write(HtmlFormatter visitor, HtmlWriter writer, QuoteBlock obj)
        {
            writer.EnsureLine();
            writer.WriteLine("<blockquote>");
            var savedImplicitParagraph = writer.ImplicitParagraph;
            writer.ImplicitParagraph = false;
            visitor.VisitContainer(writer, obj);
            writer.ImplicitParagraph = savedImplicitParagraph;
            writer.WriteLine("</blockquote>");
        }
    }
}