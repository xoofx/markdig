// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Globalization;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    /// <summary>
    /// An HTML renderer for a <see cref="HeadingBlock"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Renderers.Html.HtmlObjectRenderer{Textamina.Markdig.Syntax.HeadingBlock}" />
    public class HeadingRenderer : HtmlObjectRenderer<HeadingBlock>
    {
        protected override void Write(HtmlRenderer renderer, HeadingBlock obj)
        {
            var heading = obj.Level.ToString(CultureInfo.InvariantCulture);
            renderer.Write("<h").Write(heading).WriteAttributes(obj).Write(">");
            renderer.WriteLeafInline(obj);
            renderer.Write("</h").Write(heading).WriteLine(">");
        }
    }
}