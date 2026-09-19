// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Tables;

// GFM cell boundaries precede *all* inline parsing, including code, HTML and links.
// Keep this block parser separate from the legacy inline-based parser so opting in
// does not change the deliberately more permissive default syntax.
internal sealed class GfmPipeTableParser : BlockParser
{
    private static readonly object _headerVisitedKey = new();
    private readonly PipeTableOptions _options;

    public GfmPipeTableParser(PipeTableOptions options)
    {
        _options = options;
        // ListBlockParser is a global parser. Stay in the same phase so list
        // markers such as "- | -" get their normal precedence over tables.
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
        if (processor.CurrentChar is not ('|' or '-' or ':' or '\v' or '\f')
            || processor.IsCodeIndent || processor.HasUnmatchedBlocks
            || processor.CurrentBlock is not ParagraphBlock paragraph || paragraph.Lines.Count == 0
            || paragraph.ContainsData(_headerVisitedKey))
        {
            return BlockState.None;
        }

        for (int i = processor.Line.Start; i <= processor.Line.End; i++)
        {
            if (processor.Line.Text[i] is not ('|' or '-' or ':' or ' ' or '\t' or '\v' or '\f'))
            {
                return BlockState.None;
            }
        }
        // A bare run of dashes belongs to the setext heading parser. Colons or
        // pipes disambiguate a one-column table with optional outer pipes.
        // Vertical tabs/form feeds are allowed by the table scanner, but not setext.
        if (processor.Line.IndexOf('|') < 0 && processor.Line.IndexOf(':') < 0
            && processor.Line.IndexOf('\v') < 0 && processor.Line.IndexOf('\f') < 0)
        {
            return BlockState.None;
        }
        var separators = SplitRow(processor.Line);
        if (separators.Count == 0) return BlockState.None;
        var definitions = new List<TableColumnDefinition>(separators.Count);
        int totalDashes = 0;
        foreach (var separator in separators)
        {
            var cell = separator;
            while (!cell.IsEmpty && IsScannerSpace(cell.CurrentChar)) cell.SkipChar();
            while (!cell.IsEmpty && IsScannerSpace(cell.Text[cell.End])) cell.End--;
            bool left = cell.CurrentChar == ':';
            if (left) cell.SkipChar();
            int dashes = cell.CountAndSkipChar('-');
            bool right = cell.CurrentChar == ':';
            if (right) cell.SkipChar();
            if (dashes == 0 || !cell.IsEmpty)
            {
                return BlockState.None;
            }
            definitions.Add(new TableColumnDefinition
            {
                // The native scanner accepts VT/FF around markers, but its cell
                // trimmer does not remove them when determining alignment.
                Alignment = separator.CurrentChar == ':'
                    ? (separator.Text[separator.End] == ':' ? TableColumnAlign.Center : TableColumnAlign.Left)
                    : separator.Text[separator.End] == ':' ? TableColumnAlign.Right : null,
                SeparatorDashCount = dashes
            });
            totalDashes += dashes;
        }

        var header = paragraph.Lines.Lines[paragraph.Lines.Count - 1];
        var headerCells = SplitRow(header.Slice);
        if (headerCells.Count != definitions.Count)
        {
            // cmark-gfm attempts a syntactically valid delimiter only once per paragraph.
            paragraph.SetData(_headerVisitedKey, true);
            return BlockState.None;
        }

        var table = new Table(this)
        {
            Line = header.Line,
            Column = header.Column,
            Span = new SourceSpan(header.Slice.Start, processor.Line.End)
        };
        foreach (var definition in definitions)
        {
            int dashes = definition.SeparatorDashCount.GetValueOrDefault();
            if (_options.InferColumnWidthsFromSeparator)
            {
                definition.Width = dashes * 100.0f / totalDashes;
                definition.SeparatorDashCount = dashes;
            }
            else
            {
                definition.SeparatorDashCount = null;
            }
            table.ColumnDefinitions.Add(definition);
        }
        AddRow(table, header, headerCells, true);

        paragraph.Lines.RemoveAt(paragraph.Lines.Count - 1);
        if (paragraph.Lines.Count == 0)
        {
            table.LinesBefore = paragraph.LinesBefore;
        }
        else
        {
            paragraph.Span.End = paragraph.Lines.Lines[paragraph.Lines.Count - 1].Slice.End;
            // cmark-gfm inserts this text directly, without interpreting link
            // reference definitions when the original paragraph becomes a table.
            var preceding = new ParagraphBlock(this)
            {
                Lines = paragraph.Lines,
                Line = paragraph.Line,
                Column = paragraph.Column,
                Span = paragraph.Span,
                LinesBefore = paragraph.LinesBefore,
                IsOpen = false
            };
            for (int i = 0; i < preceding.Lines.Count; i++)
            {
                ref var slice = ref preceding.Lines.Lines[i].Slice;
                while (!slice.IsEmpty && slice.CurrentChar.IsSpaceOrTab()) slice.SkipChar();
            }
            ref var last = ref preceding.Lines.Lines[preceding.Lines.Count - 1].Slice;
            while (!last.IsEmpty && last.Text[last.End].IsSpaceOrTab()) last.End--;
            paragraph.Parent!.Insert(paragraph.Parent.IndexOf(paragraph), preceding);
            paragraph.Lines = new StringLineGroup(1);
        }
        processor.Close(paragraph);

        // A temporary leaf lets the normal block parsers decide whether the
        // next line interrupts the table. RowParser handles only the remaining lines.
        // Replace it on close, before the document's inline parsing pass.
        processor.NewBlocks.Push(new TableBlock(this, table));
        return BlockState.ContinueDiscard;
    }

