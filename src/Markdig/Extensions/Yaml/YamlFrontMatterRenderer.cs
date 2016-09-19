// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Extensions.Yaml
{
    /// <summary>
    /// Empty renderer for a <see cref="YamlFrontMatterBlock"/>
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{YamlFrontMatterBlock}" />
    public class YamlFrontMatterRenderer : HtmlObjectRenderer<YamlFrontMatterBlock>
    {
        protected override void Write(HtmlRenderer renderer, YamlFrontMatterBlock obj)
        {
        }
    }
}