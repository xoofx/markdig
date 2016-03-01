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