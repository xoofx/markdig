using System.Collections.Generic;
using System.Linq.Expressions;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions
{
    public class PiprTableDelimiterInline : DelimiterInline
    {
        public PiprTableDelimiterInline(InlineParser parser) : base(parser)
        {
        }

        public int LineIndex { get; set; }

        public override string ToLiteral()
        {
            return "|";
        }
    }

    public class TableBlock : ContainerBlock
    {
        public TableBlock() : base(null)
        {
        }
    }

    public class TableRow : ContainerBlock
    {
        public TableRow() : base(null)
        {
        }

        public bool IsHeader { get; set; }
    }

    public class TableCell : LeafBlock
    {
        public TableCell() : base(null)
        {
        }
    }


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
            TableRow currentRow = null;
            state.BlockNew = table;
            TableRow firstRow = null;
            int columnCount = 0;
            int maxColumn = 0;
            TableRow previousRow = null;

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
                    currentRow = new TableRow {Parent = table};
                    table.Children.Add(currentRow);
                    columnCount = 0;
                    //startNewRow = true;
                }

                if (maxColumn > 0 && columnCount == maxColumn)
                {
                    delimiter.Remove();
                    ((ContainerInline)((TableCell)currentRow.LastChild).Inline).AppendChild(delimiter);
                    continue;
                }

                // If a '|' is the first in a line not starting at the begining of a line, we need
                // to add the previous content as an implicit column
                if (HasPreviousColumn(delimiter))
                {
                    var cellContainer = new ContainerInline();
                    var tableCell = new TableCell { Inline = cellContainer, Parent = currentRow };
                    currentRow.Children.Add(tableCell);
                    var previousInline = delimiters[i - 1];
                    CopyCellDown(previousInline, cellContainer);
                    columnCount++;
                }

                // otherwise we have a regular cell
                {
                    var cellContainer = new ContainerInline();
                    var tableCell = new TableCell { Inline = cellContainer, Parent = currentRow };
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

            return false;
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
                    var literal = previousSibling as LiteralInline;
                    if (literal != null)
                    {
                        literal.Content.TrimEnd();
                    }

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