using System.Collections.Generic;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions
{
    public class TableRowBlock : ContainerBlock
    {
        public TableRowBlock() : base(null)
        {
        }

        public bool IsHeader { get; set; }

        public List<TableColumnAlignType> ColumnAlignments { get; set; }
    }
}