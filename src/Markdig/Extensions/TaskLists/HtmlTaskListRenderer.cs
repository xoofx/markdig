// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.TaskLists
{
    /// <summary>
    /// A HTML renderer for a <see cref="TaskList"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{TaskList}" />
    public class HtmlTaskListRenderer : HtmlObjectRenderer<TaskList>
    {
        protected override void Write(HtmlRenderer renderer, TaskList obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("<input").WriteAttributes(obj).Write(" disabled=\"disabled\" type=\"checkbox\"");
                if (obj.Checked)
                {
                    renderer.Write(" checked=\"checked\"");
                }
                renderer.Write(" />");
            }
            else
            {
                renderer.Write('[');
                renderer.Write(obj.Checked ? "x" : " ");
                renderer.Write(']');
            }
        }
    }
}