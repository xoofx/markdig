using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax.Inlines
{
    public class LinkDelimiterInline : DelimiterInline
    {
        public LinkDelimiterInline(InlineParser parser) : base(parser)
        {
        }

        public bool IsImage;

        public string Label;

        public override string ToLiteral()
        {
            return IsImage ? "![" : "[";
        }
    }
}