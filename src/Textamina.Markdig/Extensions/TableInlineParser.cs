using System.Collections.Generic;
using System.Linq.Expressions;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions
{
    public class TableDelimiterInline : DelimiterInline
    {
        public TableDelimiterInline(InlineParser parser) : base(parser)
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


    public class TableInlineParser : InlineParser, IDelimiterProcessor
    {
        public TableInlineParser()
        {
            OpeningCharacters = new[] { '|' };
        }

        public override bool Match(InlineParserState state, ref StringSlice slice)
        {
            state.Inline = new TableDelimiterInline(this) {LineIndex = state.LocalLineIndex};

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
            while (child != null)
            {
                var tableDelimiter = child as TableDelimiterInline;
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

                    delimiters.Add(tableDelimiter);
                }
                child = child.LastChild as ContainerInline;
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

            for (int i = 0; i < delimiters.Count; i++)
            {
                var delimiter = delimiters[i];
                bool startNewRow = false;
                if (delimiter.LineIndex != lineIndex)
                {
                    currentRow = new TableRow {Parent = table};
                    table.Children.Add(currentRow);
                    //startNewRow = true;
                }

                var cellContainer = new ContainerInline();
                var tableCell = new TableCell {Inline = cellContainer, Parent = currentRow};
                currentRow.Children.Add(tableCell);

                delimiter.Remove();
                delimiter.ReplaceBy(cellContainer);

                lineIndex = delimiter.LineIndex;
            }

            return false;
        }

        private class TableState
        {
            public TableState()
            {
                Delimiters = new List<TableDelimiterInline>();
            }

            public List<TableDelimiterInline> Delimiters { get; }
        }
    }
}