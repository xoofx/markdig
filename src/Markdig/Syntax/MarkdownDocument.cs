// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Syntax;

/// <summary>
/// The root Markdown document.
/// </summary>
/// <seealso cref="ContainerBlock" />
public class MarkdownDocument : ContainerBlock
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MarkdownDocument"/> class.
    /// </summary>
    public MarkdownDocument() : base(null)
    {
    }

    /// <summary>
    /// Gets the number of lines in this <see cref="MarkdownDocument"/>
    /// </summary>
    public int LineCount;

    /// <summary>
    /// Gets a list of zero-based indexes of line beginnings in the source span
    /// <para>Available if <see cref="MarkdownPipelineBuilder.PreciseSourceLocation"/> is used, otherwise null</para>
    /// </summary>
    public List<int>? LineStartIndexes { get; set; }
}