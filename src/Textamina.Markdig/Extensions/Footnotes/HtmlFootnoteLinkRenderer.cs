// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
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

        protected override void Write(HtmlRenderer renderer, FootnoteLink link)
        {
            var order = link.Footnote.Order ?? 0;
            renderer.Write(link.IsBackLink
                ? $"<a href=\"#fnref:{link.Index}\" class=\"{FootnoteBackLinkClass}\">{HtmlHelper.Unescape(BackLinkString)}</a>"
                : $"<a id=\"fnref:{link.Index}\" href=\"#fn:{order}\" class=\"{FootnoteLinkClass}\"><sup>{order}</sup></a>");
        }
    }
}