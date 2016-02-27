using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a indented code block.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.4 Indented code blocks 
    /// </remarks>
    public class IndentedCodeBlock : LeafBlock
    {
        public IndentedCodeBlock(BlockParser parser) : base(parser)
        {
        }
    }
}