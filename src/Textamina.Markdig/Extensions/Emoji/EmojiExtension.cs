// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Textamina.Markdig.Extensions.Emoji
{
    /// <summary>
    /// Extension to allow emoji and smiley replacement.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.IMarkdownExtension" />
    public class EmojiExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            if (!pipeline.InlineParsers.Contains<EmojiParser>())
            {
                // Insert the parser before any other parsers
                pipeline.InlineParsers.Insert(0, new EmojiParser());
            }
        }
    }
}