    public override bool Close(BlockProcessor processor, Block block)
    {
        if (block is TableBlock pending)
        {
            var parent = pending.Parent!;
            pending.Table.IsOpen = false;
            parent.Insert(parent.IndexOf(pending), pending.Table);
            return false;
        }
        return true;
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
        // Reached only when all enclosing containers matched this line. Defer
        // consuming it until other block openers have had their normal precedence.
        ((TableBlock)block).MatchedLine = processor.LineIndex;
        return BlockState.None;
    }

    private static List<StringSlice> SplitRow(StringSlice line)
    {
        while (!line.IsEmpty && line.CurrentChar.IsSpaceOrTab()) line.SkipChar();
        var cells = new List<StringSlice>();
        if (line.CurrentChar == '|')
        {
            line.SkipChar();
            while (!line.IsEmpty && IsScannerSpace(line.CurrentChar)) line.SkipChar();
        }
        int start = line.Start;
        for (int i = start; i <= line.End; i++)
        {
            char c = line.Text[i];
            if (c == '\\' && i < line.End && line.Text[i + 1] == '|')
            {
                i++;
            }
            else if (c == '|')
            {
                var cell = new StringSlice(line.Text, start, i - 1);
                TrimCell(ref cell);
                cells.Add(cell);
                if (cells.Count > ushort.MaxValue) return [];
                while (i < line.End && IsScannerSpace(line.Text[i + 1])) i++;
                start = i + 1;
            }
        }
        if (start <= line.End)
        {
            var cell = new StringSlice(line.Text, start, line.End);
            TrimCell(ref cell);
            cells.Add(cell);
        }
        return cells.Count > ushort.MaxValue ? [] : cells;
    }

    private static bool IsScannerSpace(char c) => c is ' ' or '\t' or '\v' or '\f';

    private static void TrimCell(ref StringSlice cell)
    {
        while (!cell.IsEmpty && cell.CurrentChar.IsSpaceOrTab()) cell.SkipChar();
        while (!cell.IsEmpty && cell.Text[cell.End].IsSpaceOrTab()) cell.End--;
    }

    private static void AddRow(Table table, StringLine line, List<StringSlice> cells, bool isHeader)
    {
        var row = new TableRow
        {
            IsHeader = isHeader,
            Line = line.Line,
            Column = line.Column,
            Span = new SourceSpan(line.Slice.Start, line.Slice.End)
        };
        for (int i = 0; i < table.ColumnDefinitions.Count; i++)
        {
            var content = i < cells.Count ? cells[i] : new StringSlice(line.Slice.Text, line.Slice.End + 1, line.Slice.End);
            int column = line.Column + content.Start - line.Slice.Start;
            var paragraph = new ParagraphBlock(table.Parser)
            {
                Line = line.Line,
                Column = column,
                Span = new SourceSpan(content.Start, content.End)
            };
            paragraph.AppendLine(ref content, column, line.Line, content.Start, false);
            var cell = new TableCell { Line = paragraph.Line, Column = column, ColumnIndex = i, Span = paragraph.Span };
            cell.Add(paragraph);
            row.Add(cell);
        }
        table.Add(row);
        table.Span.End = Math.Max(table.Span.End, line.Slice.End);
    }

    // Pipe unescaping precedes *all* inline parsing (including reference lookup
    // and autolinks). Retain a map into the original slice for source locations.
    internal static int[]? UnescapePipes(ref StringSlice text)
    {
        if (text.AsSpan().IndexOf("\\|".AsSpan()) < 0) return null;
        var builder = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);
        var offsets = new int[text.Length + 1];
        for (int i = text.Start; i <= text.End; i++)
        {
            if (text.Text[i] == '\\' && i < text.End && text.Text[i + 1] == '|') i++;
            offsets[builder.Length] = i;
            builder.Append(text.Text[i]);
        }
        offsets[builder.Length] = text.End + 1;
        Array.Resize(ref offsets, builder.Length + 1);
        text = new StringSlice(builder.ToString());
        return offsets;
    }

    private sealed class TableBlock : LeafBlock
    {
        public TableBlock(BlockParser parser, Table table) : base(parser)
        {
            Table = table;
            ProcessInlines = false;
            Span = table.Span;
            Lines = new StringLineGroup(1);
        }

        public Table Table { get; }

        public int MatchedLine { get; set; } = -1;

        public int AutocompletedCells { get; set; }
    }

    internal sealed class RowParser : BlockParser
    {
        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (processor.CurrentBlock is not TableBlock pending || processor.IsBlankLine) return BlockState.None;
            // Tables, unlike paragraphs, cannot lazily continue a quote/list.
            if (pending.MatchedLine != processor.LineIndex) return BlockState.None;
            // Match cmark-gfm's bound on amplification from padding short rows.
            if (pending.AutocompletedCells > 0x80000) return BlockState.None;
            var line = new StringLine(processor.Line, processor.LineIndex, processor.Column, processor.Line.Start, processor.Line.NewLine);
            var cells = SplitRow(line.Slice);
            if (cells.Count == 0) return BlockState.None;
            pending.AutocompletedCells += Math.Max(0, pending.Table.ColumnDefinitions.Count - cells.Count);
            AddRow(pending.Table, line, cells, false);
            pending.IsOpen = true;
            return BlockState.BreakDiscard;
        }
    }

    internal static void UpdateListTightness(MarkdownDocument document)
    {
        foreach (var table in document.Descendants<Table>())
        {
            // cmark-gfm marks a header-only table as ending in a blank line.
            // Only direct list-item children affect list tightness this way.
            if (table.Parser is GfmPipeTableParser && table.Count == 1
                && table.Parent is ListItemBlock item && item.Parent is ListBlock list
                && (item.LastChild != table || list.LastChild != item))
            {
                list.IsLoose = true;
            }
        }
    }
}
