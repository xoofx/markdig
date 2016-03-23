// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;

namespace Textamina.Markdig.Extensions.Figures
{
    /// <summary>
    /// A HTML renderer for a <see cref="FigureCaption"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Renderers.Html.HtmlObjectRenderer{FigureCaption}" />
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