using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public class HardlineBreakInlineParser : InlineParser
    {
        public static readonly HardlineBreakInlineParser Default = new HardlineBreakInlineParser();

        public HardlineBreakInlineParser()
        {
            OpeningCharacters = new[] {'\n'};
        }

        public override bool Match(InlineParserState state, ref StringSlice text)
        {
            // Hard line breaks are for separating inline content within a block. Neither syntax for hard line breaks works at the end of a paragraph or other block element:
            if (!(state.Block is ParagraphBlock) || text.Column == 0 || !text.PeekCharExtra(-1).IsSpace() || !text.PeekCharExtra(-2).IsSpace())
            {
                return false;
            }

            // A line break (not in a code span or HTML tag) that is preceded by two or more spaces 
            // and does not occur at the end of a block is parsed as a hard line break (rendered in HTML as a <br /> tag)
            var literal = state.Inline as LiteralInline;
            if (literal != null)
            {
                // TODO: TRIM END
                // literal.TrimEnd = true;
            }

            state.Inline = new HardlineBreakInline();
            text.NextChar(); // Skip \n

            //// A line break (not in a code span or HTML tag) that is preceded by two or more spaces 
            //// and does not occur at the end of a block is parsed as a hard line break (rendered in HTML as a <br /> tag)
            //var text = state.Lines;
            //int spaceCount = 0;
            //var c = text.CurrentChar;
            //while (c.IsSpaceOrTab())
            //{
            //    c = text.NextChar();
            //    spaceCount++;
            //}
            //if (c == '\\')
            //{
            //    c = text.NextChar();
            //    spaceCount = 2;
            //}
            //if (c != '\n' || spaceCount < 2)
            //{
            //    return false;
            //}

            return true;
        }
    }
}