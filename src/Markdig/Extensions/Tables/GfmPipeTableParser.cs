// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.Tables;

// GFM cell boundaries precede *all* inline parsing, including code, HTML and links.
// Keep this block parser separate from the legacy inline-based parser so opting in
// does not change the deliberately more permissive default syntax.
internal sealed class GfmPipeTableParser : BlockParser
{
    private readonly PipeTableOptions _options;

    public GfmPipeTableParser(PipeTableOptions options)
    {
        _options = options;
        OpeningCharacters = ['|', '-', ':'];
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
        if (processor.IsCodeIndent || processor.HasUnmatchedBlocks
            || processor.CurrentBlock is not ParagraphBlock paragraph || paragraph.Lines.Count == 0)
        {
            return BlockState.None;
        }

        for (int i = processor.Line.Start; i <= processor.Line.End; i++)
        {
            if (processor.Line.Text[i] is not ('|' or '-' or ':' or ' ' or '\t'))
            {
                return BlockState.None;
            }
        }
        // A bare run of dashes belongs to the setext heading parser. Colons or
        // pipes disambiguate a one-column table with optional outer pipes.
        if (processor.Line.IndexOf('|') < 0 && processor.Line.IndexOf(':') < 0)
        {
            return BlockState.None;
        }
        var separators = SplitRow(processor.Line);
        var definitions = new List<TableColumnDefinition>(separators.Count);
        int totalDashes = 0;
        foreach (var separator in separators)
        {
            var cell = separator;
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
                Alignment = left ? (right ? TableColumnAlign.Center : TableColumnAlign.Left)
                    : right ? TableColumnAlign.Right : null,
                SeparatorDashCount = dashes
            });
            totalDashes += dashes;
        }

        var header = paragraph.Lines.Lines[paragraph.Lines.Count - 1];
        var headerCells = SplitRow(header.Slice);
        if (definitions.Count == 0 || headerCells.Count != definitions.Count)
        {
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
        line.Trim();
        var cells = new List<StringSlice>();
        if (line.CurrentChar == '|') line.SkipChar();
        int start = line.Start;
        for (int i = start; i <= line.End; i++)
        {
            char c = line.Text[i];
            if (c == '\\' && i < line.End && (line.Text[i + 1] == '\\' || line.Text[i + 1] == '|'))
            {
                i++;
            }
            else if (c == '|')
            {
                var cell = new StringSlice(line.Text, start, i - 1);
                cell.Trim();
                cells.Add(cell);
                start = i + 1;
            }
        }
        if (start <= line.End)
        {
            var cell = new StringSlice(line.Text, start, line.End);
            cell.Trim();
            cells.Add(cell);
        }
        return cells;
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
            var paragraph = new ParagraphBlock
            {
                Line = line.Line,
                Column = column,
                Span = new SourceSpan(content.Start, content.End)
            };
            paragraph.AppendLine(ref content, column, line.Line, content.Start, false);
            // Normal Markdown already unescapes pipes in text and links, but not
            // in code or raw HTML. Do that locally, retaining original source spans.
            paragraph.ProcessInlinesEnd += UnescapePipes;
            var cell = new TableCell { Line = paragraph.Line, Column = column, ColumnIndex = i, Span = paragraph.Span };
            cell.Add(paragraph);
            row.Add(cell);
        }
        table.Add(row);
        table.Span.End = Math.Max(table.Span.End, line.Slice.End);
    }

    private static void UnescapePipes(InlineProcessor processor, Inline? inline)
    {
        if (processor.Block is ParagraphBlock { Inline: { } container })
        {
            foreach (var child in container.Descendants())
            {
                if (child is CodeInline code)
                {
                    code.Content = code.Content.Replace("\\|", "|");
                }
                else if (child is HtmlInline html)
                {
                    html.Tag = html.Tag.Replace("\\|", "|");
                }
            }
        }
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
    }

    internal sealed class RowParser : BlockParser
    {
        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (processor.CurrentBlock is not TableBlock pending || processor.IsBlankLine) return BlockState.None;
            // Tables, unlike paragraphs, cannot lazily continue a quote/list.
            if (pending.MatchedLine != processor.LineIndex) return BlockState.None;
            var line = new StringLine(processor.Line, processor.LineIndex, processor.Column, processor.Line.Start, processor.Line.NewLine);
            AddRow(pending.Table, line, SplitRow(line.Slice), false);
            pending.IsOpen = true;
            return BlockState.BreakDiscard;
        }
    }
}
