// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

namespace Markdig.Extensions.TableOfContents;

/// <summary>
/// Options for the <see cref="TableOfContentsExtension"/>.
/// </summary>
public sealed class TableOfContentsOptions
{
    /// <summary>
    /// Gets or sets the kind of list that the table of contents is rendered as (<see cref="TableOfContentsListKind.Unordered"/> by default).
    /// </summary>
    public TableOfContentsListKind ListKind { get; set; } = TableOfContentsListKind.Unordered;

    /// <summary>
    /// Gets or sets the minimum level of the headings to list, where 1 stands for `#` (1 by default). It must be at least 1.
    /// </summary>
    public int MinLevel { get; set; } = 1;

    /// <summary>
    /// Gets or sets the maximum level of the headings to list (6 by default). It must not be less than <see cref="MinLevel"/>. Levels above 6 occur only if <see cref="Parsers.HeadingBlockParser.MaxLeadingCount"/> is raised.
    /// </summary>
    public int MaxLevel { get; set; } = 6;
}
