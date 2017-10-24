// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Tables
{
    /// <summary>
    /// Defines a cell in a <see cref="TableRow"/>
    /// </summary>
    /// <seealso cref="Markdig.Syntax.LeafBlock" />
    public class TableCell : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableCell"/> class.
        /// </summary>
        public TableCell() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableCell"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public TableCell(BlockParser parser) : base(parser)
        {
            AllowClose = true;
            ColumnSpan = 1;
            ColumnIndex = -1;
            RowSpan = 1;
        }

        /// <summary>
        /// Gets or sets the index of the column to which this cell belongs.
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// Gets or sets the column span this cell is covering. Default is 1.
        /// </summary>
        public int ColumnSpan { get; set; }

        /// <summary>
        /// Gets or sets the row span this cell is covering. Default is 1.
        /// </summary>
        public int RowSpan { get; set; }

        /// <summary>
        /// Gets or sets whether this cell can be closed.
        /// </summary>
        public bool AllowClose { get; set; }
    }
}