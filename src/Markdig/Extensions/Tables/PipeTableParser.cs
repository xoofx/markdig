// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Diagnostics;

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Parsers.Inlines;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.Tables;

/// <summary>
/// The inline parser used to transform a <see cref="ParagraphBlock"/> into a <see cref="Table"/> at inline parsing time.
/// </summary>
/// <seealso cref="InlineParser" />
/// <seealso cref="IPostInlineProcessor" />
public class PipeTableParser : InlineParser, IPostInlineProcessor
{
    private readonly LineBreakInlineParser lineBreakParser;

    /// <summary>
    /// Initializes a new instance of the <see cref="PipeTableParser" /> class.
    /// </summary>
    /// <param name="lineBreakParser">The line break parser to use</param>
    /// <param name="options">The options.</param>
    public PipeTableParser(LineBreakInlineParser lineBreakParser, PipeTableOptions? options = null)
    {
        this.lineBreakParser = lineBreakParser ?? throw new ArgumentNullException(nameof(lineBreakParser));
        OpeningCharacters = ['|', '\n', '\r'];
        Options = options ?? new PipeTableOptions();
    }

    /// <summary>
    /// Gets the options.
    /// </summary>
    public PipeTableOptions Options { get; }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        // Only working on Paragraph block
        if (!processor.Block!.IsParagraphBlock)
        {
            return false;
        }

        var c = slice.CurrentChar;
        var isNewLineFollowedByPipe = (c == '\n' || c == '\r') && slice.PeekChar() == '|';

        // If we have not a delimiter on the first line of a paragraph, don't bother to continue
        // tracking other delimiters on following lines
        var tableState = processor.ParserStates[Index] as TableState;
        bool isFirstLineEmpty = false;


        var position = processor.GetSourcePosition(slice.Start, out int globalLineIndex, out int column);
        var localLineIndex = globalLineIndex - processor.LineIndex;

        if (tableState is null)
        {
            // A table could be preceded by an empty line or a line containing an inline
            // that has not been added to the stack, so we consider this as a valid
            // start for a table. Typically, with this, we can have an attributes {...}
            // starting on the first line of a pipe table, even if the first line
            // doesn't have a pipe
            if (processor.Inline != null && (localLineIndex > 0 || c == '\n' || c == '\r') && !isNewLineFollowedByPipe)
            {
                return false;
            }

            if (processor.Inline is null || isNewLineFollowedByPipe)
            {
                isFirstLineEmpty = true;
            }
            // Else setup a table processor
            tableState = new TableState();
            processor.ParserStates[Index] = tableState;
        }

