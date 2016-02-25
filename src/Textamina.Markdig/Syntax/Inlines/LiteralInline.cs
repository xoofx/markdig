using System.Text;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax.Inlines
{
    public class LiteralInline : LeafInline
    {
        public LiteralInline()
        {
            IsClosable = true;
        }

        public StringBuilder ContentBuilder { get; set; }

        public string Content { get; set; }

        public bool TrimEnd { get; set; }

        protected override void Close(InlineParserState state)
        {
            var str = ContentBuilder.ToString();
            if (TrimEnd)
            {
                str = str.TrimEnd();
            }
            Content = HtmlHelper.Unescape(str, false);
            state.StringBuilders.Release(ContentBuilder);
            ContentBuilder = null;
        }

        public override string ToString()
        {
            return Content ?? (ContentBuilder != null ? ContentBuilder.ToString() : string.Empty);
        }
    }
}