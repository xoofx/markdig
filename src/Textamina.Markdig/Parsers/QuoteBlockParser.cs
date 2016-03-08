// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    /// <summary>
    /// A block parser for a <see cref="QuoteBlock"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Parsers.BlockParser" />
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

            var column = processor.Column;

            // 5.1 Block quotes 
            // A block quote marker consists of 0-3 spaces of initial indent, plus (a) the character > together with a following space, or (b) a single character > not followed by a space.
            var quoteChar = processor.CurrentChar;
            var c = processor.NextChar();
            if (c.IsSpaceOrTab())
            {
                processor.NextColumn();
            }
            processor.NewBlocks.Push(new QuoteBlock(this) {QuoteChar = quoteChar, Column = column});
            return BlockState.Continue;
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            var quote = (QuoteBlock) block;

            // 5.1 Block quotes 
            // A block quote marker consists of 0-3 spaces of initial indent, plus (a) the character > together with a following space, or (b) a single character > not followed by a space.
            var c = processor.CurrentChar;
            if (c != quote.QuoteChar)
            {
                return processor.IsBlankLine ? BlockState.BreakDiscard : BlockState.None;
            }

            c = processor.NextChar(); // Skip opening char
            if (c.IsSpace())
            {
                processor.NextChar(); // Skip following space
            }

            return BlockState.Continue;
        }
    }
}