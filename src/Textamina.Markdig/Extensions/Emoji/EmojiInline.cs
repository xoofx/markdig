// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Emoji
{
    /// <summary>
    /// An emoji inline 
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Syntax.Inlines.Inline" />
    public class EmojiInline : LiteralInline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmojiInline"/> class.
        /// </summary>
        public EmojiInline()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmojiInline"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public EmojiInline(string content)
        {
            Content = new StringSlice(content);
        }

        /// <summary>
        /// Gets or sets the original match string (either an emoji or a text smiley)
        /// </summary>
        public string Match { get; set; }
    }
}