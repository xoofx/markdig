// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Footers
{
    /// <summary>
    /// A HTML renderer for a <see cref="FooterBlock"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Markdig.Extensions.Footers.FooterBlock}" />
    public class HtmlFooterBlockRenderer : HtmlObjectRenderer<FooterBlock>
    {
        protected override void Write(HtmlRenderer renderer, FooterBlock footer)
        {
            renderer.EnsureLine();
            renderer.Write("<footer").WriteAttributes(footer).Write(">");
            var implicitParagraph = renderer.ImplicitParagraph;
            renderer.ImplicitParagraph = true;
            renderer.WriteChildren(footer);
            renderer.ImplicitParagraph = implicitParagraph;
            renderer.WriteLine("</footer>");
        }
    }
}