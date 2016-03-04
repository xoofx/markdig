// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    public class QuoteBlockRenderer : HtmlObjectRenderer<QuoteBlock>
    {
        protected override void Write(HtmlRenderer renderer, QuoteBlock obj)
        {
            renderer.EnsureLine();
            renderer.Write("<blockquote").WriteAttributes(obj).WriteLine(">");
            var savedImplicitParagraph = renderer.ImplicitParagraph;
            renderer.ImplicitParagraph = false;
            renderer.WriteChildren(obj);
            renderer.ImplicitParagraph = savedImplicitParagraph;
            renderer.WriteLine("</blockquote>");
        }
    }
}