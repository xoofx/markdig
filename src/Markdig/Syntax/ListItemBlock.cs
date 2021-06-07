// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// A list item (Section 5.2 CommonMark specs)
    /// </summary>
    /// <seealso cref="ContainerBlock" />
    public class ListItemBlock : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemBlock"/> class.
        /// </summary>
        public ListItemBlock() : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public ListItemBlock(BlockParser parser) : base(parser)
        {
        }

        public int ColumnWidth { get; set; }

        /// <summary>
        /// The number defined for this <see cref="ListItemBlock"/> in an ordered list
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the bullet as parsed in the source document.
        /// Trivia: only parsed when <see cref="MarkdownParser.TrackTrivia"/> is enabled, otherwise
        /// <see cref="StringSlice.IsEmpty"/>.
        /// </summary>
        public StringSlice SourceBullet { get; set; }
    }
}