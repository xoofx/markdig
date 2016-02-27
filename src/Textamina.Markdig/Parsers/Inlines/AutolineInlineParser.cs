using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public class AutolineInlineParser : InlineParser
    {
        public AutolineInlineParser()
        {
            OpeningCharacters = new[] {'<'};
        }

        public override bool Match(InlineParserState state, ref StringSlice slice)
        {
            string link;
            bool isEmail;
            var saved = slice;
            if (LinkHelper.TryParseAutolink(ref slice, out link, out isEmail))
            {
                state.Inline = new AutolinkInline() {IsEmail = isEmail, Url = link};
            }
            else
            {
                slice = saved;
                string htmlTag;
                if (!HtmlHelper.TryParseHtmlTag(ref slice, out htmlTag))
                {
                    return false;
                }

                state.Inline = new HtmlInline() { Tag = htmlTag };
            }

            return true;
        }
    }
}