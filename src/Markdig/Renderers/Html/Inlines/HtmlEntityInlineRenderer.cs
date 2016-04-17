// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Html.Inlines
{
    /// <summary>
    /// A HTML renderer for a <see cref="HtmlEntityInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Markdig.Syntax.Inlines.HtmlEntityInline}" />
    public class HtmlEntityInlineRenderer : HtmlObjectRenderer<HtmlEntityInline>
    {
        protected override void Write(HtmlRenderer renderer, HtmlEntityInline obj)
        {
            renderer.WriteEscape(obj.Transcoded);
        }
    }
}