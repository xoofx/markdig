// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Normalize.Inlines
{
    /// <summary>
    /// A Normalize renderer for a <see cref="CodeInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Normalize.NormalizeObjectRenderer{Markdig.Syntax.Inlines.CodeInline}" />
    public class CodeInlineRenderer : NormalizeObjectRenderer<CodeInline>
    {
        protected override void Write(NormalizeRenderer renderer, CodeInline obj)
        {
            var delimiter = obj.Content.Contains(obj.Delimiter + "") ? new string(obj.Delimiter, 2) : obj.Delimiter + "";

            renderer.Write(delimiter);
            renderer.Write(obj.Content);
            renderer.Write(delimiter);
        }
    }
}