// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Normalize.Inlines
{
    /// <summary>
    /// A Normalize renderer for a <see cref="LinkInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Normalize.NormalizeObjectRenderer{Markdig.Syntax.Inlines.LinkInline}" />
    public class LinkInlineRenderer : NormalizeObjectRenderer<LinkInline>
    {
        protected override void Write(NormalizeRenderer renderer, LinkInline link)
        {
            if (link.IsImage)
            {
                renderer.Write('!');
            }
            renderer.Write('[');
            renderer.WriteChildren(link);
            renderer.Write(']');

            if (link.Label != null)
            {

                var literal = link.FirstChild as LiteralInline;
                if (literal != null && literal.Content.Match(link.Label) && literal.Content.Length == link.Label.Length)
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
                    renderer.Write('[').Write(link.Label).Write(']');
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(link.Url))
                {
                    renderer.Write('(').Write(link.Url);

                    if (!string.IsNullOrEmpty(link.Title))
                    {
                        renderer.Write(" \"");
                        renderer.Write(link.Title.Replace(@"""", @"\"""));
                        renderer.Write("\"");
                    }

                    renderer.Write(')');
                }
            }
        }
    }
}