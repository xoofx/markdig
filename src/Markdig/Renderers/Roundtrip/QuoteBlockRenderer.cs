// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Roundtrip;

/// <summary>
/// A Roundtrip renderer for a <see cref="QuoteBlock"/>.
/// </summary>
public class QuoteBlockRenderer : RoundtripObjectRenderer<QuoteBlock>
{
    protected override void Write(RoundtripRenderer renderer, QuoteBlock quoteBlock)
    {
        renderer.RenderLinesBefore(quoteBlock);
        renderer.Write(quoteBlock.TriviaBefore);

        var indents = new string[quoteBlock.QuoteLines.Count];
        for (int i = 0; i < quoteBlock.QuoteLines.Count; i++)
        {
            var quoteLine = quoteBlock.QuoteLines[i];
            var wsb = quoteLine.TriviaBefore.ToString();
            var quoteChar = quoteLine.QuoteChar ? ">" : "";
            var spaceAfterQuoteChar = quoteLine.HasSpaceAfterQuoteChar ? " " : "";
            var wsa = quoteLine.TriviaAfter.ToString();
            indents[i] = (wsb + quoteChar + spaceAfterQuoteChar + wsa);
        }
        bool noChildren = false;
        if (quoteBlock.Count == 0)
        {
            noChildren = true;
            // since this QuoteBlock instance has no children, indents will not be rendered. We
            // work around this by adding empty LineBreakInlines to a ParagraphBlock.
            // Wanted: a more elegant/better solution (although this is not *that* bad).
            foreach (var quoteLine in quoteBlock.QuoteLines)
            {
                var emptyLeafBlock = new ParagraphBlock
                {
                    NewLine = quoteLine.NewLine
                };
                var newLine = new LineBreakInline
                {
                    NewLine = quoteLine.NewLine
                };
                var container = new ContainerInline();
                container.AppendChild(newLine);
                emptyLeafBlock.Inline = container;
                quoteBlock.Add(emptyLeafBlock);
            }
        }

        renderer.PushIndent(indents);
        renderer.WriteChildren(quoteBlock);
        renderer.PopIndent();

        if (!noChildren)
        {
            renderer.RenderLinesAfter(quoteBlock);
        }
    }
}