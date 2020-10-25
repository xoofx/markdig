// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Roundtrip.Inlines
{
    /// <summary>
    /// A Normalize renderer for a <see cref="LineBreakInline"/>.
    /// </summary>
    /// <seealso cref="NormalizeObjectRenderer{LineBreakInline}" />
    public class LineBreakInlineRenderer : RoundtripObjectRenderer<LineBreakInline>
    {
        /// <summary>
        /// Gets or sets a value indicating whether to render this softline break as a Normalize hardline break tag (&lt;br /&gt;)
        /// </summary>
        public bool RenderAsHardlineBreak { get; set; }

        protected override void Write(RoundtripRenderer renderer, LineBreakInline obj)
        {
            if (obj.IsHard && obj.IsBackslash)
            {
                renderer.Write("\\");
                //renderer.Write(obj.IsBackslash ? "\\" : "  ");
            }
            renderer.WriteLine(obj.Newline);
        }
    }
}