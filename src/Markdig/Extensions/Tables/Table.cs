// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Text;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Syntax;

namespace Markdig.Extensions.Tables
{
    /// <summary>
    /// Defines a table that contains an optional <see cref="TableRow"/>.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.ContainerBlock" />
    public class Table : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        public Table() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public Table(BlockParser parser) : base(parser)
        {
            ColumnDefinitions = new List<TableColumnDefinition>();
        }

        /// <summary>
        /// Gets or sets the column alignments. May be null.
        /// </summary>
        public List<TableColumnDefinition> ColumnDefinitions { get; }

        /// <summary>
        /// Checks if the table structure is valid.
        /// </summary>
        /// <returns><c>True</c> if the table has rows and the number of cells per row is correct, other wise <c>false</c>.</returns>
        public bool IsValid()
        {
            // A table with no rows is not valid.
            if (Count == 0)
            {
                return false;
            }
            var columnCount = ColumnDefinitions.Count;
            var rows = new int[Count];
            for (int i = 0; i < Count; i++)
            {
                var row = (TableRow)this[i];
                for (int j = 0; j < row.Count; j++)
                {
                    var cell = (TableCell)row[j];
                    rows[i] += cell.ColumnSpan;
                    var rowSpan = cell.RowSpan - 1;
                    while (rowSpan > 0)
                    {
                        rows[i + rowSpan] += cell.ColumnSpan;
                        rowSpan--;
                    }
                }
                if (rows[i] > columnCount)
                {
                    return false;
                }
            }
            return true;
        }

        
        /// <summary>
        /// Normalizes the number of columns of this table by taking the maximum columns and appending empty cells.
        /// </summary>
        public void Normalize()
        {
            var maxColumn = 0;
            for (int i = 0; i < this.Count; i++)
            {
                var row = this[i] as TableRow;
                if (row != null && row.Count > maxColumn)
                {
                    maxColumn = row.Count;
                }
            }

            for (int i = 0; i < this.Count; i++)
            {
                var row = this[i] as TableRow;
                if (row != null)
                {
                    for (int j = row.Count; j < maxColumn; j++)
                    {
                        row.Add(new TableCell());
                    }
                }
            }

            if (this.Count <= 0) return; //Now rows - return.

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            HtmlRenderer r = new HtmlRenderer(sw);

            var firstRow = this[0] as TableRow;
            if (firstRow.Count <= 1) return; //One or less columns. Break.

            var secondCell = firstRow[1] as TableCell;
            r.Write(secondCell);
            if (!sb.ToString().Equals("<p></p>\n")) return; //First row, second cell is not empty. Break.

            var enable = false; //Is there at least one row with first column empty?

            for (int i = 1; i < this.Count; i++)
            {
                sb.Capacity = sb.Length = 0;
                var row = this[i] as TableRow;
                var cell = row[0] as TableCell;
                r.Write(cell);
                if (sb.ToString().Equals("<p></p>\n"))
                {
                    cell.IsEmptyCell = true;
                    enable = true;
                }
            }

            if (!enable) return; //No row does have first column empty. Break.

            //Merge cells which does have second column empty.
            firstRow.RemoveAt(1);
            var firstCell = firstRow[0] as TableCell;
            firstCell.ColumnSpan = 2;

            for (int i = 1; i < this.Count; i++)
            {
                sb.Capacity = sb.Length = 0;
                var row = this[i] as TableRow;
                var cell = row[1] as TableCell;
                r.Write(cell);
                if (sb.ToString().Equals("<p></p>\n"))
                {
                    row.RemoveAt(1);
                    cell = row[0] as TableCell;
                    cell.ColumnSpan = 2;
                }
            }
        }
    }
}