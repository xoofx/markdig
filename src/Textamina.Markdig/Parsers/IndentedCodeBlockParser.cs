// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    /// <summary>
    /// Block parser for an indented <see cref="CodeBlock"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Parsers.BlockParser" />
    public class IndentedCodeBlockParser : BlockParser
    {
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
                state.GoToCodeIndent();
            }
            return BlockState.Continue;
        }

        public override bool Close(BlockParserState state, Block block)
        {
            var codeBlock = (CodeBlock)block;
            if (codeBlock != null)
            {
                var lines = codeBlock.Lines;
                // Remove any trailing blankline
                for (int i = lines.Count - 1; i >= 0; i--)
                {
                    if (lines.Lines[i].Slice.IsEmpty)
                    {
                        lines.RemoveAt(i);
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