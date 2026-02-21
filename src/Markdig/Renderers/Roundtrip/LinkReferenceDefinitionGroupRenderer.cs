// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Roundtrip;

/// <summary>
/// Represents the LinkReferenceDefinitionGroupRenderer type.
/// </summary>
public class LinkReferenceDefinitionGroupRenderer : RoundtripObjectRenderer<LinkReferenceDefinitionGroup>
{
    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(RoundtripRenderer renderer, LinkReferenceDefinitionGroup obj)
    {
        renderer.WriteChildren(obj);
    }
}
