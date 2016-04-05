// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Parsers.Inlines;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html.Inlines;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Cites
{
    /// <summary>
    /// Extension for cite ""...""
    /// </summary>
    /// <seealso cref="Textamina.Markdig.IMarkdownExtension" />
    public class CiteExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            var parser = pipeline.InlineParsers.FindExact<EmphasisInlineParser>();
            if (parser != null)
            {
                foreach (var emphasis in parser.EmphasisDescriptors)
                {
                    if (emphasis.Character == '"')
                    {
                        return;
                    }
                }
                parser.EmphasisDescriptors.Add(new EmphasisDescriptor('"', 2, 2, false));
            }

            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                // Extend the rendering here.
                var emphasisRenderer = htmlRenderer.ObjectRenderers.FindExact<EmphasisInlineRenderer>();
                if (emphasisRenderer != null)
                {
                    var previousTag = emphasisRenderer.GetTag;
                    emphasisRenderer.GetTag = inline => GetTag(inline) ?? previousTag(inline);
                }
            }
        }

        private static string GetTag(EmphasisInline emphasisInline)
        {
            return emphasisInline.IsDouble && emphasisInline.DelimiterChar == '"' ? "cite" : null;
        }
    }
}