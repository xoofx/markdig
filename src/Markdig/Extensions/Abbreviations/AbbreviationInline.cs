// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Diagnostics;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.Abbreviations
{
    /// <summary>
    /// The inline abbreviation.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.Inlines.LeafInline" />
    [DebuggerDisplay("{Abbreviation}")]
    public class AbbreviationInline : LeafInline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbbreviationInline"/> class.
        /// </summary>
        public AbbreviationInline()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbbreviationInline"/> class.
        /// </summary>
        /// <param name="abbreviation">The abbreviation.</param>
        public AbbreviationInline(Abbreviation abbreviation)
        {
            Abbreviation = abbreviation;
        }

        public Abbreviation Abbreviation { get; set; }
    }
}