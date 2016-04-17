// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Syntax;

namespace Markdig.Extensions.Tables
{
    /// <summary>
    /// Defines a row in a <see cref="Table"/>, contains <see cref="TableCell"/>, parent is <see cref="Table"/>.
    /// </summary>
    /// <seealso cref="Markdig.Syntax.ContainerBlock" />
    public class TableRow : ContainerBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableRow"/> class.
        /// </summary>
        public TableRow() : base(null)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is header row.
        /// </summary>
        public bool IsHeader { get; set; }
    }
}