// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Parsers.Inlines
{
    /// <summary>
    /// An inline parser for <see cref="LineBreakInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.InlineParser" />
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

            var startPosition = slice.Start;
            var hasDoubleSpacesBefore = slice.PeekCharExtra(-1).IsSpace() && slice.PeekCharExtra(-2).IsSpace();
            slice.NextChar(); // Skip \n

            int line;
            int column;
            processor.Inline = new LineBreakInline
            {
                Span = { Start = processor.GetSourcePosition(startPosition, out line, out column)},
                IsHard = EnableSoftAsHard || (slice.Start != 0 && hasDoubleSpacesBefore),
                Line = line,
                Column = column
            };
            processor.Inline.Span.End = processor.Inline.Span.Start;
            return true;
        }
    }
}