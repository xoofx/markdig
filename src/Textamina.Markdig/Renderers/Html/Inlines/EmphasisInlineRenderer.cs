using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html.Inlines
{
    public class EmphasisInlineRenderer : HtmlObjectRenderer<EmphasisInline>
    {
        protected override void Write(HtmlMarkdownRenderer renderer, EmphasisInline obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write(obj.Strong ? "<strong>" : "<em>");
            }
            renderer.WriteChildren(obj);
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write(obj.Strong ? "</strong>" : "</em>");
            }
        }
    }
}