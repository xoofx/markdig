// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Renderers.Roundtrip;

public class LinkReferenceDefinitionRenderer : RoundtripObjectRenderer<LinkReferenceDefinition>
{
    protected override void Write(RoundtripRenderer renderer, LinkReferenceDefinition linkDef)
    {
        renderer.RenderLinesBefore(linkDef);

        renderer.Write(linkDef.TriviaBefore);
        renderer.Write('[');            
        renderer.Write(linkDef.LabelWithTrivia);
        renderer.Write("]:");

        renderer.Write(linkDef.TriviaBeforeUrl);
        if (linkDef.UrlHasPointyBrackets)
        {
            renderer.Write('<');
        }
        renderer.Write(linkDef.UnescapedUrl);
        if (linkDef.UrlHasPointyBrackets)
        {
            renderer.Write('>');
        }

        renderer.Write(linkDef.TriviaBeforeTitle);
        if (linkDef.Title != null)
        {
            var open = linkDef.TitleEnclosingCharacter;
            var close = linkDef.TitleEnclosingCharacter;
            if (linkDef.TitleEnclosingCharacter == '(')
            {
                close = ')';
            }
            renderer.Write(open);
            renderer.Write(linkDef.UnescapedTitle);
            renderer.Write(close);
        }
        renderer.Write(linkDef.TriviaAfter);
        renderer.Write(linkDef.NewLine.AsString());

        renderer.RenderLinesAfter(linkDef);
    }
}