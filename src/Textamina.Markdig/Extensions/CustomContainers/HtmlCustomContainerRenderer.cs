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
            renderer.Write("<div");
            
            // If the custom container has already some attributes, try to use them before adding the class for the target
            var attributes = obj.TryGetAttributes();
            if (attributes == null)
            {
                if (!string.IsNullOrEmpty(obj.Language))
                {
                    renderer.Write(" class=\"");
                    renderer.WriteEscape(obj.Language);
                    renderer.Write("\"");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(obj.Language))
                {
                    attributes.AddClass(obj.Language);
                }
                renderer.WriteAttributes(obj);
            }
            renderer.Write(">");
            renderer.WriteLeafRawLines(obj, true, true);
            renderer.WriteLine("</div>");
        }
    }
}