// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Normalize;

/// <summary>
/// A Normalize renderer for a <see cref="ThematicBreakBlock"/>.
/// </summary>
/// <seealso cref="NormalizeObjectRenderer{ThematicBreakBlock}" />
public class ThematicBreakRenderer : NormalizeObjectRenderer<ThematicBreakBlock>
{
    protected override void Write(NormalizeRenderer renderer, ThematicBreakBlock obj)
    {
        renderer.WriteLine(new string(obj.ThematicChar, obj.ThematicCharCount));

        renderer.FinishBlock(renderer.Options.EmptyLineAfterThematicBreak);
    }
}