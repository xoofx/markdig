// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Renderers.Html.Inlines
{
    public class CodeInlineRenderer : HtmlObjectRenderer<CodeInline>
    {
        protected override void Write(HtmlRenderer renderer, CodeInline obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("<code").WriteAttributes(obj).Write(">");
            }
            renderer.WriteEscape(obj.Content);
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("</code>");
            }
        }
    }
}