// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Mathematics;

/// <summary>
/// A HTML renderer for a <see cref="MathInline"/>.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{Figure}" />
public class HtmlMathInlineRenderer : HtmlObjectRenderer<MathInline>
{
    protected override void Write(HtmlRenderer renderer, MathInline obj)
    {
        if (renderer.EnableHtmlForInline)
        {
            renderer.Write("<span").WriteAttributes(obj).Write(">\\(");
        }

        if (renderer.EnableHtmlEscape)
        {
            renderer.WriteEscape(ref obj.Content);
        }
        else
        {
            renderer.Write(ref obj.Content);
        }

        if (renderer.EnableHtmlForInline)
        {
            renderer.Write("\\)</span>");
        }
    }
}