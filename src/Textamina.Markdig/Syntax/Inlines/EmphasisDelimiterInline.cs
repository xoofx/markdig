// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax.Inlines
{
    public class EmphasisDelimiterInline : DelimiterInline
    {
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