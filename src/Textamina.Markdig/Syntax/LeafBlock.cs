using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class LeafBlock : Block
    {
        protected LeafBlock(BlockParser parser) : base(parser)
        {
            Lines = new StringSliceList();
        }

        public StringSliceList Lines { get; set; }

        public Inline Inline { get; set; }

        public bool NoInline { get; set; }
    }
}