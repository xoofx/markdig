using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    public class QuoteBlock : ContainerBlock
    {
        public QuoteBlock(BlockParser parser) : base(parser)
        {
        }

        public char QuoteChar { get; set; }
    }
}