// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.Tables
{
    /// <summary>
    /// Defines a cell in a <see cref="TableRow"/>
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Syntax.LeafBlock" />
    public class TableCell : ContainerBlock
    {
        public TableCell() : this(null)
        {
        }

        public TableCell(BlockParser parser) : base(parser)
        {
            ColumnSpan = 1;
        }

        /// <summary>
        /// Gets or sets the column span this cell is covering. Default is 1.
        /// </summary>
        public int ColumnSpan { get; set; }
    }
}