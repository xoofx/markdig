// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Roundtrip.Inlines;

/// <summary>
/// A Normalize renderer for a <see cref="LinkInline"/>.
/// </summary>
/// <seealso cref="RoundtripObjectRenderer{LinkInline}" />
public class LinkInlineRenderer : RoundtripObjectRenderer<LinkInline>
{
    protected override void Write(RoundtripRenderer renderer, LinkInline link)
    {
        if (link.IsImage)
        {
            renderer.Write('!');
        }
        // link text
        renderer.Write('[');
        renderer.WriteChildren(link);
        renderer.Write(']');

        if (link.Label != null)
        {
            if (link.LocalLabel == LocalLabel.Local || link.LocalLabel == LocalLabel.Empty)
            {
                renderer.Write('[');
                if (link.LocalLabel == LocalLabel.Local)
                {
                    renderer.Write(link.LabelWithTrivia);
                }
                renderer.Write(']');
            }
        }
        else
        {
            if (link.Url != null)
            {
                renderer.Write('(');
                renderer.Write(link.TriviaBeforeUrl);
                if (link.UrlHasPointyBrackets)
                {
                    renderer.Write('<');
                }
                renderer.Write(link.UnescapedUrl);
                if (link.UrlHasPointyBrackets)
                {
                    renderer.Write('>');
                }
                renderer.Write(link.TriviaAfterUrl);

                if (!string.IsNullOrEmpty(link.Title))
                {
                    var open = link.TitleEnclosingCharacter;
                    var close = link.TitleEnclosingCharacter;
                    if (link.TitleEnclosingCharacter == '(')
                    {
                        close = ')';
                    }
                    renderer.Write(open);
                    renderer.Write(link.UnescapedTitle);
                    renderer.Write(close);
                    renderer.Write(link.TriviaAfterTitle);
                }

                renderer.Write(')');
            }
        }
    }
}