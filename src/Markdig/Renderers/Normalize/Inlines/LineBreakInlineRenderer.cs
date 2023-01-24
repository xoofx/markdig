// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Normalize.Inlines;

/// <summary>
/// A Normalize renderer for a <see cref="LineBreakInline"/>.
/// </summary>
/// <seealso cref="NormalizeObjectRenderer{LineBreakInline}" />
public class LineBreakInlineRenderer : NormalizeObjectRenderer<LineBreakInline>
{
    /// <summary>
    /// Gets or sets a value indicating whether to render this softline break as a Normalize hardline break tag (&lt;br /&gt;)
    /// </summary>
    public bool RenderAsHardlineBreak { get; set; }

    protected override void Write(NormalizeRenderer renderer, LineBreakInline obj)
    {
        if (obj.IsHard)
        {
            renderer.Write(obj.IsBackslash ? "\\" : "  ");
        }
        renderer.WriteLine();
    }
}