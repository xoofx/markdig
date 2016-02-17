using System.Text;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class LiteralInline : LeafInline
    {
        public static readonly InlineParser Parser = new ParserInternal();

        private StringBuilder tempBuilder;

        public string Content;

        private class ParserInternal : InlineParser
        {
            public override bool Match(ref MatchInlineState state)
            {
                // A literal will always match
                var literal = state.Inline as LiteralInline;
                if (literal == null)
                {
                    literal = new LiteralInline {tempBuilder = state.Builder};
                    // We take ownership of the string builder
                    state.Builder = null;
                    state.Inline = literal;
                }

                var builder = literal.tempBuilder;
                var c = state.Lines.Current;
                if (c != '\0')
                {
                    builder.Append(c);
                    state.Lines.NextChar();
                }
                return true;
            }

            public override void Close(ref MatchInlineState state, Inline inline)
            {
                var literal = (LiteralInline)inline;
                literal.Content = literal.tempBuilder.ToString();
                state.Builder = literal.tempBuilder;
            }
        }
    }
}