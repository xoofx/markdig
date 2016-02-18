using System.Text;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class LiteralInline : LeafInline
    {
        public static readonly InlineParser Parser = new ParserInternal();

        private StringBuilder tempBuilder;

        public string Content;

        protected internal override void Close(MatchInlineState state)
        {
            if (Content == null && tempBuilder != null)
            {
                Content = tempBuilder.ToString();
                state.StringBuilders.Release(tempBuilder);
                tempBuilder = null;
            }
        }

        private class ParserInternal : InlineParser
        {
            public override bool Match(MatchInlineState state)
            {
                // A literal will always match
                var literal = state.Inline as LiteralInline;
                if (literal == null)
                {
                    literal = new LiteralInline {tempBuilder = state.StringBuilders.Get()};
                    state.Inline = literal;
                }

                var builder = literal.tempBuilder;
                var c = state.Lines.CurrentChar;
                if (c != '\0')
                {
                    builder.Append(c);
                    state.Lines.NextChar();
                }
                return true;
            }
        }
    }
}