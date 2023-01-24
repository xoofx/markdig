// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Html.Inlines;

/// <summary>
/// A HTML renderer for a <see cref="CodeInline"/>.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{CodeInline}" />
public class CodeInlineRenderer : HtmlObjectRenderer<CodeInline>
{
    protected override void Write(HtmlRenderer renderer, CodeInline obj)
    {
        if (renderer.EnableHtmlForInline)
        {
            renderer.Write("<code");
            renderer.WriteAttributes(obj);
            renderer.WriteRaw('>');
        }
        if (renderer.EnableHtmlEscape)
        {
            renderer.WriteEscape(obj.ContentSpan);
        }
        else
        {
            renderer.Write(obj.ContentSpan);
        }
        if (renderer.EnableHtmlForInline)
        {
            renderer.WriteRaw("</code>");
        }
    }
}