// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;
using System.Diagnostics;

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
                Span = new SourceSpan(processor.Line.Start, processor.Line.End),
                LinesBefore = processor.UseLinesBefore(),
                Newline = processor.Line.Newline,
            });
            return BlockState.Continue;
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            if (processor.IsBlankLine)
            {
                return BlockState.BreakDiscard;
            }

            if (!processor.IsCodeIndent && ParseSetexHeadings)
            {
                return TryParseSetexHeading(processor, block);
            }
            block.Newline = processor.Line.Newline;
            block.UpdateSpanEnd(processor.Line.End);
            return BlockState.Continue;
        }

        public override bool Close(BlockProcessor processor, Block block)
        {
            if (block is ParagraphBlock paragraph)
            {
                ref var lines = ref paragraph.Lines;

                if (processor.TrackTrivia)
                {
                    TryMatchLinkReferenceDefinitionWhitespace(ref lines, processor, paragraph);
                }
                else
                {
                    TryMatchLinkReferenceDefinition(ref lines, processor);
                }

                int lineCount = lines.Count;

                // If Paragraph is empty, we can discard it
                if (lineCount == 0)
                {
                    return false;
                }

                if (!processor.TrackTrivia)
                {
                    for (int i = 0; i < lineCount; i++)
                    {
                        lines.Lines[i].Slice.TrimStart();
                    }
                    lines.Lines[lineCount - 1].Slice.TrimEnd();
                }
            }

            return true;
        }

        private BlockState TryParseSetexHeading(BlockProcessor state, Block block)
        {
            var line = state.Line;
            var sourcePosition = line.Start;
            int count = 0;
            char headingChar = GetHeadingChar(ref line, ref count);

            if (headingChar != 0)
            {
                var paragraph = (ParagraphBlock)block;

                bool foundLrd;
                if (state.TrackTrivia)
                {
                    foundLrd = TryMatchLinkReferenceDefinitionWhitespace(ref paragraph.Lines, state, paragraph);
                }
                else
                {
                    foundLrd = TryMatchLinkReferenceDefinition(ref paragraph.Lines, state);
                }

                // If we matched a LinkReferenceDefinition before matching the heading, and the remaining
                // lines are empty, we can early exit and remove the paragraph
                var parent = block.Parent;
                bool isSetTextHeading = !state.IsLazy || paragraph.Column == state.Column || !(parent is QuoteBlock || parent is ListItemBlock);
                if (!(foundLrd && paragraph.Lines.Count == 0) && isSetTextHeading)
                {
                    // We discard the paragraph that will be transformed to a heading
                    state.Discard(paragraph);

                    while (state.CurrentChar == headingChar)
                    {
                        state.NextChar();
                    }

                    int level = headingChar == '=' ? 1 : 2;

                    var heading = new HeadingBlock(this)
                    {
                        Column = paragraph.Column,
                        Span = new SourceSpan(paragraph.Span.Start, line.Start),
                        Level = level,
                        Lines = paragraph.Lines,
                        WhitespaceBefore = state.UseWhitespace(sourcePosition - 1), // remove dashes
                        WhitespaceAfter = new StringSlice(state.Line.Text, state.Start, line.End),
                        LinesBefore = paragraph.LinesBefore,
                        Newline = state.Line.Newline,
                        IsSetext = true,
                        HeaderCharCount = count,
                        SetextNewline = paragraph.Newline,
                    };
                    if (!state.TrackTrivia)
                    {
                        heading.Lines.Trim();
                    }

                    // Remove the paragraph as a pending block
                    state.NewBlocks.Push(heading);

                    return BlockState.BreakDiscard;
                }
            }

            block.UpdateSpanEnd(state.Line.End);

            return BlockState.Continue;
        }

        private static char GetHeadingChar(ref StringSlice line, ref int count)
        {
            char headingChar = line.CurrentChar;

            if (headingChar == '=' || headingChar == '-')
            {
                count = line.CountAndSkipChar(headingChar);

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
                    state.Document.SetLinkReferenceDefinition(linkReferenceDefinition.Label, linkReferenceDefinition, true);
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

        private static bool TryMatchLinkReferenceDefinitionWhitespace(ref StringLineGroup lines, BlockProcessor state, ParagraphBlock paragraph)
        {
            bool atLeastOneFound = false;

            while (true)
            {
                // If we have found a LinkReferenceDefinition, we can discard the previous paragraph
                var iterator = lines.ToCharIterator();
                if (LinkReferenceDefinition.TryParseWhitespace(
                    ref iterator,
                    out LinkReferenceDefinition lrd,
                    out SourceSpan whitespaceBeforeLabel,
                    out SourceSpan labelWithWhitespace,
                    out SourceSpan whitespaceBeforeUrl,
                    out SourceSpan unescapedUrl,
                    out SourceSpan whitespaceBeforeTitle,
                    out SourceSpan unescapedTitle,
                    out SourceSpan whitespaceAfterTitle))
                {
                    state.Document.SetLinkReferenceDefinition(lrd.Label, lrd, false);
                    lrd.Parent = null; // remove LRDG parent from lrd
                    atLeastOneFound = true;

                    // Correct the locations of each field
                    lrd.Line = lines.Lines[0].Line;
                    var text = lines.Lines[0].Slice.Text;
                    int startPosition = lines.Lines[0].Slice.Start;

                    whitespaceBeforeLabel = whitespaceBeforeLabel.MoveForward(startPosition);
                    labelWithWhitespace = labelWithWhitespace.MoveForward(startPosition);
                    whitespaceBeforeUrl = whitespaceBeforeUrl.MoveForward(startPosition);
                    unescapedUrl = unescapedUrl.MoveForward(startPosition);
                    whitespaceBeforeTitle = whitespaceBeforeTitle.MoveForward(startPosition);
                    unescapedTitle = unescapedTitle.MoveForward(startPosition);
                    whitespaceAfterTitle = whitespaceAfterTitle.MoveForward(startPosition);
                    lrd.Span = lrd.Span.MoveForward(startPosition);
                    lrd.WhitespaceBefore = new StringSlice(text, whitespaceBeforeLabel.Start, whitespaceBeforeLabel.End);
                    lrd.LabelSpan = lrd.LabelSpan.MoveForward(startPosition);
                    lrd.LabelWithWhitespace = new StringSlice(text, labelWithWhitespace.Start, labelWithWhitespace.End);
                    lrd.WhitespaceBeforeUrl = new StringSlice(text, whitespaceBeforeUrl.Start, whitespaceBeforeUrl.End);
                    lrd.UrlSpan = lrd.UrlSpan.MoveForward(startPosition);
                    lrd.UnescapedUrl = new StringSlice(text, unescapedUrl.Start, unescapedUrl.End);
                    lrd.WhitespaceBeforeTitle = new StringSlice(text, whitespaceBeforeTitle.Start, whitespaceBeforeTitle.End);
                    lrd.TitleSpan = lrd.TitleSpan.MoveForward(startPosition);
                    lrd.UnescapedTitle = new StringSlice(text, unescapedTitle.Start, unescapedTitle.End);
                    lrd.WhitespaceAfter = new StringSlice(text, whitespaceAfterTitle.Start, whitespaceAfterTitle.End);
                    lrd.LinesBefore = paragraph.LinesBefore;

                    state.LinesBefore = paragraph.LinesAfter; // ensure closed paragraph with linesafter placed back on stack

                    lines = iterator.Remaining();
                    var index = paragraph.Parent.IndexOf(paragraph);
                    paragraph.Parent.Insert(index, lrd);
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