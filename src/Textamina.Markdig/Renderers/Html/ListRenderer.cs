// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Globalization;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    public class ListRenderer : HtmlObjectRenderer<ListBlock>
    {
        protected override void Write(HtmlRenderer renderer, ListBlock listBlock)
        {
            renderer.EnsureLine();
            if (listBlock.IsOrdered)
            {
                renderer.Write("<ol");
                if (listBlock.OrderedStart != 1)
                {
                    renderer.Write(" start=\"");
                    renderer.Write(listBlock.OrderedStart.ToString(CultureInfo.InvariantCulture));
                    renderer.Write("\"");
                }
                renderer.WriteAttributes(listBlock);
                renderer.WriteLine(">");
            }
            else
            {
                renderer.WriteLine("<ul>");
            }
            foreach (var item in listBlock.Children)
            {
                var listItem = (ListItemBlock)item;
                var previousImplicit = renderer.ImplicitParagraph;
                renderer.ImplicitParagraph = !listBlock.IsLoose;

                renderer.EnsureLine();
                renderer.Write("<li").WriteAttributes(listItem).Write(">");
                renderer.WriteChildren(listItem);
                renderer.WriteLine("</li>");

                renderer.ImplicitParagraph = previousImplicit;
            }
            renderer.WriteLine(listBlock.IsOrdered ? "</ol>" : "</ul>");
        }
    }
}