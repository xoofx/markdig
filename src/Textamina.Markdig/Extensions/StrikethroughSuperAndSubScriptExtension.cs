// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;
using Textamina.Markdig.Parsers.Inlines;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html.Inlines;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions
{
    public class StrikethroughSuperAndSubScriptExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            var parser = pipeline.InlineParsers.Find<EmphasisInlineParser>();
            if (parser != null)
            {
                var chars = new List<char>(parser.OpeningCharacters);
                if (!chars.Contains('~'))
                {
                    chars.Add('~');
                }
                if (!chars.Contains('^'))
                {
                    chars.Add('^');
                }
                parser.OpeningCharacters = chars.ToArray();
            }

            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                var emphasisRenderer = htmlRenderer.ObjectRenderers.Find<EmphasisInlineRenderer>();
                if (emphasisRenderer != null)
                {
                    var previousTag = emphasisRenderer.GetTag;
                    emphasisRenderer.GetTag = inline => GetTag(inline) ?? previousTag(inline);
                }
            }
        }

        private string GetTag(EmphasisInline emphasisInline)
        {
            var c = emphasisInline.DelimiterChar;
            if (c == '~')
            {
                return emphasisInline.Strong ? "del" : "sub";
            }
            else if (c == '^')
            {
                return "sup";
            }
            return null;
        }
    }
}