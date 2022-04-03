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
    /// <seealso cref="HtmlObjectRenderer{CodeBlock}" />
    public class CodeBlockRenderer : HtmlObjectRenderer<CodeBlock>
    {
        private HashSet<string>? _blocksAsDiv;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeBlockRenderer"/> class.
        /// </summary>
        public CodeBlockRenderer() => IsInternalRenderer = GetType() == typeof(CodeBlockRenderer);

        public bool OutputAttributesOnPre { get; set; }

        /// <summary>
        /// Gets a map of fenced code block infos that should be rendered as div blocks instead of pre/code blocks.
        /// </summary>
        public HashSet<string> BlocksAsDiv => _blocksAsDiv ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        protected override void Write(HtmlRenderer renderer, CodeBlock obj)
        {
            renderer.EnsureLine();

            if (_blocksAsDiv is not null && (obj as FencedCodeBlock)?.Info is string info && _blocksAsDiv.Contains(info))
            {
                var infoPrefix = (obj.Parser as FencedCodeBlockParser)?.InfoPrefix ??
                                 FencedCodeBlockParser.DefaultInfoPrefix;

                // We are replacing the HTML attribute `language-mylang` by `mylang` only for a div block
                // NOTE that we are allocating a closure here

                if (renderer.EnableHtmlForBlock)
                {
                    renderer.Write("<div")
                            .WriteAttributes(obj.TryGetAttributes(),
                                cls => cls.StartsWith(infoPrefix, StringComparison.Ordinal) ? cls.Substring(infoPrefix.Length) : cls)
                            .WriteRaw('>');
                }

                renderer.WriteLeafRawLines(obj, true, true, true);

                if (renderer.EnableHtmlForBlock)
                {
                    renderer.WriteLine("</div>");
                }
            }
            else
            {
                if (renderer.EnableHtmlForBlock)
                {
                    renderer.Write("<pre");

                    if (OutputAttributesOnPre)
                    {
                        renderer.WriteAttributes(obj);
                    }

                    renderer.WriteRaw("><code");

                    if (!OutputAttributesOnPre)
                    {
                        renderer.WriteAttributes(obj);
                    }

                    renderer.WriteRaw('>');
                }

                renderer.WriteLeafRawLines(obj, true, true);

                if (renderer.EnableHtmlForBlock)
                {
                    renderer.WriteLine("</code></pre>");
                }
            }

            renderer.EnsureLine();
        }
    }
}