        if (c == '\n' || c == '\r')
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
                tableState.ColumnAndLineDelimiters.Add(processor.Inline!);
                tableState.EndOfLines.Add(processor.Inline!);
            }
        }
        else
        {
            processor.Inline = new PipeTableDelimiterInline(this)
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
            slice.SkipChar(); // Skip the `|` character

            tableState.ColumnAndLineDelimiters.Add(processor.Inline);
        }

        return true;
    }

    public bool PostProcess(InlineProcessor state, Inline? root, Inline? lastChild, int postInlineProcessorIndex, bool isFinalProcessing)
    {
        var container = root as ContainerInline;
        var tableState = state.ParserStates[Index] as TableState;

        // If the delimiters are being processed by an image link, we need to transform them back to literals
        if (!isFinalProcessing)
        {
            if (container is null || tableState is null)
            {
                return true;
            }

            var child = container.LastChild;
            List<PipeTableDelimiterInline>? delimitersToRemove = null;

            while (child != null)
            {
                if (child is PipeTableDelimiterInline pipeDelimiter)
                {
                    delimitersToRemove ??= new List<PipeTableDelimiterInline>();

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
                    pipeDelimiter.ReplaceByLiteral();

                    // Check that the pipe that is being removed is not going to make a line without pipe delimiters
                    var tableDelimiters = tableState.ColumnAndLineDelimiters;
                    var delimiterIndex = tableDelimiters.IndexOf(pipeDelimiter);

                    if (i == 0)
                    {
                        leftIsDelimiter = delimiterIndex > 0 && tableDelimiters[delimiterIndex - 1] is PipeTableDelimiterInline;
                    }
                    else if (i + 1 == delimitersToRemove.Count)
                    {
                        rightIsDelimiter = delimiterIndex + 1 < tableDelimiters.Count &&
                                           tableDelimiters[delimiterIndex + 1] is PipeTableDelimiterInline;
                    }
                    // Remove this delimiter from the table processor
                    tableState.ColumnAndLineDelimiters.Remove(pipeDelimiter);
                }

                // If we didn't have any delimiter before and after the delimiters we just removed, we mark the processor of the current line as no pipe
                if (!leftIsDelimiter && !rightIsDelimiter)
                {
                    tableState.LineHasPipe = false;
                }
            }

            return true;
        }

        // Remove previous state
        state.ParserStates[Index] = null!;

        // Continue
        if (tableState is null || container is null || tableState.IsInvalidTable || !tableState.LineHasPipe ) //|| tableState.LineIndex != state.LocalLineIndex)
        {
            if (tableState is not null)
            {
                foreach (var inline in tableState.ColumnAndLineDelimiters)
                {
                    if (inline is PipeTableDelimiterInline pipeDelimiter)
                    {
                        pipeDelimiter.ReplaceByLiteral();
                    }
                }
            }
            return true;
        }

        // Detect the header row
        var delimiters = tableState.ColumnAndLineDelimiters;
        // TODO: we could optimize this by merging FindHeaderRow and the cell loop
        var aligns = FindHeaderRow(delimiters);

        if (Options.RequireHeaderSeparator && aligns is null)
        {
            return true;
        }

        var table = new Table();

        // If the current paragraph block has any attributes attached, we can copy them
        var attributes = state.Block!.TryGetAttributes();
        if (attributes != null)
        {
            attributes.CopyTo(table.GetAttributes());
        }

        state.BlockNew = table;
        var cells = tableState.Cells;
        cells.Clear();

        //delimiters[0].DumpTo(state.DebugLog);

        // delimiters contain a list of `|` and `\n` delimiters
        // The `|` delimiters are created as child containers.
        // So the following:
        // | a | b \n
        // | d | e \n
        //
        // Will generate a tree of the following node:
        // |
        //   a
        //   |
        //     b
        //     \n
        //     |
        //       d
        //       |
        //         e
        //         \n
        // When parsing delimiters, we need to recover whether a row is of the following form:
        // 0)  | a | b | \n
        // 1)  | a | b \n
        // 2)    a | b \n
        // 3)    a | b | \n

        // If the last element is not a line break, add a line break to homogenize parsing in the next loop
        var lastElement = delimiters[delimiters.Count - 1];
        if (!(lastElement is LineBreakInline))
        {
            while (true)
            {
                if (lastElement is ContainerInline lastElementContainer)
                {
                    var nextElement = lastElementContainer.LastChild;
                    if (nextElement != null)
                    {
                        lastElement = nextElement;
                        continue;
                    }
                }
                break;
            }

            var endOfTable = new LineBreakInline();
            // If the last element is a container, we have to add the EOL to its child
            // otherwise only next sibling
            if (lastElement is ContainerInline)
            {
                ((ContainerInline)lastElement).AppendChild(endOfTable);
            }
            else
            {
                lastElement.InsertAfter(endOfTable);
            }
            delimiters.Add(endOfTable);
            tableState.EndOfLines.Add(endOfTable);
        }

        int lastPipePos = 0;

        // Cell loop
        // Reconstruct the table from the delimiters
        TableRow? row = null;
        TableRow? firstRow = null;
        for (int i = 0; i < delimiters.Count; i++)
        {
            var delimiter = delimiters[i];
            var pipeSeparator = delimiter as PipeTableDelimiterInline;
            var isLine = delimiter is LineBreakInline;

            if (row is null)
            {
                row = new TableRow();

                firstRow ??= row;

                // If the first delimiter is a pipe and doesn't have any parent or previous sibling, for cases like:
                // 0)  | a | b | \n
                // 1)  | a | b \n
                if (pipeSeparator != null && (delimiter.PreviousSibling is null || delimiter.PreviousSibling is LineBreakInline))
                {
                    delimiter.Remove();
                    if (table.Span.IsEmpty)
                    {
                        table.Span = delimiter.Span;
                        table.Line = delimiter.Line;
                        table.Column = delimiter.Column;
                    }
                    continue;
                }
            }

            // We need to find the beginning/ending of a cell from a right delimiter. From the delimiter 'x', we need to find a (without the delimiter start `|`)
            // So we iterate back to the first pipe or line break
            //         x
            // 1)  | a | b \n
            // 2)    a | b \n
            Inline? endOfCell = null;
            Inline? beginOfCell = null;
            var cellContentIt = delimiter;
            while (true)
            {
                cellContentIt = cellContentIt.PreviousSibling ?? cellContentIt.Parent;

                if (cellContentIt is null || cellContentIt is LineBreakInline)
                {
                    break;
                }

                // The cell begins at the first effective child after a | or the top ContainerInline (which is not necessary to bring into the tree + it contains an invalid span calculation)
                if (cellContentIt is PipeTableDelimiterInline || (cellContentIt.GetType() == typeof(ContainerInline) && cellContentIt.Parent is null ))
                {
                    beginOfCell = ((ContainerInline)cellContentIt).FirstChild;
                    if (endOfCell is null)
                    {
                        endOfCell = beginOfCell;
                    }
                    break;
                }

                beginOfCell = cellContentIt;
                if (endOfCell is null)
                {
                    endOfCell = beginOfCell;
                }
            }

            // If the current deilimiter is a pipe `|` OR
            // the beginOfCell/endOfCell are not null and
            // either they are :
            // - different
            // - they contain a single element, but it is not a line break (\n) or an empty/whitespace Literal.
            // Then we can add a cell to the current row
            if (!isLine || (beginOfCell != null && endOfCell != null && ( beginOfCell != endOfCell || !(beginOfCell is LineBreakInline || (beginOfCell is LiteralInline beingOfCellLiteral && beingOfCellLiteral.Content.IsEmptyOrWhitespace())))))
            {
                if (!isLine)
                {
                    // If the delimiter is a pipe, we need to remove it from the tree
                    // so that previous loop looking for a parent will not go further on subsequent cells
                    delimiter.Remove();
                    lastPipePos = delimiter.Span.End;
                }

                // We trim whitespace at the beginning and ending of the cell
                TrimStart(beginOfCell);
                TrimEnd(endOfCell);

                var cellContainer = new ContainerInline();

                // Copy elements from beginOfCell on the first level
                var cellIt = beginOfCell;
                while (cellIt != null && !IsLine(cellIt) && !(cellIt is PipeTableDelimiterInline))
                {
                    var nextSibling = cellIt.NextSibling;
                    cellIt.Remove();
                    if (cellContainer.Span.IsEmpty)
                    {
                        cellContainer.Line = cellIt.Line;
                        cellContainer.Column = cellIt.Column;
                        cellContainer.Span = cellIt.Span;
                    }
                    cellContainer.AppendChild(cellIt);
                    cellContainer.Span.End = cellIt.Span.End;
                    cellIt = nextSibling;
                }

                // Create the cell and add it to the pending row
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
                if (row.Span.IsEmpty)
                {
                    row.Span = cellContainer.Span;
                    row.Line = cellContainer.Line;
                    row.Column = cellContainer.Column;
                }
                row.Add(tableCell);
                cells.Add(tableCell);
            }

            // If we have a new line, we can add the row
            if (isLine)
            {
                Debug.Assert(row != null);
                if (table.Span.IsEmpty)
                {
                    table.Span = row!.Span;
                    table.Line = row.Line;
                    table.Column = row.Column;
                }
                table.Add(row!);
                row = null;
            }
        }

        if (lastPipePos > table.Span.End)
        {
          table.UpdateSpanEnd(lastPipePos);
        }

        // Once we are done with the cells, we can remove all end of lines in the table tree
        foreach (var endOfLine in tableState.EndOfLines)
        {
            endOfLine.Remove();
        }

        // If we have a header row, we can remove it
        // TODO: we could optimize this by merging FindHeaderRow and the previous loop
        var tableRow = (TableRow)table[0];
        tableRow.IsHeader = Options.RequireHeaderSeparator;
        if (aligns != null)
        {
            tableRow.IsHeader = true;
            table.RemoveAt(1);
            table.ColumnDefinitions.AddRange(aligns);
        }

        // Perform delimiter processor that are coming after this processor
        foreach (var cell in cells)
        {
            var paragraph = (ParagraphBlock) cell[0];
            state.PostProcessInlines(postInlineProcessorIndex + 1, paragraph.Inline, null, true);
            if (paragraph.Inline?.LastChild is not null)
            {
                paragraph.Inline.Span.End = paragraph.Inline.LastChild.Span.End;
                paragraph.UpdateSpanEnd(paragraph.Inline.LastChild.Span.End);
            }
        }

        // Clear cells when we are done
        cells.Clear();

        // Normalize the table
        if (Options.UseHeaderForColumnCount)
        {
            table.NormalizeUsingHeaderRow();
        }
        else
        {
            table.NormalizeUsingMaxWidth();
        }

        // We don't want to continue procesing delimiters, as we are already processing them here
        return false;
    }

    private static bool ParseHeaderString(Inline? inline, out TableColumnAlign? align, out int delimiterCount)
    {
        align = 0;
        delimiterCount = 0;
        var literal = inline as LiteralInline;
        if (literal is null)
        {
            return false;
        }

        // Work on a copy of the slice
        var line = literal.Content;
        if (TableHelper.ParseColumnHeader(ref line, '-', out align, out delimiterCount))
        {
            if (line.CurrentChar != '\0')
            {
                return false;
            }
            return true;
        }

        return false;
    }

    private List<TableColumnDefinition>? FindHeaderRow(List<Inline> delimiters)
    {
        bool isValidRow = false;
        int totalDelimiterCount = 0;
        List<TableColumnDefinition>? columnDefinitions = null;
        for (int i = 0; i < delimiters.Count; i++)
        {
            if (!IsLine(delimiters[i]))
            {
                continue;
            }

            // The last delimiter is always null,
            for (int j = i + 1; j < delimiters.Count; j++)
            {
                var delimiter = delimiters[j];
                var nextDelimiter = j + 1 < delimiters.Count ? delimiters[j + 1] : null;

                var columnDelimiter = delimiter as PipeTableDelimiterInline;
                if (j == i + 1 && IsStartOfLineColumnDelimiter(columnDelimiter))
                {
                    continue;
                }

                // Check the left side of a `|` delimiter
                TableColumnAlign? align = null;
                int delimiterCount = 0;
                if (delimiter.PreviousSibling != null &&
                    !(delimiter.PreviousSibling is LiteralInline li && li.Content.IsEmptyOrWhitespace()) && // ignore parsed whitespace
                    !ParseHeaderString(delimiter.PreviousSibling, out align, out delimiterCount))
                {
                    break;
                }

                // Create aligns until we may have a header row

                columnDefinitions ??= new List<TableColumnDefinition>();
                totalDelimiterCount += delimiterCount;
                columnDefinitions.Add(new TableColumnDefinition() { Alignment =  align, Width = delimiterCount});

                // If this is the last delimiter, we need to check the right side of the `|` delimiter
                if (nextDelimiter is null)
                {
                    var nextSibling = columnDelimiter != null
                        ? columnDelimiter.FirstChild
                        : delimiter.NextSibling;

                    // If there is no content after
                    if (IsNullOrSpace(nextSibling))
                    {
                        isValidRow = true;
                        break;
                    }

                    if (!ParseHeaderString(nextSibling, out align, out delimiterCount))
                    {
                        break;
                    }
                    totalDelimiterCount += delimiterCount;
                    isValidRow = true;
                    columnDefinitions.Add(new TableColumnDefinition() { Alignment = align, Width = delimiterCount});
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

        // calculate the width of the columns in percent based on the delimiter count
        if (!isValidRow || columnDefinitions == null)
        {
            return null;
        }

        if (Options.InferColumnWidthsFromSeparator)
        {
            foreach (var columnDefinition in columnDefinitions)
            {
                columnDefinition.Width = (columnDefinition.Width * 100) / totalDelimiterCount;
            }
        }
        else
        {
            foreach (var columnDefinition in columnDefinitions)
            {
                columnDefinition.Width = 0;
            }
        }
        return columnDefinitions;
    }

    private static bool IsLine(Inline inline)
    {
        return inline is LineBreakInline;
    }

    private static bool IsStartOfLineColumnDelimiter(Inline? inline)
    {
        if (inline is null)
        {
            return false;
        }

        var previous = inline.PreviousSibling;
        if (previous is null)
        {
            return true;
        }

        if (previous is LiteralInline literal)
        {
            if (!literal.Content.IsEmptyOrWhitespace())
            {
                return false;
            }
            previous = previous.PreviousSibling;
        }
        return previous is null || IsLine(previous);
    }

    private static void TrimStart(Inline? inline)
    {
        while (inline is ContainerInline && !(inline is DelimiterInline))
        {
            inline = ((ContainerInline)inline).FirstChild;
        }

        if (inline is LiteralInline literal)
        {
            literal.Content.TrimStart();
        }
    }

    private static void TrimEnd(Inline? inline)
    {
        if (inline is LiteralInline literal)
        {
            literal.Content.TrimEnd();
        }
    }

    private static bool IsNullOrSpace(Inline? inline)
    {
        if (inline is null)
        {
            return true;
        }

        if (inline is LiteralInline literal)
        {
            return literal.Content.IsEmptyOrWhitespace();
        }
        return false;
    }

    private sealed class TableState
    {
        public bool IsInvalidTable { get; set; }

        public bool LineHasPipe { get; set; }

        public int LineIndex { get; set; }

        public List<Inline> ColumnAndLineDelimiters { get; } = [];

        public List<TableCell> Cells { get; } = [];

        public List<Inline> EndOfLines { get; } = [];
    }
}