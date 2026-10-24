// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.TableOfContents;

/// <summary>
/// An HTML renderer for a <see cref="TableOfContentsBlock"/>. It writes a `nav` element with the `doc-toc` role around a nested list of the entries, and nothing if there are no entries.
/// </summary>
/// <remarks>
/// An entry contains the plain text of its heading. Dropping the formatting keeps the entries uniform and keeps links and element IDs from the heading content, such as footnote references, out of the table of contents.
/// If the heading has an ID, the entry text links to it. The link is written with <see cref="HtmlRenderer.WriteEscapeUrl"/>, so <see cref="HtmlRenderer.BaseUrl"/> and <see cref="HtmlRenderer.LinkRewriter"/> apply as they do to Markdown links.
/// </remarks>
/// <seealso href="https://www.w3.org/TR/dpub-aria-1.1/#doc-toc"/>
public class HtmlTableOfContentsRenderer : HtmlObjectRenderer<TableOfContentsBlock>
{
    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(HtmlRenderer renderer, TableOfContentsBlock obj)
    {
        renderer.EnsureLine();
        if (obj.Entries.Count == 0)
        {
            return;
        }

        if (!renderer.EnableHtmlForBlock)
        {
            WritePlainText(renderer, obj.Entries);
            return;
        }

        renderer.Write("<nav").WriteAttributes(obj).WriteLine('>');
        WriteList(renderer, obj.Entries, obj.ListKind == TableOfContentsListKind.Ordered);
        renderer.WriteLine("</nav>");
    }

    private static void WriteList(HtmlRenderer renderer, IReadOnlyList<TableOfContentsEntry> entries, bool ordered)
    {
        renderer.WriteLine(ordered ? "<ol>" : "<ul>");
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            renderer.Write("<li>");
            string? id = entry.Heading.TryGetAttributes()?.Id;
            if (string.IsNullOrEmpty(id))
            {
                WriteEntryText(renderer, entry);
            }
            else
            {
                renderer.Write("<a href=\"").WriteEscapeUrl("#" + id).WriteRaw("\">");
                WriteEntryText(renderer, entry);
                renderer.WriteRaw("</a>");
            }

            if (entry.Children.Count > 0)
            {
                renderer.WriteLine();
                WriteList(renderer, entry.Children, ordered);
            }
            renderer.WriteLine("</li>");
        }
        renderer.WriteLine(ordered ? "</ol>" : "</ul>");
    }

    private static void WritePlainText(HtmlRenderer renderer, IReadOnlyList<TableOfContentsEntry> entries)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            WriteEntryText(renderer, entries[i]);
            renderer.EnsureLine();
            WritePlainText(renderer, entries[i].Children);
        }
    }

    private static void WriteEntryText(HtmlRenderer renderer, TableOfContentsEntry entry)
    {
        bool enableHtmlForInline = renderer.EnableHtmlForInline;
        renderer.EnableHtmlForInline = false;
        renderer.WriteLeafInline(entry.Heading);
        renderer.EnableHtmlForInline = enableHtmlForInline;
    }
}
