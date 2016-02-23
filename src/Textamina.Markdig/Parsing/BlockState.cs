using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public class BlockState
    {
        public BlockParser Parser { get; internal set; }

        public Block Block { get; internal set; }

        public bool IsOpen { get; internal set; }

        public void Reset()
        {
            Parser = null;
            Block = null;
            IsOpen = true;
        }
    }
}