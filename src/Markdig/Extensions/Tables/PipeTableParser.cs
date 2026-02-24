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
    private readonly LineBreakInlineParser _lineBreakParser;

    /// <summary>
    /// Initializes a new instance of the <see cref="PipeTableParser" /> class.
    /// </summary>
    /// <param name="lineBreakParser">The line break parser to use</param>
    /// <param name="options">The options.</param>
    public PipeTableParser(LineBreakInlineParser lineBreakParser, PipeTableOptions? options = null)
    {
        _lineBreakParser = lineBreakParser ?? throw new ArgumentNullException(nameof(lineBreakParser));
        OpeningCharacters = ['|', '\n', '\r'];
        Options = options ?? new PipeTableOptions();
    }

    /// <summary>
    /// Gets the options.
    /// </summary>
    public PipeTableOptions Options { get; }

    /// <summary>
    /// Attempts to match the parser at the current position.
    /// </summary>
    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        // Only working on Paragraph block
        if (!processor.Block!.IsParagraphBlock)
        {
            return false;
        }

        var c = slice.CurrentChar;

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
            if (processor.Inline != null && (c == '\n' || c == '\r'))
            {
                return false;
            }

            if (processor.Inline is null)
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
            _lineBreakParser.Match(processor, ref slice);
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
                LocalLineIndex = localLineIndex,
                IsClosed = true  // Creates flat sibling structure for O(n) traversal
            };

            tableState.LineHasPipe = true;
            slice.SkipChar(); // Skip the `|` character

            tableState.ColumnAndLineDelimiters.Add(processor.Inline);
        }

        return true;
    }

    /// <summary>
    /// Performs the post process operation.
    /// </summary>
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

            // With flat structure, pipes are siblings at root level
            // Walk backwards from the last child to find pipe delimiters
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

                // Walk siblings instead of descending into containers
                child = child.PreviousSibling;
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

        // Abort if not a valid table
        if (tableState is null || container is null || tableState.IsInvalidTable || !tableState.LineHasPipe)
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
        var aligns = FindHeaderRow(delimiters);

        if (Options.RequireHeaderSeparator && aligns is null)
        {
            // No valid header separator found - convert all pipe delimiters to literals
            foreach (var inline in delimiters)
            {
                if (inline is PipeTableDelimiterInline pipeDelimiter)
                {
                    pipeDelimiter.ReplaceByLiteral();
                }
            }
            return true;
        }

        var table = new Table();

        // If the current paragraph block has any attributes attached, we can copy them
        var attributes = state.Block!.TryGetAttributes();
        if (attributes != null)
        {
            attributes.CopyTo(table.GetAttributes());
        }

        var cells = tableState.Cells;
        cells.Clear();

        // Pipes may end up nested inside unmatched emphasis delimiters, e.g.:
        //     *a | b*|
        // Promote them to root level so we have a flat sibling structure.
        PromoteNestedPipesToRootLevel(delimiters, container);

        // The inline tree is now flat: all pipes and line breaks are siblings at root level.
        // For example, `| a | b \n| c | d \n` produces:
        //     [|] [a] [|] [b] [\n] [|] [c] [|] [d] [\n]
        //
        // Tables support four row formats:
        //     | a | b |    (leading and trailing pipes)
        //     | a | b      (leading pipe only)
        //       a | b      (no leading or trailing pipes)
        //       a | b |    (trailing pipe only)

        // Ensure the table ends with a line break to simplify row detection
        var lastElement = delimiters[delimiters.Count - 1];
        if (!(lastElement is LineBreakInline))
        {
            // Find the actual last sibling (there may be content after the last delimiter)
            while (lastElement.NextSibling != null)
            {
                lastElement = lastElement.NextSibling;
            }

            var endOfTable = new LineBreakInline();
            lastElement.InsertAfter(endOfTable);
            delimiters.Add(endOfTable);
            tableState.EndOfLines.Add(endOfTable);
        }

        int lastPipePos = 0;

        // Build table rows and cells by iterating through delimiters
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

                // Skip leading pipe at start of row (e.g., `| a | b` or `| a | b |`)
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

            // Find cell content by walking backwards from this delimiter to the previous pipe or line break.
            // For `| a | b \n` at delimiter 'x':
            //       [|] [a] [x] [b] [\n]
            //                ^--- current delimiter
            // Walk back: [a] is the cell content (stop at [|])
            Inline? endOfCell = null;
            Inline? beginOfCell = null;
            var cellContentIt = delimiter.PreviousSibling;
            while (cellContentIt != null)
            {
                if (cellContentIt is LineBreakInline || cellContentIt is PipeTableDelimiterInline)
                    break;

                // Stop at the root ContainerInline (which is not necessary to bring into the tree + it contains an invalid span calculation)
                if (cellContentIt.GetType() == typeof(ContainerInline) && cellContentIt.Parent is null)
                    break;

                beginOfCell = cellContentIt;
                endOfCell ??= beginOfCell;

                cellContentIt = cellContentIt.PreviousSibling;
            }

            // If the current delimiter is a pipe `|` OR
            // the beginOfCell/endOfCell are not null and
            // either they are:
            // - different
            // - they contain a single element, but it is not a line break (\n) or an empty/whitespace Literal.
            // Then we can add a cell to the current row
            if (!isLine || (beginOfCell != null && endOfCell != null && ( beginOfCell != endOfCell || !(beginOfCell is LineBreakInline || (beginOfCell is LiteralInline beingOfCellLiteral && beingOfCellLiteral.Content.IsEmptyOrWhitespace())))))
            {
                // We trim whitespace at the beginning and ending of the cell
                TrimStart(beginOfCell);
                TrimEnd(endOfCell);

                var cellContainer = new ContainerInline();

                // Copy elements from beginOfCell on the first level
                // The pipe delimiter serves as a boundary - stop when we hit it
                var cellIt = beginOfCell;
                while (cellIt != null && !IsLine(cellIt) && !(cellIt is PipeTableDelimiterInline))
                {
                    var nextSibling = cellIt.NextSibling;

                    // Skip empty literals (can result from trimming)
                    if (cellIt is LiteralInline { Content.IsEmpty: true })
                    {
                        cellIt.Remove();
                        cellIt = nextSibling;
                        continue;
                    }

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

                if (!isLine)
                {
                    // Remove the pipe delimiter AFTER copying cell content
                    // This preserves the sibling chain during the copy loop
                    delimiter.Remove();
                    lastPipePos = delimiter.Span.End;
                }

                // Create the cell and add it to the pending row
                var tableParagraph = new ParagraphBlock
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

        // Mark first row as header and remove the separator row if present
        var tableRow = (TableRow)table[0];
        tableRow.IsHeader = Options.RequireHeaderSeparator;
        if (aligns != null)
        {
            tableRow.IsHeader = true;
            table.RemoveAt(1);
            table.ColumnDefinitions.AddRange(aligns);
        }

        // Perform all post-processors on cell content
        // With InsertAfter, emphasis runs before pipe table, so we need to re-run from index 0
        // to ensure emphasis delimiters in cells are properly matched
        foreach (var cell in cells)
        {
            var paragraph = (ParagraphBlock) cell[0];
            state.PostProcessInlines(0, paragraph.Inline, null, true);
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

        if (state.Block is ParagraphBlock { Inline.FirstChild: not null } leadingParagraph)
        {
            // The table was preceded by a non-empty paragraph, e.g.
            // ```md
            // Some text
            // | Header |
            // ```
            //
            // Keep the paragraph as-is and insert the table after it.
            // Since we've already processed all the inlines in this table block,
            // we can't insert it while the parent is still being processed.
            // Hook up a callback that inserts the table after we're done with ProcessInlines for the parent block.

            // We've processed inlines in the table, but not the leading paragraph itself yet.
            state.PostProcessInlines(0, leadingParagraph.Inline, null, isFinalProcessing: true);

            ContainerBlock parent = leadingParagraph.Parent!;

            parent.ProcessInlinesEnd += (_, _) =>
            {
                parent.Insert(parent.IndexOf(leadingParagraph) + 1, table);
            };
        }
        else
        {
            // Nothing interesting in the existing block, just replace it.
            state.BlockNew = table;
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

            // Parse the separator row (second row) to extract column alignments
            for (int j = i + 1; j < delimiters.Count; j++)
            {
                var delimiter = delimiters[j];
                var nextDelimiter = j + 1 < delimiters.Count ? delimiters[j + 1] : null;

                var columnDelimiter = delimiter as PipeTableDelimiterInline;
                if (j == i + 1 && IsStartOfLineColumnDelimiter(columnDelimiter))
                {
                    continue;
                }

                // Parse the content before this delimiter as a column definition (e.g., `:---`, `---:`, `:---:`)
                // Skip if previous sibling is a pipe (empty cell) or whitespace
                TableColumnAlign? align = null;
                int delimiterCount = 0;
                if (delimiter.PreviousSibling != null &&
                    !(delimiter.PreviousSibling is PipeTableDelimiterInline) &&
                    !(delimiter.PreviousSibling is LiteralInline li && li.Content.IsEmptyOrWhitespace()) &&
                    !ParseHeaderString(delimiter.PreviousSibling, out align, out delimiterCount))
                {
                    break;
                }

                // Create aligns until we may have a header row

                columnDefinitions ??= new List<TableColumnDefinition>();
                totalDelimiterCount += delimiterCount;
                columnDefinitions.Add(new TableColumnDefinition() { Alignment =  align, Width = delimiterCount});

                // If this is the last pipe, check for a trailing column definition (row without trailing pipe)
                // e.g., `| :--- | ---:` has content after the last pipe
                if (nextDelimiter is null)
                {
                    var nextSibling = delimiter.NextSibling;

                    // No trailing content means row ends with pipe: `| :--- |`
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
        while (inline is ContainerInline containerInline && !(containerInline is DelimiterInline))
        {
            inline = containerInline.FirstChild;
        }

        if (inline is LiteralInline literal)
        {
            literal.Content.TrimStart();
        }
    }

    private static void TrimEnd(Inline? inline)
    {
        // Walk into containers to find the last leaf to trim
        // Skip PipeTableDelimiterInline but walk into other containers (including emphasis)
        while (inline is ContainerInline container && !(inline is PipeTableDelimiterInline))
        {
            inline = container.LastChild;
        }

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

    /// <summary>
    /// Promotes nested pipe delimiters and line breaks to root level.
    /// </summary>
    /// <remarks>
    /// Handles cases like `*a | b*|` where the pipe ends up inside an unmatched emphasis container.
    /// After promotion, all delimiters become siblings at root level for consistent cell boundary detection.
    /// </remarks>
    private static void PromoteNestedPipesToRootLevel(List<Inline> delimiters, ContainerInline root)
    {
        for (int i = 0; i < delimiters.Count; i++)
        {
            var delimiter = delimiters[i];

            // Handle both pipe delimiters and line breaks
            bool isPipe = delimiter is PipeTableDelimiterInline;
            bool isLineBreak = delimiter is LineBreakInline;
            if (!isPipe && !isLineBreak)
                continue;

            // Skip if already at root level
            if (delimiter.Parent == root)
                continue;

            // Find the top-level ancestor (direct child of root)
            var ancestor = delimiter.Parent;
            while (ancestor?.Parent != null && ancestor.Parent != root)
            {
                ancestor = ancestor.Parent;
            }

            if (ancestor is null || ancestor.Parent != root)
                continue;

            // Split: promote delimiter to be sibling of ancestor
            SplitContainerAtDelimiter(delimiter, ancestor);
        }
    }

    /// <summary>
    /// Splits a container at the delimiter, promoting the delimiter to root level.
    /// </summary>
    /// <remarks>
    /// For input `*a | b*`, the pipe is inside the emphasis container:
    ///     EmphasisDelimiter { "a", Pipe, "b" }
    /// After splitting:
    ///     EmphasisDelimiter { "a" }, Pipe, Container { "b" }
    /// </remarks>
    private static void SplitContainerAtDelimiter(Inline delimiter, Inline ancestor)
    {
        if (delimiter.Parent is not { } parent) return;

        // Collect content after the delimiter
        var contentAfter = new List<Inline>();
        var current = delimiter.NextSibling;
        while (current != null)
        {
            contentAfter.Add(current);
            current = current.NextSibling;
        }

        // Remove content after delimiter from parent
        foreach (var inline in contentAfter)
        {
            inline.Remove();
        }

        // Remove delimiter from parent
        delimiter.Remove();

        // Insert delimiter after the ancestor (at root level)
        ancestor.InsertAfter(delimiter);

        // If there's content after, wrap in new container and insert after delimiter
        if (contentAfter.Count > 0)
        {
            // Create new container matching the original parent type
            var newContainer = CreateMatchingContainer(parent);
            foreach (var inline in contentAfter)
            {
                newContainer.AppendChild(inline);
            }
            delimiter.InsertAfter(newContainer);
        }
    }

    /// <summary>
    /// Creates a container to wrap content split from the source container.
    /// </summary>
    private static ContainerInline CreateMatchingContainer(ContainerInline source)
    {
        // Emphasis processing runs before pipe table processing, so emphasis delimiters
        // are already resolved. A plain ContainerInline suffices.
        return new ContainerInline
        {
            Span = source.Span,
            Line = source.Line,
            Column = source.Column
        };
    }

    private sealed class TableState
    {
        public bool IsInvalidTable { get; set; }

        public bool LineHasPipe { get; set; }

        public List<Inline> ColumnAndLineDelimiters { get; } = [];

        public List<TableCell> Cells { get; } = [];

        public List<Inline> EndOfLines { get; } = [];
    }
}
