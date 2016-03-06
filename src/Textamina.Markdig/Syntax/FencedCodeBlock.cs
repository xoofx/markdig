// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a fenced code block.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.5 Fenced code blocks
    /// </remarks>
    public class FencedCodeBlock : CodeBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public FencedCodeBlock(BlockParser parser) : base(parser)
        {
            // Fenced code blocks are not breakable, unless
            // we reach the end of the document (or the end of a table cell in a grid table)
            IsBreakable = false;
        }

        /// <summary>
        /// Gets or sets the language parsed after the first line of 
        /// the fenced code block. May be null.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the arguments after the <see cref="Language"/>.
        /// May be null.
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// Gets or sets the fenced character count used to open this fenced code block.
        /// </summary>
        public int FencedCharCount { get; set; }

        /// <summary>
        /// Gets or sets the fenced character used to open and close this fenced code block.
        /// </summary>
        public char FencedChar { get; set; }

        /// <summary>
        /// Gets or sets the indent count when the fenced code block was indented 
        /// and we need to remove up to indent count chars spaces from the begining of a line.
        /// </summary>
        internal int IndentCount { get; set; }
    }
}