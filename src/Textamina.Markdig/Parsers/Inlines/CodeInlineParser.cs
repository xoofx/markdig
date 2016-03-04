// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public class CodeInlineParser : InlineParser
    {
        public CodeInlineParser()
        {
            OpeningCharacters = new[] { '`' };
        }

        public override bool Match(InlineParserState state, ref StringSlice slice)
        {
            int openSticks = 0;
            if (slice.PeekCharExtra(-1) == '`')
            {
                return false;
            }

            var c = slice.CurrentChar;
            while (c == '`')
            {
                openSticks++;
                c = slice.NextChar();
            }

            bool isMatching = false;

            var builder = state.StringBuilders.Get();
            int closeSticks = 0;

            // A backtick string is a string of one or more backtick characters (`) that is neither preceded nor followed by a backtick.
            // A code span begins with a backtick string and ends with a backtick string of equal length. 
            // The contents of the code span are the characters between the two backtick strings, with leading and trailing spaces and line endings removed, and whitespace collapsed to single spaces.
            var pc = ' ';

            while (c != '\0')
            {
                // Transform '\n' into a single space
                if (c == '\n')
                {
                    state.LocalLineIndex++;
                    state.LineIndex++;
                    c = ' ';
                }

                if (c != '`' && (c != ' ' || pc != ' '))
                {
                    builder.Append(c);
                }
                else
                {
                    while (c == '`')
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
                    builder.Append('`', closeSticks);
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
                state.Inline = new CodeInline() { Content = builder.ToString() };
                isMatching = true;
            }

            // Release the builder if not used
            state.StringBuilders.Release(builder);
            return isMatching;
        }
    }
}