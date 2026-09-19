using System.Linq;
using Markdig.Renderers.Normalize;

namespace Markdig.Extensions.Tables;

/// <summary>
/// A Normalize renderer for a <see cref="Table"/> in normalized form.
/// </summary>
/// <seealso cref="NormalizeObjectRenderer{Table}" />
public class NormalizeTableRenderer : NormalizeObjectRenderer<Table>
{
    private const string PipeSeparator = "|";
    private const string AlignmentChar = ":";
    private const string MarginSeparator = " ";

    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(NormalizeRenderer renderer, Table obj)
    {
        renderer.EnsureLine();

        bool firstRow = true;

        foreach (var row in obj.OfType<TableRow>())
        {
            if (!firstRow)
            {
                renderer.WriteLine();
            }
            firstRow = false;

            renderer.Write(PipeSeparator);

            foreach (var tableCell in row)
            {
                renderer.Write(MarginSeparator);

                renderer.Render(tableCell);

                renderer.Write(MarginSeparator);
                renderer.Write(PipeSeparator);
            }

            if (row.IsHeader)
            {
                renderer.WriteLine();

                renderer.Write(PipeSeparator);

                for (var columnIndex = 0; columnIndex < row.Count; columnIndex++)
                {
                    var column = columnIndex < obj.ColumnDefinitions.Count ? obj.ColumnDefinitions[columnIndex] : null;
                    // Match the HTML renderer's alignment fallback for expanded rows.
                    var alignment = obj.ColumnDefinitions.Count > 0
                        ? obj.ColumnDefinitions[Math.Min(columnIndex, obj.ColumnDefinitions.Count - 1)].Alignment
                        : null;
                    renderer.Write(MarginSeparator);
                    if (alignment == TableColumnAlign.Left || alignment == TableColumnAlign.Center)
                    {
                        renderer.Write(AlignmentChar);
                    }
                    renderer.Write('-', column is { SeparatorDashCount: > 0 } ? column.SeparatorDashCount : 3);
                    if (alignment == TableColumnAlign.Right || alignment == TableColumnAlign.Center)
                    {
                        renderer.Write(AlignmentChar);
                    }
                    renderer.Write(MarginSeparator);
                    renderer.Write(PipeSeparator);
                }
            }
        }

        renderer.FinishBlock(true);
    }
}
