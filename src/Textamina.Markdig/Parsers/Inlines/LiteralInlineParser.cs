using System.Text;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public class LiteralInlineParser : InlineParser
    {
        public static readonly LiteralInlineParser Default = new LiteralInlineParser();

        public override bool Match(InlineParserState state, ref StringSlice text)
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

            var str = text.Text;
            // Sligthly faster to perform our
            //var nextStart = state.Parsers.IndexOfOpeningCharacter(str, text.Start + 1, text.End);
            var nextStart = str.IndexOfAny(state.SpecialCharacters, text.Start + 1, text.Length - 1);
            if (nextStart < 0)
            {
                nextStart = text.End + 1;
            }

            var length = nextStart - text.Start;
            if (length == 1)
            {
                builder.Append(text.CurrentChar);
            }
            else
            {
                builder.Append(str, text.Start, length);
            }
            text.Start = nextStart;
            return true;
        }
    }
}