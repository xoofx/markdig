// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Parsers;

namespace Markdig.Syntax
{
    /// <summary>
    /// A list (Section 5.3 CommonMark specs)
    /// </summary>
    /// <seealso cref="Markdig.Syntax.ContainerBlock" />
    public class ListBlock : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser used to create this block.</param>
        public ListBlock(BlockParser parser) : base(parser)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the list is ordered.
        /// </summary>
        public bool IsOrdered { get; set; }

        /// <summary>
        /// Gets or sets the bullet character used by this list.
        /// </summary>
        public char BulletType { get; set; }

        /// <summary>
        /// Gets or sets the ordered start number (valid when <see cref="IsOrdered"/> is <c>true</c>)
        /// </summary>
        public string OrderedStart { get; set; }

        /// <summary>
        /// Gets or sets the default ordered start ("1" for BulletType = '1')
        /// </summary>
        public string DefaultOrderedStart { get; set; }

        /// <summary>
        /// Gets or sets the ordered delimiter character (usually `.` or `)`) found after an ordered list item.
        /// </summary>
        public char OrderedDelimiter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loose.
        /// </summary>
        public bool IsLoose { get; set; }

        internal int CountAllBlankLines { get; set; }

        internal int CountBlankLinesReset { get; set; }
    }
}