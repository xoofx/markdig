using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    public class HtmlBlock : LeafBlock
    {
        public HtmlBlock(BlockParser parser) : base(parser)
        {
        }

        public HtmlBlockType Type { get; set; }
    }
}