// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Extensions.TableOfContents;

/// <summary>
/// An entry of a <see cref="TableOfContentsBlock"/>: a heading and the entries of the headings nested below it.
/// </summary>
public sealed class TableOfContentsEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableOfContentsEntry"/> class.
    /// </summary>
    /// <param name="heading">The heading that the entry lists.</param>
    /// <exception cref="ArgumentNullException"><paramref name="heading"/> is <c>null</c>.</exception>
    public TableOfContentsEntry(HeadingBlock heading)
    {
        if (heading is null) ThrowHelper.ArgumentNullException(nameof(heading));

        Heading = heading;
    }

    /// <summary>
    /// Gets the heading that the entry lists.
    /// </summary>
    public HeadingBlock Heading { get; }

    /// <summary>
    /// Gets the entries of the headings nested below <see cref="Heading"/>.
    /// </summary>
    public List<TableOfContentsEntry> Children { get; } = [];
}
