using System.Collections.Generic;
using System.Linq.Expressions;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions
{
    public class PipeTableInlineParser : InlineParser, IDelimiterProcessor
    {
        public PipeTableInlineParser()
        {
            OpeningCharacters = new[] { '|' };
        }

        public override bool Match(InlineParserState state, ref StringSlice slice)
        {
            state.Inline = new PiprTableDelimiterInline(this) {LineIndex = state.LocalLineIndex};

            // Store that we have at least one delimiter
            // TODO: Use cache for this kind of obejcts
            state.ParserStates[Index] = new TableState();

            slice.NextChar();

            return true;
        }

        public bool ProcessDelimiters(InlineParserState state, Inline root, Inline lastChild)
        {
            // Continue
            var container = root as ContainerInline;
            var tableState = state.ParserStates[Index] as TableState;
            if (tableState == null || container == null)
            {
                return true;
            }

            var lastLineIndex = state.LocalLineIndex;

            var child = container;
            var delimiters = tableState.Delimiters;
            int lineIndex = -1;
            var previousLine = -1;
            while (child != null)
            {
                var tableDelimiter = child as PiprTableDelimiterInline;
                if (tableDelimiter != null)
                {
                    if (lineIndex < 0)
                    {
                        lineIndex = tableDelimiter.LineIndex;
                        // Table delimiter must start on first line
                        if (lineIndex > 0)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // We are requiring at least one delimiter per line
                        // We should not skip any lines between rows
                        var deltaLine = tableDelimiter.LineIndex - lineIndex;
                        if (deltaLine > 1)
                        {
                            return true;
                        }
                        lineIndex = tableDelimiter.LineIndex;
                    }

                    // We have a new row starting with a |, so we are going to track back the 
                    if (previousLine != lineIndex)
                    {
                        if (HasPreviousColumn(tableDelimiter))
                        {
                            var beginOfLine = FindBeginOfPreviousLine(tableDelimiter);
                            delimiters.Add(beginOfLine);
                        }
                    }

                    delimiters.Add(tableDelimiter);
                }
                child = child.LastChild as ContainerInline;
                previousLine = lineIndex;
            }

            // The last line index must be equal to the last line of the leaf block
            if (lineIndex != lastLineIndex)
            {
                return true;
            }

            lineIndex = -1;
            var table = new TableBlock();
            TableRowBlock currentRow = null;
            state.BlockNew = table;
            TableRowBlock firstRow = null;
            int columnCount = 0;
            int maxColumn = 0;
            for (int i = 0; i < delimiters.Count; i++)
            {
                var delimiter = delimiters[i] as PiprTableDelimiterInline;
                if (delimiter == null)
                {
                    continue;
                }

                bool startNewRow = false;
                if (delimiter.LineIndex != lineIndex)
                {
                    if (firstRow == null)
                    {
                        firstRow = currentRow;
                        maxColumn = columnCount;
                    }
                    currentRow = new TableRowBlock {Parent = table};
                    table.Children.Add(currentRow);
                    columnCount = 0;
                    //startNewRow = true;
                }

                if (maxColumn > 0 && columnCount == maxColumn)
                {
                    delimiter.Remove();
                    ((ContainerInline)((TableCellBlock)currentRow.LastChild).Inline).AppendChild(delimiter);
                    continue;
                }

                // If a '|' is the first in a line not starting at the begining of a line, we need
                // to add the previous content as an implicit column
                if (HasPreviousColumn(delimiter))
                {
                    var cellContainer = new ContainerInline();
                    var tableCell = new TableCellBlock { Inline = cellContainer, Parent = currentRow };
                    currentRow.Children.Add(tableCell);
                    var previousInline = delimiters[i - 1];
                    CopyCellDown(previousInline, cellContainer);
                    columnCount++;
                }

                // otherwise we have a regular cell
                {
                    var cellContainer = new ContainerInline();
                    var tableCell = new TableCellBlock { Inline = cellContainer, Parent = currentRow };
                    currentRow.Children.Add(tableCell);

                    var literal = delimiter.FirstChild as LiteralInline;
                    if (literal != null)
                    {
                        literal.Content.TrimStart();
                    }

                    CopyCellDown(delimiter, cellContainer);
                    columnCount++;
                }

                lineIndex = delimiter.LineIndex;
            }


            TableRowBlock previousRow = null;
            for (int rowIndex = 0; rowIndex < table.Children.Count; rowIndex++)
            {
                var rowObj = table.Children[rowIndex];
                var row = (TableRowBlock) rowObj;

                List<TableColumnAlignType> aligns;
                if (rowIndex == 1 && TryParseRowHeaderSeparator(row, out aligns))
                {
                    previousRow.IsHeader = true;
                    previousRow.ColumnAlignments = aligns;
                    table.Children.RemoveAt(rowIndex);
                    rowIndex--;
                    continue;
                }

                var tableCell = (TableCellBlock) row.LastChild;
                TrimLiteral(tableCell.Inline.LastChild);
                previousRow = row;
            }

            return false;
        }

        private bool TryParseRowHeaderSeparator(TableRowBlock row, out List<TableColumnAlignType> aligns)
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
                    aligns = new List<TableColumnAlignType>();
                }

                aligns.Add(hasLeft && hasRight ? TableColumnAlignType.Center :  hasRight ? TableColumnAlignType.Right : TableColumnAlignType.Left);
            }

            return true;
        }



        private static bool HasPreviousColumn(Inline delimiter)
        {
            return delimiter.PreviousSibling != null &&
                   (!(delimiter.PreviousSibling is SoftlineBreakInline) &&
                    !(delimiter.PreviousSibling is HardlineBreakInline));
        }

        private static bool CopyCellDown(Inline fromElement, ContainerInline dest)
        {
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
            Inline previousSibling = null;
            while (child != null)
            {
                var nextSibling = child.NextSibling;
                if (child is SoftlineBreakInline || child is HardlineBreakInline || child is PiprTableDelimiterInline)
                {
                    TrimLiteral(previousSibling);
                    found = true;
                    break;
                }

                var childContainer = child as ContainerInline;
                if (childContainer != null)
                {
                    var newParent = new ContainerInline();
                    dest.AppendChild(newParent);
                    if (CopyCellDown(childContainer, newParent))
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

                previousSibling = child;
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

        private static void TrimLiteral(Inline inline)
        {
            while (inline is ContainerInline)
            {
                inline = ((ContainerInline) inline).LastChild;
            }

            var previousInline = inline.PreviousSibling;
            while (inline is HardlineBreakInline || inline is SoftlineBreakInline)
            {
                inline.Remove();
                inline = previousInline;
            }

            var literal = inline as LiteralInline;
            if (literal != null)
            {
                literal.Content.TrimEnd();
            }
        }

        private static Inline FindBeginOfPreviousLine(ContainerInline container)
        {
            var previousSibling = (Inline)container.PreviousSibling;
            while (previousSibling != null)
            {
                if (previousSibling is SoftlineBreakInline || previousSibling is HardlineBreakInline)
                {
                    return previousSibling.NextSibling;
                }

                previousSibling = previousSibling.PreviousSibling;
            }

            if (container.Parent == null)
            {
                return container.FirstChild;
            }

            return FindBeginOfPreviousLine(container.Parent);
        }

        private class TableState
        {
            public TableState()
            {
                Delimiters = new List<Inline>();
            }

            public List<Inline> Delimiters { get; }
        }
    }
}