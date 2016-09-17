// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.Tables
{
    /// <summary>
    /// The delimiter used to separate the columns of a pipe table.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.Inlines.DelimiterInline" />
    public class PipeTableDelimiterInline : DelimiterInline
    {
        public PipeTableDelimiterInline(InlineParser parser) : base(parser)
        {
        }

        /// <summary>
        /// Gets or sets the index of line where this delimiter was found relative to the current block.
        /// </summary>
        public int LocalLineIndex { get; set; }

        public override string ToLiteral()
        {
            return "|";
        }
    }
}