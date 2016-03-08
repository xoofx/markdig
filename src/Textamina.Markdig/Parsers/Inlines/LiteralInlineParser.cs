// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    /// <summary>
    /// An inline parser for parsing <see cref="LiteralInline"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Parsers.InlineParser" />
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
            processor.Inline = length > 0 ? new LiteralInline {Content = new StringSlice(slice.Text, slice.Start, slice.Start + length - 1)} : new LiteralInline();
            slice.Start = nextStart;

            // Call only PostMatch if necessary
            if (PostMatch != null)
            {
                PostMatch(processor, ref slice);
            }

            return true;
        }
    }
}