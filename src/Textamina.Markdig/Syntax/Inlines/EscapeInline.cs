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
                FirstChar = '\\';
            }

            public override bool Match(ref MatchInlineState state)
            {
                var lines = state.Lines;

                if (!Utility.IsASCIIPunctuation(lines.Current))
                {
                    state.Inline = new EscapeInline() {EscapedChar = lines.Current};
                    return true;
                }
                return false;
            }
        }
    }
}