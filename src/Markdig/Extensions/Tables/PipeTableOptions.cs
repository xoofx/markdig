// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

namespace Markdig.Extensions.Tables;

/// <summary>
/// Options for the extension <see cref="PipeTableExtension"/>
/// </summary>
public class PipeTableOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PipeTableOptions"/> class.
    /// </summary>
    public PipeTableOptions()
    {
        RequireHeaderSeparator = true;
        UseHeaderForColumnCount = false;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to require header separator. <c>true</c> by default (Kramdown is using <c>false</c>)
    /// </summary>
    public bool RequireHeaderSeparator { get; set; }

    /// <summary>
    /// Defines whether table should be normalized to the amount of columns as defined in the table header.
    /// <c>false</c> by default
    ///
    /// If <c>true</c>, this will insert empty cells in rows with fewer tables than the header row and remove cells
    /// that are exceeding the header column count.
    /// If <c>false</c>, this will use the row with the most columns to determine how many cells should be inserted
    /// in all other rows (default behavior).
    /// </summary>
    public bool UseHeaderForColumnCount { get; set; }


    /// <summary>
    /// Gets or sets a value indicating whether column widths should be inferred based on the number of dashes
    /// in the header separator row. Each column's width will be proportional to the dash count in its respective column.
    /// </summary>
    public bool InferColumnWidthsFromSeparator { get; set; }
}