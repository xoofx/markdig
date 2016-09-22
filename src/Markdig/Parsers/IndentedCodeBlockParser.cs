// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Syntax;

namespace Markdig.Parsers
{
    /// <summary>
    /// Block parser for an indented <see cref="CodeBlock"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.BlockParser" />
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
                // Save the column where we need to go back
                var column = processor.Column;

                // Unwind all indents all spaces before in order to calculate correct span
                processor.UnwindAllIndents();

                processor.NewBlocks.Push(new CodeBlock(this)
                {
                    Column = processor.Column,
                    Span = new SourceSpan(processor.Start, processor.Line.End)
                });

                // Go back to the correct column
                processor.GoToColumn(column);
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
            if (processor.Indent > 4)
            {
                processor.GoToCodeIndent();
            }
            if (block != null)
            {
                block.UpdateSpanEnd(processor.Line.End);
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