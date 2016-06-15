// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Html.Inlines
{
    /// <summary>
    /// A HTML renderer for a <see cref="LineBreakInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Markdig.Syntax.Inlines.LineBreakInline}" />
    public class LineBreakInlineRenderer : HtmlObjectRenderer<LineBreakInline>
    {
        /// <summary>
        /// Gets or sets a value indicating whether to render this softline break as a HTML hardline break tag (&lt;br /&gt;)
        /// </summary>
        public bool RenderAsHardlineBreak { get; set; }

        protected override void Write(HtmlRenderer renderer, LineBreakInline obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                if (obj.IsHard || RenderAsHardlineBreak)
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