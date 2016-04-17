// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// Represents a group of lines that is treated as raw HTML (and will not be escaped in HTML output).
    /// </summary>
    /// <seealso cref="Markdig.Syntax.LeafBlock" />
    public class HtmlBlock : LeafBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public HtmlBlock(BlockParser parser) : base(parser)
        {
        }

        /// <summary>
        /// Gets or sets the type of block.
        /// </summary>
        public HtmlBlockType Type { get; set; }
    }
}