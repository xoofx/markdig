// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Normalize
{
    public class LinkReferenceDefinitionRenderer : NormalizeObjectRenderer<LinkReferenceDefinition>
    {
        protected override void Write(NormalizeRenderer renderer, LinkReferenceDefinition linkDef)
        {
            renderer.RenderLinesBefore(linkDef);

            renderer.Write(linkDef.BeforeWhitespace);
            renderer.Write('[');            
            renderer.Write(linkDef.LabelWithWhitespace);
            renderer.Write("]:");

            renderer.Write(linkDef.WhitespaceBeforeUrl);
            if (linkDef.UrlHasPointyBrackets)
            {
                renderer.Write('<');
            }
            renderer.Write(linkDef.Url);
            if (linkDef.UrlHasPointyBrackets)
            {
                renderer.Write('>');
            }

            renderer.Write(linkDef.WhitespaceBeforeTitle);
            if (linkDef.Title != null)
            {
                var open = linkDef.TitleEnclosingCharacter;
                var close = linkDef.TitleEnclosingCharacter;
                if (linkDef.TitleEnclosingCharacter == '(')
                {
                    close = ')';
                }
                renderer.Write(open);
                renderer.Write(linkDef.Title.Replace("\"", "\\\"")); // TODO: RTP: should this always be done?
                renderer.Write(close);
            }
            renderer.Write(linkDef.AfterWhitespace);

            renderer.RenderLinesAfter(linkDef);
        }
    }
}