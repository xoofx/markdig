using System.Text;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public class MatchInlineState
    {
        public MatchInlineState(StringBuilderCache stringBuilders)
        {
            StringBuilders = stringBuilders;
        }

        public StringLineGroup Lines;

        public Inline Inline;

        public StringBuilderCache StringBuilders { get;  }
    }
}