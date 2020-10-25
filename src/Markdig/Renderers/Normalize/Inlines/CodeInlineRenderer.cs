// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Normalize.Inlines
{
    /// <summary>
    /// A Normalize renderer for a <see cref="CodeInline"/>.
    /// </summary>
    /// <seealso cref="NormalizeObjectRenderer{CodeInline}" />
    public class CodeInlineRenderer : NormalizeObjectRenderer<CodeInline>
    {
        protected override void Write(NormalizeRenderer renderer, CodeInline obj)
        {
            var delimiterCount = 0;
            for (var i = 0; i < obj.Content.Length; i++)
            {
                var index = obj.Content.IndexOf(obj.Delimiter, i);
                if (index == -1) break;

                var count = 1;
                for (i = index + 1; i < obj.Content.Length; i++)
                {
                    if (obj.Content[i] == obj.Delimiter) count++;
                    else break;
                }

                if (delimiterCount < count)
                    delimiterCount = count;
            }
            var delimiterRun = new string(obj.Delimiter, delimiterCount + 1);
            renderer.Write(delimiterRun);
            if (obj.Content.Length != 0)
            {
                if (obj.Content[0] == obj.Delimiter)
                {
                    renderer.Write(' ');
                }
                renderer.Write(obj.Content);
                if (obj.Content[obj.Content.Length - 1] == obj.Delimiter)
                {
                    renderer.Write(' ');
                }
            }
            else
            {
                renderer.Write(' ');
            }
            renderer.Write(delimiterRun);
        }
    }
}