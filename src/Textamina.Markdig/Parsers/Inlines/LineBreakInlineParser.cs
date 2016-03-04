// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public class LineBreakInlineParser : InlineParser
    {
        public LineBreakInlineParser()
        {
            OpeningCharacters = new[] {'\n'};
        }

        public bool EnableSoftAsHard { get; set; }

        public override bool Match(InlineParserState state, ref StringSlice slice)
        {
            // Hard line breaks are for separating inline content within a block. Neither syntax for hard line breaks works at the end of a paragraph or other block element:
            if (!(state.Block is ParagraphBlock))
            {
                return false;
            }

            var hasDoubleSpacesBefore = slice.PeekCharExtra(-1).IsSpace() && slice.PeekCharExtra(-2).IsSpace();
            slice.NextChar(); // Skip \n

            state.Inline = !EnableSoftAsHard && (slice.Column == 0 || !hasDoubleSpacesBefore) ? (Inline)new SoftlineBreakInline() : new HardlineBreakInline();
            return true;
        }
    }
}