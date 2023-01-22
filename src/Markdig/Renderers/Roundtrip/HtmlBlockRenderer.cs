// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Roundtrip;

public class HtmlBlockRenderer : RoundtripObjectRenderer<HtmlBlock>
{
    protected override void Write(RoundtripRenderer renderer, HtmlBlock obj)
    {
        renderer.RenderLinesBefore(obj);
        //renderer.Write(obj.BeforeWhitespace); // Lines content is written, including whitespace
        renderer.WriteLeafRawLines(obj);
        renderer.RenderLinesAfter(obj);
    }
}