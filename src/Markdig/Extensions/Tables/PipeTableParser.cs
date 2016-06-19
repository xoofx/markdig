// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Parsers.Inlines;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.Tables
{
    /// <summary>
    /// The inline parser used to transform a <see cref="ParagraphBlock"/> into a <see cref="Table"/> at inline parsing time.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.InlineParser" />
    /// <seealso cref="Markdig.Parsers.IDelimiterProcessor" />
    public class PipeTableParser : InlineParser, IDelimiterProcessor
    {
        private LineBreakInlineParser lineBreakParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeTableParser" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public PipeTableParser(PipeTableOptions options = null)
        {
            OpeningCharacters = new[] { '|', '\n' };
            Options = options ?? new PipeTableOptions();
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        public PipeTableOptions Options { get; }

        public override void Initialize(InlineProcessor processor)
        {
            // We are using the linebreak parser
            lineBreakParser = processor.Parsers.Find<LineBreakInlineParser>() ?? new LineBreakInlineParser();
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            // Only working on Paragraph block
            if (!(processor.Block is ParagraphBlock))
            {
                return false;
            }

            var c = slice.CurrentChar;

            // If we have not a delimiter on the first line of a paragraph, don't bother to continue 
            // tracking other delimiters on following lines
            var tableState = processor.ParserStates[Index] as TableState;
            bool isFirstLineEmpty = false;


            int globalLineIndex;
            int column;
            var position = processor.GetSourcePosition(slice.Start, out globalLineIndex, out column);
            var localLineIndex = globalLineIndex - processor.LineIndex;

            if (tableState == null)
            {

                // A table could be preceded by an empty line or a line containing an inline
                // that has not been added to the stack, so we consider this as a valid 
                // start for a table. Typically, with this, we can have an attributes {...}
                // starting on the first line of a pipe table, even if the first line
                // doesn't have a pipe
                if (processor.Inline != null && (localLineIndex > 0 || c == '\n'))
                {
                    return false;
                }

                if (processor.Inline == null)
                {
                    isFirstLineEmpty = true;
                }
                // Else setup a table processor
                tableState = new TableState();
                processor.ParserStates[Index] = tableState;
            }

            if (c == '\n')
            {
                if (!isFirstLineEmpty && !tableState.LineHasPipe)
                {
                    tableState.IsInvalidTable = true;
                }
                tableState.LineHasPipe = false;
                lineBreakParser.Match(processor, ref slice);
                tableState.LineIndex++;
                if (!isFirstLineEmpty)
                {
                    tableState.ColumnAndLineDelimiters.Add(processor.Inline);
                }
            }
            else
            {
                processor.Inline = new PiprTableDelimiterInline(this)
                {
                    Span = new SourceSpan(position, position),
                    Line = globalLineIndex,
                    Column = column,
                    LocalLineIndex = localLineIndex
                };
                var deltaLine = localLineIndex - tableState.LineIndex;
                if (deltaLine > 0)
                {
                    tableState.IsInvalidTable = true;
                }
                tableState.LineHasPipe = true;
                tableState.LineIndex = localLineIndex;
                slice.NextChar(); // Skip the `|` character

                tableState.ColumnAndLineDelimiters.Add(processor.Inline);
            }


            return true;
        }

        public bool ProcessDelimiters(InlineProcessor state, Inline root, Inline lastChild, int delimiterProcessorIndex, bool isFinalProcessing)
        {
            var container = root as ContainerInline;
            var tableState = state.ParserStates[Index] as TableState;

            // If the delimiters are being processed by an image link, we need to transform them back to literals
            if (!isFinalProcessing)
            {
                if (container == null || tableState == null)
                {
                    return true;
                }

                var child = container.LastChild;
                List<PiprTableDelimiterInline> delimitersToRemove = null;

                while (child != null)
                {
                    var pipeDelimiter = child as PiprTableDelimiterInline;
                    if (pipeDelimiter != null)
                    {
                        if (delimitersToRemove == null)
                        {
                            delimitersToRemove = new List<PiprTableDelimiterInline>();
                        }
                        delimitersToRemove.Add(pipeDelimiter);
                    }

                    if (child == lastChild)
                    {
                        break;
                    }

                    var subContainer = child as ContainerInline;
                    child = subContainer?.LastChild;
                }

                // If we have found any delimiters, transform them to literals
                if (delimitersToRemove != null)
                {
                    bool leftIsDelimiter = false;
                    bool rightIsDelimiter = false;
                    for (int i = 0; i < delimitersToRemove.Count; i++)
                    {
                        var pipeDelimiter = delimitersToRemove[i];
                        var literalInline = new LiteralInline() {Content = new StringSlice("|"), IsClosed = true};
                        pipeDelimiter.ReplaceBy(literalInline);

                        // Check that the pipe that is being removed is not going to make a line without pipe delimiters
                        var tableDelimiters = tableState.ColumnAndLineDelimiters;
                        var delimiterIndex = tableDelimiters.IndexOf(pipeDelimiter);

                        if (i == 0)
                        {
                            leftIsDelimiter = delimiterIndex > 0 && tableDelimiters[delimiterIndex - 1] is PiprTableDelimiterInline;
                        }
                        else if (i + 1 == delimitersToRemove.Count)
                        {
                            rightIsDelimiter = delimiterIndex + 1 < tableDelimiters.Count &&
                                               tableDelimiters[delimiterIndex + 1] is PiprTableDelimiterInline;
                        }
                        // Remove this delimiter from the table processor
                        tableState.ColumnAndLineDelimiters.Remove(pipeDelimiter);
                    }

                    // If we didn't have any delimiter before and after the delimiters we jsut removed, we mark the processor of the current line as no pipe
                    if (!leftIsDelimiter && !rightIsDelimiter)
                    {
                        tableState.LineHasPipe = false;
                    }
                }

                return true;
            }

            // Continue
            if (tableState == null || container == null || tableState.IsInvalidTable || !tableState.LineHasPipe ) //|| tableState.LineIndex != state.LocalLineIndex)
            {
                return true;
            }

            var delimiters = tableState.ColumnAndLineDelimiters;
            delimiters.Add(null);
            var aligns = FindHeaderRow(delimiters);

            if (Options.RequireHeaderSeparator && aligns == null)
            {
                return true;
            }

            var table = new Table();

            // If the current paragraph block has any attributes attached, we can copy them
            var attributes = state.Block.TryGetAttributes();
            if (attributes != null)
            {
                attributes.CopyTo(table.GetAttributes());
            }

            state.BlockNew = table;
            TableRow firstRow = null;
            int maxColumn = 0;
            var cells = tableState.Cells;
            cells.Clear();

            Inline column = container.FirstChild;
            if (column is PiprTableDelimiterInline)
            {
                column = ((PiprTableDelimiterInline)column).FirstChild;
            }

            // TODO: This is not accurate for the table
            table.Span.Start = column.Span.Start;
            table.Span.End = column.Span.End;
            table.Line = column.Line;
            table.Column = column.Column;
            
            int lastIndex = 0;
            for (int i = 0; i < delimiters.Count; i++)
            {
                var delimiter = delimiters[i];
                if (delimiter == null || IsLine(delimiter))
                {
                    var beforeDelimiter = delimiter?.PreviousSibling;
                    var nextLineColumn = delimiter?.NextSibling;

                    TableRow row = null;

                    for (int j = lastIndex; j <= i; j++)
                    {
                        var columnSeparator = delimiters[j];
                        var pipeSeparator = columnSeparator as PiprTableDelimiterInline;

                        var endOfColumn = columnSeparator?.PreviousSibling;

                        // This is the first column empty
                        if (j == lastIndex && pipeSeparator != null && endOfColumn == null)
                        {
                            columnSeparator.Remove();
                            column = pipeSeparator.FirstChild;
                            continue;
                        }

                        if (pipeSeparator != null && IsTrailingColumnDelimiter(pipeSeparator))
                        {
                            TrimEnd(endOfColumn);
                            columnSeparator.Remove();
                            continue;
                        }

                        var cellContainer = new ContainerInline();
                        var item = column;
                        var isFirstItem = true;
                        TrimStart(item);
                        while (item != null && !IsLine(item) && !(item is PiprTableDelimiterInline))
                        {
                            var nextSibling = item.NextSibling;
                            item.Remove();
                            cellContainer.AppendChild(item);
                            if (isFirstItem)
                            {
                                cellContainer.Line = item.Line;
                                cellContainer.Column = item.Column;
                                cellContainer.Span.Start = item.Span.Start;
                                isFirstItem = false;
                            }
                            cellContainer.Span.End = item.Span.End;
                            item = nextSibling;
                        }

                        var tableParagraph = new ParagraphBlock()
                        {
                            Span = cellContainer.Span,
                            Line = cellContainer.Line,
                            Column = cellContainer.Column,
                            Inline = cellContainer
                        };

                        var tableCell = new TableCell()
                        {
                            Span = cellContainer.Span,
                            Line = cellContainer.Line,
                            Column = cellContainer.Column,
                        };

                        tableCell.Add(tableParagraph);

                        if (row == null)
                        {
                            row = new TableRow()
                            {
                                Span = cellContainer.Span,
                                Line = cellContainer.Line,
                                Column = cellContainer.Column,
                            };
                        }
                        row.Add(tableCell);
                        cells.Add(tableCell);

                        // If we have reached the end, we can add remaining delimiters as pure child of the current cell
                        if (row.Count == maxColumn && columnSeparator is PiprTableDelimiterInline)
                        {
                            columnSeparator.Remove();
                            tableParagraph.Inline.AppendChild(columnSeparator);
                            break;
                        }
                        TrimEnd(endOfColumn);

                        //TrimEnd(previousSibling);
                        if (columnSeparator != null)
                        {
                            if (pipeSeparator != null)
                            {
                                column = pipeSeparator.FirstChild;
                            }
                            columnSeparator.Remove();
                        }
                    }

                    if (row != null)
                    {
                        table.Add(row);
                    }

                    TrimEnd(beforeDelimiter);

                    if (delimiter != null)
                    {
                        delimiter.Remove();
                    }

                    if (nextLineColumn != null)
                    {
                        column = nextLineColumn;
                    }

                    if (firstRow == null)
                    {
                        firstRow = row;
                        maxColumn = firstRow.Count;
                    }

                    lastIndex = i + 1;
                }
            }

            // If we have a header row, we can remove it
            if (aligns != null)
            {
                table.RemoveAt(1);
                var tableRow = (TableRow) table[0];
                table.ColumnDefinitions.AddRange(aligns);
                tableRow.IsHeader = true;
            }

            // Perform delimiter processor that are coming after this processor
            var delimiterProcessors = state.Parsers.DelimiterProcessors;
            for (int i = 0; i < delimiterProcessors.Length; i++)
            {
                if (delimiterProcessors[i] == this)
                {
                    foreach (var cell in cells)
                    {
                        var paragraph = (ParagraphBlock) cell[0];

                        state.ProcessDelimiters(i + 1, paragraph.Inline, null, true);
                    }
                    break;
                }
            }
            // Clear cells when we are done
            cells.Clear();

            // We don't want to continue procesing delimiters, as we are already processing them here
            return false;
        }

        private static bool ParseHeaderString(Inline inline, out TableColumnAlign align)
        {
            align = 0;
            var literal = inline as LiteralInline;
            if (literal == null)
            {
                return false;
            }

            // Work on a copy of the slice
            var line = literal.Content;
            if (TableHelper.ParseColumnHeader(ref line, '-', out align))
            {
                if (line.CurrentChar != '\0')
                {
                    return false;
                }
                return true;
            }

            return false;
        }

        private List<TableColumnDefinition> FindHeaderRow(List<Inline> delimiters) 
        {
            bool isValidRow = false;
            List<TableColumnDefinition> aligns = null;
            for (int i = 0; i < delimiters.Count; i++)
            {
                if (delimiters[i] != null && IsLine(delimiters[i]))
                {
                    // The last delimiter is always null,
                    for (int j = i + 1; j < delimiters.Count - 1; j++)
                    {
                        var delimiter = delimiters[j];
                        var nextDelimiter = delimiters[j + 1];

                        var columnDelimiter = delimiter as PiprTableDelimiterInline;
                        if (j == i + 1 && IsStartOfLineColumnDelimiter(columnDelimiter))
                        {
                            continue;
                        }

                        // Check the left side of a `|` delimiter
                        TableColumnAlign align = TableColumnAlign.Left;
                        if (delimiter.PreviousSibling != null && !ParseHeaderString(delimiter.PreviousSibling, out align))
                        {
                            break;
                        }

                        // Create aligns until we may have a header row
                        if (aligns == null)
                        {
                            aligns = new List<TableColumnDefinition>();
                        }
                        aligns.Add(new TableColumnDefinition() { Alignment =  align });

                        // If this is the last delimiter, we need to check the right side of the `|` delimiter
                        if (nextDelimiter == null)
                        {
                            var nextSibling = columnDelimiter != null
                                ? columnDelimiter.FirstChild
                                : delimiter.NextSibling;

                            if (!ParseHeaderString(nextSibling, out align))
                            {
                                break;
                            }

                            isValidRow = true;
                            aligns.Add(new TableColumnDefinition() { Alignment = align });
                            break;
                        }

                        // If we are on a Line delimiter, exit
                        if (IsLine(delimiter))
                        {
                            isValidRow = true;
                            break;
                        }
                    }
                    break;
                }
            }

            return isValidRow ? aligns : null;
        }

        private static bool IsLine(Inline inline)
        {
            return inline is LineBreakInline;
        }

        private static bool IsStartOfLineColumnDelimiter(Inline inline)
        {
            if (inline == null)
            {
                return false;
            }

            var previous = inline.PreviousSibling;
            if (previous == null)
            {
                return true;
            }
            var literal = previous as LiteralInline;
            if (literal != null)
            {
                if (!literal.Content.IsEmptyOrWhitespace())
                {
                    return false;
                }
                previous = previous.PreviousSibling;
            }
            return previous == null || IsLine(previous);
        }

        private static bool IsTrailingColumnDelimiter(PiprTableDelimiterInline inline)
        {
            var child = inline.FirstChild;
            var literal = child as LiteralInline;
            if (literal != null)
            {
                if (!literal.Content.IsEmptyOrWhitespace())
                {
                    return false;
                }
                child = child.NextSibling;
            }
            return child == null || IsLine(child);
        }

        private static void TrimStart(Inline inline)
        {
            while (inline is ContainerInline && !(inline is DelimiterInline))
            {
                inline = ((ContainerInline)inline).FirstChild;
            }
            var literal = inline as LiteralInline;
            if (literal != null)
            {
                literal.Content.TrimStart();
            }
        }

        private static void TrimEnd(Inline inline)
        {
            var literal = inline as LiteralInline;
            if (literal != null)
            {
                literal.Content.TrimEnd();
            }
        }

        private class TableState
        {
            public TableState()
            {
                ColumnAndLineDelimiters = new List<Inline>();
                Cells = new List<TableCell>();
            }

            public bool IsInvalidTable { get; set; }

            public bool LineHasPipe { get; set; }

            public int LineIndex { get; set; }

            public List<Inline> ColumnAndLineDelimiters { get; }

            public List<TableCell> Cells { get; }
        }
    }
}