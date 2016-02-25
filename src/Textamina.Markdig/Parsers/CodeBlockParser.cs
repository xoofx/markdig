using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public class CodeBlockParser : BlockParser
    {
        public CodeBlockParser()
        {
        }

        public override bool CanInterrupt(BlockParserState state, Block block)
        {
            return !(block is ParagraphBlock);
        }

        public override BlockState TryOpen(BlockParserState state)
        {
            var result = TryContinue(state, null);
            if (result == BlockState.Continue)
            {
                state.NewBlocks.Push(new CodeBlock(this) { Column = state.Column });
            }
            return result;
        }

        public override BlockState TryContinue(BlockParserState state, Block block)
        {
            if (!state.IsCodeIndent || state.IsBlankLine)
            {
                return state.IsBlankLine && block != null ? BlockState.BreakDiscard : BlockState.None;
            }
            state.MoveTo(state.StartBeforeIndent + 4);
            return BlockState.Continue;
        }
    }
}