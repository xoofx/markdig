using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// There is actually no EscapeInline inheriting from Inline, as 
    /// the parser will transform it to a LiteralInline
    /// </summary>
    public static class EscapeInline
    {
        public static readonly InlineParser Parser = new ParserInternal();

        private class ParserInternal : InlineParser
        {
            public ParserInternal()
            {
                FirstChars = new[] {'\\'};
            }

            public override bool Match(InlineParserState state)
            {
                var lines = state.Text;

                // Go to escape character
                lines.NextChar();
                if (lines.CurrentChar.IsAsciiPunctuation())
                {
                    var literal = state.Inline as LiteralInline ??
                                  new LiteralInline() {ContentBuilder = state.StringBuilders.Get()};
                    literal.ContentBuilder.Append(lines.CurrentChar);
                    state.Inline = literal;
                    lines.NextChar();
                    return true;
                }
                return false;
            }
        }
    }
}