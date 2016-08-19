// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Markdig.Extensions.Mathematics
{
    /// <summary>
    /// An inline parser for <see cref="MathInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.InlineParser" />
    /// <seealso cref="IPostInlineProcessor" />
    public class MathInlineParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MathInlineParser"/> class.
        /// </summary>
        public MathInlineParser()
        {
            OpeningCharacters = new[] {'$'};
            DefaultClass = "math";
        }

        /// <summary>
        /// Gets or sets the default class to use when creating a math inline block.
        /// </summary>
        public string DefaultClass { get; set; }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            var match = slice.CurrentChar;
            var pc = slice.PeekCharExtra(-1);
            if (pc == match)
            {
                return false;
            }

            var startPosition = slice.Start;

            // Match the opened $ or $$
            int openDollars = 1; // we have at least a $
            var c = slice.NextChar();
            if (c == match)
            {
                openDollars++;
                c = slice.NextChar();
            }

            bool canOpen;
            bool canClose;
            // Check that opening $/$$ is correct, using the same heuristics than for emphasis delimiters
            CharHelper.CheckOpenCloseDelimiter(pc, c, false, out canOpen, out canClose);
            if (!canOpen)
            {
                return false;
            }

            bool isMatching = false;
            int closeDollars = 0;

            var start = slice.Start;
            var end = 0;
            pc = match;
            while (c != '\0')
            {
                // Don't process sticks if we have a '\' as a previous char
                if (pc != '\\' )
                {
                    while (c == match)
                    {
                        closeDollars++;
                        c = slice.NextChar();
                    }

                    if (closeDollars >= openDollars)
                    {
                        break;
                    }
                    pc = match;
                }

                if (closeDollars > 0)
                {
                    closeDollars = 0;
                }
                else
                {
                    pc = c;
                    c = slice.NextChar();
                }
            }

            if (closeDollars >= openDollars)
            {
                // Check that closing $/$$ is correct
                CharHelper.CheckOpenCloseDelimiter(pc, c, false, out canOpen, out canClose);
                if (!canClose || c.IsDigit())
                {
                    return false;
                }
                end = slice.Start - 1;
                // Create a new MathInline
                int line;
                int column;
                var inline = new MathInline()
                {
                    Span = new SourceSpan(processor.GetSourcePosition(startPosition, out line, out column), processor.GetSourcePosition(slice.End)),
                    Line = line,
                    Column = column,
                    Delimiter = match,
                    DelimiterCount = openDollars,
                    Content = slice
                };
                inline.Content.Start = start;
                // We substract the end to the number of opening $ to keep inside the block the additionals $
                inline.Content.End = end - openDollars;

                // Add the default class if necessary
                if (DefaultClass != null)
                {
                    inline.GetAttributes().AddClass(DefaultClass);
                }
                processor.Inline = inline;
                isMatching = true;
            }

            return isMatching;
        }
    }
}