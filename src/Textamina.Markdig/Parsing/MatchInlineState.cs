using System.Text;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public struct MatchInlineState
    {
        public MatchInlineState(StringLineList lines) : this()
        {
            Lines = lines;
        }

        public readonly StringLineList Lines;

        public Inline Inline;

        public StringBuilder Builder;

        public InlineParser Parser;
    }
}