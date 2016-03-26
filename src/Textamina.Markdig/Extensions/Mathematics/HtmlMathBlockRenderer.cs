// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;

namespace Textamina.Markdig.Extensions.Mathematics
{
    /// <summary>
    /// A HTML renderer for a <see cref="MathBlock"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Renderers.Html.HtmlObjectRenderer{T}" />
    public class HtmlMathBlockRenderer : HtmlObjectRenderer<MathBlock>
    {
        protected override void Write(HtmlRenderer renderer, MathBlock obj)
        {
            renderer.EnsureLine();
            renderer.Write("<div").WriteAttributes(obj).Write(">");
            renderer.WriteLeafRawLines(obj, true, true);
            renderer.WriteLine("</div>");
        }
    }
}