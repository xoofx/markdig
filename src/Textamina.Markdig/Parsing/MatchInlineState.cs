using System.Text;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public class MatchInlineState
    {
        public MatchInlineState(StringLineList lines)
        {
            Lines = lines;
        }

        public readonly StringLineList Lines;

        public Inline Inline;

        public StringBuilder Builder;
    }
}