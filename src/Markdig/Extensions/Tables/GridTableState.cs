// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Extensions.Tables;

/// <summary>
/// Internal state used by the <see cref="GridTableParser"/>
/// </summary>
internal sealed class GridTableState(int start, bool expectRow)
{
    public int Start { get; } = start;

    public StringLineGroup Lines;

    public List<ColumnSlice>? ColumnSlices { get; private set; }

    public bool ExpectRow { get; } = expectRow;

    public int StartRowGroup { get; set; }

    public void AddLine(ref StringSlice line)
    {
        if (Lines.Lines is null)
        {
            Lines = new StringLineGroup(4);
        }

        Lines.Add(line);
    }

    public void AddColumn(int start, int end, TableColumnAlign? align)
    {
        ColumnSlices ??= new List<ColumnSlice>();
        
        ColumnSlices.Add(new ColumnSlice(start, end, align));
    }

    public sealed class ColumnSlice(int start, int end, TableColumnAlign? align)
    {
        /// <summary>
        /// Gets or sets the index position of this column (after the |)
        /// </summary>
        public int Start { get; } = start;

        public int End { get; } = end;

        public TableColumnAlign? Align { get; } = align;

        public int CurrentColumnSpan { get; set; } = -1;

        public int PreviousColumnSpan { get; set; }

        public BlockProcessor? BlockProcessor { get; set; }

        public TableCell? CurrentCell { get; set; }
    }
}