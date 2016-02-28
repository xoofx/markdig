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

        public int LineIndex { get; set; }

        public override string ToLiteral()
        {
            return "|";
        }
    }

    public class TableInlineParser : InlineParser, IDelimiterProcessor
    {
        public TableInlineParser()
        {
            OpeningCharacters = new[] { '|' };
        }

        public override bool Match(InlineParserState state, ref StringSlice slice)
        {
            state.Inline = new TableDelimiterInline(this) {LineIndex = state.LineIndex};

            // Store that we have at least one delimiter
            state.ParserStates[Index] = state.Inline;

            return true;
        }

        public bool ProcessDelimiters(InlineParserState state, Inline root, Inline lastChild)
        {









            return false;
        }
    }
}