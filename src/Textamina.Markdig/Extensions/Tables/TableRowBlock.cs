// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.Tables
{
    public class TableRowBlock : ContainerBlock
    {
        public TableRowBlock() : base(null)
        {
        }

        public bool IsHeader { get; set; }

        public List<TableColumnAlign> ColumnAlignments { get; set; }
    }
}