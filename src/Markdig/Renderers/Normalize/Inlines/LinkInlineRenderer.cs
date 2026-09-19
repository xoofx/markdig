// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Normalize.Inlines;

/// <summary>
/// A Normalize renderer for a <see cref="LinkInline"/>.
/// </summary>
/// <seealso cref="NormalizeObjectRenderer{LinkInline}" />
public class LinkInlineRenderer : NormalizeObjectRenderer<LinkInline>
{
    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(NormalizeRenderer renderer, LinkInline link)
    {
        if (link.IsAutoLink && !renderer.Options.ExpandAutoLinks)
        {
            renderer.Write(link.Url);
            return;
        }

        if (link.IsImage)
        {
            renderer.Write('!');
        }
        renderer.Write('[');
        renderer.WriteChildren(link);
        renderer.Write(']');

        if (link.Label != null)
        {
            if (link.FirstChild is LiteralInline literal && literal.Content.Length == link.Label.Length && literal.Content.Match(link.Label))
            {
                // collapsed reference and shortcut links
                if (!link.IsShortcut)
                {
                    renderer.Write("[]");
                }
            }
            else
            {
                // full link
                renderer.Write('[').Write(renderer.EscapeTablePipes ? link.Label.Replace("|", "\\|") : link.Label).Write(']');
            }
        }
        else
        {
            if (link.Url is { Length: > 0 } url)
            {
                renderer.Write('(').Write(renderer.EscapeTablePipes
                    ? url.Replace("\\", "\\\\").Replace("|", "\\|") : url);

                if (link.Title is { Length: > 0 })
                {
                    renderer.Write(" \"");
                    var title = renderer.EscapeTablePipes
                        ? link.Title.Replace("\\", "\\\\").Replace("|", "\\|") : link.Title;
                    renderer.Write(title.Replace(@"""", @"\"""));
                    renderer.Write('"');
                }

                renderer.Write(')');
            }
        }
    }
}
