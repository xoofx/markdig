// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Normalize.Inlines;

/// <summary>
/// A Normalize renderer for an <see cref="EmphasisInline"/>.
/// </summary>
/// <seealso cref="NormalizeObjectRenderer{EmphasisInline}" />
public class EmphasisInlineRenderer : NormalizeObjectRenderer<EmphasisInline>
{
    protected override void Write(NormalizeRenderer renderer, EmphasisInline obj)
    {
        renderer.Write(obj.DelimiterChar, obj.DelimiterCount);
        renderer.WriteChildren(obj);
        renderer.Write(obj.DelimiterChar, obj.DelimiterCount);
    }
}