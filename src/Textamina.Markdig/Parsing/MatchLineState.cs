


using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public class MatchLineState
    {
        public StringLine Line;

        public Block Block;

        public Block LastBlock;

        internal void Reset(StringLine line)
        {
            Line = line;
            Block = null;
        }
    }
}