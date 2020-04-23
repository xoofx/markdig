// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Normalize.Inlines
{
    /// <summary>
    /// A Normalize renderer for a <see cref="HtmlInline"/>.
    /// </summary>
    public class NormalizeHtmlInlineRenderer : NormalizeObjectRenderer<HtmlInline>
    {
        protected override void Write(NormalizeRenderer renderer, HtmlInline obj)
        {
            renderer.Write(obj.Tag);
        }
    }
}