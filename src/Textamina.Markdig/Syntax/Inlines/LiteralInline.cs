using System.Text;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class LiteralInline : LeafInline
    {
        public static readonly InlineParser Parser = new ParserInternal();

        public StringBuilder ContentBuilder { get; set; }

        public string Content { get; set; }

        protected override void Close(MatchInlineState state)
        {
            Content = ContentBuilder.ToString();
            state.StringBuilders.Release(ContentBuilder);
            ContentBuilder = null;
        }

        private class ParserInternal : InlineParser
        {
            public override bool Match(MatchInlineState state)
            {
                // A literal will always match
                var literal = state.Inline as LiteralInline;
                var text = state.Lines;
                if (literal == null)
                {
                    literal = new LiteralInline {ContentBuilder = state.StringBuilders.Get()};
                    state.Inline = literal;
                }

                var builder = literal.ContentBuilder;
                var c = text.CurrentChar;
                if (c != '\0')
                {
                    builder.Append(c);
                    text.NextChar();
                }
                return true;
            }
        }

        public override string ToString()
        {
            return Content ?? (ContentBuilder != null ? ContentBuilder.ToString() : string.Empty);
        }
    }
}