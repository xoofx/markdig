// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Renderers.Normalize;

namespace Markdig.Extensions.Footnotes;

/// <summary>
/// A Normalize renderer for a <see cref="FootnoteLink"/>. It writes a reference such as `[^1]`, and nothing for a back link, which the parser adds.
/// </summary>
/// <seealso cref="NormalizeObjectRenderer{FootnoteLink}" />
public class NormalizeFootnoteLinkRenderer : NormalizeObjectRenderer<FootnoteLink>
{
    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(NormalizeRenderer renderer, FootnoteLink obj)
    {
        if (obj.IsBackLink)
        {
            return;
        }

        renderer.Write('[').Write(obj.Footnote.Label).Write(']');
    }
}
