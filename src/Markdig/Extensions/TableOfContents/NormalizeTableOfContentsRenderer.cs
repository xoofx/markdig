// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers.Normalize;

namespace Markdig.Extensions.TableOfContents;

/// <summary>
/// A Normalize renderer for a <see cref="TableOfContentsBlock"/>. It writes the `[TOC]` marker back.
/// </summary>
public class NormalizeTableOfContentsRenderer : NormalizeObjectRenderer<TableOfContentsBlock>
{
    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(NormalizeRenderer renderer, TableOfContentsBlock obj)
    {
        renderer.Write(TableOfContentsExtension.Marker);
        renderer.FinishBlock(!renderer.CompactParagraph);
    }
}
