// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using Markdig.Parsers;
using Markdig.Parsers.Inlines;

namespace Markdig.Syntax.Inlines
{
    /// <summary>
    /// A delimiter used for parsing emphasis.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.Inlines.DelimiterInline" />
    public class EmphasisDelimiterInline : DelimiterInline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmphasisDelimiterInline" /> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="descriptor">The descriptor.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public EmphasisDelimiterInline(InlineParser parser, EmphasisDescriptor descriptor) : base(parser)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            Descriptor = descriptor;
            DelimiterChar = descriptor.Character;
        }

        /// <summary>
        /// Gets the descriptor for this emphasis.
        /// </summary>
        public EmphasisDescriptor Descriptor { get; }

        /// <summary>
        /// The delimiter character found.
        /// </summary>
        public char DelimiterChar { get; }

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