// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.Tables
{
    /// <summary>
    /// Defines a row in a <see cref="TableBlock"/>, contains <see cref="TableCellBlock"/>, parent is <see cref="TableBlock"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Syntax.ContainerBlock" />
    public class TableRowBlock : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableRowBlock"/> class.
        /// </summary>
        public TableRowBlock() : base(null)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is header row.
        /// </summary>
        public bool IsHeader { get; set; }

        /// <summary>
        /// Gets or sets the column alignments. May be null.
        /// </summary>
        public List<TableColumnAlign> ColumnAlignments { get; set; }
    }
}