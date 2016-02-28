using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public class TableDelimiterInline : DelimiterInline
    {
        public TableDelimiterInline(InlineParser parser) : base(parser)
        {
        }

        public override string ToLiteral()
        {
            return "|";
        }
    }

    public class TableInlineParser : InlineParser
    {
        public TableInlineParser()
        {
            OpeningCharacters = new[] { '|' };
        }

        public override bool Match(InlineParserState state, ref StringSlice slice)
        {
            state.Inline = new TableDelimiterInline(this);

            // We don't have an emphasis
            return false;
        }
    }
}