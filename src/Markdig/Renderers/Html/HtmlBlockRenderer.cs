// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Syntax;

namespace Markdig.Renderers.Html
{
    /// <summary>
    /// A HTML renderer for a <see cref="HtmlBlock"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Markdig.Syntax.HtmlBlock}" />
    public class HtmlBlockRenderer : HtmlObjectRenderer<HtmlBlock>
    {
        protected override void Write(HtmlRenderer renderer, HtmlBlock obj)
        {
            renderer.WriteLeafRawLines(obj, true, false);
        }
    }
}