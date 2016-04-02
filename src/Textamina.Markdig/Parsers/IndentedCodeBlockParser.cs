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
        public override bool CanInterrupt(BlockProcessor processor, Block block)
        {
            return !(block is ParagraphBlock);
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            var result = TryContinue(processor, null);
            if (result == BlockState.Continue)
            {
                processor.NewBlocks.Push(new CodeBlock(this) { Column = processor.Column });
            }
            return result;
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            if (!processor.IsCodeIndent || processor.IsBlankLine)
            {
                if (block == null || !processor.IsBlankLine)
                {
                    return BlockState.None;
                }
            }

            // If we don't have a blank line, we reset to the indent
            if (processor.Indent >= 4)
            {
                processor.GoToCodeIndent();
            }
            return BlockState.Continue;
        }

        public override bool Close(BlockProcessor processor, Block block)
        {
            var codeBlock = (CodeBlock)block;
            if (codeBlock != null)
            {
                // Remove any trailing blankline
                for (int i = codeBlock.Lines.Count - 1; i >= 0; i--)
                {
                    if (codeBlock.Lines.Lines[i].Slice.IsEmpty)
                    {
                        codeBlock.Lines.RemoveAt(i);
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