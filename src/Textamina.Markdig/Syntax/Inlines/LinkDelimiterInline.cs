using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class LinkDelimiterInline : DelimiterInline
    {
        public LinkDelimiterInline(InlineParser parser) : base(parser)
        {
        }

        public bool IsImage;

        public override string ToLiteral()
        {
            return IsImage ? "![" : "[";
        }
    }
}