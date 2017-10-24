// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Parsers.Inlines
{
    /// <summary>
    /// An inline parser for parsing <see cref="LiteralInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.InlineParser" />
    public sealed class LiteralInlineParser : InlineParser
    {
        public delegate void PostMatchDelegate(InlineProcessor processor, ref StringSlice slice);

        /// <summary>
        /// We don't expect the LiteralInlineParser to be instantiated a end-user, as it is part
        /// of the default parser pipeline (and should always be the last), working as a literal character
        /// collector.
        /// </summary>
        public LiteralInlineParser()
        {
        }

        /// <summary>
        /// Gets or sets the post match delegate called after the inline has been processed.
        /// </summary>
        public PostMatchDelegate PostMatch { get; set; }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            var text = slice.Text;

            int line;
            int column;
            var startPosition = processor.GetSourcePosition(slice.Start, out line, out column);

            // Sligthly faster to perform our own search for opening characters
            var nextStart = processor.Parsers.IndexOfOpeningCharacter(text, slice.Start + 1, slice.End);
            //var nextStart = str.IndexOfAny(processor.SpecialCharacters, slice.Start + 1, slice.Length - 1);
            int length;

            if (nextStart < 0)
            {
                nextStart = slice.End + 1;
                length = nextStart - slice.Start;
            }
            else
            {
                // Remove line endings if the next char is a new line
                length = nextStart - slice.Start;
                if (text[nextStart] == '\n')
                {
                    int end = nextStart - 1;
                    while (length > 0 && text[end].IsSpace())
                    {
                        length--;
                        end--;
                    }
                }
            }

            // The LiteralInlineParser is always matching (at least an empty string)
            var endPosition = slice.Start + length - 1;

            var previousInline = processor.Inline as LiteralInline;
            if (previousInline != null && ReferenceEquals(previousInline.Content.Text, slice.Text) &&
                previousInline.Content.End + 1 == slice.Start)
            {
                previousInline.Content.End = endPosition;
                previousInline.Span.End = processor.GetSourcePosition(endPosition);
            }
            else
            {
                // Create a new LiteralInline only if it is not empty
                var newSlice = length > 0 ? new StringSlice(slice.Text, slice.Start, endPosition) : StringSlice.Empty;
                if (!newSlice.IsEmpty)
                {
                    processor.Inline = new LiteralInline()
                    {
                        Content = length > 0 ? newSlice : StringSlice.Empty,
                        Span = new SourceSpan(startPosition, processor.GetSourcePosition(endPosition)),
                        Line = line,
                        Column = column,
                    };
                }
            }

            slice.Start = nextStart;

            // Call only PostMatch if necessary
            if (processor.Inline is LiteralInline)
            {
                PostMatch?.Invoke(processor, ref slice);
            }

            return true;
        }
    }
}