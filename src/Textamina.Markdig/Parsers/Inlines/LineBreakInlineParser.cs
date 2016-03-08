// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    /// <summary>
    /// An inline parser for <see cref="SoftlineBreakInline"/> and <see cref="HardlineBreakInline"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Parsers.InlineParser" />
    public class LineBreakInlineParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineBreakInlineParser"/> class.
        /// </summary>
        public LineBreakInlineParser()
        {
            OpeningCharacters = new[] {'\n'};
        }

        /// <summary>
        /// Gets or sets a value indicating whether to interpret softline breaks as hardline breaks. Default is false
        /// </summary>
        public bool EnableSoftAsHard { get; set; }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            // Hard line breaks are for separating inline content within a block. Neither syntax for hard line breaks works at the end of a paragraph or other block element:
            if (!(processor.Block is ParagraphBlock))
            {
                return false;
            }

            var hasDoubleSpacesBefore = slice.PeekCharExtra(-1).IsSpace() && slice.PeekCharExtra(-2).IsSpace();
            slice.NextChar(); // Skip \n

            processor.Inline = !EnableSoftAsHard && (slice.Start == 0 || !hasDoubleSpacesBefore) ? (Inline)new SoftlineBreakInline() : new HardlineBreakInline();
            return true;
        }
    }
}