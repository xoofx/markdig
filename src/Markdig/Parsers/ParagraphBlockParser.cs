// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Parsers
{
    /// <summary>
    /// Block parser for a <see cref="ParagraphBlock"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.BlockParser" />
    public class ParagraphBlockParser : BlockParser
    {
        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (processor.IsBlankLine)
            {
                return BlockState.None;
            }

            // We continue trying to match by default
            processor.NewBlocks.Push(new ParagraphBlock(this)
            {
                Column = processor.Column,
                Span = new SourceSpan(processor.Line.Start, processor.Line.End)
            });
            return BlockState.Continue;
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            if (processor.IsBlankLine)
            {
                return BlockState.BreakDiscard;
            }

            if (!processor.IsCodeIndent && !(block.Parent is QuoteBlock))
            {
                return TryParseSetexHeading(processor, block);
            }

            block.UpdateSpanEnd(processor.Line.End);
            return BlockState.Continue;
        }

        public override bool Close(BlockProcessor processor, Block block)
        {
            var paragraph = block as ParagraphBlock;
            if (paragraph != null)
            {
                TryMatchLinkReferenceDefinition(ref paragraph.Lines, processor);

                // If Paragraph is empty, we can discard it
                if (paragraph.Lines.Count == 0)
                {
                    return false;
                }

                var lineCount = paragraph.Lines.Count;
                for (int i = 0; i < lineCount; i++)
                {
                    paragraph.Lines.Lines[i].Slice.TrimStart();
                }
                if (lineCount > 0)
                {
                    paragraph.Lines.Lines[lineCount - 1].Slice.TrimEnd();
                }
            }

            return true;
        }

        private BlockState TryParseSetexHeading(BlockProcessor state, Block block)
        {
            var paragraph = (ParagraphBlock) block;
            var headingChar = (char)0;
            bool checkForSpaces = false;
            var line = state.Line;
            var c = line.CurrentChar;
            while (c != '\0')
            {
                if (headingChar == 0)
                {
                    if (c == '=' || c == '-')
                    {
                        headingChar = c;
                        continue;
                    }
                    break;
                }

                if (checkForSpaces)
                {
                    if (!c.IsSpaceOrTab())
                    {
                        headingChar = (char)0;
                        break;
                    }
                }
                else if (c != headingChar)
                {
                    if (c.IsSpaceOrTab())
                    {
                        checkForSpaces = true;
                    }
                    else
                    {
                        headingChar = (char)0;
                        break;
                    }
                }
                c = line.NextChar();
            }

            if (headingChar != 0)
            {
                // We dicard the paragraph that will be transformed to a heading
                state.Discard(paragraph);

                // If we matched a LinkReferenceDefinition before matching the heading, and the remaining 
                // lines are empty, we can early exit and remove the paragraph
                if (!(TryMatchLinkReferenceDefinition(ref paragraph.Lines, state) && paragraph.Lines.Count == 0))
                {
                    var level = headingChar == '=' ? 1 : 2;

                    var heading = new HeadingBlock(this)
                    {
                        Column = paragraph.Column,
                        Span = new SourceSpan(paragraph.Span.Start, line.Start),
                        Level = level,
                        Lines = paragraph.Lines,
                    };
                    heading.Lines.Trim();

                    // Remove the paragraph as a pending block
                    state.NewBlocks.Push(heading);
                }
                return BlockState.BreakDiscard;
            }

            block.UpdateSpanEnd(state.Line.End);

            return BlockState.Continue;
        }

        private bool TryMatchLinkReferenceDefinition(ref StringLineGroup lines, BlockProcessor state)
        {
            bool atLeastOneFound = false;

            while (true)
            {
                // If we have found a LinkReferenceDefinition, we can discard the previous paragraph
                var iterator = lines.ToCharIterator();
                LinkReferenceDefinition linkReferenceDefinition;
                if (LinkReferenceDefinition.TryParse(ref iterator, out linkReferenceDefinition))
                {
                    if (!state.Document.ContainsLinkReferenceDefinition(linkReferenceDefinition.Label))
                    {
                        state.Document.SetLinkReferenceDefinition(linkReferenceDefinition.Label, linkReferenceDefinition);
                    }
                    atLeastOneFound = true;

                    // Remove lines that have been matched
                    if (iterator.Start > iterator.End)
                    {
                        lines.Clear();
                    }
                    else
                    {
                        for (int i = iterator.SliceIndex - 1; i >= 0; i--)
                        {
                            lines.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return atLeastOneFound;
        }
    }
}