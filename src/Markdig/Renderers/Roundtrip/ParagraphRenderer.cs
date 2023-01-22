// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;
using System.Diagnostics;

namespace Markdig.Renderers.Roundtrip;

/// <summary>
/// A Roundtrip renderer for a <see cref="ParagraphBlock"/>.
/// </summary>
/// <seealso cref="RoundtripObjectRenderer{ParagraphBlock}" />
[DebuggerDisplay("renderer.Writer.ToString()")]
public class ParagraphRenderer : RoundtripObjectRenderer<ParagraphBlock>
{
    protected override void Write(RoundtripRenderer renderer, ParagraphBlock paragraph)
    {
        renderer.RenderLinesBefore(paragraph);
        renderer.Write(paragraph.TriviaBefore);
        renderer.WriteLeafInline(paragraph);
        //renderer.Write(paragraph.Newline); // paragraph typically has LineBreakInlines as closing inline nodes
        renderer.RenderLinesAfter(paragraph);
    }
}