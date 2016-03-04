// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax.Inlines
{
    /// <summary>
    /// A delimiter used for parsing emphasis.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Syntax.Inlines.DelimiterInline" />
    public class EmphasisDelimiterInline : DelimiterInline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmphasisDelimiterInline"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public EmphasisDelimiterInline(InlineParser parser) : base(parser)
        {
        }

        /// <summary>
        /// The delimiter character found.
        /// </summary>
        public char DelimiterChar { get; set; }

        /// <summary>
        /// The number of delimiter characters found for this delimiter.
        /// </summary>
        public int DelimiterCount { get; set; }

        public override string ToLiteral()
        {
            return DelimiterCount > 0 ? new string(DelimiterChar, DelimiterCount) : string.Empty;
        }
    }
}