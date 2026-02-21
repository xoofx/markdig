// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Footnotes;

/// <summary>
/// A HTML renderer for a <see cref="FootnoteLink"/>.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{FootnoteLink}" />
public class HtmlFootnoteLinkRenderer : HtmlObjectRenderer<FootnoteLink>
{
    /// <summary>
    /// Initializes a new instance of the HtmlFootnoteLinkRenderer class.
    /// </summary>
    public HtmlFootnoteLinkRenderer()
    {
        BackLinkString = "&#8617;";
        FootnoteLinkClass = "footnote-ref";
        FootnoteBackLinkClass = "footnote-back-ref";
    }
    /// <summary>
    /// Gets or sets the back link string.
    /// </summary>
    public string BackLinkString { get; set; }

    /// <summary>
    /// Gets or sets the footnote link class.
    /// </summary>
    public string FootnoteLinkClass { get; set; }

    /// <summary>
    /// Gets or sets the footnote back link class.
    /// </summary>
    public string FootnoteBackLinkClass { get; set; }

    /// <summary>
    /// Writes the object to the specified renderer.
    /// </summary>
    protected override void Write(HtmlRenderer renderer, FootnoteLink link)
    {
        var order = link.Footnote.Order;
        renderer.Write(link.IsBackLink
            ? $"<a href=\"#fnref:{link.Index}\" class=\"{FootnoteBackLinkClass}\">{BackLinkString}</a>"
            : $"<a id=\"fnref:{link.Index}\" href=\"#fn:{order}\" class=\"{FootnoteLinkClass}\"><sup>{order}</sup></a>");
    }
}
