// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Globalization;
using Markdig.Syntax;

namespace Markdig.Renderers.Html
{
    /// <summary>
    /// An HTML renderer for a <see cref="HeadingBlock"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Markdig.Syntax.HeadingBlock}" />
    public class HeadingRenderer : HtmlObjectRenderer<HeadingBlock>
    {
        private static readonly string[] HeadingTexts = {
            "h1",
            "h2",
            "h3",
            "h4",
            "h5",
            "h6",
        };

        protected override void Write(HtmlRenderer renderer, HeadingBlock obj)
        {
            var headingText = obj.Level > 0 && obj.Level <= 6
                ? HeadingTexts[obj.Level - 1]
                : "<h" + obj.Level.ToString(CultureInfo.InvariantCulture);

            renderer.Write("<").Write(headingText).WriteAttributes(obj).Write(">");
            renderer.WriteLeafInline(obj);
            renderer.Write("</").Write(headingText).WriteLine(">");
        }
    }
}