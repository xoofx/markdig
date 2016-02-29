using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html.Inlines
{
    public class LiteralInlineRenderer : HtmlObjectRenderer<LiteralInline>
    {
        protected override void Write(HtmlMarkdownRenderer renderer, LiteralInline obj)
        {
            renderer.WriteEscape(ref obj.Content);
        }
    }
}