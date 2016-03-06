// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.Tables
{
    public class GridTableBlockParser : BlockParser
    {
        public GridTableBlockParser()
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

            // A grid table line:
            // + ------------- + ------------ + ---------------------------------------- +
            // Spaces are optional

            GridTableParserState tableParserState = null;
            var c = line.CurrentChar;
            while (true)
            {
                if (c == '+')
                {
                    var startCharacter = line.Start;
                    line.NextChar();
                    if (line.IsEmptyOrWhitespace())
                    {
                        if (tableParserState == null)
                        {
                            return BlockState.None;
                        }
                        break;
                    }

                    TableColumnAlign align;
                    if (TableHelper.ParseColumnHeader(ref line, '-', out align))
                    {
                        if (tableParserState == null)
                        {
                            tableParserState = new GridTableParserState()
                            {
                                StartColumn = state.Column,
                                ExpectRow = true,
                            };
                        }
                        tableParserState.AddColumn(startCharacter, line.Start, align);

                        c = line.CurrentChar;
                        continue;
                    }
                }

                // If we have any other characters, this is an invalid line
                return BlockState.None;
            }

            // Store the line (if we need later to build a ParagraphBlock because the GridTable was in fact invalid)
            tableParserState.AddLine(ref state.Line);

            // Create the grid table
            state.NewBlocks.Push(new GridTableBlock() { State = tableParserState });

            return BlockState.ContinueDiscard;
        }

        public override BlockState TryContinue(BlockParserState state, Block block)
        {
            var gridTable = (GridTableBlock) block;
            var tableState = gridTable.State;

            if (state.Start != tableState.StartColumn)
            {
                TerminateLastRow(state, tableState, gridTable, true);
                return BlockState.None;
            }

            if (state.CurrentChar == '+')
            {
                // TODO: Parse row separator
            }
            else if (state.CurrentChar == '|')
            {
                var line = state.Line;

                // 
                // | ------------- | ------------ | ---------------------------------------- |
                // Spaces are optional

                // Initialize sub parsers
                var columns = tableState.ColumnSlices;
                foreach (var columnSlice in columns)
                {
                    if (columnSlice.BlockParserState == null)
                    {
                        columnSlice.BlockParserState = state.CreateChild();
                    }
                    columnSlice.PreviousColumnSpan = columnSlice.CurrentColumnSpan;
                    columnSlice.CurrentColumnSpan = 0;
                }


                int columnIndex = -1;
                foreach (var columnSlice in columns)
                {
                    if (line.PeekCharExtra(columnSlice.ColumnStart) == '|')
                    {
                        columnIndex++;
                    }
                    // TODO: Check error
                    columns[columnIndex].CurrentColumnSpan++;
                }

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
                    var nextColumn = nextColumnIndex < columns.Count ? columns[nextColumnIndex] : null;

                    var sliceForCell = line;
                    sliceForCell.Start = column.ColumnStart + 1;
                    if (nextColumn != null)
                    {
                        sliceForCell.End = nextColumn.ColumnStart - 1;
                    }

                    // Process the content of the cell
                    column.BlockParserState.ProcessLine(sliceForCell);

                    // Go to next column
                    i = nextColumnIndex;
                }

                return BlockState.ContinueDiscard;
            }

            TerminateLastRow(state, tableState, gridTable, true);
            return BlockState.None;
        }

        private void TerminateLastRow(BlockParserState state, GridTableParserState tableState, TableBlock gridTable, bool isFinal)
        {
            var columns = tableState.ColumnSlices;
            TableRowBlock currentRow = null;
            foreach (var columnSlice in columns)
            {
                if (columnSlice.CurrentCell != null)
                {
                    if (currentRow == null)
                    {
                        currentRow = new TableRowBlock();
                    }
                    currentRow.Children.Add(columnSlice.CurrentCell);
                    columnSlice.BlockParserState.Close(columnSlice.CurrentCell);

                    // Renew the block parser state
                    columnSlice.BlockParserState.ReleaseChild();
                    columnSlice.BlockParserState = !isFinal ? state.CreateChild() : null;

                    // Create new cell
                    if (!isFinal && columnSlice.CurrentColumnSpan > 0)
                    {
                        columnSlice.CurrentCell = new TableCellBlock()
                        {
                            ColumnSpan = columnSlice.CurrentColumnSpan
                        };

                        columnSlice.BlockParserState.Open(columnSlice.CurrentCell);
                    }
                    else
                    {
                        columnSlice.CurrentCell = null;
                    }
                }
            }

            if (currentRow != null)
            {
                gridTable.Children.Add(currentRow);
            }
        }
    }
}