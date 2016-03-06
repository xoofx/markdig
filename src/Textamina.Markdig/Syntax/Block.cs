// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Base class for a block structure. Either a <see cref="LeafBlock"/> or a <see cref="ContainerBlock"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Syntax.MarkdownObject" />
    public abstract class Block : MarkdownObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Block"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        protected Block(BlockParser parser)
        {
            Parser = parser;
            IsOpen = true;
            IsBreakable = true;
        }

        /// <summary>
        /// Gets or sets the text column this instance was declared (zero-based).
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the text line this instance was declared (zero-based).
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Gets the parent of this container. May be null.
        /// </summary>
        public ContainerBlock Parent { get; internal set;  }

        /// <summary>
        /// Gets the parser associated to this instance.
        /// </summary>
        public BlockParser Parser { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is still open.
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this block is breakable. Default is true.
        /// </summary>
        public bool IsBreakable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this block must be removed from its container after inlines have been processed.
        /// </summary>
        public bool RemoveAfterProcessInlines { get; set; }

        /// <summary>
        /// Occurs when the process of inlines begin.
        /// </summary>
        public event Action<InlineParserState> ProcessInlinesBegin;

        /// <summary>
        /// Occurs when the process of inlines ends for this instance.
        /// </summary>
        public event Action<InlineParserState> ProcessInlinesEnd;

        /// <summary>
        /// Called when the process of inlines begin.
        /// </summary>
        /// <param name="state">The inline parser state.</param>
        internal void OnProcessInlinesBegin(InlineParserState state)
        {
            ProcessInlinesBegin?.Invoke(state);
        }

        /// <summary>
        /// Called when the process of inlines ends.
        /// </summary>
        /// <param name="state">The inline parser state.</param>
        internal void OnProcessInlinesEnd(InlineParserState state)
        {
            ProcessInlinesEnd?.Invoke(state);
        }
    }
}