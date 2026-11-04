// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers.Normalize;

namespace Markdig.Extensions.CustomContainers;

/// <summary>
/// A Normalize renderer for a <see cref="CustomContainer"/>. It writes the content between an opening fence with the info and arguments and a closing fence of the same length.
/// </summary>
/// <seealso cref="NormalizeObjectRenderer{CustomContainer}" />
public class NormalizeCustomContainerRenderer : NormalizeObjectRenderer<CustomContainer>
{
    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(NormalizeRenderer renderer, CustomContainer obj)
    {
        char fencedChar = obj.FencedChar == '\0' ? ':' : obj.FencedChar;
        int fencedCharCount = Math.Max(obj.OpeningFencedCharCount, 3);

        renderer.EnsureLine();
        renderer.Write(fencedChar, fencedCharCount);
        if (!string.IsNullOrEmpty(obj.Info))
        {
            renderer.Write(' ').Write(obj.Info);
        }
        if (!string.IsNullOrEmpty(obj.Arguments))
        {
            renderer.Write(' ').Write(obj.Arguments);
        }
        renderer.WriteLine();

        renderer.WriteChildren(obj);

        // The closing fence is written even if the source had none, so that the content ends where the container ends
        renderer.EnsureLine();
        renderer.Write(fencedChar, fencedCharCount);
        renderer.FinishBlock(true);
    }
}
