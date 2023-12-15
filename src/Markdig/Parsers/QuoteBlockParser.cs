// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Parsers;

/// <summary>
/// A block parser for a <see cref="QuoteBlock"/>.
/// </summary>
/// <seealso cref="BlockParser" />
public class QuoteBlockParser : BlockParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuoteBlockParser"/> class.
    /// </summary>
    public QuoteBlockParser()
    {
        OpeningCharacters = ['>'];
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
        if (processor.IsCodeIndent)
        {
            return BlockState.None;
        }

        var sourcePosition = processor.Start;

        // 5.1 Block quotes 
        // A block quote marker consists of 0-3 spaces of initial indent, plus (a) the character > together with a following space, or (b) a single character > not followed by a space.
        var quoteChar = processor.CurrentChar;
        var column = processor.Column;
        var c = processor.NextChar();

        var quoteBlock = new QuoteBlock(this)
        {
            QuoteChar = quoteChar,
            Column = column,
            Span = new SourceSpan(sourcePosition, processor.Line.End),
        };

        if (processor.TrackTrivia)
        {
            quoteBlock.LinesBefore = processor.UseLinesBefore();
        }

        bool hasSpaceAfterQuoteChar = false;
        if (c == ' ')
        {
            processor.NextColumn();
            hasSpaceAfterQuoteChar = true;
            processor.SkipFirstUnwindSpace = true;
        }
        else if (c == '\t')
        {
            processor.NextColumn();
        }

        if (processor.TrackTrivia)
        {
            var triviaBefore = processor.UseTrivia(sourcePosition - 1);
            StringSlice triviaAfter = StringSlice.Empty;
            bool wasEmptyLine = false;
            if (processor.Line.IsEmptyOrWhitespace())
            {
                processor.TriviaStart = processor.Start;
                triviaAfter = processor.UseTrivia(processor.Line.End);
                wasEmptyLine = true;
            }

            if (!wasEmptyLine)
            {
                processor.TriviaStart = processor.Start;
            }

            quoteBlock.QuoteLines.Add(new QuoteBlockLine
            {
                TriviaBefore = triviaBefore,
                TriviaAfter = triviaAfter,
                QuoteChar = true,
                HasSpaceAfterQuoteChar = hasSpaceAfterQuoteChar,
                NewLine = processor.Line.NewLine,
            });
        }

        processor.NewBlocks.Push(quoteBlock);
        return BlockState.Continue;
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
        if (processor.IsCodeIndent)
        {
            return BlockState.None;
        }

        var quote = (QuoteBlock) block;
        var sourcePosition = processor.Start;

        // 5.1 Block quotes 
        // A block quote marker consists of 0-3 spaces of initial indent, plus (a) the character > together with a following space, or (b) a single character > not followed by a space.
        var c = processor.CurrentChar;
        if (c != quote.QuoteChar)
        {
            if (processor.IsBlankLine)
            {
                return BlockState.BreakDiscard;
            }
            else
            {
                if (processor.TrackTrivia)
                {
                    quote.QuoteLines.Add(new QuoteBlockLine
                    {
                        QuoteChar = false,
                        NewLine = processor.Line.NewLine,
                    });
                }
                return BlockState.None;
            }
        }

        bool hasSpaceAfterQuoteChar = false;
        c = processor.NextChar(); // Skip quote marker char
        if (c == ' ')
        {
            processor.NextColumn();
            hasSpaceAfterQuoteChar = true;
            processor.SkipFirstUnwindSpace = true;
        }
        else if (c == '\t')
        {
            processor.NextColumn();
        }

        if (processor.TrackTrivia)
        {
            var triviaSpaceBefore = processor.UseTrivia(sourcePosition - 1);
            StringSlice triviaAfter = StringSlice.Empty;
            bool wasEmptyLine = false;
            if (processor.Line.IsEmptyOrWhitespace())
            {
                processor.TriviaStart = processor.Start;
                triviaAfter = processor.UseTrivia(processor.Line.End);
                wasEmptyLine = true;
            }
            quote.QuoteLines.Add(new QuoteBlockLine
            {
                QuoteChar = true,
                HasSpaceAfterQuoteChar = hasSpaceAfterQuoteChar,
                TriviaBefore = triviaSpaceBefore,
                TriviaAfter = triviaAfter,
                NewLine = processor.Line.NewLine,
            });

            if (!wasEmptyLine)
            {
                processor.TriviaStart = processor.Start;
            }
        }

        block.UpdateSpanEnd(processor.Line.End);
        return BlockState.Continue;
    }
}