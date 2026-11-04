// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Normalize;

/// <summary>
/// A Normalize renderer for a <see cref="LinkReferenceDefinitionGroup"/>.
/// </summary>
/// <remarks>
/// A definition that creates its own inline through <see cref="LinkReferenceDefinition.CreateLinkInline"/>, such as the definition of a footnote or of a heading from the auto-identifiers extension, does not come from a `[label]: url` line and is not written.
/// </remarks>
public class LinkReferenceDefinitionGroupRenderer : NormalizeObjectRenderer<LinkReferenceDefinitionGroup>
{
    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(NormalizeRenderer renderer, LinkReferenceDefinitionGroup obj)
    {
        int lastIndex = -1;
        for (int i = obj.Count - 1; i >= 0; i--)
        {
            if (IsWritten(obj[i]))
            {
                lastIndex = i;
                break;
            }
        }

        if (lastIndex < 0)
        {
            return;
        }

        renderer.EnsureLine();
        bool saveIsLastInContainer = renderer.IsLastInContainer;
        for (int i = 0; i <= lastIndex; i++)
        {
            if (IsWritten(obj[i]))
            {
                renderer.IsLastInContainer = i == lastIndex;
                renderer.Write(obj[i]);
            }
        }
        renderer.IsLastInContainer = saveIsLastInContainer;
        renderer.FinishBlock(true);
    }

    private static bool IsWritten(Block block) => block is not LinkReferenceDefinition { CreateLinkInline: not null };
}
