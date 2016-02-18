using System.Text;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public class MatchInlineState
    {
        public MatchInlineState(StringLineGroup lines)
        {
            Lines = lines;
        }

        public readonly StringLineGroup Lines;

        public Inline Inline;

        public StringBuilder Builder;
    }
}