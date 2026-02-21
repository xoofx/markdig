// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers.Normalize;

namespace Markdig.Extensions.JiraLinks;

/// <summary>
/// Represents the NormalizeJiraLinksRenderer type.
/// </summary>
public class NormalizeJiraLinksRenderer : NormalizeObjectRenderer<JiraLink>
{
    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(NormalizeRenderer renderer, JiraLink obj)
    {
        renderer.Write(obj.ProjectKey);
        renderer.Write("-");
        renderer.Write(obj.Issue);
    }
}
