using System;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Renderers.Html;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.Attributes
{
    public class AttributesExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            if (!pipeline.InlineParsers.Contains<AttributesParser>())
            {
                pipeline.InlineParsers.Insert(0, new AttributesParser());
            }
            var fencedParser = pipeline.BlockParsers.Find<FencedCodeBlockParser>();
            if (fencedParser != null)
            {
                InstallInfoParserForFenced(fencedParser);
            }
        }

        private static void InstallInfoParserForFenced(FencedCodeBlockParser parser)
        {
            var infoParser = parser.InfoParser;

            parser.InfoParser = (BlockParserState state, ref StringSlice line, FencedCodeBlock fenced) =>
            {
                // Try to find if there is any attributes { in the info string on the first line of a FencedCodeBlock
                var indexOfAttributes = line.Text.IndexOf('{', line.Start);
                if (indexOfAttributes >= 0)
                {
                    // Work on a copy
                    var copy = line;
                    copy.Start = indexOfAttributes;
                    AttributesInline attributes;
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