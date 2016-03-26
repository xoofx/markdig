// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;

namespace Textamina.Markdig.Extensions.Mathematics
{
    /// <summary>
    /// A HTML renderer for a <see cref="MathInline"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Renderers.Html.HtmlObjectRenderer{Figure}" />
    public class HtmlMathInlineRenderer : HtmlObjectRenderer<MathInline>
    {
        protected override void Write(HtmlRenderer renderer, MathInline obj)
        {
            renderer.Write("<span").WriteAttributes(obj).Write(">");
            renderer.WriteEscape(ref obj.Content);
            renderer.Write("</span>");
        }
    }
}