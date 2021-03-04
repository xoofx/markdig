// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// Represents a fenced code block.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.5 Fenced code blocks
    /// </remarks>
    public class FencedCodeBlock : CodeBlock, IFencedBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FencedCodeBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public FencedCodeBlock(BlockParser parser) : base(parser)
        {
            // Fenced code blocks are not breakable, unless
            // we reach:
            // - a fenced line terminator
            // - the closing of the container that is holding this fenced block
            IsBreakable = false;
        }

        /// <summary>
        /// Gets or sets the indent count when the fenced code block was indented
        /// and we need to remove up to indent count chars spaces from the begining of a line.
        /// </summary>
        public int IndentCount { get; set; }

        /// <inheritdoc />
        public char FencedChar { get; set; }

        /// <inheritdoc />
        public int OpeningFencedCharCount { get; set; }

        /// <inheritdoc />
        public StringSlice TriviaAfterFencedChar { get; set; }

        /// <inheritdoc />
        public string Info { get; set; }

        /// <inheritdoc />
        public StringSlice UnescapedInfo { get; set; }

        /// <inheritdoc />
        public StringSlice TriviaAfterInfo { get; set; }

        /// <inheritdoc />
        public string Arguments { get; set; }

        /// <inheritdoc />
        public StringSlice UnescapedArguments { get; set; }

        /// <inheritdoc />
        public StringSlice TriviaAfterArguments { get; set; }

        /// <inheritdoc />
        public NewLine InfoNewLine { get; set; }

        /// <inheritdoc />
        public StringSlice TriviaBeforeClosingFence { get; set; }

        /// <inheritdoc />
        public int ClosingFencedCharCount { get; set; }
    }
}