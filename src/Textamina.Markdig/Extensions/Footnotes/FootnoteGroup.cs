using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.Footnotes
{
    public class FootnoteGroup : ContainerBlock
    {
        public FootnoteGroup(BlockParser parser) : base(parser)
        {
        }

        public int CurrentOrder { get; set; }
    }
}