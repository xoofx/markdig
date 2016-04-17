// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// A block quote (Section 5.1 CommonMark specs)
    /// </summary>
    /// <seealso cref="Markdig.Syntax.ContainerBlock" />
    public class QuoteBlock : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public QuoteBlock(BlockParser parser) : base(parser)
        {
        }

        /// <summary>
        /// Gets or sets the quote character (usually `&gt;`)
        /// </summary>
        public char QuoteChar { get; set; }
    }
}