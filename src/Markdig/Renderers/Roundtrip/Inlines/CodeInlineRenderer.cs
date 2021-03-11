// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Roundtrip.Inlines
{
    /// <summary>
    /// A Normalize renderer for a <see cref="CodeInline"/>.
    /// </summary>
    /// <seealso cref="RoundtripObjectRenderer{CodeInline}" />
    public class CodeInlineRenderer : RoundtripObjectRenderer<CodeInline>
    {
        protected override void Write(RoundtripRenderer renderer, CodeInline obj)
        {
            var delimiterRun = new string(obj.Delimiter, obj.DelimiterCount);
            renderer.Write(delimiterRun);
            if (obj.Content is { Length: > 0 })
            {
                renderer.Write(obj.ContentWithTrivia);
            }
            renderer.Write(delimiterRun);
        }
    }
}