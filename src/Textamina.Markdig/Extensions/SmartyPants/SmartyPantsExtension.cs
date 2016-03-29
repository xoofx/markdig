// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Parsers.Inlines;
using Textamina.Markdig.Renderers;

namespace Textamina.Markdig.Extensions.SmartyPants
{
    /// <summary>
    /// Extension to enable SmartyPants.
    /// </summary>
    public class SmartyPantsExtension : IMarkdownExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SmartyPantsExtension"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public SmartyPantsExtension(SmartyPantOptions options)
        {
            Options = options ?? new SmartyPantOptions();
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        public SmartyPantOptions Options { get; }

        public void Setup(MarkdownPipeline pipeline)
        {
            if (!pipeline.InlineParsers.Contains<SmaryPantsInlineParser>())
            {
                // Insert the parser after the code span parser
                pipeline.InlineParsers.InsertAfter<CodeInlineParser>(new SmaryPantsInlineParser());
            }

            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<HtmlSmartyPantRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Add(new HtmlSmartyPantRenderer(Options));
                }
            }
        }
    }
}