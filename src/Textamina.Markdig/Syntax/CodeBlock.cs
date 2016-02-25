using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a indented code block.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.4 Indented code blocks 
    /// </remarks>
    public class CodeBlock : LeafBlock
    {
        public new static readonly BlockParser Parser = new ParserInternal();

        public CodeBlock(BlockParser parser) : base(parser)
        {
            NoInline = true;
        }

        private class ParserInternal : BlockParser
        {
            public ParserInternal()
            {
                CanInterruptParagraph = false;
            }

            public override MatchLineResult Match(BlockParserState state)
            {
                if (!state.IsCodeIndent || state.IsBlankLine)
                {
                    return state.IsBlankLine && state.Pending != null ? MatchLineResult.LastDiscard : MatchLineResult.None;
                }
                if (state.Pending == null)
                {
                    state.NewBlocks.Push(new CodeBlock(this) { Column = state.Column });
                }
                return MatchLineResult.Continue;
            }
        }
    }
}