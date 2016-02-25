


using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    public class ListItemBlock : ContainerBlock
    {
        public ListItemBlock(BlockParser parser) : base(parser)
        {
        }

        internal int NumberOfSpaces { get; set; }
    }
}