// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Figures
{
    /// <summary>
    /// A HTML renderer for a <see cref="Figure"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Figure}" />
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