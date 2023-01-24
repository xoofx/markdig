// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Html;

/// <summary>
/// A HTML renderer for a <see cref="ListBlock"/>.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{ListBlock}" />
public class ListRenderer : HtmlObjectRenderer<ListBlock>
{
    protected override void Write(HtmlRenderer renderer, ListBlock listBlock)
    {
        renderer.EnsureLine();
        if (renderer.EnableHtmlForBlock)
        {
            if (listBlock.IsOrdered)
            {
                renderer.Write("<ol");
                if (listBlock.BulletType != '1')
                {
                    renderer.WriteRaw(" type=\"");
                    renderer.WriteRaw(listBlock.BulletType);
                    renderer.WriteRaw('"');
                }

                if (listBlock.OrderedStart is not null && listBlock.OrderedStart != "1")
                {
                    renderer.Write(" start=\"");
                    renderer.WriteRaw(listBlock.OrderedStart);
                    renderer.WriteRaw('"');
                }
                renderer.WriteAttributes(listBlock);
                renderer.WriteLine('>');
            }
            else
            {
                renderer.Write("<ul");
                renderer.WriteAttributes(listBlock);
                renderer.WriteLine('>');
            }
        }

        foreach (var item in listBlock)
        {
            var listItem = (ListItemBlock)item;
            var previousImplicit = renderer.ImplicitParagraph;
            renderer.ImplicitParagraph = !listBlock.IsLoose;

            renderer.EnsureLine();
            if (renderer.EnableHtmlForBlock)
            {
                renderer.Write("<li");
                renderer.WriteAttributes(listItem);
                renderer.WriteRaw('>');
            }

            renderer.WriteChildren(listItem);

            if (renderer.EnableHtmlForBlock)
            {
                renderer.WriteLine("</li>");
            }

            renderer.EnsureLine();
            renderer.ImplicitParagraph = previousImplicit;
        }

        if (renderer.EnableHtmlForBlock)
        {
            renderer.WriteLine(listBlock.IsOrdered ? "</ol>" : "</ul>");
        }

        renderer.EnsureLine();
    }
}