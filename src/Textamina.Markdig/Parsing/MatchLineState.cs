


using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public class MatchLineState
    {
        public MatchLineState(StringBuilderCache stringBuilders)
        {
            StringBuilders = stringBuilders;
        }

        public StringLine Line;

        public Block Block;

        public Block LastBlock;

        public object Context;

        public StringBuilderCache StringBuilders { get; }

        internal void Reset(StringLine line)
        {
            Line = line;
            Block = null;
        }
    }
}