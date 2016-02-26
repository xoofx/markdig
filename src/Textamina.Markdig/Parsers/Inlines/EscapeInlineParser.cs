using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public class EscapeInlineParser : InlineParser
    {
        public static readonly EscapeInlineParser Default = new EscapeInlineParser();

        public EscapeInlineParser()
        {
            OpeningCharacters = new[] {'\\'};
        }

        public override bool Match(InlineParserState state, ref StringSlice text)
        {
            // Go to escape character
            var c = text.NextChar();
            if (c.IsAsciiPunctuation())
            {
                var literal = new LiteralInline() {Content = new string(c, 1)};
                state.Inline = literal;
                text.NextChar();
                return true;
            }
            return false;
        }
    }
}