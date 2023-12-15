// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Html;

/// <summary>
/// An HTML renderer for a <see cref="HeadingBlock"/>.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{HeadingBlock}" />
public class HeadingRenderer : HtmlObjectRenderer<HeadingBlock>
{
    private static readonly string[] HeadingTexts = [
        "h1",
        "h2",
        "h3",
        "h4",
        "h5",
        "h6",
    ];

    protected override void Write(HtmlRenderer renderer, HeadingBlock obj)
    {
        int index = obj.Level - 1;
        string[] headings = HeadingTexts;
        string headingText = ((uint)index < (uint)headings.Length)
            ? headings[index]
            : $"h{obj.Level}";

        if (renderer.EnableHtmlForBlock)
        {
            renderer.Write('<');
            renderer.WriteRaw(headingText);
            renderer.WriteAttributes(obj);
            renderer.WriteRaw('>');
        }

        renderer.WriteLeafInline(obj);

        if (renderer.EnableHtmlForBlock)
        {
            renderer.Write("</");
            renderer.WriteRaw(headingText);
            renderer.WriteLine('>');
        }

        renderer.EnsureLine();
    }
}