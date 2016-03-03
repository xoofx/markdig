using System;
using Textamina.Markdig.Extensions.Footnotes;
using Textamina.Markdig.Parsers.Inlines;

namespace Textamina.Markdig.Extensions
{
    public class SoftlineBreakAsHardlineExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipeline pipeline)
        {
            var parser = pipeline.InlineParsers.Find<LineBreakInlineParser>();
            if (parser != null)
            {
                parser.EnableSoftAsHard = true;
            }
        }
    }
}