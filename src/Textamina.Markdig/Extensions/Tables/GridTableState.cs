using System.Collections.Generic;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Extensions.Tables
{
    internal class GridTableState
    {
        public int Start { get; set; }

        public StringLineGroup Lines { get; private set; }

        public List<ColumnSlice> ColumnSlices { get; private set; }

        public bool ExpectRow { get; set; }

        public int StartRowGroup { get; set; }

        public void AddLine(ref StringSlice line)
        {
            if (Lines == null)
            {
                Lines = new StringLineGroup();
            }
            Lines.Add(line);
        }

        public void AddColumn(int start, int end, TableColumnAlign align)
        {
            if (ColumnSlices == null)
            {
                ColumnSlices = new List<ColumnSlice>();
            }

            ColumnSlices.Add(new ColumnSlice()
            {
                Start = start,
                End = end,
                Align = align,
            });
        }

        public class ColumnSlice
        {
            public ColumnSlice()
            {
                CurrentColumnSpan = -1;
            }

            /// <summary>
            /// Gets or sets the index position of this column (after the |)
            /// </summary>
            public int Start { get; set; }

            public int End { get; set; }

            public TableColumnAlign Align { get; set; }

            public int CurrentColumnSpan { get; set; }

            public int PreviousColumnSpan { get; set; }

            public BlockProcessor BlockProcessor { get; set; }

            public TableCell CurrentCell { get; set; }
        }
    }
}