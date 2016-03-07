// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Collections.Generic;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.Tables
{
    public class GridTableParser : BlockParser
    {
        public GridTableParser()
        {
            OpeningCharacters = new[] {'+'};
        }

        public override BlockState TryOpen(BlockParserState state)
        {
            // A grid table cannot start more than an indent
            if (state.IsCodeIndent)
            {
                return BlockState.None;
            }

            var line = state.Line;

            // A grid table must start with a line like this:
            // + ------------- + ------------ + ---------------------------------------- +
            // Spaces are optional

            GridTableState tableState = null;
            var c = line.CurrentChar;
            while (true)
            {
                if (c == '+')
                {
                    var startCharacter = line.Start;
                    line.NextChar();
                    if (line.IsEmptyOrWhitespace())
                    {
                        if (tableState == null)
                        {
                            return BlockState.None;
                        }
                        break;
                    }

                    TableColumnAlign align;
                    if (TableHelper.ParseColumnHeader(ref line, '-', out align))
                    {
                        if (tableState == null)
                        {
                            tableState = new GridTableState()
                            {
                                Start = state.Column,
                                ExpectRow = true,
                            };
                        }
                        tableState.AddColumn(startCharacter, line.Start - 1, align);

                        c = line.CurrentChar;
                        continue;
                    }
                }

                // If we have any other characters, this is an invalid line
                return BlockState.None;
            }

            // Store the line (if we need later to build a ParagraphBlock because the GridTable was in fact invalid)
            tableState.AddLine(ref state.Line);

            // Create the grid table
            var table = new Table(this);

            table.SetData(typeof(GridTableState), tableState);


            // Calculate the total width of all columns
            int totalWidth = 0;
            foreach (var columnSlice in tableState.ColumnSlices)
            {
                totalWidth += columnSlice.End - columnSlice.Start + 1;
            }

            // Store the column width and alignment
            foreach (var columnSlice in tableState.ColumnSlices)
            {
                var columnDefinition = new TableColumnDefinition
                {
                    // Column width proportional to the total width
                    Width = (float)(columnSlice.End - columnSlice.Start + 1) * 100.0f / totalWidth,
                    Alignment = columnSlice.Align,
                };
                table.ColumnDefinitions.Add(columnDefinition);
            }

            state.NewBlocks.Push(table);

            return BlockState.ContinueDiscard;
        }

        public override BlockState TryContinue(BlockParserState state, Block block)
        {
            var gridTable = (Table) block;
            var tableState = (GridTableState)block.GetData(typeof(GridTableState));

            // We expect to start at the same 
            if (state.Start == tableState.Start)
            {
                var columns = tableState.ColumnSlices;

                foreach (var columnSlice in columns)
                {
                    columnSlice.PreviousColumnSpan = columnSlice.CurrentColumnSpan;
                    columnSlice.CurrentColumnSpan = 0;
                }

                if (state.CurrentChar == '+')
                {
                    var result = ParseRowSeparator(state, tableState, gridTable);
                    if (result != BlockState.None)
                    {
                        return result;
                    }
                }
                else if (state.CurrentChar == '|')
                {
                    var line = state.Line;

                    // | ------------- | ------------ | ---------------------------------------- |
                    // Calculate the colspan for the new row
                    int columnIndex = -1;
                    foreach (var columnSlice in columns)
                    {
                        if (line.PeekCharExtra(columnSlice.Start) == '|')
                        {
                            columnIndex++;
                        }
                        if (columnIndex >= 0)
                        {
                            columns[columnIndex].CurrentColumnSpan++;
                        }
                    }

                    // Check if the colspan of the current row is the same than the previous row
                    bool continueRow = true;
                    foreach (var columnSlice in columns)
                    {
                        if (columnSlice.PreviousColumnSpan != columnSlice.CurrentColumnSpan)
                        {
                            continueRow = false;
                            break;
                        }
                    }

                    // If the current row doesn't continue the previous row (col span are different)
                    // Close the previous row
                    if (!continueRow)
                    {
                        TerminateLastRow(state, tableState, gridTable, false);
                    }

                    for (int i = 0; i < columns.Count;)
                    {
                        var column = columns[i];
                        var nextColumnIndex = i + column.CurrentColumnSpan;
                        // If the span is 0, we exit
                        if (nextColumnIndex == i)
                        {
                            break;
                        }
                        var nextColumn = nextColumnIndex < columns.Count ? columns[nextColumnIndex] : null;

                        var sliceForCell = line;
                        sliceForCell.Start = column.Start + 1;
                        if (nextColumn != null)
                        {
                            sliceForCell.End = nextColumn.Start - 1;
                        }
                        else
                        {
                            var columnEnd = columns[columns.Count - 1].End;
                            // If there is a `|` exactly at the expected end of the table row, we cut the line
                            // otherwise we allow to have the last cell of a row to be open for longer cell content
                            if (line.PeekCharExtra(columnEnd + 1) == '|')
                            {
                                sliceForCell.End = columnEnd;
                            }
                        }

                        // Process the content of the cell
                        column.BlockParserState.LineIndex = state.LineIndex;
                        column.BlockParserState.ProcessLine(sliceForCell);

                        // Go to next column
                        i = nextColumnIndex;
                    }

                    return BlockState.ContinueDiscard;
                }
            }

            TerminateLastRow(state, tableState, gridTable, true);

            // If we don't have a row, it means that only the header was valid
            // So we need to remove the grid table, and create a ParagraphBlock
            // with the 2 slices 
            if (gridTable.Count == 0)
            {
                var parser = state.Parsers.Find<ParagraphBlockParser>();
                // Discard the grid table
                var parent = gridTable.Parent;
                state.Discard(gridTable);
                var paragraphBlock = new ParagraphBlock(parser)
                {
                    Lines = tableState.Lines,
                };
                parent.Add(paragraphBlock);
                state.Open(paragraphBlock);
            }

            return BlockState.Break;
        }


        private BlockState ParseRowSeparator(BlockParserState state, GridTableState tableState, Table gridTable)
        {
            // A grid table must start with a line like this:
            // + ------------- + ------------ + ---------------------------------------- +
            // Spaces are optional

            var line = state.Line;
            var c = line.CurrentChar;
            bool isFirst = true;
            var delimiterChar = '\0';
            while (true)
            {
                if (c == '+')
                {
                    line.NextChar();
                    if (line.IsEmptyOrWhitespace())
                    {
                        if (isFirst)
                        {
                            return BlockState.None;
                        }
                        break;
                    }

                    TableColumnAlign align;
                    if (TableHelper.ParseColumnHeaderDetect(ref line, ref delimiterChar, out align))
                    {
                        isFirst = false;
                        c = line.CurrentChar;
                        continue;
                    }
                }

                // If we have any other characters, this is an invalid line
                return BlockState.None;
            }

            // If we have an header row
            var isHeader = delimiterChar == '=';

            // Terminate the current row
            TerminateLastRow(state, tableState, gridTable, false);

            // If we had a header row separator, we can mark all rows since last row separator 
            // to be header rows
            if (isHeader)
            {
                for (int i = tableState.StartRowGroup; i < gridTable.Count; i++)
                {
                    var row = (TableRow) gridTable[i];
                    row.IsHeader = true;
                }
            }

            // Makr the next start row group continue on the next row
            tableState.StartRowGroup = gridTable.Count;

            // We don't keep the line
            return BlockState.ContinueDiscard;
        }

        private void TerminateLastRow(BlockParserState state, GridTableState tableState, Table gridTable, bool isLastRow)
        {
            var columns = tableState.ColumnSlices;
            TableRow currentRow = null;
            foreach (var columnSlice in columns)
            {
                if (columnSlice.CurrentCell != null)
                {
                    if (currentRow == null)
                    {
                        currentRow = new TableRow();
                    }
                    currentRow.Add(columnSlice.CurrentCell);
                    columnSlice.BlockParserState.Close(columnSlice.CurrentCell);
                }

                // Renew the block parser state (or reset it for the last row)
                if (columnSlice.BlockParserState != null)
                {
                    columnSlice.BlockParserState.ReleaseChild();
                    columnSlice.BlockParserState = isLastRow ? null : state.CreateChild();
                }

                // Create or erase the cell
                if (isLastRow || columnSlice.CurrentColumnSpan == 0)
                {
                    // We don't need the cell anymore if we have a last row
                    // Or the cell has a columnspan == 0
                    columnSlice.CurrentCell = null;
                }
                else
                {
                    // Else we can create a new cell
                    columnSlice.CurrentCell = new TableCell(this)
                    {
                        ColumnSpan = columnSlice.CurrentColumnSpan
                    };

                    if (columnSlice.BlockParserState == null)
                    {
                        columnSlice.BlockParserState = state.CreateChild();
                    }

                    // Ensure that the BlockParser is aware that the TableCell is the top-level container
                    columnSlice.BlockParserState.Open(columnSlice.CurrentCell);
                }
            }

            if (currentRow != null)
            {
                gridTable.Add(currentRow);
            }
        }
    }
}