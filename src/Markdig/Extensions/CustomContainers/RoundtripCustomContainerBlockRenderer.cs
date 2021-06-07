// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;

namespace Markdig.Extensions.CustomContainers
{
    /// <summary>
    /// An Normalize renderer for a <see cref="CodeBlock"/> and <see cref="FencedCodeBlock"/>.
    /// </summary>
    /// <seealso cref="NormalizeObjectRenderer{TObject}" />
    public class RoundtripCustomContainerBlockRenderer : NormalizeObjectRenderer<CustomContainer>
    {
        protected override void Write(NormalizeRenderer renderer, CustomContainer customContainer)
        {
            var fencedCharCount = Math.Min(customContainer.OpeningFencedCharCount, customContainer.ClosingFencedCharCount);
            var opening = new string(customContainer.FencedChar, fencedCharCount);
            renderer.Write(opening);
            if (customContainer.Info != null)
            {
                renderer.Write(customContainer.Info);
            }
            if (!string.IsNullOrEmpty(customContainer.Arguments))
            {
                renderer.Write(' ').Write(customContainer.Arguments);
            }

            /* TODO do we need this causes a empty space and would render html attributes to markdown.
            var attributes = obj.TryGetAttributes();
            if (attributes != null)
            {
                renderer.Write(' ');
                renderer.Write(attributes);
            }
            */
            renderer.WriteLine();

            renderer.WriteChildren(customContainer);

            renderer.Write(opening);

            renderer.FinishBlock(renderer.Options.EmptyLineAfterCodeBlock);
        }
    }
}