using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    public class HtmlBlock : LeafBlock
    {
        public static readonly BlockParser Parser = new HtmlBlockParser();

        public HtmlBlock(BlockParser parser) : base(parser)
        {
            // We don't process inline of an html block, as we will copy the content as-is
            ProcessInlines = false;
        }

        public HtmlBlockType Type { get; set; }
    }
}