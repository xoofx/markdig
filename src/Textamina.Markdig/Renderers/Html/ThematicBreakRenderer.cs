// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Renderers.Html
{
    public class ThematicBreakRenderer : HtmlObjectRenderer<ThematicBreakBlock>
    {
        protected override void Write(HtmlRenderer renderer, ThematicBreakBlock obj)
        {
            renderer.Write("<hr").WriteAttributes(obj).WriteLine(" />");
        }
    }
}