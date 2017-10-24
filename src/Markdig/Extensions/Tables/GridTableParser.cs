
// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Tables
{
    public class GridTableParser : BlockParser
    {
        public GridTableParser()
        {
            OpeningCharacters = new[] { '+' };
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            // A grid table cannot start more than an indent
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            var line = processor.Line;
            GridTableState tableState = null;

            // Match the first row that should be of the minimal form: +---------------
            var c = line.CurrentChar;
            var lineStart = line.Start;
            while (c == '+')
            {
                var columnStart = line.Start;
                line.NextChar();
                line.TrimStart();

                // if we have reached the end of the line, exit
                c = line.CurrentChar;
                if (c == 0)
                {
                    break;
                }

                // Parse a column alignment
                TableColumnAlign? columnAlign;
                if (!TableHelper.ParseColumnHeader(ref line, '-', out columnAlign))
                {
                    return BlockState.None;
                }

                tableState = tableState ?? new GridTableState { Start = processor.Start, ExpectRow = true };
                tableState.AddColumn(columnStart - lineStart, line.Start - lineStart, columnAlign);

                c = line.CurrentChar;
            }

            if (c != 0 || tableState == null)
            {
                return BlockState.None;
            }
            // Store the line (if we need later to build a ParagraphBlock because the GridTable was in fact invalid)
            tableState.AddLine(ref processor.Line);
            var table = new Table(this);
            table.SetData(typeof(GridTableState), tableState);

            // Calculate the total width of all columns
            int totalWidth = 0;
            foreach (var columnSlice in tableState.ColumnSlices)
            {
                totalWidth += columnSlice.End - columnSlice.Start - 1;
            }

            // Store the column width and alignment
            foreach (var columnSlice in tableState.ColumnSlices)
            {
                var columnDefinition = new TableColumnDefinition
                {
                    // Column width proportional to the total width
                    Width = (float)(columnSlice.End - columnSlice.Start - 1) * 100.0f / totalWidth,
                    Alignment = columnSlice.Align
                };
                table.ColumnDefinitions.Add(columnDefinition);
            }

            processor.NewBlocks.Push(table);

            return BlockState.ContinueDiscard;
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            var gridTable = (Table)block;
            var tableState = (GridTableState)block.GetData(typeof(GridTableState));
            tableState.AddLine(ref processor.Line);
            if (processor.CurrentChar == '+')
            {
                return HandleNewRow(processor, tableState, gridTable);
            }
            if (processor.CurrentChar == '|')
            {
                return HandleContents(processor, tableState, gridTable);
            }
            TerminateCurrentRow(processor, tableState, gridTable, true);
            // If the table is not valid we need to remove the grid table, 
            // and create a ParagraphBlock with the slices 
            if (!gridTable.IsValid())
            {
                Undo(processor, tableState, gridTable);
            }
            return BlockState.Break;
        }

        private BlockState HandleNewRow(BlockProcessor processor, GridTableState tableState, Table gridTable)
        {
            bool isHeaderRow, hasRowSpan;
            var columns = tableState.ColumnSlices;
            SetRowSpanState(columns, processor.Line, out isHeaderRow, out hasRowSpan);
            SetColumnSpanState(columns, processor.Line);
            TerminateCurrentRow(processor, tableState, gridTable, false);
            if (isHeaderRow)
            {
                for (int i = 0; i < gridTable.Count; i++)
                {
                    var row = (TableRow)gridTable[i];
                    row.IsHeader = true;
                }
            }
            tableState.StartRowGroup = gridTable.Count;
            if (hasRowSpan)
            {
                HandleContents(processor, tableState, gridTable);
            }
            return BlockState.ContinueDiscard;
        }

        private static void SetRowSpanState(List<GridTableState.ColumnSlice> columns, StringSlice line, out bool isHeaderRow, out bool hasRowSpan)
        {
            var lineStart = line.Start;
            isHeaderRow = line.PeekChar(1) == '=' || line.PeekChar(2) == '=';
            hasRowSpan = false;
            foreach (var columnSlice in columns)
            {
                if (columnSlice.CurrentCell != null)
                {
                    line.Start = lineStart + columnSlice.Start + 1;
                    line.End = lineStart + columnSlice.End - 1;
                    line.Trim();
                    if (line.IsEmptyOrWhitespace() || !IsRowSeperator(line))
                    {
                        hasRowSpan = true;
                        columnSlice.CurrentCell.RowSpan++;
                        columnSlice.CurrentCell.AllowClose = false;
                    }
                    else
                    {
                        columnSlice.CurrentCell.AllowClose = true;
                    }
                }
            }
        }

        private static bool IsRowSeperator(StringSlice slice)
        {
            while (slice.Length > 0)
            {
                if (slice.CurrentChar != '-' && slice.CurrentChar != '=' && slice.CurrentChar != ':')
                {
                    return false;
                }
                slice.NextChar();
            }
            return true;
        }

        private static void TerminateCurrentRow(BlockProcessor processor, GridTableState tableState, Table gridTable, bool isLastRow)
        {
            var columns = tableState.ColumnSlices;
            TableRow currentRow = null;
            for (int i = 0; i < columns.Count; i++)
            {
                var columnSlice = columns[i];
                if (columnSlice.CurrentCell != null)
                {
                    if (currentRow == null)
                    {
                        currentRow = new TableRow();
                    }
                    // If this cell does not already belong to a row
                    if (columnSlice.CurrentCell.Parent == null)
                    {
                        currentRow.Add(columnSlice.CurrentCell);
                    }
                    // If the cell is not going to span through to the next row
                    if (columnSlice.CurrentCell.AllowClose)
                    {
                        columnSlice.BlockProcessor.Close(columnSlice.CurrentCell);
                    }
                }

                // Renew the block parser processor (or reset it for the last row)
                if (columnSlice.BlockProcessor != null && (columnSlice.CurrentCell == null || columnSlice.CurrentCell.AllowClose))
                {
                    columnSlice.BlockProcessor.ReleaseChild();
                    columnSlice.BlockProcessor = isLastRow ? null : processor.CreateChild();
                }

                // Create or erase the cell
                if (isLastRow || columnSlice.CurrentColumnSpan == 0 || (columnSlice.CurrentCell != null && columnSlice.CurrentCell.AllowClose))
                {
                    // We don't need the cell anymore if we have a last row
                    // Or the cell has a columnspan == 0
                    // And the cell does not have to be kept open to span rows
                    columnSlice.CurrentCell = null;
                }
            }

            if (currentRow != null && currentRow.Count > 0)
            {
                gridTable.Add(currentRow);
            }
        }

        private BlockState HandleContents(BlockProcessor processor, GridTableState tableState, Table gridTable)
        {
            var isRowLine = processor.CurrentChar == '+';
            var columns = tableState.ColumnSlices;
            var line = processor.Line;
            SetColumnSpanState(columns, line);
            if (!isRowLine && !CanContinueRow(columns))
            {
                TerminateCurrentRow(processor, tableState, gridTable, false);
            }
            for (int i = 0; i < columns.Count;)
            {
                var columnSlice = columns[i];
                var nextColumnIndex = i + columnSlice.CurrentColumnSpan;
                // If the span is 0, we exit
                if (nextColumnIndex == i)
                {
                    break;
                }
                var nextColumn = nextColumnIndex < columns.Count ? columns[nextColumnIndex] : null;

                var sliceForCell = line;
                sliceForCell.Start = line.Start + columnSlice.Start + 1;
                if (nextColumn != null)
                {
                    sliceForCell.End = line.Start + nextColumn.Start - 1;
                }
                else
                {
                    var columnEnd = columns[columns.Count - 1].End;
                    var columnEndChar = line.PeekCharExtra(columnEnd);
                    // If there is a `|` (or a `+` in the case that we are dealing with a row line 
                    // with spanned contents) exactly at the expected end of the table row, we cut the line
                    // otherwise we allow to have the last cell of a row to be open for longer cell content
                    if (columnEndChar == '|' || (isRowLine && columnEndChar == '+'))
                    {
                        sliceForCell.End = line.Start + columnEnd - 1;
                    }
                    else if (line.PeekCharExtra(line.End) == '|')
                    {
                        sliceForCell.End = line.End - 1;
                    }
                }
                sliceForCell.TrimEnd();

                if (!isRowLine || !IsRowSeperator(sliceForCell))
                {
                    if (columnSlice.CurrentCell == null)
                    {
                        columnSlice.CurrentCell = new TableCell(this)
                        {
                            ColumnSpan = columnSlice.CurrentColumnSpan,
                            ColumnIndex = i
                        };

                        if (columnSlice.BlockProcessor == null)
                        {
                            columnSlice.BlockProcessor = processor.CreateChild();
                        }

                        // Ensure that the BlockParser is aware that the TableCell is the top-level container
                        columnSlice.BlockProcessor.Open(columnSlice.CurrentCell);
                    }
                    // Process the content of the cell
                    columnSlice.BlockProcessor.LineIndex = processor.LineIndex;
                    columnSlice.BlockProcessor.ProcessLine(sliceForCell);
                }

                // Go to next column
                i = nextColumnIndex;
            }
            return BlockState.ContinueDiscard;
        }

        private static void SetColumnSpanState(List<GridTableState.ColumnSlice> columns, StringSlice line)
        {
            foreach (var columnSlice in columns)
            {
                columnSlice.PreviousColumnSpan = columnSlice.CurrentColumnSpan;
                columnSlice.CurrentColumnSpan = 0;
            }
            // | ------------- | ------------ | ---------------------------------------- |
            // Calculate the colspan for the new row
            int columnIndex = -1;
            for (int i = 0; i < columns.Count; i++)
            {
                var columnSlice = columns[i];
                var peek = line.PeekChar(columnSlice.Start);
                if (peek == '|' || peek == '+')
                {
                    columnIndex = i;
                }
                if (columnIndex >= 0)
                {
                    columns[columnIndex].CurrentColumnSpan++;
                }
            }
        }

        private static bool CanContinueRow(List<GridTableState.ColumnSlice> columns)
        {
            foreach (var columnSlice in columns)
            {
                if (columnSlice.PreviousColumnSpan != columnSlice.CurrentColumnSpan)
                {
                    return false;
                }
            }
            return true;
        }

        private static void Undo(BlockProcessor processor, GridTableState tableState, Table gridTable)
        {
            var parser = processor.Parsers.FindExact<ParagraphBlockParser>();
            // Discard the grid table
            var parent = gridTable.Parent;
            processor.Discard(gridTable);
            var paragraphBlock = new ParagraphBlock(parser)
            {
                Lines = tableState.Lines,
            };
            parent.Add(paragraphBlock);
            processor.Open(paragraphBlock);
        }

        public override bool Close(BlockProcessor processor, Block block)
        {
            // Work only on Table, not on TableCell
            var gridTable = block as Table;
            if (gridTable != null)
            {
                var tableState = (GridTableState)block.GetData(typeof(GridTableState));
                TerminateCurrentRow(processor, tableState, gridTable, true);
                if (!gridTable.IsValid())
                {
                    Undo(processor, tableState, gridTable);
                }
            }
            return true;
        }
    }
}
