using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public abstract class InlineParser : ParserBase<InlineParserState>
    {
        public abstract bool Match(InlineParserState state, ref StringSlice slice);
    }
}