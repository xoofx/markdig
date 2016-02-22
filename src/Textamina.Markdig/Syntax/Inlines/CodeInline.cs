using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class CodeInline : LeafInline
    {
        public static readonly InlineParser Parser = new ParserInternal();

        public string Content { get; set; }

        private class ParserInternal : InlineParser
        {
            public ParserInternal()
            {
                FirstChars = new[] { '`' };
            }

            public override bool Match(MatchInlineState state)
            {
                var lines = state.Lines;

                int countSticks = 0;
                while (lines.CurrentChar == '`')
                {
                    countSticks++;
                    lines.NextChar();
                }

                bool isMatching = false;

                var builder = state.StringBuilders.Get();
                int countClosedSticks = 0;
                var c = lines.CurrentChar;
                bool lastWhiteSpace = false;
                bool hasBackslash = false;
                while (c != '\0')
                {
                    // Skip new lines
                    var isWhitespace = c.IsWhitespace();
                    if (!(c.IsNewLine() || (builder.Length == 0 && isWhitespace) || (lastWhiteSpace && isWhitespace)))
                    {
                        if (c == '`')
                        {
                            countClosedSticks++;
                        }
                        else
                        {
                            countClosedSticks = 0;
                        }
                        lastWhiteSpace = isWhitespace;

                        hasBackslash = c == '\\';
                        builder.Append(c);
                    }
                    c = lines.NextChar();

                    if (hasBackslash)
                    {
                        if (c == '\n')
                        {
                            c = ' ';
                        }
                        hasBackslash = false;
                    }

                    if (countClosedSticks == countSticks)
                    {
                        break;
                    }
                }

                if (countClosedSticks == countSticks)
                {
                    builder.Length = builder.Length - countSticks;
                    int newLength = builder.Length;
                    for (int i = builder.Length - 1; i >= 0; i--)
                    {
                        if (builder[i].IsWhitespace())
                        {
                            newLength--;
                        }
                        else
                        {
                            break;
                        }
                    }

                    builder.Length = newLength;

                    state.Inline = new CodeInline() { Content = builder.ToString() };
                    isMatching = true;
                }

                // Release the builder if not used
                state.StringBuilders.Release(builder);
                return isMatching;
            }
        }
    }
}