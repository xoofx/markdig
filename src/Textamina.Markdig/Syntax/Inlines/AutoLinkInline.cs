using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class AutolinkInline : LeafInline
    {
        public static readonly InlineParser Parser = new ParserInternal();

        public bool IsEmail { get; set; }

        public string Url { get; set; }

        private class ParserInternal : InlineParser
        {
            public ParserInternal()
            {
                FirstChars = new[] {'<'};
            }

            public override bool Match(InlineParserState state)
            {
                var text = state.Lines;

                string link;
                bool isEmail;
                var saved = text.Save();
                if (LinkHelper.TryParseAutolink(text, out link, out isEmail))
                {
                    state.Inline = new AutolinkInline() {IsEmail = isEmail, Url = link};
                }
                else
                {
                    text.Restore(ref saved);
                    string htmlTag;
                    if (!HtmlHelper.TryParseHtmlTag(text, out htmlTag))
                    {
                        return false;
                    }

                    state.Inline = new HtmlInline() { Tag = htmlTag };
                }

                return true;
            }
        }

        public override string ToString()
        {
            return Url;
        }
    }
}