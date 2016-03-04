// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public class QuoteBlockParser : BlockParser
    {
        public QuoteBlockParser()
        {
            OpeningCharacters = new[] {'>'};
        }

        public override BlockState TryOpen(BlockParserState state)
        {
            if (state.IsCodeIndent)
            {
                return BlockState.None;
            }

            var column = state.Column;

            // 5.1 Block quotes 
            // A block quote marker consists of 0-3 spaces of initial indent, plus (a) the character > together with a following space, or (b) a single character > not followed by a space.
            var quoteChar = state.CurrentChar;
            var c = state.NextChar();
            if (c.IsSpaceOrTab())
            {
                state.NextColumn();
            }
            state.NewBlocks.Push(new QuoteBlock(this) {QuoteChar = quoteChar, Column = column});
            return BlockState.Continue;
        }

        public override BlockState TryContinue(BlockParserState state, Block block)
        {
            if (state.IsCodeIndent)
            {
                return BlockState.None;
            }

            var quote = (QuoteBlock) block;

            // 5.1 Block quotes 
            // A block quote marker consists of 0-3 spaces of initial indent, plus (a) the character > together with a following space, or (b) a single character > not followed by a space.
            var c = state.CurrentChar;
            if (c != quote.QuoteChar)
            {
                return state.IsBlankLine ? BlockState.BreakDiscard : BlockState.None;
            }

            c = state.NextChar(); // Skip opening char
            if (c.IsSpace())
            {
                state.NextChar(); // Skip following space
            }

            return BlockState.Continue;
        }
    }
}