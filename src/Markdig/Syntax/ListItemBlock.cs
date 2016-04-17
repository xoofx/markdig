// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// A list item (Section 5.2 CommonMark specs)
    /// </summary>
    /// <seealso cref="Markdig.Syntax.ContainerBlock" />
    public class ListItemBlock : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public ListItemBlock(BlockParser parser) : base(parser)
        {
        }

        internal int ColumnWidth { get; set; }
    }
}