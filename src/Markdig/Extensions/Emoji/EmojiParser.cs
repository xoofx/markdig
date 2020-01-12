// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Extensions.Emoji
{
    /// <summary>
    /// The inline parser used for emoji.
    /// </summary>
    /// <seealso cref="InlineParser" />
    public class EmojiParser : InlineParser
    {
        private readonly EmojiMapping _emojiMapping;
        private CompactPrefixTree<string> _emojiPrefixTree;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmojiParser"/> class.
        /// </summary>
        public EmojiParser(bool enableSmiley = true, EmojiMapping emojiMapping = null)
        {
            EnableSmiley = enableSmiley;
            _emojiMapping = emojiMapping ?? EmojiMapping.DefaultEmojiMapping;
            OpeningCharacters = null;
        }

        /// <summary>
        /// Gets or sets a boolean indicating whether to process smiley.
        /// </summary>
        public bool EnableSmiley { get; set; }

        public override void Initialize()
        {
            OpeningCharacters = EnableSmiley ? _emojiMapping.EmojiSmileyOpeningCharacters : _emojiMapping.EmojiOpeningCharacters;
            _emojiPrefixTree = EnableSmiley ? _emojiMapping.EmojiSmileyPrefixTree : _emojiMapping.EmojiPrefixTree;
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            // Previous char must be a space
            if (!slice.PeekCharExtra(-1).IsWhiteSpaceOrZero())
            {
                return false;
            }

            // Try to match an emoji
            if (!_emojiPrefixTree.TryMatchLongest(slice.Text, slice.Start, slice.Length, out KeyValuePair<string, string> match))
            {
                return false;
            }

            // Push the EmojiInline
            processor.Inline = new EmojiInline(match.Value)
            {
                Span =
                {
                    Start = processor.GetSourcePosition(slice.Start, out int line, out int column),
                },
                Line = line,
                Column = column,
                Match = match.Key
            };
            processor.Inline.Span.End = processor.Inline.Span.Start + match.Key.Length - 1;

            // Move the cursor to the character after the matched string
            slice.Start += match.Key.Length;

            return true;
        }
    }
}