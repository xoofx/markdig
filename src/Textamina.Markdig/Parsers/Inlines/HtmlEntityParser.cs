using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public class HtmlEntityParser : InlineParser
    {
        public static readonly HtmlEntityParser Default = new HtmlEntityParser();

        public HtmlEntityParser()
        {
            OpeningCharacters = new[] {'&'};
        }

        public override bool Match(InlineParserState state, ref StringSlice slice)
        {
            string entityName;
            int entityValue;
            int match = HtmlHelper.ScanEntity(slice.Text, slice.Start, slice.Length, out entityName, out entityValue);
            if (match == 0)
            {
                return false;
            }

            string literal = null;
            if (entityName != null)
            {
                literal = EntityHelper.DecodeEntity(entityName);
            }
            else if (entityValue >= 0)
            {
                literal = (entityValue == 0 ? null : EntityHelper.DecodeEntity(entityValue)) ?? CharHelper.ZeroSafeString;
            }
            if (literal != null)
            {
                state.Inline = new LiteralInline() {Content = literal};
                slice.Start = slice.Start + match;
                return true;
            }

            return false;
        }
    }
}