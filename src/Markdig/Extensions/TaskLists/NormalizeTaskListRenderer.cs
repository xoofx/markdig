// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers.Normalize;

namespace Markdig.Extensions.TaskLists;

public class NormalizeTaskListRenderer : NormalizeObjectRenderer<TaskList>
{
    protected override void Write(NormalizeRenderer renderer, TaskList obj)
    {
        renderer.Write("[");
        renderer.Write(obj.Checked ? "X" : " ");
        renderer.Write("]");
    }
}
