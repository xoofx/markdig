// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Roundtrip;

/// <summary>
/// A Roundtrip renderer for a <see cref="ThematicBreakBlock"/>.
/// </summary>
/// <seealso cref="RoundtripObjectRenderer{ThematicBreakBlock}" />
public class ThematicBreakRenderer : RoundtripObjectRenderer<ThematicBreakBlock>
{
    protected override void Write(RoundtripRenderer renderer, ThematicBreakBlock obj)
    {
        renderer.RenderLinesBefore(obj);

        renderer.Write(obj.Content);
        renderer.WriteLine(obj.NewLine);
        renderer.RenderLinesAfter(obj);
    }
}