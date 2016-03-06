// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;

namespace Textamina.Markdig.Extensions.Tables
{
    /// <summary>
    /// A HTML renderer for a <see cref="TableBlock"/>
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Renderers.Html.HtmlObjectRenderer{Textamina.Markdig.Extensions.Tables.TableBlock}" />
    public class HtmlTableRenderer : HtmlObjectRenderer<TableBlock>
    {
        protected override void Write(HtmlRenderer renderer, TableBlock tableBlock)
        {
            renderer.EnsureLine();
            renderer.Write("<table").WriteAttributes(tableBlock).WriteLine(">");

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
                    renderer.WriteLine("<thead>");
                }
                else if (!hasBody)
                {
                    renderer.WriteLine("<tbody>");
                    hasBody = true;
                }
                renderer.WriteLine("<tr>");
                for (int i = 0; i < row.Children.Count; i++)
                {
                    var cellObj = row.Children[i];
                    var cell = (TableCellBlock)cellObj;

                    renderer.EnsureLine();
                    if (row.IsHeader)
                    {
                        renderer.Write("<th");
                    }
                    else
                    {
                        renderer.Write("<td");
                    }
                    if (header != null && i < tableBlock.ColumnAlignments.Count)
                    {
                        switch (tableBlock.ColumnAlignments[i])
                        {
                            case TableColumnAlign.Center:
                                renderer.Write(" style=\"text-align: center;\"");
                                break;
                            case TableColumnAlign.Right:
                                renderer.Write(" style=\"text-align: right;\"");
                                break;
                        }
                    }
                    renderer.Write(">");

                    var previousImplicitParagraph = renderer.ImplicitParagraph;
                    if (cell.Children.Count == 1)
                    {
                        renderer.ImplicitParagraph = true;
                    }
                    renderer.Write(cell);
                    renderer.ImplicitParagraph = previousImplicitParagraph;
                    
                    renderer.WriteLine(row.IsHeader ? "</th>" : "</td>");
                }
                renderer.WriteLine("</tr>");
                if (row.IsHeader)
                {
                    renderer.WriteLine("</thead>");
                }
            }

            if (hasBody)
            {
                renderer.WriteLine("</tbody>");
            }
            renderer.WriteLine("</table>");
        }
    }
}