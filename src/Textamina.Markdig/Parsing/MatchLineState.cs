


using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public class MatchLineState
    {
        public MatchLineState(StringBuilderCache stringBuilders, Document root)
        {
            StringBuilders = stringBuilders;
            Root = root;
        }

        public StringLine Line;

        public Block Block;

        public Block LastBlock;

        public readonly Document Root;

        public StringBuilderCache StringBuilders { get; }

        internal void Reset(StringLine line)
        {
            Line = line;
            Block = null;
        }
    }
}