// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Footnotes
{
    /// <summary>
    /// A block for a footnote.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.ContainerBlock" />
    public class Footnote : ContainerBlock
    {
        public Footnote(BlockParser parser) : base(parser)
        {
            Links = new List<FootnoteLink>();
            Order = -1;
        }

        /// <summary>
        /// Gets or sets the label used by this footnote.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the order of this footnote (determined by the order of the <see cref="FootnoteLink"/> in the document)
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets the links referencing this footnote.
        /// </summary>
        public List<FootnoteLink> Links { get; private set; }

        /// <summary>
        /// The label span
        /// </summary>
        public SourceSpan LabelSpan;

        internal bool IsLastLineEmpty { get; set; }
    }
}