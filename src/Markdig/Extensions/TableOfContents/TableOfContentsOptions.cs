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
}
