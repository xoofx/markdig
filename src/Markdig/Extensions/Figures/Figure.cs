// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Figures
{
    /// <summary>
    /// Defines a figure container.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.ContainerBlock" />
    public class Figure : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Figure"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public Figure(BlockParser parser) : base(parser)
        {
        }

        /// <summary>
        /// Gets or sets the opening character count used to open this figure code block.
        /// </summary>
        public int OpeningCharacterCount { get; set; }

        /// <summary>
        /// Gets or sets the opening character used to open and close this figure code block.
        /// </summary>
        public char OpeningCharacter { get; set; }
    }
}