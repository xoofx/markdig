// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

namespace Markdig.Extensions.Tables
{
    /// <summary>
    /// Defines a column.
    /// </summary>
    public class TableColumnDefinition
    {
        /// <summary>
        /// Gets or sets the width (in percentage) of this column. A value of 0 is unspecified.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets the column alignment.
        /// </summary>
        public TableColumnAlign? Alignment { get; set; }
    }
}