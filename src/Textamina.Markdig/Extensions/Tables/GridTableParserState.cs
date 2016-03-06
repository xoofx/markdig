using System.Collections.Generic;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Extensions.Tables
{
    internal class GridTableParserState
    {
        public int StartColumn { get; set; }

        public StringLineGroup Lines { get; private set; }

        public List<ColumnSlice> ColumnSlices { get; private set; }

        public bool ExpectRow { get; set; }

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
                ColumnStart = start,
                ColumnEnd = end,
                Align = align,
            });
        }

        public class ColumnSlice
        {
            public ColumnSlice()
            {
                CurrentColumnSpan = -1;
            }

            public int ColumnStart { get; set; }

            public int ColumnEnd { get; set; }

            public TableColumnAlign Align { get; set; }

            public int CurrentColumnSpan { get; set; }

            public int PreviousColumnSpan { get; set; }

            public BlockParserState BlockParserState { get; set; }

            public TableCellBlock CurrentCell { get; set; }
        }
    }
}