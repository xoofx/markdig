using System.Collections.Generic;
using System.Reflection;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Parsers.Inlines;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Tables
{
    public class PipeTableInlineParser : InlineParser, IDelimiterProcessor
    {
        private LineBreakInlineParser lineBreakParser;
        public PipeTableInlineParser()
        {
            OpeningCharacters = new[] { '|', '\n' };
        }


        public override void Initialize(InlineParserState state)
        {
            lineBreakParser = state.Parsers.Find<LineBreakInlineParser>() ?? new LineBreakInlineParser();
        }

        public override bool Match(InlineParserState state, ref StringSlice slice)
        {
            // Only working on Paragraph block
            if (!(state.Block is ParagraphBlock))
            {
                return false;
            }

            var c = slice.CurrentChar;

            // If we have not a delimiter on the first line of a paragraph, don't bother to continue 
            // tracking other delimiters on following lines
            var tableState = state.ParserStates[Index] as TableState;
            if (tableState == null)
            {
                if (state.LocalLineIndex > 0 || c == '\n')
                {
                    return false;
                }
                // Else setup a table state
                tableState = new TableState();
                state.ParserStates[Index] = tableState;
            }

            if (c == '\n')
            {
                if (!tableState.LineHasPipe)
                {
                    tableState.IsInvalidTable = true;
                }
                tableState.LineHasPipe = false;
                lineBreakParser.Match(state, ref slice);
                tableState.LineIndex++;
            }
            else
            {
                state.Inline = new PiprTableDelimiterInline(this) { LineIndex = state.LocalLineIndex };
                var deltaLine = state.LocalLineIndex - tableState.LineIndex;
                if (deltaLine > 0)
                {
                    tableState.IsInvalidTable = true;
                }
                tableState.LineHasPipe = true;
                tableState.LineIndex = state.LocalLineIndex;
                slice.NextChar(); // Skip the `|` character
            }

            tableState.ColumnAndLineDelimiters.Add(state.Inline);

            return true;
        }

        public bool ProcessDelimiters(InlineParserState state, Inline root, Inline lastChild, int delimiterProcessorIndex)
        {
            // Continue
            var container = root as ContainerInline;
            var tableState = state.ParserStates[Index] as TableState;
            if (tableState == null || container == null || tableState.IsInvalidTable || !tableState.LineHasPipe || tableState.LineIndex != state.LocalLineIndex)
            {
                return true;
            }

            var table = new TableBlock();
            state.BlockNew = table;
            TableRowBlock firstRow = null;
            int maxColumn = 0;
            var cells = tableState.Cells;
            cells.Clear();
            TableRowBlock currentRow = null;

            Inline column = container.FirstChild;
            if (column is PiprTableDelimiterInline)
            {
                column = ((PiprTableDelimiterInline) column).FirstChild;
            }

            var delimiters = tableState.ColumnAndLineDelimiters;
            delimiters.Add(null);
            int lastIndex = 0;
            for (int i = 0; i < delimiters.Count; i++)
            {
                var delimiter = delimiters[i];
                if (delimiter == null || IsLine(delimiter))
                {
                    var beforeEndOfLine = delimiter?.PreviousSibling;
                    var nextColumn = delimiter?.NextSibling;

                    var row = new TableRowBlock { Parent = table };
                    table.Children.Add(row);

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

                        var columnContainer = new ContainerInline();
                        var item = column;
                        TrimStart(item);
                        while (item != null && !IsLine(item) && !(item is PiprTableDelimiterInline))
                        {
                            var nextSibling = item.NextSibling;
                            item.Remove();
                            columnContainer.AppendChild(item);
                            item = nextSibling;
                        }

                        var tableCell = new TableCellBlock { Inline = columnContainer, Parent = row };
                        row.Children.Add(tableCell);
                        cells.Add(tableCell);

                        // If we have reached the end, we can add remaining delimiters as pure child of the current cell
                        if (row.Children.Count == maxColumn && columnSeparator is PiprTableDelimiterInline)
                        {
                            columnSeparator.Remove();
                            tableCell.Inline.AppendChild(columnSeparator);
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

                    TrimEnd(beforeEndOfLine);

                    if (delimiter != null)
                    {
                        delimiter.Remove();
                    }

                    if (nextColumn != null)
                    {
                        column = nextColumn;
                    }

                    if (firstRow == null)
                    {
                        firstRow = row;
                        maxColumn = firstRow.Children.Count;
                    }

                    lastIndex = i + 1;
                }
            }

            TableRowBlock previousRow = null;
            for (int rowIndex = 0; rowIndex < table.Children.Count; rowIndex++)
            {
                var rowObj = table.Children[rowIndex];
                var row = (TableRowBlock) rowObj;

                List<TableColumnAlign> aligns;
                if (rowIndex == 1 && TryParseRowHeaderSeparator(row, out aligns))
                {
                    previousRow.IsHeader = true;
                    previousRow.ColumnAlignments = aligns;
                    table.Children.RemoveAt(rowIndex);
                    rowIndex--;
                    break;
                }
                previousRow = row;
            }

            // Perform delimiter processor that are coming after this processor
            var delimiterProcessors = state.Parsers.DelimiterProcessors;
            for (int i = 0; i < delimiterProcessors.Length; i++)
            {
                if (delimiterProcessors[i] == this)
                {
                    foreach (var cell in cells)
                    {
                        state.ProcessDelimiters(i + 1, cell.Inline);
                    }
                    break;
                }
            }
            // Clear cells when we are done
            cells.Clear();

            // We don't want to continue procesing delimiters, as we are already processing them here
            return false;
        }

        private bool TryParseRowHeaderSeparator(TableRowBlock row, out List<TableColumnAlign> aligns)
        {
            aligns = null;
            foreach (var cellObj in row.Children)
            {
                var cell = (TableCellBlock) cellObj;
                var literal = cell.Inline.FirstChild as LiteralInline;
                if (literal == null)
                {
                    return false;
                }

                if (literal.NextSibling != null && !(literal.NextSibling is PiprTableDelimiterInline))
                {
                    return false;
                }

                // Work on a copy of the slice
                var line = literal.Content;
                line.Trim();
                var c = line.CurrentChar;
                bool hasLeft = false;
                bool hasRight = false;
                if (c == ':')
                {
                    hasLeft = true;
                    c = line.NextChar();
                }

                int count = 0;
                while (c == '-')
                {
                    c = line.NextChar();
                    count++;
                }

                if (count == 0)
                {
                    return false;
                }

                if (c == ':')
                {
                    hasRight = true;
                    c = line.NextChar();
                }

                if (c != '\0')
                {
                    return false;
                }

                if (aligns == null)
                {
                    aligns = new List<TableColumnAlign>();
                }

                aligns.Add(hasLeft && hasRight ? TableColumnAlign.Center :  hasRight ? TableColumnAlign.Right : TableColumnAlign.Left);
            }

            return true;
        }

        private static bool IsLine(Inline inline)
        {
            return inline is SoftlineBreakInline || inline is HardlineBreakInline;
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
                Cells = new List<TableCellBlock>();
            }

            public bool IsInvalidTable { get; set; }

            public bool LineHasPipe { get; set; }

            public int LineIndex { get; set; }

            public List<Inline> ColumnAndLineDelimiters { get; }

            public List<TableCellBlock> Cells { get; }
        }
    }
}