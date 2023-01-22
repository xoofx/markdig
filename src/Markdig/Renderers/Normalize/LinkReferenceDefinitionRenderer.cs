// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Normalize;

public class LinkReferenceDefinitionRenderer : NormalizeObjectRenderer<LinkReferenceDefinition>
{
    protected override void Write(NormalizeRenderer renderer, LinkReferenceDefinition linkDef)
    {
        renderer.EnsureLine();
        renderer.Write('[');            
        renderer.Write(linkDef.Label);
        renderer.Write("]: ");

        renderer.Write(linkDef.Url);

        if (linkDef.Title != null)
        {
            renderer.Write(" \"");
            renderer.Write(linkDef.Title.Replace("\"", "\\\""));
            renderer.Write('"');
        }
        renderer.FinishBlock(false);
    }
}