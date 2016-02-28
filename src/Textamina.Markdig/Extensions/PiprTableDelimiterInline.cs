using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions
{
    public class PiprTableDelimiterInline : DelimiterInline
    {
        public PiprTableDelimiterInline(InlineParser parser) : base(parser)
        {
        }

        public int LineIndex { get; set; }

        public override string ToLiteral()
        {
            return "|";
        }
    }
}