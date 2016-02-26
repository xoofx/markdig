using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a thematic break.
    /// </summary>
    public class ThematicBreakBlock : LeafBlock
    {
        public ThematicBreakBlock(BlockParser parser) : base(parser)
        {
        }
    }
}