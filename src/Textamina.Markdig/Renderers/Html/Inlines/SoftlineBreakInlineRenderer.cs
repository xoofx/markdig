// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html.Inlines
{
    public class SoftlineBreakInlineRenderer : HtmlObjectRenderer<SoftlineBreakInline>
    {
        public bool RenderAsHardlineBreak { get; set; }

        protected override void Write(HtmlRenderer renderer, SoftlineBreakInline obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                if (RenderAsHardlineBreak)
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