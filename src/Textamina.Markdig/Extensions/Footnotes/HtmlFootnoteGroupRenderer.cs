// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;

namespace Textamina.Markdig.Extensions.Footnotes
{
    public class HtmlFootnoteGroupRenderer : HtmlObjectRenderer<FootnoteGroup>
    {
        protected override void Write(HtmlRenderer writer, FootnoteGroup footnoteGroup)
        {
            var footnotes = footnoteGroup.Children;
            writer.EnsureLine();
            writer.WriteLine("<div class=\"footnotes\">");
            writer.WriteLine("<hr />");
            writer.WriteLine("<ol>");

            for (int i = 0; i < footnotes.Count; i++)
            {
                var footnote = (Footnote)footnotes[i];
                writer.WriteLine($"<li id=\"fn:{footnote.Order}\">");
                writer.WriteChildren(footnote);
                writer.WriteLine("</li>");
            }
            writer.WriteLine("</ol>");
            writer.WriteLine("</div>");
        }
    }
}