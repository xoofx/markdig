using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a thematic break.
    /// </summary>
    public class ThematicBreakBlock : LeafBlock
    {
        public new static readonly BlockParser Parser = new ThematicBreakParser();

        public ThematicBreakBlock(BlockParser parser) : base(parser)
        {
            ProcessInlines = false;
        }
    }
}