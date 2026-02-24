// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Diagnostics;

namespace Markdig.Syntax.Inlines;

/// <summary>
/// An emphasis and strong emphasis (Section 6.4 CommonMark specs).
/// </summary>
/// <seealso cref="ContainerInline" />
[DebuggerDisplay("{DelimiterChar} Count: {DelimiterCount}")]
public class EmphasisInline : ContainerInline
{
    /// <summary>
    /// Gets or sets the delimiter character of this emphasis.
    /// </summary>
    public char DelimiterChar { get; set; }

    /// <summary>
    /// Gets or sets the number of delimiter characters for this emphasis.
    /// </summary>
    public int DelimiterCount { get; set; }
}