using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class LiteralInline : LeafInline
    {
        public static readonly InlineParser Parser = new ParserInternal();

        public string Content;

        private class ParserInternal : InlineParser
        {
            public override bool Match(ref MatchInlineState state)
            {
                // A literal will always match
                if (state.Inline == null)
                {
                    state.Inline = new LiteralInline();
                }
                state.Builder.Append(state.Lines.Current);
                return true;
            }

            public override void Close(ref MatchInlineState state, Inline inline)
            {
                var literal = (LiteralInline)inline;
                literal.Content = state.Builder.ToString();
            }
        }
    }
}