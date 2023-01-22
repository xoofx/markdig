// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Roundtrip.Inlines;

/// <summary>
/// A Normalize renderer for an <see cref="AutolinkInline"/>.
/// </summary>
/// <seealso cref="RoundtripObjectRenderer{AutolinkInline}" />
public class AutolinkInlineRenderer : RoundtripObjectRenderer<AutolinkInline>
{
    protected override void Write(RoundtripRenderer renderer, AutolinkInline obj)
    {
        renderer.Write('<').Write(obj.Url).Write('>');
    }
}