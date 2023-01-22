// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

using static Markdig.Syntax.CodeBlock;

namespace Markdig.Parsers;

/// <summary>
/// Block parser for an indented <see cref="CodeBlock"/>.
/// </summary>
/// <seealso cref="BlockParser" />
public class IndentedCodeBlockParser : BlockParser
{
    public override bool CanInterrupt(BlockProcessor processor, Block block)
    {
        return !block.IsParagraphBlock;
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
            };

            if (processor.TrackTrivia)
            {
                codeBlock.LinesBefore = processor.UseLinesBefore();
                codeBlock.NewLine = processor.Line.NewLine;
            }

            var codeBlockLine = new CodeBlockLine
            {
                TriviaBefore = processor.UseTrivia(sourceStartPosition - 1)
            };
            codeBlock.CodeBlockLines.Add(codeBlockLine);
            processor.NewBlocks.Push(codeBlock);

            // Go back to the correct column
            processor.GoToColumn(column);
        }
        return result;
    }

    public override BlockState TryContinue(BlockProcessor processor, Block? block)
    {
        if (!processor.IsCodeIndent || processor.IsBlankLine)
        {
            if (block is null || !processor.IsBlankLine)
            {
                if (block != null)
                {
                    var codeBlock = (CodeBlock)block;
                    // add trailing blank lines to blank lines stack of processor
                    for (int i = codeBlock.Lines.Count - 1; i >= 0; i--)
                    {
                        var line = codeBlock.Lines.Lines[i];
                        if (line.Slice.IsEmpty)
                        {
                            codeBlock.Lines.RemoveAt(i);

                            if (processor.TrackTrivia)
                            {
                                processor.LinesBefore ??= new List<StringSlice>();
                                processor.LinesBefore.Add(line.Slice);
                            }
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
            var codeBlockLine = new CodeBlockLine();

            cb.CodeBlockLines.Add(codeBlockLine);

            if (processor.TrackTrivia)
            {
                codeBlockLine.TriviaBefore = processor.UseTrivia(processor.Start - 1);
                cb.NewLine = processor.Line.NewLine; // ensure block newline is last newline
            }
        }

        return BlockState.Continue;
    }

    public override bool Close(BlockProcessor processor, Block block)
    {
        var codeBlock = (CodeBlock)block;
        if (codeBlock is null)
        {
            return true;
        }

        // Remove any trailing blankline
        for (int i = codeBlock.Lines.Count - 1; i >= 0; i--)
        {
            var line = codeBlock.Lines.Lines[i];
            if (line.Slice.IsEmpty)
            {
                codeBlock.Lines.RemoveAt(i);

                // if there are newlines after an indented codeblock, we must transform them
                // into empty lines after the block. as whitespace is stripped from the Line
                // we get that back from the beforeWhitespace on the CodeBlockLine.
                if (processor.TrackTrivia)
                {
                    var quoteLine = codeBlock.CodeBlockLines[i];
                    var emptyLine = new StringSlice(line.Slice.Text, quoteLine.TriviaBefore.Start, line.Slice.End, line.NewLine);
                    block.LinesAfter ??= new List<StringSlice>();
                    block.LinesAfter.Add(emptyLine);
                }
            }
            else
            {
                break;
            }
        }
        return true;
    }
}