// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Renderers.Roundtrip;

/// <summary>
/// An Roundtrip renderer for a <see cref="CodeBlock"/> and <see cref="FencedCodeBlock"/>.
/// </summary>
/// <seealso cref="RoundtripObjectRenderer{CodeBlock}" />
public class CodeBlockRenderer : RoundtripObjectRenderer<CodeBlock>
{
    protected override void Write(RoundtripRenderer renderer, CodeBlock obj)
    {
        renderer.RenderLinesBefore(obj);
        if (obj is FencedCodeBlock fencedCodeBlock)
        {
            renderer.Write(obj.TriviaBefore);
            renderer.Write(fencedCodeBlock.FencedChar, fencedCodeBlock.OpeningFencedCharCount);

            if (!fencedCodeBlock.TriviaAfterFencedChar.IsEmpty)
            {
                renderer.Write(fencedCodeBlock.TriviaAfterFencedChar);
            }
            if (fencedCodeBlock.Info != null)
            {
                renderer.Write(fencedCodeBlock.UnescapedInfo);
            }
            if (!fencedCodeBlock.TriviaAfterInfo.IsEmpty)
            {
                renderer.Write(fencedCodeBlock.TriviaAfterInfo);
            }
            if (!string.IsNullOrEmpty(fencedCodeBlock.Arguments))
            {
                renderer.Write(fencedCodeBlock.UnescapedArguments);
            }
            if (!fencedCodeBlock.TriviaAfterArguments.IsEmpty)
            {
                renderer.Write(fencedCodeBlock.TriviaAfterArguments);
            }

            /* TODO do we need this causes a empty space and would render html attributes to markdown.
            var attributes = obj.TryGetAttributes();
            if (attributes != null)
            {
                renderer.Write(" ");
                renderer.Write(attributes);
            }
            */
            renderer.WriteLine(fencedCodeBlock.InfoNewLine);

            renderer.WriteLeafRawLines(obj);

            renderer.Write(fencedCodeBlock.TriviaBeforeClosingFence);
            renderer.Write(fencedCodeBlock.FencedChar, fencedCodeBlock.ClosingFencedCharCount);
            if (fencedCodeBlock.ClosingFencedCharCount > 0)
            {
                // See example 207: "> ```\nfoo\n```"
                renderer.WriteLine(obj.NewLine);
            }
            renderer.Write(obj.TriviaAfter);
        }
        else
        {
            var indents = new string[obj.CodeBlockLines.Count];
            for (int i = 0; i < obj.CodeBlockLines.Count; i++)
            {
                indents[i] = obj.CodeBlockLines[i].TriviaBefore.ToString();
            }
            renderer.PushIndent(indents);
            WriteLeafRawLines(renderer, obj);
            renderer.PopIndent();
            
            // ignore block newline, as last line references it
        }

        renderer.RenderLinesAfter(obj);
    }

    public void WriteLeafRawLines(RoundtripRenderer renderer, LeafBlock leafBlock)
    {
        if (leafBlock.Lines.Lines != null)
        {
            var lines = leafBlock.Lines;
            var slices = lines.Lines;
            for (int i = 0; i < lines.Count; i++)
            {
                ref StringSlice slice = ref slices[i].Slice;
                renderer.Write(ref slice);
                renderer.WriteLine(slice.NewLine);
            }
        }
    }
}