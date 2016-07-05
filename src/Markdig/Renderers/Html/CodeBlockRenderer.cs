// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Markdig.Renderers.Html
{
    /// <summary>
    /// An HTML renderer for a <see cref="CodeBlock"/> and <see cref="FencedCodeBlock"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Html.HtmlObjectRenderer{Markdig.Syntax.CodeBlock}" />
    public class CodeBlockRenderer : HtmlObjectRenderer<CodeBlock>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeBlockRenderer"/> class.
        /// </summary>
        public CodeBlockRenderer()
        {
            BlocksAsDiv = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public bool OutputAttributesOnPre { get; set; }

        /// <summary>
        /// Gets a map of fenced code block infos that should be rendered as div blocks instead of pre/code blocks.
        /// </summary>
        public HashSet<string> BlocksAsDiv { get; }

        protected override void Write(HtmlRenderer renderer, CodeBlock obj)
        {
            renderer.EnsureLine();

            var fencedCodeBlock = obj as FencedCodeBlock;
            if (fencedCodeBlock?.Info != null && BlocksAsDiv.Contains(fencedCodeBlock.Info))
            {
                var infoPrefix = (obj.Parser as FencedCodeBlockParser)?.InfoPrefix ??
                                 FencedCodeBlockParser.DefaultInfoPrefix;

                // We are replacing the HTML attribute `language-mylang` by `mylang` only for a div block
                // NOTE that we are allocating a closure here
                renderer.Write("<div")
                    .WriteAttributes(obj.TryGetAttributes(),
                        cls => cls.StartsWith(infoPrefix) ? cls.Substring(infoPrefix.Length) : cls)
                    .Write(">");
                renderer.WriteLeafRawLines(obj, true, true, true);
                renderer.WriteLine("</div>");

            }
            else
            {
                renderer.Write("<pre");
                if (OutputAttributesOnPre)
                {
                    renderer.WriteAttributes(obj);
                }
                renderer.Write("><code");
                if (!OutputAttributesOnPre)
                {
                    renderer.WriteAttributes(obj);
                }
                renderer.Write(">");
                renderer.WriteLeafRawLines(obj, true, true);
                renderer.WriteLine("</code></pre>");
            }
        }
    }
}