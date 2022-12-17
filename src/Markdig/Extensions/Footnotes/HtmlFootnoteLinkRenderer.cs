// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Footnotes
{
    /// <summary>
    /// A HTML renderer for a <see cref="FootnoteLink"/>.
    /// </summary>
    /// <seealso cref="HtmlObjectRenderer{FootnoteLink}" />
    public class HtmlFootnoteLinkRenderer : HtmlObjectRenderer<FootnoteLink>
    {
        public HtmlFootnoteLinkRenderer(FootnoteOptions options)
        {
            BackLinkString = "&#8617;";
            FootnoteLinkClass = "footnote-ref";
            FootnoteBackLinkClass = "footnote-back-ref";
            Options = options;
        }

        public string BackLinkString { get; set; }

        public string FootnoteLinkClass { get; set; }

        public string FootnoteBackLinkClass { get; set; }

        public FootnoteOptions Options { get; private set; }

        protected override void Write(HtmlRenderer renderer, FootnoteLink link)
        {
            if (link.IsBackLink && Options.OmitBackLink)
            {
                return;
            }

            string footnoteLabel = Options.GetFootnoteLabel(link.Footnote.Order.ToString(), link.Footnote.Label);
            renderer.Write(link.IsBackLink
                ? $"<a href=\"#fnref:{link.Index}\" class=\"{FootnoteBackLinkClass}\">{BackLinkString}</a>"
                : $"<a id=\"fnref:{link.Index}\" href=\"#fn:{footnoteLabel}\" class=\"{FootnoteLinkClass}\"><sup>{footnoteLabel}</sup></a>");
        }
    }
}
