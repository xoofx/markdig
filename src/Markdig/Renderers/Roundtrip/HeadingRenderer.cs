// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Roundtrip
{
    /// <summary>
    /// An Normalize renderer for a <see cref="HeadingBlock"/>.
    /// </summary>
    /// <seealso cref="NormalizeObjectRenderer{HeadingBlock}" />
    public class HeadingRenderer : RoundtripObjectRenderer<HeadingBlock>
    {
        private static readonly string[] HeadingTexts = {
            "#",
            "##",
            "###",
            "####",
            "#####",
            "######",
        };

        protected override void Write(RoundtripRenderer renderer, HeadingBlock obj)
        {
            if (obj.IsSetext)
            {
                renderer.RenderLinesBefore(obj);

                var headingChar = obj.Level == 1 ? '=' : '-';
                var line = new string(headingChar, obj.HeaderCharCount);

                renderer.WriteLeafInline(obj);
                renderer.WriteLine(obj.SetextNewline);
                renderer.Write(obj.WhitespaceBefore);
                renderer.Write(line);
                renderer.WriteLine(obj.Newline);
                renderer.Write(obj.WhitespaceAfter);

                renderer.RenderLinesAfter(obj);
            }
            else
            {
                renderer.RenderLinesBefore(obj);

                var headingText = obj.Level > 0 && obj.Level <= 6
                    ? HeadingTexts[obj.Level - 1]
                    : new string('#', obj.Level);

                renderer.Write(obj.WhitespaceBefore);
                renderer.Write(headingText);
                renderer.Write(obj.WhitespaceAfterAtxHeaderChar);
                renderer.WriteLeafInline(obj);
                renderer.Write(obj.WhitespaceAfter);
                renderer.WriteLine(obj.Newline);

                renderer.RenderLinesAfter(obj);
            }
        }
    }
}