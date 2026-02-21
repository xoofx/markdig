// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Roundtrip;

/// <summary>
/// Represents the EmptyBlockRenderer type.
/// </summary>
public class EmptyBlockRenderer : RoundtripObjectRenderer<EmptyBlock>
{
    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(RoundtripRenderer renderer, EmptyBlock  noBlocksFoundBlock)
    {
        renderer.RenderLinesAfter(noBlocksFoundBlock);
    }
}
