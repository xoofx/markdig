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
                while (lines.Current == '`')
                {
                    countSticks++;
                    lines.NextChar();
                }

                var builder = state.Builder;
                int countClosedSticks = 0;
                var c = lines.Current;
                bool lastWhiteSpace = false;
                while (c != '\0')
                {
                    // Skip new lines
                    var isWhitespace = Utility.IsWhiteSpace(c);
                    if (!(Utility.IsNewLine(c) || (builder.Length == 0 && isWhitespace) || (lastWhiteSpace && isWhitespace)))
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
                        if (Utility.IsWhiteSpace(builder[i]))
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
                    return true;
                }

                return false;
            }
        }
    }
}