// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers.Normalize;
using Markdig.Syntax;

namespace Markdig.Extensions.Footnotes;

/// <summary>
/// A Normalize renderer for a <see cref="FootnoteGroup"/>. It writes every footnote as `[^label]: content`, with the continuation lines indented by four spaces.
/// </summary>
/// <remarks>
/// The parser moves the footnotes to the end of the document, sorts them by their first reference and removes the footnotes without references, so they are written in that order.
/// </remarks>
/// <seealso cref="NormalizeObjectRenderer{FootnoteGroup}" />
public class NormalizeFootnoteGroupRenderer : NormalizeObjectRenderer<FootnoteGroup>
{
    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(NormalizeRenderer renderer, FootnoteGroup obj)
    {
        renderer.EnsureLine();
        bool first = true;
        foreach (var block in obj)
        {
            if (block is not Footnote footnote)
            {
                continue;
            }

            if (!first)
            {
                renderer.EnsureLine();
                renderer.WriteLine();
            }
            first = false;
            WriteFootnote(renderer, footnote);
        }
        renderer.FinishBlock(true);
    }

    private static void WriteFootnote(NormalizeRenderer renderer, Footnote footnote)
    {
        renderer.Write('[').Write(footnote.Label).Write("]:");

        // The parser adds a paragraph for the back links if the footnote does not end with one
        int lastIndex = -1;
        for (int i = footnote.Count - 1; i >= 0; i--)
        {
            if (!IsBackLinkParagraph(footnote[i]))
            {
                lastIndex = i;
                break;
            }
        }

        if (lastIndex < 0)
        {
            return;
        }

        renderer.Write(' ');
        renderer.PushIndent("    ");
        bool saveIsLastInContainer = renderer.IsLastInContainer;
        for (int i = 0; i <= lastIndex; i++)
        {
            if (!IsBackLinkParagraph(footnote[i]))
            {
                renderer.IsLastInContainer = i == lastIndex;
                renderer.Write(footnote[i]);
            }
        }
        renderer.IsLastInContainer = saveIsLastInContainer;
        renderer.PopIndent();
    }

    private static bool IsBackLinkParagraph(Block block)
    {
        if (block is not ParagraphBlock paragraph)
        {
            return false;
        }

        for (var inline = paragraph.Inline?.FirstChild; inline is not null; inline = inline.NextSibling)
        {
            if (inline is not FootnoteLink { IsBackLink: true })
            {
                return false;
            }
        }
        return true;
    }
}
