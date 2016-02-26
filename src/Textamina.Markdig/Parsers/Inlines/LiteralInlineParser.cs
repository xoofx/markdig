using System.Text;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public sealed class LiteralInlineParser : InlineParser
    {
        public static readonly LiteralInlineParser Default = new LiteralInlineParser();

        /// <summary>
        /// We don't expect the LiteralInlineParser to be instantiated a end-user, as it is part
        /// of the default parser pipeline (and should always be the last), working as a literal character
        /// collector.
        /// </summary>
        private LiteralInlineParser()
        {
        }

        public override bool Match(InlineParserState state, ref StringSlice text)
        {
            // Sligthly faster to perform our own search for opening characters
            var nextStart = state.Parsers.IndexOfOpeningCharacter(text.Text, text.Start + 1, text.End);
            //var nextStart = str.IndexOfAny(state.SpecialCharacters, text.Start + 1, text.Length - 1);
            if (nextStart < 0)
            {
                nextStart = text.End + 1;
            }

            var length = nextStart - text.Start;
            state.Inline = new LiteralInline { Content = text.Text.Substring(text.Start, length) };
            text.Start = nextStart;
            return true;
        }
    }
}