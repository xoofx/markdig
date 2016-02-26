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
                if (block == null || !state.IsBlankLine)
                {
                    return BlockState.None;
                }
            }

            // If we don't have a blank line, we reset to the indent
            if (state.Indent >= 4)
            {
                state.ResetToPosition(state.StartBeforeIndent + 4);
            }
            return BlockState.Continue;
        }

        public override bool Close(BlockParserState state, Block block)
        {
            var codeBlock = (CodeBlock)block;
            if (codeBlock != null)
            {
                var lines = codeBlock.Lines;
                // Remove trailing blankline
                for (int i = lines.Count - 1; i >= 0; i--)
                {
                    if (lines.Slices[i].IsEmpty)
                    {
                        lines.Count--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return true;
        }
    }
}