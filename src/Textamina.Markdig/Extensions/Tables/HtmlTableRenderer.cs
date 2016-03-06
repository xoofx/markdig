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
            bool hasAlreadyHeader = false;
            bool isHeaderOpen = false;

            foreach (var rowObj in tableBlock.Children)
            {
                var row = (TableRowBlock)rowObj;
                if (row.IsHeader)
                {
                    // Allow a single thead
                    if (!hasAlreadyHeader)
                    {
                        renderer.WriteLine("<thead>");
                        isHeaderOpen = true;
                    }
                    hasAlreadyHeader = true;
                }
                else if (!hasBody)
                {
                    if (isHeaderOpen)
                    {
                        renderer.WriteLine("</thead>");
                        isHeaderOpen = false;
                    }

                    renderer.WriteLine("<tbody>");
                    hasBody = true;
                }
                renderer.WriteLine("<tr>");
                for (int i = 0; i < row.Children.Count; i++)
                {
                    var cellObj = row.Children[i];
                    var cell = (TableCellBlock)cellObj;

                    renderer.EnsureLine();
                    renderer.Write(row.IsHeader ? "<th" : "<td");
                    if (cell.ColumnSpan != 1)
                    {
                        renderer.Write($" colspan=\"{cell.ColumnSpan}\"");
                    }

                    if (tableBlock.ColumnAlignments != null && i < tableBlock.ColumnAlignments.Count)
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
                    renderer.WriteAttributes(cell);
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
            }

            if (hasBody)
            {
                renderer.WriteLine("</tbody>");
            }
            else if (isHeaderOpen)
            {
                renderer.WriteLine("</thead>");
            }
            renderer.WriteLine("</table>");
        }
    }
}