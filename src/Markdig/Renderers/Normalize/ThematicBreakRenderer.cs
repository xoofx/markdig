// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Renderers.Normalize
{
    /// <summary>
    /// A Normalize renderer for a <see cref="ThematicBreakBlock"/>.
    /// </summary>
    /// <seealso cref="NormalizeObjectRenderer{ThematicBreakBlock}" />
    public class ThematicBreakRenderer : NormalizeObjectRenderer<ThematicBreakBlock>
    {
        protected override void Write(NormalizeRenderer renderer, ThematicBreakBlock obj)
        {
            renderer.RenderLinesBefore(obj);

            // for now, render always a newline
            // TODO: only render a newline when not last line
            //renderer.Write(obj.BeforeWhitespace);
            renderer.Write(obj.Content);
            //renderer.Write(obj.AfterWhitespace);
            renderer.RenderLineAfterIfNeeded(obj);
            renderer.RenderLinesAfter(obj);
        }
    }
}