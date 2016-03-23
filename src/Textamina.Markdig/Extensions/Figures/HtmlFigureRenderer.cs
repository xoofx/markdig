// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;

namespace Textamina.Markdig.Extensions.Figures
{
    /// <summary>
    /// A HTML renderer for a <see cref="Figure"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Renderers.Html.HtmlObjectRenderer{Figure}" />
    public class HtmlFigureRenderer : HtmlObjectRenderer<Figure>
    {
        protected override void Write(HtmlRenderer renderer, Figure obj)
        {
            renderer.EnsureLine();
            renderer.Write("<figure").WriteAttributes(obj).WriteLine(">");
            renderer.WriteChildren(obj);
            renderer.WriteLine("</figure>");
        }
    }
}