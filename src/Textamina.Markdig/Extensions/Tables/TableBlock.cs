// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Generic;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.Tables
{
    /// <summary>
    /// Defines a table that contains an optional <see cref="TableRowBlock"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Syntax.ContainerBlock" />
    public class TableBlock : ContainerBlock
    {
        public TableBlock() : this(null)
        {
        }

        public TableBlock(BlockParser parser) : base(parser)
        {
            ColumnDefinitions = new List<TableColumnDefinition>();
        }

        /// <summary>
        /// Gets or sets the column alignments. May be null.
        /// </summary>
        public List<TableColumnDefinition> ColumnDefinitions { get; private set; }
    }
}