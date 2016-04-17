// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.Footnotes
{
    /// <summary>
    /// A inline link to a <see cref="Footnote"/>.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.Inlines.Inline" />
    public class FootnoteLink : Inline
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is back link (from a footnote to the link)
        /// </summary>
        public bool IsBackLink { get; set; }

        /// <summary>
        /// Gets or sets the global index number of this link.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the footnote this link refers to.
        /// </summary>
        public Footnote Footnote { get; set; }
    }
}