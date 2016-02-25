using Textamina.Markdig.Parsers;
using Textamina.Markdig.Parsers.Inlines;

namespace Textamina.Markdig.Syntax.Inlines
{
    public class CodeInline : LeafInline
    {
        public static readonly InlineParser Parser = new CodeInlineParser();

        public string Content { get; set; }
    }
}