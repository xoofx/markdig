// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Footnotes
{
    /// <summary>
    /// A HTML renderer for a <see cref="FootnoteGroup"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Markdig.Extensions.Footnotes.FootnoteGroup}" />
    public class HtmlFootnoteGroupRenderer : HtmlObjectRenderer<FootnoteGroup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlFootnoteGroupRenderer"/> class.
        /// </summary>
        public HtmlFootnoteGroupRenderer()
        {
            GroupClass = "footnotes";
        }

        /// <summary>
        /// Gets or sets the CSS group class used when rendering the &lt;div&gt; of this instance.
        /// </summary>
        public string GroupClass { get; set; }

        protected override void Write(HtmlRenderer renderer, FootnoteGroup footnotes)
        {
            renderer.EnsureLine();
            renderer.WriteLine($"<div class=\"{GroupClass}\">");
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