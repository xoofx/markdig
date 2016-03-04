using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html.Inlines
{
    public class CodeInlineRenderer : HtmlObjectRenderer<CodeInline>
    {
        protected override void Write(HtmlRenderer renderer, CodeInline obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("<code").WriteAttributes(obj).Write(">");
            }
            renderer.WriteEscape(obj.Content);
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("</code>");
            }
        }
    }
}