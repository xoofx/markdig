using System;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class EmphasisInline : ContainerInline
    {
        public static readonly InlineParser Parser = new ParserInternal();

        public char EscapedChar;

        private class ParserInternal : InlineParser
        {
            public ParserInternal()
            {
                FirstChars = new[] { '*', '_' };
            }

            public override bool Match(MatchInlineState state)
            {
                var lines = state.Lines;

                // First, some definitions. A delimiter run is either a sequence of one or more * characters that 
                // is not preceded or followed by a * character, or a sequence of one or more _ characters that 
                // is not preceded or followed by a _ character.

                var pc = lines.PreviousChar1;
                var delimiterChar = lines.CurrentChar;

                int delimiterCount = 0;
                char c;
                do
                {
                    delimiterCount++;
                    c = lines.NextChar();
                } while (c == delimiterChar);


                // A left-flanking delimiter run is a delimiter run that is 
                // (a) not followed by Unicode whitespace, and
                // (b) either not followed by a punctuation character, or preceded by Unicode whitespace 
                // or a punctuation character. 
                // For purposes of this definition, the beginning and the end of the line count as Unicode whitespace.
                var afterIsPunctuation = CharHelper.IsAsciiPunctuation(c);
                bool canOpen = !CharHelper.IsWhiteSpaceOrZero(c) &&
                                (!afterIsPunctuation ||
                                !CharHelper.IsWhiteSpaceOrZero(pc) ||
                                !CharHelper.IsAsciiPunctuation(pc));


                // A right-flanking delimiter run is a delimiter run that is 
                // (a) not preceded by Unicode whitespace, and 
                // (b) either not preceded by a punctuation character, or followed by Unicode whitespace 
                // or a punctuation character. 
                // For purposes of this definition, the beginning and the end of the line count as Unicode whitespace.
                var beforeIsPunctuation = CharHelper.IsAsciiPunctuation(pc);
                bool canClose = !CharHelper.IsWhiteSpaceOrZero(pc) &&
                                (!beforeIsPunctuation ||
                                    !CharHelper.IsWhiteSpaceOrZero(c) ||
                                    !CharHelper.IsAsciiPunctuation(c));

                if (delimiterChar == '_')
                {
                    var temp = canOpen;
                    // A single _ character can open emphasis iff it is part of a left-flanking delimiter run and either 
                    // (a) not part of a right-flanking delimiter run or 
                    // (b) part of a right-flanking delimiter run preceded by punctuation.
                    canOpen = canOpen && (!canClose || beforeIsPunctuation);

                    // A single _ character can close emphasis iff it is part of a right-flanking delimiter run and either
                    // (a) not part of a left-flanking delimiter run or 
                    // (b) part of a left-flanking delimiter run followed by punctuation.
                    canClose = canClose && (!temp || afterIsPunctuation);
                }

                //// If we can close, try to find a matching open
                //if (canClose && state.Inline != null)
                //{
                //    var matching = DelimiterInline.FindMatchingOpen(state.Inline, 0, delimiterRun, delimiterCount);

                //    // transform matching into

                //    return true;
                //}

                // We have potentially an open or close emphasis
                if (canOpen || canClose)
                {
                    var delimiterType = DelimiterType.None;
                    if (canOpen)
                    {
                        delimiterType |= DelimiterType.Open;
                    }
                    if (canClose)
                    {
                        delimiterType |= DelimiterType.Close;
                    }

                    var delimiter = new EmphasisDelimiterInline(this)
                    {
                        DelimiterChar = delimiterChar,
                        DelimiterCount = delimiterCount,
                        Type = delimiterType,
                    };

                    state.Inline = delimiter;
                    return true;
                }

                // We don't have an emphasis
                return false;
            }
        }
    }
}