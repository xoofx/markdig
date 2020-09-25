// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;
using System.Diagnostics;

namespace Markdig.Renderers.Normalize
{
    /// <summary>
    /// A Normalize renderer for a <see cref="ParagraphBlock"/>.
    /// </summary>
    /// <seealso cref="NormalizeObjectRenderer{ParagraphBlock}" />
    [DebuggerDisplay("renderer.Writer.ToString()")]
    public class ParagraphRenderer : NormalizeObjectRenderer<ParagraphBlock>
    {
        protected override void Write(NormalizeRenderer renderer, ParagraphBlock parapgraph)
        {
            renderer.RenderLinesBefore(parapgraph);
            renderer.Write(parapgraph.BeforeWhitespace);
            renderer.WriteLeafInline(parapgraph);
            renderer.FinishBlock();
            renderer.RenderLinesAfter(parapgraph);
        }
    }
}