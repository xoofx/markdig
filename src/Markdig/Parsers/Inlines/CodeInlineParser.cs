// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Parsers.Inlines
{
    /// <summary>
    /// An inline parser for a <see cref="CodeInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.InlineParser" />
    public class CodeInlineParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeInlineParser"/> class.
        /// </summary>
        public CodeInlineParser()
        {
            OpeningCharacters = new[] { '`' };
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            int openSticks = 0;
            var match = slice.CurrentChar;
            if (slice.PeekCharExtra(-1) == match)
            {
                return false;
            }

            var startPosition = slice.Start;

            // Match the opened sticks
            var c = slice.CurrentChar;
            while (c == match)
            {
                openSticks++;
                c = slice.NextChar();
            }

            bool isMatching = false;

            var builder = processor.StringBuilders.Get();
            int closeSticks = 0;

            // A backtick string is a string of one or more backtick characters (`) that is neither preceded nor followed by a backtick.
            // A code span begins with a backtick string and ends with a backtick string of equal length. 
            // The contents of the code span are the characters between the two backtick strings, with leading and trailing spaces and line endings removed, and whitespace collapsed to single spaces.
            var pc = ' ';

            int newLinesFound = 0;
            while (c != '\0')
            {
                // Transform '\n' into a single space
                if (c == '\n')
                {
                    newLinesFound++;
                    c = ' ';
                }

                if (c != match && (c != ' ' || pc != ' '))
                {
                    builder.Append(c);
                }
                else
                {
                    while (c == match)
                    {
                        closeSticks++;
                        pc = c;
                        c = slice.NextChar();
                    }

                    if (openSticks == closeSticks)
                    {
                        break;
                    }
                }

                if (closeSticks > 0)
                {
                    builder.Append(match, closeSticks);
                    closeSticks = 0;
                }
                else
                {
                    pc = c;
                    c = slice.NextChar();
                }
            }

            if (closeSticks == openSticks)
            {
                // Remove trailing space
                if (builder.Length > 0)
                {
                    if (builder[builder.Length - 1].IsWhitespace())
                    {
                        builder.Length--;
                    }
                }
                int line;
                int column;
                processor.Inline = new CodeInline()
                {
                    Delimiter = match,
                    Content = builder.ToString(),
                    Span = new SourceSpan(processor.GetSourcePosition(startPosition, out line, out column), processor.GetSourcePosition(slice.Start - 1)),
                    Line = line,
                    Column = column
                };
                isMatching = true;
            }

            // Release the builder if not used
            processor.StringBuilders.Release(builder);
            return isMatching;
        }
    }
}