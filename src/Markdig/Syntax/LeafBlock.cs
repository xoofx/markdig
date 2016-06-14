// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Diagnostics;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax.Inlines;

namespace Markdig.Syntax
{
    /// <summary>
    /// Base class for all leaf blocks.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.Block" />
    [DebuggerDisplay("{GetType().Name} Line: {Line}, {Lines}")]
    public abstract class LeafBlock : Block
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeafBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        protected LeafBlock(BlockParser parser) : base(parser)
        {
        }

        /// <summary>
        /// Gets or sets the string lines accumulated for this leaf block. 
        /// May be null after process inlines have occured.
        /// </summary>
        public StringLineGroup Lines;

        /// <summary>
        /// Gets or sets the inline syntax tree (may be null).
        /// </summary>
        public ContainerInline Inline { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Lines"/> must be processed
        /// as inline into the <see cref="Inline"/> property.
        /// </summary>
        public bool ProcessInlines { get; set; }

        /// <summary>
        /// Appends the specified line to this instance.
        /// </summary>
        /// <param name="slice">The slice.</param>
        /// <param name="column">The column.</param>
        /// <param name="line">The line.</param>
        /// <param name="sourceLinePosition"></param>
        public void AppendLine(ref StringSlice slice, int column, int line, int sourceLinePosition)
        {
            if (Lines.Lines == null)
            {
                Lines = new StringLineGroup(4);
            }

            var stringLine = new StringLine(ref slice, line, column, sourceLinePosition);
            // Regular case, we are not in the middle of a tab
            if (slice.CurrentChar != '\t' || !CharHelper.IsAcrossTab(column))
            {
                Lines.Add(ref stringLine);
            }
            else
            {
                // We need to expand tabs to spaces
                var builder = StringBuilderCache.Local();
                for (int i = column; i < CharHelper.AddTab(column); i++)
                {
                    builder.Append(' ');
                }
                builder.Append(slice.Text, slice.Start + 1, slice.Length - 1);
                stringLine.Slice = new StringSlice(builder.ToString());
                Lines.Add(ref stringLine);
            }
        }
    }
}