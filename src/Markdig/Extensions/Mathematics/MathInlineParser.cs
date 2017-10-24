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

            bool openPrevIsPunctuation;
            bool openPrevIsWhiteSpace;
            bool openNextIsPunctuation;
            bool openNextIsWhiteSpace;
            bool openNextIsDigit = c.IsDigit();
            pc.CheckUnicodeCategory(out openPrevIsWhiteSpace, out openPrevIsPunctuation);
            c.CheckUnicodeCategory(out openNextIsWhiteSpace, out openNextIsPunctuation);
            
            // Check that opening $/$$ is correct, using the different heuristics than for emphasis delimiters
            // If a $/$$ is not preceded by a whitespace or punctuation, or followed by a digit
            // this is a not a math block
            if ((!openPrevIsWhiteSpace && !openPrevIsPunctuation) || openNextIsDigit)
            {
                return false;
            }

            bool isMatching = false;
            int closeDollars = 0;

            // Eat any leading spaces
            while (c.IsSpaceOrTab())
            {
                c = slice.NextChar();
            }

            var start = slice.Start;
            var end = 0;

            pc = match;
            var lastWhiteSpace = -1;
            while (c != '\0')
            {
                // Don't allow newline in an inline math expression
                if (c == '\r' || c == '\n')
                {
                    return false;
                }

                // Don't process sticks if we have a '\' as a previous char
                if (pc != '\\' )
                {
                    // Record continous whitespaces at the end
                    if (c.IsSpaceOrTab())
                    {
                        if (lastWhiteSpace < 0)
                        {
                            lastWhiteSpace = slice.Start;
                        }
                    }
                    else
                    {
                        bool hasClosingDollars = c == match;
                        if (hasClosingDollars)
                        {
                            while (c == match)
                            {
                                closeDollars++;
                                c = slice.NextChar();
                            }
                        }

                        if (closeDollars >= openDollars)
                        {
                            break;
                        }

                        lastWhiteSpace = -1;
                        if (hasClosingDollars)
                        {
                            pc = match;
                            continue;
                        }
                    }
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
                bool closePrevIsPunctuation;
                bool closePrevIsWhiteSpace;
                bool closeNextIsPunctuation;
                bool closeNextIsWhiteSpace;
                pc.CheckUnicodeCategory(out closePrevIsWhiteSpace, out closePrevIsPunctuation);
                c.CheckUnicodeCategory(out closeNextIsWhiteSpace, out closeNextIsPunctuation);

                // A closing $/$$ should be followed by at least a punctuation or a whitespace
                // and if the character after an openning $/$$ was a whitespace, it should be 
                // a whitespace as well for the character preceding the closing of $/$$
                if ((!closeNextIsPunctuation && !closeNextIsWhiteSpace) || (openNextIsWhiteSpace != closePrevIsWhiteSpace))
                {
                    return false;
                }

                if (closePrevIsWhiteSpace && lastWhiteSpace > 0)
                {
                    end = lastWhiteSpace + openDollars - 1;
                }
                else
                {
                    end = slice.Start - 1;
                }

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