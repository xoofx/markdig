using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html.Inlines
{
    public class SoftlineBreakInlineRenderer : HtmlObjectRenderer<SoftlineBreakInline>
    {
        public bool RenderAsHardlineBreak { get; set; }

        protected override void Write(HtmlMarkdownRenderer renderer, SoftlineBreakInline obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                if (RenderAsHardlineBreak)
                {
                    renderer.WriteLine("<br />");
                }
                else
                {
                    renderer.WriteLine();
                }
            }
            else
            {
                renderer.Write(" ");
            }
        }
    }
}