// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Html
{
    /// <summary>
    /// A HTML renderer for a <see cref="QuoteBlock"/>.
    /// </summary>
    /// <seealso cref="HtmlObjectRenderer{QuoteBlock}" />
    public class QuoteBlockRenderer : HtmlObjectRenderer<QuoteBlock>
    {
        protected override void Write(HtmlRenderer renderer, QuoteBlock obj)
        {
            renderer.EnsureLine();
            if (renderer.EnableHtmlForBlock)
            {
                renderer.Write("<blockquote").WriteAttributes(obj).WriteLine(">");
            }
            var savedImplicitParagraph = renderer.ImplicitParagraph;
            renderer.ImplicitParagraph = false;
            renderer.WriteChildren(obj);
            renderer.ImplicitParagraph = savedImplicitParagraph;
            if (renderer.EnableHtmlForBlock)
            {
                renderer.WriteLine("</blockquote>");
            }
            renderer.EnsureLine();
        }
    }
}