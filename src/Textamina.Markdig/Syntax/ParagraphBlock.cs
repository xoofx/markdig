using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a paragraph.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.5 Fenced code blocks
    /// </remarks>
    public class ParagraphBlock : LeafBlock
    {
        public ParagraphBlock() : this(null)
        {
        }

        public ParagraphBlock(BlockParser parser) : base(parser)
        {
            ProcessInlines = true;
        }
    }
}