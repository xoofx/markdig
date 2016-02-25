using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public class AutolineInlineParser : InlineParser
    {
        public AutolineInlineParser()
        {
            OpeningCharacters = new[] {'<'};
        }

        public override bool Match(InlineParserState state)
        {
            string link;
            bool isEmail;
            var saved = state.Text;
            if (LinkHelper.TryParseAutolink(ref state.Text, out link, out isEmail))
            {
                state.Inline = new AutolinkInline() {IsEmail = isEmail, Url = link};
            }
            else
            {
                state.Text = saved;
                string htmlTag;
                if (!HtmlHelper.TryParseHtmlTag(ref state.Text, out htmlTag))
                {
                    return false;
                }

                state.Inline = new HtmlInline() { Tag = htmlTag };
            }

            return true;
        }
    }
}