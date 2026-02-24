// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Html.Inlines;

/// <summary>
/// A HTML renderer for an <see cref="AutolinkInline"/>.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{AutolinkInline}" />
public class AutolinkInlineRenderer : HtmlObjectRenderer<AutolinkInline>
{
    /// <summary>
    /// Gets or sets the literal string in property rel for links
    /// </summary>
    public string? Rel { get; set; }

    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(HtmlRenderer renderer, AutolinkInline obj)
    {
        if (renderer.EnableHtmlForInline)
        {
            renderer.Write(obj.IsEmail ? "<a href=\"mailto:" : "<a href=\"");
            renderer.WriteEscapeUrl(obj.Url);
            renderer.WriteRaw('"');
            renderer.WriteAttributes(obj);

            if (!obj.IsEmail && !string.IsNullOrWhiteSpace(Rel))
            {
                renderer.WriteRaw(" rel=\"");
                renderer.WriteRaw(Rel);
                renderer.WriteRaw('"');
            }

            renderer.WriteRaw('>');
        }

        renderer.WriteEscape(obj.Url);

        if (renderer.EnableHtmlForInline)
        {
            renderer.WriteRaw("</a>");
        }
    }
}
