using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public class AutolineInlineParser : InlineParser
    {
        public static readonly AutolineInlineParser Default = new AutolineInlineParser();

        public AutolineInlineParser()
        {
            OpeningCharacters = new[] {'<'};
        }

        public override bool Match(InlineParserState state, ref StringSlice text)
        {
            string link;
            bool isEmail;
            var saved = text;
            if (LinkHelper.TryParseAutolink(ref text, out link, out isEmail))
            {
                state.Inline = new AutolinkInline() {IsEmail = isEmail, Url = link};
            }
            else
            {
                text = saved;
                string htmlTag;
                if (!HtmlHelper.TryParseHtmlTag(ref text, out htmlTag))
                {
                    return false;
                }

                state.Inline = new HtmlInline() { Tag = htmlTag };
            }

            return true;
        }
    }
}