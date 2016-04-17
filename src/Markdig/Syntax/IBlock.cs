// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// Base interface for a block structure. Either a <see cref="LeafBlock"/> or a <see cref="ContainerBlock"/>.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.IMarkdownObject" />
    public interface IBlock : IMarkdownObject
    {
        /// <summary>
        /// Gets or sets the text column this instance was declared (zero-based).
        /// </summary>
        int Column { get; set; }

        /// <summary>
        /// Gets or sets the text line this instance was declared (zero-based).
        /// </summary>
        int Line { get; set; }

        /// <summary>
        /// Gets the parent of this container. May be null.
        /// </summary>
        ContainerBlock Parent { get; }

        /// <summary>
        /// Gets the parser associated to this instance.
        /// </summary>
        BlockParser Parser { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is still open.
        /// </summary>
        bool IsOpen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this block is breakable. Default is true.
        /// </summary>
        bool IsBreakable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this block must be removed from its container after inlines have been processed.
        /// </summary>
        bool RemoveAfterProcessInlines { get; set; }

        /// <summary>
        /// Occurs when the process of inlines begin.
        /// </summary>
        event ProcessInlineDelegate ProcessInlinesBegin;

        /// <summary>
        /// Occurs when the process of inlines ends for this instance.
        /// </summary>
        event ProcessInlineDelegate ProcessInlinesEnd;
    }
}