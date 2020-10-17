// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Normalize.Inlines
{
    /// <summary>
    /// A Normalize renderer for a <see cref="LinkInline"/>.
    /// </summary>
    /// <seealso cref="NormalizeObjectRenderer{LinkInline}" />
    public class LinkInlineRenderer : NormalizeObjectRenderer<LinkInline>
    {
        protected override void Write(NormalizeRenderer renderer, LinkInline link)
        {
            //foreach (var child in link)
            //{
            //    if (child is LiteralInline li)
            //    {
            //        renderer.Write(li.Content.Text);
            //    }    
            //}
            //return;
            if (link.IsImage)
            {
                renderer.Write('!');
            }
            renderer.Write('[');
            renderer.WriteChildren(link);
            renderer.Write(']');

            if (link.Label != null)
            {
                if (link.FirstChild is LiteralInline literal &&
                    literal.Content.Length == link.Label.Length &&
                    literal.Content.Match(link.Label))
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
                    renderer.Write('[').Write(link.LabelWithWhitespace).Write(']');
                }
            }
            else
            {
                if (link.Url != null)
                {
                    renderer.Write('(');
                    renderer.Write(link.WhitespaceBeforeUrl);
                    if (link.UrlHasPointyBrackets)
                    {
                        renderer.Write('<');
                    }
                    renderer.Write(link.Url);
                    if (link.UrlHasPointyBrackets)
                    {
                        renderer.Write('>');
                    }
                    renderer.Write(link.WhitespaceAfterUrl);

                    if (!string.IsNullOrEmpty(link.Title))
                    {
                        var open = link.TitleEnclosingCharacter;
                        var close = link.TitleEnclosingCharacter;
                        if (link.TitleEnclosingCharacter == '(')
                        {
                            close = ')';
                        }
                        renderer.Write(open);
                        renderer.Write(link.Title.Replace(@"""", @"\""")); // TODO: RTP: should this always be done?
                        renderer.Write(close);
                        renderer.Write(link.WhitespaceAfterTitle);
                    }

                    renderer.Write(')');
                }
            }
        }
    }
}