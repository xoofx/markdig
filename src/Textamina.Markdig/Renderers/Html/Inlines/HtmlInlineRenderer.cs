// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html.Inlines
{
    public class HtmlInlineRenderer : HtmlObjectRenderer<HtmlInline>
    {
        protected override void Write(HtmlRenderer renderer, HtmlInline obj)
        {
            renderer.Write(obj.Tag);
        }
    }
}