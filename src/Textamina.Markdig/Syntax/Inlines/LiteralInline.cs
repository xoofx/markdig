using System.Text;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class LiteralInline : LeafInline
    {
        public static readonly InlineParser Parser = new ParserInternal();
        public LiteralInline()
        {
            IsClosable = true;
        }

        public StringBuilder ContentBuilder { get; set; }

        public string Content { get; set; }

        protected override void Close(InlineParserState state)
        {
            Content = HtmlHelper.Unescape(ContentBuilder.ToString(), false);
            state.StringBuilders.Release(ContentBuilder);
            ContentBuilder = null;
        }

        private class ParserInternal : InlineParser
        {
            public override bool Match(InlineParserState state)
            {
                // A literal will always match
                var literal = state.Inline as LiteralInline;
                StringBuilder builder;
                if (literal == null)
                {
                    builder = state.StringBuilders.Get();
                    literal = new LiteralInline {ContentBuilder = builder};
                    state.Inline = literal;
                }
                else
                {
                    builder = literal.ContentBuilder;
                }

                var text = state.Text;
                builder.Append(text.CurrentChar);
                text.NextChar();
                return true;
            }
        }

        public override string ToString()
        {
            return Content ?? (ContentBuilder != null ? ContentBuilder.ToString() : string.Empty);
        }
    }
}