// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Renderers.Html;
using Textamina.Markdig.Syntax;


namespace Textamina.Markdig.Extensions.Attributes
{
    /// <summary>
    /// Extension that allows to attach HTML attributes to the previous <see cref="Inline"/> or current <see cref="Block"/>.
    /// This extension should be enabled last after enabling other extensions.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.IMarkdownExtension" />
    public class AttributesExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            if (!pipeline.InlineParsers.Contains<AttributesParser>())
            {
                pipeline.InlineParsers.Insert(0, new AttributesParser());
            }

            // Modify all existing FencedBlockParser
            foreach (var parser in pipeline.BlockParsers)
            {
                var fencedParser = parser as FencedCodeBlockParser;
                if (fencedParser != null)
                {
                    InstallInfoParserForFenced(fencedParser);
                }
            }
        }

        private static void InstallInfoParserForFenced(FencedCodeBlockParser parser)
        {
            // Special case for FencedCodeBlock, as we need to plug into the InfoParser in order
            // to parse correctly an attributes
            var infoParser = parser.InfoParser;

            parser.InfoParser = (BlockProcessor state, ref StringSlice line, FencedCodeBlock fenced) =>
            {
                // Try to find if there is any attributes { in the info string on the first line of a FencedCodeBlock
                var indexOfAttributes = line.Text.IndexOf('{', line.Start);
                if (indexOfAttributes >= 0)
                {
                    // Work on a copy
                    var copy = line;
                    copy.Start = indexOfAttributes;
                    HtmlAttributes attributes;
                    if (AttributesParser.TryParse(ref copy, out attributes))
                    {
                        var htmlAttributes = fenced.GetAttributes();
                        attributes.CopyTo(htmlAttributes);
                        line.End = indexOfAttributes - 1;
                    }
                }

                if (infoParser != null)
                {
                    return infoParser(state, ref line, fenced);
                }
                return true;
            };
        }

    }
}