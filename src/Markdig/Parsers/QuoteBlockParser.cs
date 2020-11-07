// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;
using System.Diagnostics;

namespace Markdig.Parsers
{
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
            OpeningCharacters = new[] {'>'};
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
                LinesBefore = processor.UseLinesBefore()
            };

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

            var beforeWhitespace = processor.UseWhitespace(sourcePosition - 1);
            StringSlice whitespaceAfter = StringSlice.Empty;
            bool wasEmptyLine = false;
            if (processor.Line.IsEmptyOrWhitespace())
            {
                processor.WhitespaceStart = processor.Start;
                whitespaceAfter = processor.UseWhitespace(processor.Line.End);
                wasEmptyLine = true;
            }
            quoteBlock.QuoteLines.Add(new QuoteBlockLine
            {
                BeforeWhitespace = beforeWhitespace,
                WhitespaceAfter = whitespaceAfter,
                QuoteChar = true,
                HasSpaceAfterQuoteChar = hasSpaceAfterQuoteChar,
                Newline = processor.Line.Newline,
            });
            processor.NewBlocks.Push(quoteBlock);
            if (!wasEmptyLine)
            {
                processor.WhitespaceStart = processor.Start;
            }
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
            bool hasSpaceAfterQuoteChar = false;
            if (c != quote.QuoteChar)
            {
                if (processor.IsBlankLine)
                {
                    return BlockState.BreakDiscard;
                }
                else
                {
                    quote.QuoteLines.Add(new QuoteBlockLine
                    {
                        QuoteChar = false,
                        Newline = processor.Line.Newline,
                    });
                    return BlockState.None;
                }
            }
            c = processor.NextChar(); // Skip quote marker char
            if (c == ' ')
            {
                //processor.NextChar();
                processor.NextColumn();
                hasSpaceAfterQuoteChar = true;
                processor.SkipFirstUnwindSpace = true;
            }
            else if (c == '\t')
            {
                processor.NextColumn();
            }
            var beforeWhiteSpace = processor.UseWhitespace(sourcePosition - 1);
            StringSlice whitespaceAfter = StringSlice.Empty;
            bool wasEmptyLine = false;
            if (processor.Line.IsEmptyOrWhitespace())
            {
                processor.WhitespaceStart = processor.Start;
                whitespaceAfter = processor.UseWhitespace(processor.Line.End);
                wasEmptyLine = true;
            }
            quote.QuoteLines.Add(new QuoteBlockLine
            {
                QuoteChar = true,
                HasSpaceAfterQuoteChar = hasSpaceAfterQuoteChar,
                BeforeWhitespace = beforeWhiteSpace,
                WhitespaceAfter = whitespaceAfter,
                Newline = processor.Line.Newline,
            });

            if (!wasEmptyLine)
            {
                processor.WhitespaceStart = processor.Start;
            }
            block.UpdateSpanEnd(processor.Line.End);
            return BlockState.Continue;
        }
    }
}