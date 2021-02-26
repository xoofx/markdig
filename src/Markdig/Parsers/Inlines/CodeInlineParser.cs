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
    /// <seealso cref="InlineParser" />
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
            var match = slice.CurrentChar;
            if (slice.PeekCharExtra(-1) == match)
            {
                return false;
            }

            var startPosition = slice.Start;

            // Match the opened sticks
            int openSticks = slice.CountAndSkipChar(match);
            int closeSticks = 0;

            char c = slice.CurrentChar;

            var builder = StringBuilderCache.Local();

            // A backtick string is a string of one or more backtick characters (`) that is neither preceded nor followed by a backtick.
            // A code span begins with a backtick string and ends with a backtick string of equal length.
            // The contents of the code span are the characters between the two backtick strings, normalized in the following ways:

            // 1. line endings are converted to spaces.

            // 2. If the resulting string both begins AND ends with a space character, but does not consist entirely
            // of space characters, a single space character is removed from the front and back.
            // This allows you to include code that begins or ends with backtick characters, which must be separated by
            // whitespace from the opening or closing backtick strings.

            bool allSpace = true;

            while (c != '\0')
            {
                // Transform '\n' into a single space
                if (c == '\n')
                {
                    c = ' ';
                }

                if (c == match)
                {
                    closeSticks = slice.CountAndSkipChar(match);

                    if (openSticks == closeSticks)
                    {
                        break;
                    }

                    allSpace = false;
                    builder.Append(match, closeSticks);
                    c = slice.CurrentChar;
                }
                else
                {
                    builder.Append(c);
                    if (c != ' ')
                    {
                        allSpace = false;
                    }
                    c = slice.NextChar();
                }
            }

            bool isMatching = false;
            if (closeSticks == openSticks)
            {
                string content;

                // Remove one space from front and back if the string is not all spaces
                if (!allSpace && builder.Length > 2 && builder[0] == ' ' && builder[builder.Length - 1] == ' ')
                {
                    content = builder.ToString(1, builder.Length - 2);
                }
                else
                {
                    content = builder.ToString();
                }

                processor.Inline = new CodeInline()
                {
                    Delimiter = match,
                    Content = content,
                    Span = new SourceSpan(processor.GetSourcePosition(startPosition, out int line, out int column), processor.GetSourcePosition(slice.Start - 1)),
                    Line = line,
                    Column = column
                };
                isMatching = true;
            }

            return isMatching;
        }
    }
}