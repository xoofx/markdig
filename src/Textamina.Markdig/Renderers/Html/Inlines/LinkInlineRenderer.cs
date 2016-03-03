using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html.Inlines
{
    public class LinkInlineRenderer : HtmlObjectRenderer<LinkInline>
    {
        protected override void Write(HtmlRenderer renderer, LinkInline link)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write(link.IsImage ? "<img src=\"" : "<a href=\"");
                renderer.WriteEscapeUrl(link.Url);
                renderer.Write("\"");
            }
            if (link.IsImage)
            {
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write(" alt=\"");
                }
                var wasEnableHtmlForInline = renderer.EnableHtmlForInline;
                renderer.EnableHtmlForInline = false;
                renderer.WriteChildren(link);
                renderer.EnableHtmlForInline = wasEnableHtmlForInline;
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write("\"");
                }
            }

            if (renderer.EnableHtmlForInline && !string.IsNullOrEmpty(link.Title))
            {
                renderer.Write(" title=\"");
                renderer.WriteEscape(link.Title);
                renderer.Write("\"");
            }

            if (link.IsImage)
            {
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write(" />");
                }
            }
            else
            {
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write(">");
                }
                renderer.WriteChildren(link);
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write("</a>");
                }
            }
        }
    }
}