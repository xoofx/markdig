using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html.Inlines
{
    public class HardlineBreakInlineRenderer : HtmlObjectRenderer<HardlineBreakInline>
    {
        protected override void Write(HtmlRenderer renderer, HardlineBreakInline obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.WriteLine("<br />");
            }
            else
            {
                renderer.Write(" ");
            }
        }
    }
}