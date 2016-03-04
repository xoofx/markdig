// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html.Inlines
{
    public class DelimiterInlineRenderer : HtmlObjectRenderer<DelimiterInline>
    {
        protected override void Write(HtmlRenderer renderer, DelimiterInline obj)
        {
            renderer.WriteEscape(obj.ToLiteral());
            renderer.WriteChildren(obj);
        }
    }
}