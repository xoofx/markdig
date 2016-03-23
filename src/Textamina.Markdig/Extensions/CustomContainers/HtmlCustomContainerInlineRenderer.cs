// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;

namespace Textamina.Markdig.Extensions.CustomContainers
{
    /// <summary>
    /// A HTML renderer for a <see cref="CustomContainerInline"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Renderers.Html.HtmlObjectRenderer{CustomContainerInline}" />
    public class HtmlCustomContainerInlineRenderer : HtmlObjectRenderer<CustomContainerInline>
    {
        protected override void Write(HtmlRenderer renderer, CustomContainerInline obj)
        {
            renderer.Write("<span").WriteAttributes(obj).Write(">");
            renderer.WriteChildren(obj);
            renderer.Write("</span>");
        }
    }
}