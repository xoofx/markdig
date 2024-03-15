// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Linq;
using Markdig.Extensions.Alerts;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Markdig.Extensions.Bootstrap;

/// <summary>
/// A HTML renderer for a <see cref="AlertBlock"/> that adds bootstrap classes.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{AlertBlock}" />
public class BootstrapAlertRenderer : HtmlObjectRenderer<AlertBlock>
{
    /// <summary>
    /// Creates a new instance of this renderer.
    /// </summary>
    public BootstrapAlertRenderer() { }

    /// <inheritdoc />
    protected override void Write(HtmlRenderer renderer, AlertBlock obj)
    {
        AddAttributes(obj);
        var lastParagraph = obj.Descendants().OfType<ParagraphBlock>().LastOrDefault();
        lastParagraph?.GetAttributes().AddProperty("style", "margin-bottom: 0");

        renderer.EnsureLine();
        if (renderer.EnableHtmlForBlock)
        {
            renderer.Write("<div");
            renderer.WriteAttributes(obj);
            renderer.WriteLine('>');
        }

        var savedImplicitParagraph = renderer.ImplicitParagraph;
        renderer.ImplicitParagraph = false;
        renderer.WriteChildren(obj);
        renderer.ImplicitParagraph = savedImplicitParagraph;
        if (renderer.EnableHtmlForBlock)
        {
            renderer.WriteLine("</div>");
        }

        renderer.EnsureLine();
    }

    private static void AddAttributes(AlertBlock obj)
    {
        var attributes = obj.GetAttributes();
        attributes.AddClass("alert");
        attributes.AddProperty("role", "alert");

        string? @class = obj.Kind.AsSpan() switch
        {
            "NOTE" => "alert-primary",
            "TIP" => "alert-success",
            "IMPORTANT" => "alert-info",
            "WARNING" => "alert-warning",
            "CAUTION" => "alert-danger",
            _ => null
        };

        if (@class is not null)
        {
            attributes.AddClass(@class);
        }
    }
}