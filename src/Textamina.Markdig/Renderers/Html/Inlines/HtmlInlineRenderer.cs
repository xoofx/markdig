using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html.Inlines
{
    public class HtmlInlineRenderer : HtmlObjectRenderer<HtmlInline>
    {
        protected override void Write(HtmlMarkdownRenderer renderer, HtmlInline obj)
        {
            renderer.Write(obj.Tag);
        }
    }
}