using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class EscapeInline : LeafInline
    {
        public static readonly InlineParser Parser = new ParserInternal();

        public char EscapedChar;

        private class ParserInternal : InlineParser
        {
            public ParserInternal()
            {
                FirstChars = new[] {'\\'};
            }

            public override bool Match(MatchInlineState state)
            {
                var lines = state.Lines;

                // Go to escape character
                lines.NextChar();
                if (CharHelper.IsAsciiPunctuation(lines.CurrentChar))
                {
                    state.Inline = new EscapeInline() {EscapedChar = lines.CurrentChar};
                    lines.NextChar();
                    return true;
                }
                return false;
            }
        }
    }
}