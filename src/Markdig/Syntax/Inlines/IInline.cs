// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Markdig.Syntax.Inlines
{
    /// <summary>
    /// Base interface for all syntax tree inlines.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.IMarkdownObject" />
    public interface IInline : IMarkdownObject
    {
        /// <summary>
        /// Gets the parent container of this inline.
        /// </summary>
        ContainerInline Parent { get; }

        /// <summary>
        /// Gets the previous inline.
        /// </summary>
        Inline PreviousSibling { get; }

        /// <summary>
        /// Gets the next sibling inline.
        /// </summary>
        Inline NextSibling { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is closed.
        /// </summary>
        bool IsClosed { get; set; }
    }
}