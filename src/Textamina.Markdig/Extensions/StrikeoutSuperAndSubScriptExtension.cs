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
    /// <summary>
    /// Extension for strikeout, subscript and superscript.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.IMarkdownExtension" />
    public class StrikeoutSuperAndSubScriptExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            var parser = pipeline.InlineParsers.Find<EmphasisInlineParser>();
            if (parser != null)
            {
                var hasTilde = false;
                var hasSup = false;
                foreach (var emphasis in parser.EmphasisDescriptors)
                {
                    if (emphasis.Character == '~')
                    {
                        hasTilde = true;
                    }
                    if (emphasis.Character == '^')
                    {
                        hasSup = true;
                    }
                }

                if (!hasTilde)
                {
                    parser.EmphasisDescriptors.Add(new EmphasisDescriptor('~', 1, 2, true));
                }
                if (!hasSup)
                {
                    parser.EmphasisDescriptors.Add(new EmphasisDescriptor('^', 1, 1, true));
                }
            }

            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                // Extend the rendering here.
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
                return emphasisInline.IsDouble ? "del" : "sub";
            }
            else if (c == '^')
            {
                return "sup";
            }
            return null;
        }
    }
}