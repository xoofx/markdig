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
            public override bool Match(MatchInlineState state)
            {
                var text = state.Lines;

                string link;
                bool isEmail;
                if (!LinkHelper.TryParseAutolink(text, out link, out isEmail))
                {
                    return false;
                }

                state.Inline = new AutolinkInline() {IsEmail = isEmail, Url = link};
                return true;
            }
        }

        public override string ToString()
        {
            return Url;
        }
    }
}