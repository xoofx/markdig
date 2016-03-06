// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Extensions.Tables
{
    /// <summary>
    /// A grid table.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Extensions.Tables.TableBlock" />
    public class GridTableBlock : TableBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridTableBlock"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        public GridTableBlock(BlockParser parser) : base(parser)
        {
        }

        // Store the parser state of the grid table here.
        internal GridTableBlockState State { get; set; } 
    }
}