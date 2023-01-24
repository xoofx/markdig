// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Roundtrip.Inlines;

/// <summary>
/// A Normalize renderer for a <see cref="LiteralInline"/>.
/// </summary>
/// <seealso cref="RoundtripObjectRenderer{LiteralInline}" />
public class LiteralInlineRenderer : RoundtripObjectRenderer<LiteralInline>
{
    protected override void Write(RoundtripRenderer renderer, LiteralInline obj)
    {
        if (obj.IsFirstCharacterEscaped && obj.Content.Length > 0 && obj.Content[obj.Content.Start].IsAsciiPunctuation())
        {
            renderer.Write('\\');
        }
        renderer.Write(ref obj.Content);
    }
}