// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;

namespace Textamina.Markdig.Extensions.CustomContainers
{
    /// <summary>
    /// A HTML renderer for a <see cref="CustomContainer"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Renderers.Html.HtmlObjectRenderer{CustomContainer}" />
    public class HtmlCustomContainerRenderer : HtmlObjectRenderer<CustomContainer>
    {
        protected override void Write(HtmlRenderer renderer, CustomContainer obj)
        {
            renderer.EnsureLine();
            renderer.Write("<div").WriteAttributes(obj).Write(">");
            // We don't escape a CustomContainer
            renderer.WriteLeafRawLines(obj, true, false);
            renderer.WriteLine("</div>");
        }
    }
}