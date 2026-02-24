// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Text;

namespace Testamina.Markdig.Benchmarks.PipeTable;

/// <summary>
/// Generates pipe table markdown content for benchmarking purposes.
/// </summary>
public static class PipeTableGenerator
{
    private const int DefaultCellWidth = 10;

    /// <summary>
    /// Generates a pipe table in markdown format.
    /// </summary>
    /// <param name="rows">Number of data rows (excluding header)</param>
    /// <param name="columns">Number of columns</param>
    /// <param name="cellWidth">Width of each cell content (default: 10)</param>
    /// <returns>Pipe table markdown string</returns>
    public static string Generate(int rows, int columns, int cellWidth = DefaultCellWidth)
    {
        var sb = new StringBuilder();

        // Header row
        sb.Append('|');
        for (int col = 0; col < columns; col++)
        {
            sb.Append(' ');
            sb.Append($"Header {col + 1}".PadRight(cellWidth));
            sb.Append(" |");
        }
        sb.AppendLine();

        // Separator row (with dashes)
        sb.Append('|');
        for (int col = 0; col < columns; col++)
        {
            sb.Append(new string('-', cellWidth + 2));
            sb.Append('|');
        }
        sb.AppendLine();

        // Data rows
        for (int row = 0; row < rows; row++)
        {
            sb.Append('|');
            for (int col = 0; col < columns; col++)
            {
                sb.Append(' ');
                sb.Append($"R{row + 1}C{col + 1}".PadRight(cellWidth));
                sb.Append(" |");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
