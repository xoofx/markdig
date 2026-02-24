// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Normalize;

/// <summary>
/// Represents the HtmlBlockRenderer type.
/// </summary>
public class HtmlBlockRenderer : NormalizeObjectRenderer<HtmlBlock>
{
    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(NormalizeRenderer renderer, HtmlBlock obj)
    {
        renderer.WriteLeafRawLines(obj, true, false);
    }
}
