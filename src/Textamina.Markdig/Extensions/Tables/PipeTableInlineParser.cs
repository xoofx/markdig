using System.Collections.Generic;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Tables
{
    public class PipeTableInlineParser : InlineParser, IDelimiterProcessor
    {
        public PipeTableInlineParser()
        {
            OpeningCharacters = new[] { '|' };
        }

        public override bool Match(InlineParserState state, ref StringSlice slice)
        {
            // If we have not a delimiter on the first line of a paragraph, don't bother to continue 
            // tracking other delimiters on following lines
            if (state.ParserStates[Index] == null)
            {
                if (state.LocalLineIndex > 0)
                {
                    return false;
                }
                // Else setup a table state
                state.ParserStates[Index] = new TableState();
            }

            state.Inline = new PiprTableDelimiterInline(this) { LineIndex = state.LocalLineIndex };
            slice.NextChar(); // Skip the `|` character

            return true;
        }

        public bool ProcessDelimiters(InlineParserState state, Inline root, Inline lastChild, int delimiterProcessorIndex)
        {
            // Continue
            var container = root as ContainerInline;
            var tableState = state.ParserStates[Index] as TableState;
            if (tableState == null || container == null)
            {
                return true;
            }

            var lastLineIndex = state.LocalLineIndex;

            var child = container.FirstChild;
            var lines = tableState.Lines;
            int lineIndex = 0;
            var previousLine = -1;
            bool lineHasAPipe = false;
            while (child != null)
            {
                if (lineIndex != previousLine)
                {
                    if (previousLine >= 0 && !lineHasAPipe)
                    {
                        return true;
                    }
                    previousLine = lineIndex;
                    lines.Add(child);
                    lineHasAPipe = false;
                }

                if (IsLine(child))
                {
                    previousLine = lineIndex;
                    lineIndex++;
                }
                else if (child is PiprTableDelimiterInline && !lineHasAPipe)
                {
                    lineHasAPipe = true;
                }

                if (child is ContainerInline)
                {
                    child = ((ContainerInline) child).FirstChild;
                }
                else
                {
                    child = child.NextSibling;
                }
            }

            // The last line index must be equal to the last line of the leaf block
            if (!lineHasAPipe || lineIndex != lastLineIndex)
            {
                return true;
            }

            var table = new TableBlock();
            state.BlockNew = table;
            TableRowBlock firstRow = null;
            int columnCount = 0;
            int maxColumn = 0;
            var cells = tableState.Cells;
            cells.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                var column = lines[i];

                var row = new TableRowBlock {Parent = table};
                table.Children.Add(row);

                if (column is PiprTableDelimiterInline)
                {
                    column = ((PiprTableDelimiterInline)column).FirstChild;
                }

                ContainerInline previousColumn = null;
                Inline lastColumn = null;
                while (true)
                {
                    if (maxColumn > 0 && row.Children.Count >= maxColumn)
                    {
                        lastColumn = null;
                        column.Remove();
                        TrimEnd(column);
                        previousColumn.AppendChild(column);
                        break;
                    }
                    else
                    {
                        if (lastColumn != null)
                        {
                            TrimEnd(lastColumn);
                        }
                        var cellContainer = new ContainerInline();
                        TrimStart(column);
                        Inline nextColumn;
                        CopyCellDown(column, cellContainer, out lastColumn, out nextColumn);

                        var tableCell = new TableCellBlock { Inline = cellContainer, Parent = row };
                        cells.Add(tableCell);
                        row.Children.Add(tableCell);

                        if (nextColumn is PiprTableDelimiterInline &&
                            IsTrailingColumnDelimiter((PiprTableDelimiterInline) nextColumn))
                        {
                            TrimEnd(column);
                            nextColumn.Remove();
                            column = null;
                        }
                        else
                        {
                            column = nextColumn;
                            previousColumn = cellContainer;

                        }
                    }

                    if (column == null || IsLine(column))
                    {
                        break;
                    }
                }

                if (lastColumn != null)
                {
                    TrimEnd(lastColumn);
                }

                if (i == 0)
                {
                    maxColumn = cells.Count;
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
                    continue;
                }

                var tableCell = (TableCellBlock) row.LastChild;
                TrimEnd(tableCell.Inline.LastChild);
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

        private static bool CopyCellDown(Inline fromElement, ContainerInline dest, out Inline last, out Inline next)
        {
            next = null;
            var container = fromElement as ContainerInline;
            Inline lastChild = null;
            Inline child;
            if (container != null)
            {
                lastChild = container.LastChild;
                child = container.FirstChild;
            }
            else
            {
                child = fromElement;
            }

            bool found = false;
            last = null;
            while (child != null)
            {
                var nextSibling = child.NextSibling;
                var isLine = IsLine(child);
                if (isLine || child is PiprTableDelimiterInline)
                {
                    child.Remove();
                    next = child;
                    found = true;
                    break;
                }

                var childContainer = child as ContainerInline;
                if (childContainer != null)
                {
                    var newParent = new ContainerInline();
                    dest.AppendChild(newParent);
                    if (CopyCellDown(childContainer, newParent, out last, out next))
                    {
                        found = true;
                        break;
                    }
                }
                else
                {
                    child.Remove();
                    dest.AppendChild(child);
                }

                last = child;
                child = nextSibling;
            }

            // If we have removed all children, the container can be removed
            if (container != null)
            {
                if (child == lastChild || child == null)
                {
                    fromElement.Remove();
                }
            }
            return found;
        }

        private static void TrimStart(Inline inline)
        {
            while (inline is ContainerInline)
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
            while (inline is ContainerInline)
            {
                inline = ((ContainerInline)inline).LastChild;
            }

            if (inline != null)
            {
                var previous = inline.PreviousSibling;
                if (IsLine(inline))
                {
                    inline.Remove();
                    inline = previous;
                }

                var literal = inline as LiteralInline;
                if (literal != null)
                {
                    literal.Content.TrimEnd();
                }
            }
        }

        private class TableState
        {
            public TableState()
            {
                Lines = new List<Inline>();
                Cells = new List<TableCellBlock>();
            }
            public List<Inline> Lines { get; }

            public List<TableCellBlock> Cells { get; }
        }
    }
}