// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Roundtrip;

public class LinkReferenceDefinitionGroupRenderer : RoundtripObjectRenderer<LinkReferenceDefinitionGroup>
{
    protected override void Write(RoundtripRenderer renderer, LinkReferenceDefinitionGroup obj)
    {
        renderer.WriteChildren(obj);
    }
}