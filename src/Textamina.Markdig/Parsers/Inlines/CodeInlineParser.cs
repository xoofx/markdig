using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public class CodeInlineParser : InlineParser
    {
        public CodeInlineParser()
        {
            OpeningCharacters = new[] { '`' };
        }

        public override bool Match(InlineParserState state)
        {
            var text = state.Text;

            int openSticks = 0;
            if (text.PeekChar(-1) == '`')
            {
                return false;
            }

            while (text.CurrentChar == '`')
            {
                openSticks++;
                text.NextChar();
            }

            bool isMatching = false;

            var builder = state.StringBuilders.Get();
            int closeSticks = 0;
            var c = text.CurrentChar;

            // A backtick string is a string of one or more backtick characters (`) that is neither preceded nor followed by a backtick.
            // A code span begins with a backtick string and ends with a backtick string of equal length. 
            // The contents of the code span are the characters between the two backtick strings, with leading and trailing spaces and line endings removed, and whitespace collapsed to single spaces.
            var pc = ' ';

            while (c != '\0')
            {
                // Transform '\n' into a single space
                if (c == '\n')
                {
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
                        c = text.NextChar();
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
                    c = text.NextChar();
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