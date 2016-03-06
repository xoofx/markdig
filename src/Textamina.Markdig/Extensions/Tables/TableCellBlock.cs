// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.Tables
{
    /// <summary>
    /// Defines a cell in a <see cref="TableRowBlock"/>
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Syntax.LeafBlock" />
    public class TableCellBlock : ContainerBlock
    {
        public TableCellBlock() : base(null)
        {
            ColumnSpan = 1;
        }

        /// <summary>
        /// Gets or sets the column span this cell is covering. Default is 1.
        /// </summary>
        public int ColumnSpan { get; set; }
    }
}