using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;

namespace Textamina.Markdig.Extensions.Tables
{
    public class HtmlTableRenderer : HtmlObjectRenderer<TableBlock>
    {
        protected override void Write(HtmlRenderer writer, TableBlock tableBlock)
        {
            writer.EnsureLine();
            writer.WriteLine("<table>");

            bool hasBody = false;
            var header = (TableRowBlock)tableBlock.Children[0];
            if (!header.IsHeader)
            {
                header = null;
            }
            foreach (var rowObj in tableBlock.Children)
            {
                var row = (TableRowBlock)rowObj;
                if (row.IsHeader)
                {
                    writer.WriteLine("<thead>");
                }
                else if (!hasBody)
                {
                    writer.WriteLine("<tbody>");
                    hasBody = true;
                }
                writer.WriteLine("<tr>");
                for (int i = 0; i < row.Children.Count; i++)
                {
                    var cellObj = row.Children[i];
                    var cell = (TableCellBlock)cellObj;

                    writer.EnsureLine();
                    if (row.IsHeader)
                    {
                        writer.Write("<th>");
                    }
                    else
                    {
                        writer.Write("<td");
                        if (header != null && i < header.ColumnAlignments.Count)
                        {
                            switch (header.ColumnAlignments[i])
                            {
                                case TableColumnAlign.Center:
                                    writer.Write(" style=\"text-align: center;\"");
                                    break;
                                case TableColumnAlign.Right:
                                    writer.Write(" style=\"text-align: right;\"");
                                    break;
                            }
                        }
                        writer.Write(">");
                    }

                    writer.WriteLeafInline(cell);
                    writer.WriteLine(row.IsHeader ? "</th>" : "</td>");
                }
                writer.WriteLine("</tr>");
                if (row.IsHeader)
                {
                    writer.WriteLine("</thead>");
                }
            }

            if (hasBody)
            {
                writer.WriteLine("</tbody>");
            }
        }
    }
}