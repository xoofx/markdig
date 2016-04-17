// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Extensions.Footnotes
{
    /// <summary>
    /// A block that contains all the footnotes at the end of a <see cref="MarkdownDocument"/>.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.ContainerBlock" />
    public class FootnoteGroup : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FootnoteGroup"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public FootnoteGroup(BlockParser parser) : base(parser)
        {
        }

        internal int CurrentOrder { get; set; }
    }
}