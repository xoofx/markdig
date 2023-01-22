// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Normalize.Inlines;

/// <summary>
/// A Normalize renderer for a <see cref="HtmlEntityInline"/>.
/// </summary>
public class NormalizeHtmlEntityInlineRenderer : NormalizeObjectRenderer<HtmlEntityInline>
{
    protected override void Write(NormalizeRenderer renderer, HtmlEntityInline obj)
    {
        renderer.Write(obj.Original);
    }
}