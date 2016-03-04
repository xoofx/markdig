// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    public class EscapeInlineParser : InlineParser
    {
        public EscapeInlineParser()
        {
            OpeningCharacters = new[] {'\\'};
        }

        public override bool Match(InlineParserState state, ref StringSlice slice)
        {
            // Go to escape character
            var c = slice.NextChar();
            if (c.IsAsciiPunctuation())
            {
                state.Inline = new LiteralInline() {Content = new StringSlice(new string(c, 1))};
                slice.NextChar();
                return true;
            }

            // A backslash at the end of the line is a [hard line break]:
            if (c == '\n')
            {
                state.Inline = new HardlineBreakInline();
                slice.NextChar();
                return true;
            }
            return false;
        }
    }
}