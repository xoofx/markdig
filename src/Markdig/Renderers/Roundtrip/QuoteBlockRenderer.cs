// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Roundtrip
{
    /// <summary>
    /// A Roundtrip renderer for a <see cref="QuoteBlock"/>.
    /// </summary>
    public class QuoteBlockRenderer : RoundtripObjectRenderer<QuoteBlock>
    {
        protected override void Write(RoundtripRenderer renderer, QuoteBlock quoteBlock)
        {
            renderer.RenderLinesBefore(quoteBlock);
            renderer.Write(quoteBlock.TriviaBefore);

            var quoteLines = quoteBlock.QuoteLines;

            bool noChildren = quoteBlock.Count == 0;

            if (quoteLines.Count > 0)
            {
                var indents = new string[quoteLines.Count];
                for (int i = 0; i < quoteLines.Count; i++)
                {
                    var quoteLine = quoteLines[i];
                    var wsb = quoteLine.TriviaBefore.ToString();
                    var quoteChar = quoteLine.QuoteChar ? ">" : "";
                    var spaceAfterQuoteChar = quoteLine.HasSpaceAfterQuoteChar ? " " : "";
                    var wsa = quoteLine.TriviaAfter.ToString();
                    indents[i] = (wsb + quoteChar + spaceAfterQuoteChar + wsa);
                }

                renderer.PushIndent(indents);

                if (noChildren)
                {
                    //// since this QuoteBlock instance has no children, indents will not be rendered. We
                    //// work around this by rendering explicit empty lines
                    foreach(var quoteLine in quoteLines)
                    {
                        renderer.WriteLine(quoteLine.NewLine);
                    }
                }
                else
                {
                    renderer.WriteChildren(quoteBlock);
                }
            }
            else
            {
                renderer.PushIndent($"{quoteBlock.QuoteChar} ");

                if (noChildren)
                {
                    renderer.WriteLine();
                }
                else
                {
                    renderer.WriteChildren(quoteBlock);
                }
            }

            renderer.PopIndent();

            if (!noChildren)
            {
                renderer.RenderLinesAfter(quoteBlock);
            }
        }
    }
}