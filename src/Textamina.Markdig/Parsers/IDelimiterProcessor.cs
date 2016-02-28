using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers
{
    public interface IDelimiterProcessor
    {
        bool ProcessDelimiters(InlineParserState state, Inline root, Inline lastChild);
    }
}