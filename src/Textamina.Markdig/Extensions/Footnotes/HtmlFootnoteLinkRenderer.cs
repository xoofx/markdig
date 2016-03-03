using Textamina.Markdig.Helpers;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;

namespace Textamina.Markdig.Extensions.Footnotes
{
    public class HtmlFootnoteLinkRenderer : HtmlObjectRenderer<FootnoteLink>
    {
        public HtmlFootnoteLinkRenderer()
        {
            BackLinkString = "&#8617;";
            FootnoteLinkClass = "footnote-ref";
            FootnoteBackLinkClass = "footnote-back-ref";
        }
        public string BackLinkString { get; set; }

        public string FootnoteLinkClass { get; set; }

        public string FootnoteBackLinkClass { get; set; }

        protected override void Write(HtmlRenderer writer, FootnoteLink link)
        {
            var order = link.Footnote.Order ?? 0;
            writer.Write(link.IsBackLink
                ? $"<a href=\"#fnref:{link.Index}\" class=\"{FootnoteBackLinkClass}\">{HtmlHelper.Unescape(BackLinkString)}</a>"
                : $"<a id=\"fnref:{link.Index}\" href=\"#fn:{order}\" class=\"{FootnoteLinkClass}\"><sup>{order}</sup></a>");
        }
    }
}