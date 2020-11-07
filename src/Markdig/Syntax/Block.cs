// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using System.Collections.Generic;

namespace Markdig.Syntax
{
    /// <summary>
    /// Base class for a block structure. Either a <see cref="LeafBlock"/> or a <see cref="ContainerBlock"/>.
    /// </summary>
    /// <seealso cref="MarkdownObject" />
    public abstract class Block : MarkdownObject, IBlock
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
        /// The last newline of this block
        /// </summary>
        public Newline Newline { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this block must be removed from its container after inlines have been processed.
        /// </summary>
        public bool RemoveAfterProcessInlines { get; set; }

        /// <summary>
        /// Gets or sets the whitespace right before this block.
        /// Trivia: only parsed when <see cref="MarkdownParser.TrackTrivia"/> is enabled, otherwise
        /// <see cref="StringSlice.IsEmpty"/>.
        /// </summary>
        public StringSlice WhitespaceBefore { get; set; }

        /// <summary>
        /// Gets or sets whitespace occurring after this block.
        /// Trivia: only parsed when <see cref="MarkdownParser.TrackTrivia"/> is enabled, otherwise
        /// <see cref="StringSlice.IsEmpty"/>.
        /// </summary>
        public StringSlice WhitespaceAfter { get; set; }

        /// <summary>
        /// Gets or sets the empty lines occurring before this block.
        /// Trivia: only parsed when <see cref="MarkdownParser.TrackTrivia"/> is enabled, otherwise null.
        /// </summary>
        public List<StringSlice> LinesBefore { get; set; }

        /// <summary>
        /// Gets or sets the empty lines occurring after this block.
        /// Trivia: only parsed when <see cref="MarkdownParser.TrackTrivia"/> is enabled, otherwise null.
        /// </summary>
        public List<StringSlice> LinesAfter { get; set; }

        /// <summary>
        /// Occurs when the process of inlines begin.
        /// </summary>
        public event ProcessInlineDelegate ProcessInlinesBegin;

        /// <summary>
        /// Occurs when the process of inlines ends for this instance.
        /// </summary>
        public event ProcessInlineDelegate ProcessInlinesEnd;

        /// <summary>
        /// Called when the process of inlines begin.
        /// </summary>
        /// <param name="state">The inline parser state.</param>
        internal void OnProcessInlinesBegin(InlineProcessor state)
        {
            ProcessInlinesBegin?.Invoke(state, null);
        }

        /// <summary>
        /// Called when the process of inlines ends.
        /// </summary>
        /// <param name="state">The inline parser state.</param>
        internal void OnProcessInlinesEnd(InlineProcessor state)
        {
            ProcessInlinesEnd?.Invoke(state, null);
        }

        public void UpdateSpanEnd(int spanEnd)
        {
            // Update parent spans
            var parent = this;
            while (parent != null)
            {
                if (spanEnd > parent.Span.End)
                {
                    parent.Span.End = spanEnd;
                }
                parent = parent.Parent;
            }
        }

        internal static Block FindRootMostContainerParent(Block block)
        {
            if (block.Parent is ContainerBlock && !(block.Parent is MarkdownDocument))
            {
                return FindRootMostContainerParent(block.Parent);
            }
            return block;
        }
    }
}