// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace Textamina.Markdig.Extensions.Tables
{
    /// <summary>
    /// A grid table.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Extensions.Tables.TableBlock" />
    public class GridTableBlock : TableBlock
    {
        // Store the parser state of the grid table here.
        internal GridTableParserState State { get; set; } 
    }
}