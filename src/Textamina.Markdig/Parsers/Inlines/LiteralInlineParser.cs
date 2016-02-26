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
            var literal = new LiteralInline();
            state.Inline = literal;

            var str = text.Text;
            // Sligthly faster to perform our
            var nextStart = state.Parsers.IndexOfOpeningCharacter(str, text.Start + 1, text.End);
            //var nextStart = str.IndexOfAny(state.SpecialCharacters, text.Start + 1, text.Length - 1);
            if (nextStart < 0)
            {
                nextStart = text.End + 1;
            }

            var length = nextStart - text.Start;
            literal.Content = str.Substring(text.Start, length);
            text.Start = nextStart;
            return true;
        }
    }
}