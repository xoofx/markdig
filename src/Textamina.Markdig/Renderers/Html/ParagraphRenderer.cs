// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    public class ParagraphRenderer : HtmlObjectRenderer<ParagraphBlock>
    {
        protected override void Write(HtmlRenderer renderer, ParagraphBlock obj)
        {
            renderer.EnsureLine();
            if (!renderer.ImplicitParagraph)
            {
                renderer.Write("<p").WriteAttributes(obj).Write(">");
            }
            renderer.WriteLeafInline(obj);
            if (!renderer.ImplicitParagraph)
            {
                renderer.WriteLine("</p>");
            }
        }
    }
}