// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

namespace Markdig.Extensions.TableOfContents;

/// <summary>
/// Defines the kind of list that a table of contents is rendered as.
/// </summary>
public enum TableOfContentsListKind
{
    /// <summary>
    /// An unordered list, rendered as a `ul` element.
    /// </summary>
    Unordered,

    /// <summary>
    /// An ordered list, rendered as an `ol` element.
    /// </summary>
    Ordered,
}
