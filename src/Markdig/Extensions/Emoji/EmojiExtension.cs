// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;

namespace Markdig.Extensions.Emoji
{
    /// <summary>
    /// Extension to allow emoji and smiley replacement.
    /// </summary>
    /// <seealso cref="Markdig.IMarkdownExtension" />
    public class EmojiExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<EmojiParser>())
            {
                // Insert the parser before any other parsers
                pipeline.InlineParsers.Insert(0, new EmojiParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }
    }
}