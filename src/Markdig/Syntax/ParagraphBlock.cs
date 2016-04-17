// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// Repressents a paragraph.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.8 Paragraphs
    /// </remarks>
    public class ParagraphBlock : LeafBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphBlock"/> class.
        /// </summary>
        public ParagraphBlock() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public ParagraphBlock(BlockParser parser) : base(parser)
        {
            // Inlines are processed for a paragraph
            ProcessInlines = true;
        }

        public int LastLine => Line + Lines.Count - 1;
    }
}