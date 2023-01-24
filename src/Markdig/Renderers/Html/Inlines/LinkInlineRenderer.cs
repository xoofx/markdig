// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Html.Inlines;

/// <summary>
/// A HTML renderer for a <see cref="LinkInline"/>.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{LinkInline}" />
public class LinkInlineRenderer : HtmlObjectRenderer<LinkInline>
{
    /// <summary>
    /// Gets or sets a value indicating whether to always add rel="nofollow" for links or not.
    /// </summary>
    [Obsolete("AutoRelNoFollow is obsolete. Please write \"nofollow\" into Property Rel.")]
    public bool AutoRelNoFollow
    {
        get
        {
            return Rel is not null && Rel.Contains("nofollow");
        }
        set
        {
            const string rel = "nofollow";
            if (value)
            {
                if (string.IsNullOrEmpty(Rel))
                    Rel = rel;
                else if (!Rel!.Contains(rel))
                    Rel += $" {rel}";
            }
            else if (!value && Rel is not null)
            {
                Rel = Rel.Replace(rel, string.Empty);
            }
        }
    }

    /// <summary>
    /// Gets or sets the literal string in property rel for links
    /// </summary>
    public string? Rel { get; set; }

    protected override void Write(HtmlRenderer renderer, LinkInline link)
    {
        if (renderer.EnableHtmlForInline)
        {
            renderer.Write(link.IsImage ? "<img src=\"" : "<a href=\"");
            renderer.WriteEscapeUrl(link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url);
            renderer.WriteRaw('"');
            renderer.WriteAttributes(link);
        }
        if (link.IsImage)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.WriteRaw(" alt=\"");
            }
            var wasEnableHtmlForInline = renderer.EnableHtmlForInline;
            renderer.EnableHtmlForInline = false;
            renderer.WriteChildren(link);
            renderer.EnableHtmlForInline = wasEnableHtmlForInline;
            if (renderer.EnableHtmlForInline)
            {
                renderer.WriteRaw('"');
            }
        }

        if (renderer.EnableHtmlForInline && !string.IsNullOrEmpty(link.Title))
        {
            renderer.WriteRaw(" title=\"");
            renderer.WriteEscape(link.Title);
            renderer.WriteRaw('"');
        }

        if (link.IsImage)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.WriteRaw(" />");
            }
        }
        else
        {
            if (renderer.EnableHtmlForInline)
            {
                if (!string.IsNullOrWhiteSpace(Rel))
                {
                    renderer.WriteRaw(" rel=\"");
                    renderer.WriteRaw(Rel);
                    renderer.WriteRaw('"');
                }
                renderer.WriteRaw('>');
            }
            renderer.WriteChildren(link);
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("</a>");
            }
        }
    }
}