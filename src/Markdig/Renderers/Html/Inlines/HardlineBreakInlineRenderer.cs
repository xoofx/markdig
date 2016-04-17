// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Html.Inlines
{
    /// <summary>
    /// A HTML renderer for a <see cref="HardlineBreakInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Markdig.Syntax.Inlines.HardlineBreakInline}" />
    public class HardlineBreakInlineRenderer : HtmlObjectRenderer<HardlineBreakInline>
    {
        protected override void Write(HtmlRenderer renderer, HardlineBreakInline obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.WriteLine("<br />");
            }
            else
            {
                renderer.Write(" ");
            }
        }
    }
}