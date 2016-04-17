// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Html.Inlines
{
    /// <summary>
    /// A HTML renderer for a <see cref="CodeInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Markdig.Syntax.Inlines.CodeInline}" />
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