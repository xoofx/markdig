// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.TableOfContents;

/// <summary>
/// A table of contents that lists the headings of the document. The <see cref="TableOfContentsExtension"/> creates it in place of a paragraph that consists of `[TOC]` alone.
/// </summary>
public sealed class TableOfContentsBlock : LeafBlock
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableOfContentsBlock"/> class.
    /// </summary>
    /// <param name="parser">The parser used to create this block, or <c>null</c> if no parser created it.</param>
    public TableOfContentsBlock(BlockParser? parser) : base(parser)
    {
    }

    /// <summary>
    /// Gets or sets the kind of list that the table of contents is rendered as.
    /// </summary>
    public TableOfContentsListKind ListKind { get; set; }

    /// <summary>
    /// Gets or sets the top-level entries. The <see cref="TableOfContentsExtension"/> sets them after the inlines of the document are processed, and every table of contents of a document shares them.
    /// </summary>
    public IReadOnlyList<TableOfContentsEntry> Entries { get; set; } = [];
}
