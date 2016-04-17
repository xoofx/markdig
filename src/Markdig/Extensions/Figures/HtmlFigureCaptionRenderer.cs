// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Figures
{
    /// <summary>
    /// A HTML renderer for a <see cref="FigureCaption"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{FigureCaption}" />
    public class HtmlFigureCaptionRenderer : HtmlObjectRenderer<FigureCaption>
    {
        protected override void Write(HtmlRenderer renderer, FigureCaption obj)
        {
            renderer.EnsureLine();
            renderer.Write("<figcaption").WriteAttributes(obj).Write(">");
            renderer.WriteLeafInline(obj);
            renderer.WriteLine("</figcaption>");
        }
    }
}