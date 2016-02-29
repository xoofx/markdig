using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html.Inlines
{
    public class DelimiterInlineRenderer : HtmlObjectRenderer<DelimiterInline>
    {
        protected override void Write(HtmlMarkdownRenderer renderer, DelimiterInline obj)
        {
            renderer.WriteEscape(obj.ToLiteral());
            renderer.WriteChildren(obj);
        }
    }
}