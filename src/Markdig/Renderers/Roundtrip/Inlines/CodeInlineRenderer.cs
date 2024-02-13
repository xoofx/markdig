// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Roundtrip.Inlines;

/// <summary>
/// A Normalize renderer for a <see cref="CodeInline"/>.
/// </summary>
/// <seealso cref="RoundtripObjectRenderer{CodeInline}" />
public class CodeInlineRenderer : RoundtripObjectRenderer<CodeInline>
{
    protected override void Write(RoundtripRenderer renderer, CodeInline obj)
    {
        renderer.Write(obj.Delimiter, obj.DelimiterCount);
        if (!obj.ContentSpan.IsEmpty)
        {
            renderer.Write(obj.ContentWithTrivia);
        }
        renderer.Write(obj.Delimiter, obj.DelimiterCount);
    }
}