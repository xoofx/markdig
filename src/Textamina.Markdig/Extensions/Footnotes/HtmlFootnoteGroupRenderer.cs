// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;

namespace Textamina.Markdig.Extensions.Footnotes
{
    public class HtmlFootnoteGroupRenderer : HtmlObjectRenderer<FootnoteGroup>
    {
        protected override void Write(HtmlRenderer renderer, FootnoteGroup footnoteGroup)
        {
            var footnotes = footnoteGroup.Children;
            renderer.EnsureLine();
            renderer.WriteLine("<div class=\"footnotes\">");
            renderer.WriteLine("<hr />");
            renderer.WriteLine("<ol>");

            for (int i = 0; i < footnotes.Count; i++)
            {
                var footnote = (Footnote)footnotes[i];
                renderer.WriteLine($"<li id=\"fn:{footnote.Order}\">");
                renderer.WriteChildren(footnote);
                renderer.WriteLine("</li>");
            }
            renderer.WriteLine("</ol>");
            renderer.WriteLine("</div>");
        }
    }
}