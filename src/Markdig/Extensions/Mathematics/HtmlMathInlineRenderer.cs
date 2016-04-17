// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Mathematics
{
    /// <summary>
    /// A HTML renderer for a <see cref="MathInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Figure}" />
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