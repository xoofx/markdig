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
                while (c != '\0')
                {
                    // Skip new lines
                    var isWhitespace = CharHelper.IsWhitespace(c);
                    if (!(CharHelper.IsNewLine(c) || (builder.Length == 0 && isWhitespace) || (lastWhiteSpace && isWhitespace)))
                    {
                        if (c == '`')
                        {
                            countClosedSticks++;
                        }
                        else
                        {
                            if (countClosedSticks == countSticks)
                            {
                                break;
                            }

                            countClosedSticks = 0;
                        }
                        lastWhiteSpace = isWhitespace;

                        builder.Append(c);
                    }
                    c = lines.NextChar();
                }

                if (countClosedSticks == countSticks)
                {
                    builder.Length = builder.Length - countSticks;
                    int newLength = builder.Length;
                    for (int i = builder.Length - 1; i >= 0; i--)
                    {
                        if (CharHelper.IsWhitespace(builder[i]))
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