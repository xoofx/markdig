// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;
using System.Collections.Generic;
using static Markdig.Syntax.CodeBlock;

namespace Markdig.Parsers
{
    /// <summary>
    /// Block parser for an indented <see cref="CodeBlock"/>.
    /// </summary>
    /// <seealso cref="BlockParser" />
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
                var sourceStartPosition = processor.Start;

                // Unwind all indents all spaces before in order to calculate correct span
                processor.UnwindAllIndents();

                var codeBlock = new CodeBlock(this)
                {
                    Column = processor.Column,
                    Span = new SourceSpan(processor.Start, processor.Line.End),
                    LinesBefore = processor.UseLinesBefore(),
                    Newline = processor.Line.Newline,
                };
                var codeBlockLine = new CodeBlockLine
                {
                    BeforeWhitespace = processor.UseWhitespace(sourceStartPosition - 1)
                };
                codeBlock.CodeBlockLines.Add(codeBlockLine);
                processor.NewBlocks.Push(codeBlock);

                // Go back to the correct column
                processor.GoToColumn(column);
            }
            return result;
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            bool isLastLine = processor.Line.Start == processor.Line.End + 1; // TODO: RTP: meh. Does this also work for \r\n?
            if (!processor.IsCodeIndent || processor.IsBlankLine)
            {
                if (block == null || !processor.IsBlankLine || (processor.IsBlankLine && isLastLine))
                {
                    if (block != null)
                    {
                        var codeBlock = (CodeBlock)block;
                        // add trailing blank lines to blank lines stack of processor
                        for (int i = codeBlock.Lines.Count - 1; i >= 0; i--)
                        {
                            if (codeBlock.Lines.Lines[i].Slice.IsEmpty)
                            {
                                StringLine line = codeBlock.Lines.Lines[i];
                                processor.BeforeLines ??= new List<StringSlice>();
                                processor.BeforeLines.Add(line.Slice);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
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

                // lines
                var cb = (CodeBlock)block;
                var codeBlockLine = new CodeBlockLine
                {
                    BeforeWhitespace = processor.UseWhitespace(processor.Start - 1)
                };
                cb.CodeBlockLines ??= new List<CodeBlockLine>();
                cb.CodeBlockLines.Add(codeBlockLine);
                cb.Newline = processor.Line.Newline; // ensure block newline is last newline
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