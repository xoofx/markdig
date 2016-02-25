using System.Text;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public class LiteralInlineParser : InlineParser
    {
        public static readonly LiteralInlineParser Default = new LiteralInlineParser();

        public override bool Match(InlineParserState state)
        {
            // A literal will always match
            var literal = state.Inline as LiteralInline;
            StringBuilder builder;
            if (literal == null)
            {
                builder = state.StringBuilders.Get();
                literal = new LiteralInline {ContentBuilder = builder};
                state.Inline = literal;
            }
            else
            {
                builder = literal.ContentBuilder;
            }

            var text = state.Text;
            builder.Append(text.CurrentChar);
            text.NextChar();
            return true;
        }
    }
}