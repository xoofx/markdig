// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;
using System.Collections.Generic;

namespace Markdig.Renderers.Normalize
{
    /// <summary>
    /// An Normalize renderer for a <see cref="CodeBlock"/> and <see cref="FencedCodeBlock"/>.
    /// </summary>
    /// <seealso cref="NormalizeObjectRenderer{CodeBlock}" />
    public class CodeBlockRenderer : NormalizeObjectRenderer<CodeBlock>
    {
        public bool OutputAttributesOnPre { get; set; }

        protected override void Write(NormalizeRenderer renderer, CodeBlock obj)
        {
            renderer.RenderLinesBefore(obj);
            if (obj is FencedCodeBlock fencedCodeBlock)
            {
                renderer.Write(obj.BeforeWhitespace);
                var opening = new string(fencedCodeBlock.FencedChar, fencedCodeBlock.OpeningFencedCharCount);
                renderer.Write(opening);

                if (fencedCodeBlock.WhitespaceAfterFencedChar != null)
                {
                    renderer.Write(fencedCodeBlock.WhitespaceAfterFencedChar);
                }
                if (fencedCodeBlock.Info != null)
                {
                    renderer.Write(fencedCodeBlock.Info);
                }
                if (fencedCodeBlock.WhitespaceAfterInfo != null)
                {
                    renderer.Write(fencedCodeBlock.WhitespaceAfterInfo);
                }
                if (!string.IsNullOrEmpty(fencedCodeBlock.Arguments))
                {
                    renderer
                        .Write(fencedCodeBlock.Arguments);
                }
                if (fencedCodeBlock.WhitespaceAfterArguments != null)
                {
                    renderer.Write(fencedCodeBlock.WhitespaceAfterArguments);
                }

                /* TODO do we need this causes a empty space and would render html attributes to markdown.
                var attributes = obj.TryGetAttributes();
                if (attributes != null)
                {
                    renderer.Write(" ");
                    renderer.Write(attributes);
                }
                */
                renderer.WriteLine(fencedCodeBlock.InfoNewline);

                renderer.WriteLeafRawLines(obj, true);
                var closing = new string(fencedCodeBlock.FencedChar, fencedCodeBlock.ClosingFencedCharCount);
                renderer.Write(closing);
                renderer.WriteLine(obj.Newline);
                renderer.Write(obj.AfterWhitespace);
            }
            else
            {
                var indents = new List<string>();
                foreach (var cbl in obj.CodeBlockLines)
                {
                    indents.Add(cbl.BeforeWhitespace.ToString());
                }
                renderer.PushIndent(indents);
                WriteLeafRawLines(renderer, obj);
                renderer.PopIndent();
                
                // ignore block newline, as last line references it
            }

            renderer.RenderLinesAfter(obj);
        }

        public void WriteLeafRawLines(NormalizeRenderer renderer, LeafBlock leafBlock)
        {
            if (leafBlock.Lines.Lines != null)
            {
                var lines = leafBlock.Lines;
                var slices = lines.Lines;
                for (int i = 0; i < lines.Count; i++)
                {
                    var slice = slices[i].Slice;
                    renderer.Write(ref slices[i].Slice);
                    renderer.WriteLine(slice.Newline);
                }
            }
        }
    }
}