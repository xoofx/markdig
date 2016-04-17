// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Footers
{
    /// <summary>
    /// A block elemeent for a footer.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.ContainerBlock" />
    public class FooterBlock : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FooterBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public FooterBlock(BlockParser parser) : base(parser)
        {
        }

        /// <summary>
        /// Gets or sets the opening character used to match this footer (by default it is ^)
        /// </summary>
        public char OpeningCharacter { get; set; }
    }
}