using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a fenced code block.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.5 Fenced code blocks
    /// </remarks>
    public class FencedCodeBlock : CodeBlock
    {
        public FencedCodeBlock(BlockParser parser) : base(parser)
        {
        }

        public string Language { get; set; }

        public string Arguments { get; set; }

        public int FencedCharCount { get; set; }

        public char FencedChar { get; set; }

        public int IndentCount { get; set; }
    }
}