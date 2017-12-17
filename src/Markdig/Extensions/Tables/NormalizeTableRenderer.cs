using Markdig.Renderers.Normalize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Markdig.Extensions.Tables
{
    public class NormalizeTableRenderer : NormalizeObjectRenderer<Table>
    {
        public const string PipeSeparator = "|";
        public const string HeaderSeparator = "---";
        public const string AlignmentChar = ":";
        public const string MarginSeparator = " ";
        protected override void Write(NormalizeRenderer renderer, Table obj)
        {
            renderer.EnsureLine();

            foreach (var row in obj.OfType<TableRow>())
            {
                renderer.Write(PipeSeparator);

                foreach (var tableCell in row)
                {
                    renderer.Write(MarginSeparator);

                    renderer.Render(tableCell);

                    renderer.Write(MarginSeparator);
                    renderer.Write(PipeSeparator);
                }

                renderer.WriteLine();


                if (row.IsHeader)
                {
                    bool alignmentEnabled = obj.ColumnDefinitions.Any(c => c.Alignment != TableColumnAlign.Left);

                    renderer.Write(PipeSeparator);

                    foreach (var column in obj.ColumnDefinitions)
                    {
                        renderer.Write(MarginSeparator);
                        if (alignmentEnabled && (column.Alignment == TableColumnAlign.Left || column.Alignment == TableColumnAlign.Center))
                        {
                            renderer.Write(AlignmentChar);
                        }
                        renderer.Write(HeaderSeparator);
                        if (alignmentEnabled && (column.Alignment == TableColumnAlign.Right || column.Alignment == TableColumnAlign.Center))
                        {
                            renderer.Write(AlignmentChar);
                        }
                        renderer.Write(MarginSeparator);
                        renderer.Write(PipeSeparator);
                    }

                    renderer.WriteLine();
                }
            }

            renderer.FinishBlock(true);
        }
    }
}
