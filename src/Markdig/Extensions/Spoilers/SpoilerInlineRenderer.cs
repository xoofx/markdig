// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Spoilers;

/// <summary>
///     The default <see cref="IMarkdownObjectRenderer"/> for <see cref="SpoilerInline"/>
/// </summary>
/// <seealso cref="SpoilerExtension"/>
public sealed class SpoilerInlineRenderer : HtmlObjectRenderer<SpoilerInline>
{
    protected override void Write(HtmlRenderer renderer, SpoilerInline obj)
    {
        if (renderer.EnableHtmlForInline)
        {
            renderer.Write("<span");
            renderer.WriteAttributes(obj);
            renderer.Write('>');
        }

        renderer.Write(obj.Content);

        if (renderer.EnableHtmlForInline)
        {
            renderer.Write("</span>");
        }
    }
}