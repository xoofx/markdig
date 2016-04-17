// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Syntax;

namespace Markdig.Renderers.Html
{
    /// <summary>
    /// A HTML renderer for a <see cref="ParagraphBlock"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Markdig.Syntax.ParagraphBlock}" />
    public class ParagraphRenderer : HtmlObjectRenderer<ParagraphBlock>
    {
        protected override void Write(HtmlRenderer renderer, ParagraphBlock obj)
        {
            if (!renderer.ImplicitParagraph)
            {
                if (!renderer.IsFirstInContainer)
                {
                    renderer.EnsureLine();
                }
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