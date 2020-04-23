// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Normalize
{
    /// <summary>
    /// An Normalize renderer for a <see cref="CodeBlock"/> and <see cref="FencedCodeBlock"/>.
    /// </summary>
    /// <seealso cref="NormalizeObjectRenderer{CodeBlock}" />
    public class CodeBlockRenderer : NormalizeObjectRenderer<CodeBlock>
    {
        public bool OutputAttributesOnPre { get; set; }

        protected override void Write(NormalizeRenderer renderer, CodeBlock obj)
        {
            if (obj is FencedCodeBlock fencedCodeBlock)
            {
                var opening = new string(fencedCodeBlock.FencedChar, fencedCodeBlock.FencedCharCount);
                renderer.Write(opening);
                if (fencedCodeBlock.Info != null)
                {
                    renderer.Write(fencedCodeBlock.Info);
                }
                if (!string.IsNullOrEmpty(fencedCodeBlock.Arguments))
                {
                    renderer.Write(" ").Write(fencedCodeBlock.Arguments);
                }

                /* TODO do we need this causes a empty space and would render html attributes to markdown.
                var attributes = obj.TryGetAttributes();
                if (attributes != null)
                {
                    renderer.Write(" ");
                    renderer.Write(attributes);
                }
                */
                renderer.WriteLine();

                renderer.WriteLeafRawLines(obj, true);
                renderer.Write(opening);
            }
            else
            {
                renderer.WriteLeafRawLines(obj, false, true);
            }

            renderer.FinishBlock(renderer.Options.EmptyLineAfterCodeBlock);
        }
    }
}