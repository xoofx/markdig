using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html.Inlines
{
    public class AutolinkInlineRenderer : HtmlObjectRenderer<AutolinkInline>
    {
        protected override void Write(HtmlRenderer renderer, AutolinkInline obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("<a href=\"");
                if (obj.IsEmail)
                {
                    renderer.Write("mailto:");
                }
                renderer.WriteEscapeUrl(obj.Url);
                renderer.WriteAttributes(obj);
                renderer.Write("\">");
            }

            renderer.WriteEscape(obj.Url);

            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("</a>");
            }
        }
    }
}