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
    /// <seealso cref="BlockParser" />
    public class ParagraphBlockParser : BlockParser
    {
        public bool ParseSetexHeadings { get; set; } = true;

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

            if (ParseSetexHeadings && !processor.IsCodeIndent && !(block.Parent is QuoteBlock))
            {
                return TryParseSetexHeading(processor, block);
            }

            block.UpdateSpanEnd(processor.Line.End);
            return BlockState.Continue;
        }

        public override bool Close(BlockProcessor processor, Block block)
        {
            if (block is ParagraphBlock paragraph)
            {
                ref var lines = ref paragraph.Lines;

                TryMatchLinkReferenceDefinition(ref lines, processor);

                int lineCount = lines.Count;

                // If Paragraph is empty, we can discard it
                if (lineCount == 0)
                {
                    return false;
                }

                for (int i = 0; i < lineCount; i++)
                {
                    lines.Lines[i].Slice.TrimStart();
                }

                lines.Lines[lineCount - 1].Slice.TrimEnd();
            }

            return true;
        }

        private BlockState TryParseSetexHeading(BlockProcessor state, Block block)
        {
            var line = state.Line;

            char headingChar = GetHeadingChar(ref line);

            if (headingChar != 0)
            {
                var paragraph = (ParagraphBlock)block;

                // If we matched a LinkReferenceDefinition before matching the heading, and the remaining
                // lines are empty, we can early exit and remove the paragraph
                if (!(TryMatchLinkReferenceDefinition(ref paragraph.Lines, state) && paragraph.Lines.Count == 0))
                {
                    // We discard the paragraph that will be transformed to a heading
                    state.Discard(paragraph);

                    int level = headingChar == '=' ? 1 : 2;

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

                    return BlockState.BreakDiscard;
                }
            }

            block.UpdateSpanEnd(state.Line.End);

            return BlockState.Continue;
        }

        private static char GetHeadingChar(ref StringSlice line)
        {
            char headingChar = line.CurrentChar;

            if (headingChar == '=' || headingChar == '-')
            {
                line.CountAndSkipChar(headingChar);

                if (line.IsEmpty)
                {
                    return headingChar;
                }

                while (line.NextChar().IsSpaceOrTab())
                {
                }

                if (line.IsEmpty)
                {
                    return headingChar;
                }
            }

            return (char)0;
        }

        private static bool TryMatchLinkReferenceDefinition(ref StringLineGroup lines, BlockProcessor state)
        {
            bool atLeastOneFound = false;

            while (true)
            {
                // If we have found a LinkReferenceDefinition, we can discard the previous paragraph
                var iterator = lines.ToCharIterator();
                if (LinkReferenceDefinition.TryParse(ref iterator, out LinkReferenceDefinition linkReferenceDefinition))
                {
                    state.Document.SetLinkReferenceDefinition(linkReferenceDefinition.Label, linkReferenceDefinition);
                    atLeastOneFound = true;

                    // Correct the locations of each field
                    linkReferenceDefinition.Line = lines.Lines[0].Line;
                    int startPosition = lines.Lines[0].Slice.Start;

                    linkReferenceDefinition.Span        = linkReferenceDefinition.Span      .MoveForward(startPosition);
                    linkReferenceDefinition.LabelSpan   = linkReferenceDefinition.LabelSpan .MoveForward(startPosition);
                    linkReferenceDefinition.UrlSpan     = linkReferenceDefinition.UrlSpan   .MoveForward(startPosition);
                    linkReferenceDefinition.TitleSpan   = linkReferenceDefinition.TitleSpan .MoveForward(startPosition);

                    lines = iterator.Remaining();
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