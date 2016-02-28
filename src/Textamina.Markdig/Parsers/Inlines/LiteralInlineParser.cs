using System.Text;
using Textamina.Markdig.Helpers;
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

        public override bool Match(InlineParserState state, ref StringSlice slice)
        {
            var text = slice.Text;

            // Sligthly faster to perform our own search for opening characters
            var nextStart = state.Parsers.IndexOfOpeningCharacter(text, slice.Start + 1, slice.End);
            //var nextStart = str.IndexOfAny(state.SpecialCharacters, slice.Start + 1, slice.Length - 1);
            int length;

            if (nextStart < 0)
            {
                nextStart = slice.End + 1;
                length = nextStart - slice.Start;
            }
            else
            {
                // Remove line endings if the next char is a new line
                length = nextStart - slice.Start;
                if (text[nextStart] == '\n')
                {
                    int end = nextStart - 1;
                    while (length > 0 && text[end].IsSpace())
                    {
                        length--;
                        end--;
                    }
                }
            }

            // The LiteralInlineParser is always matching (at least an empty string)
            state.Inline = length > 0 ? new LiteralInline {Content = new StringSlice(slice.Text, slice.Start, slice.Start + length - 1)} : new LiteralInline();
            slice.Start = nextStart;
            return true;
        }
    }
}