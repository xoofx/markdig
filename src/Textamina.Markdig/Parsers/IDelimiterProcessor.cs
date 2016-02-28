using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers
{
    public interface IDelimiterProcessor
    {
        void ProcessDelimiters(Inline root, Inline lastChild);
    }
}