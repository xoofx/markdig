using System.Text;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax.Inlines
{
    public class LiteralInline : LeafInline
    {
        public LiteralInline()
        {
            Content = new StringSlice(null);
        }

        public StringSlice Content;

        protected override void Close(InlineParserState state)
        {
        }

        public override string ToString()
        {
            return Content.ToString();
        }
    }
}