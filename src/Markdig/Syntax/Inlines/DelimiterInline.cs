// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Diagnostics;
using Markdig.Helpers;
using Markdig.Parsers;

namespace Markdig.Syntax.Inlines
{
    /// <summary>
    /// Internal delimiter used by some parsers (e.g emphasis, tables).
    /// </summary>
    /// <seealso cref="Markdig.Syntax.Inlines.ContainerInline" />
    [DebuggerDisplay("{ToLiteral()} {Type}")]
    public abstract class DelimiterInline : ContainerInline
    {
        protected DelimiterInline(InlineParser parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            Parser = parser;
            IsActive = true;
        }

        /// <summary>
        /// Gets the parser.
        /// </summary>
        public InlineParser Parser { get; }

        /// <summary>
        /// Gets or sets the type of this delimiter.
        /// </summary>
        public DelimiterType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Converts this delimiter to a literal.
        /// </summary>
        /// <returns>The string representation of this delimiter</returns>
        public abstract string ToLiteral();

        public void ReplaceByLiteral()
        {
            var literalInline = new LiteralInline()
            {
                Content = new StringSlice(ToLiteral()),
                Span = Span,
                Line = Line,
                Column = Column,
                IsClosed = true
            };
            ReplaceBy(literalInline);
        }
    }
}