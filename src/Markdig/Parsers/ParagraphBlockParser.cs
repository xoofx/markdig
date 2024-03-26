// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Parsers;

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
        var paragraph = new ParagraphBlock(this)
        {
            Column = processor.Column,
            Span = new SourceSpan(processor.Line.Start, processor.Line.End),
        };

        if (processor.TrackTrivia)
        {
            paragraph.LinesBefore = processor.UseLinesBefore();
            paragraph.NewLine = processor.Line.NewLine;
        }

        processor.NewBlocks.Push(paragraph);
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
        block.NewLine = processor.Line.NewLine;
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
                TryMatchLinkReferenceDefinitionTrivia(ref lines, processor, paragraph);
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
                foundLrd = TryMatchLinkReferenceDefinitionTrivia(ref paragraph.Lines, state, paragraph);
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
                    Span = new SourceSpan(paragraph.Span.Start, line.End),
                    Level = level,
                    Lines = paragraph.Lines,
                    IsSetext = true,
                    HeaderCharCount = count,
                };

                if (state.TrackTrivia)
                {
                    heading.LinesBefore = paragraph.LinesBefore;
                    heading.TriviaBefore = state.UseTrivia(sourcePosition - 1); // remove dashes
                    heading.TriviaAfter = new StringSlice(state.Line.Text, state.Start, line.End);
                    heading.NewLine = state.Line.NewLine;
                    heading.SetextNewline = paragraph.NewLine;
                }
                else
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

            while (line.CurrentChar.IsSpaceOrTab())
            {
                line.NextChar();
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
            if (LinkReferenceDefinition.TryParse(ref iterator, out LinkReferenceDefinition? linkReferenceDefinition))
            {
                state.Document.SetLinkReferenceDefinition(linkReferenceDefinition.Label!, linkReferenceDefinition, true);
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

    private static bool TryMatchLinkReferenceDefinitionTrivia(ref StringLineGroup lines, BlockProcessor state, ParagraphBlock paragraph)
    {
        bool atLeastOneFound = false;

        while (true)
        {
            // If we have found a LinkReferenceDefinition, we can discard the previous paragraph
            var iterator = lines.ToCharIterator();
            if (LinkReferenceDefinition.TryParseTrivia(
                ref iterator,
                out LinkReferenceDefinition? lrd,
                out SourceSpan triviaBeforeLabel,
                out SourceSpan labelWithTrivia,
                out SourceSpan triviaBeforeUrl,
                out SourceSpan unescapedUrl,
                out SourceSpan triviaBeforeTitle,
                out SourceSpan unescapedTitle,
                out SourceSpan triviaAfterTitle))
            {
                state.Document.SetLinkReferenceDefinition(lrd.Label!, lrd, false);
                lrd.Parent = null; // remove LRDG parent from lrd
                atLeastOneFound = true;

                // Correct the locations of each field
                lrd.Line = lines.Lines[0].Line;
                var text = lines.Lines[0].Slice.Text;
                int startPosition = lines.Lines[0].Slice.Start;

                triviaBeforeLabel = triviaBeforeLabel.MoveForward(startPosition);
                labelWithTrivia = labelWithTrivia.MoveForward(startPosition);
                triviaBeforeUrl = triviaBeforeUrl.MoveForward(startPosition);
                unescapedUrl = unescapedUrl.MoveForward(startPosition);
                triviaBeforeTitle = triviaBeforeTitle.MoveForward(startPosition);
                unescapedTitle = unescapedTitle.MoveForward(startPosition);
                triviaAfterTitle = triviaAfterTitle.MoveForward(startPosition);
                lrd.Span = lrd.Span.MoveForward(startPosition);
                lrd.TriviaBefore = new StringSlice(text, triviaBeforeLabel.Start, triviaBeforeLabel.End);
                lrd.LabelSpan = lrd.LabelSpan.MoveForward(startPosition);
                lrd.LabelWithTrivia = new StringSlice(text, labelWithTrivia.Start, labelWithTrivia.End);
                lrd.TriviaBeforeUrl = new StringSlice(text, triviaBeforeUrl.Start, triviaBeforeUrl.End);
                lrd.UrlSpan = lrd.UrlSpan.MoveForward(startPosition);
                lrd.UnescapedUrl = new StringSlice(text, unescapedUrl.Start, unescapedUrl.End);
                lrd.TriviaBeforeTitle = new StringSlice(text, triviaBeforeTitle.Start, triviaBeforeTitle.End);
                lrd.TitleSpan = lrd.TitleSpan.MoveForward(startPosition);
                lrd.UnescapedTitle = new StringSlice(text, unescapedTitle.Start, unescapedTitle.End);
                lrd.TriviaAfter = new StringSlice(text, triviaAfterTitle.Start, triviaAfterTitle.End);
                lrd.LinesBefore = paragraph.LinesBefore;

                state.LinesBefore = paragraph.LinesAfter; // ensure closed paragraph with linesafter placed back on stack

                lines = iterator.Remaining();
                var index = paragraph.Parent!.IndexOf(paragraph);
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