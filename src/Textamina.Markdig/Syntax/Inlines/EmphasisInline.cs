using System;
using System.Collections.Generic;
using System.Linq;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class EmphasisInline : ContainerInline
    {
        public static readonly InlineParser Parser = new ParserInternal();

        public char DelimiterChar { get; set; }

        public bool Strong { get; set; }

        public override string ToString()
        {
            return Strong ? "<strong>" : "<em>";
        }

        public static void ProcessEmphasis(Inline root)
        {
            var container = root as ContainerInline;
            if (container == null)
            {
                return;
            }

            var delimiters = new Stack<EmphasisDelimiterInline>();
            ProcessEmphasis(container, delimiters);
        }

        private static void ProcessEmphasis(ContainerInline inline, Stack<EmphasisDelimiterInline> delimiters)
        {
            // Move current_position forward in the delimiter stack (if needed) until 
            // we find the first potential closer with delimiter * or _. (This will be the potential closer closest to the beginning of the input – the first one in parse order.)
            var child = inline.FirstChild;
            while (child != null)
            {
                var next = child.NextSibling;

                var delimiter = child as EmphasisDelimiterInline;
                if (delimiter != null)
                {
                    if ((delimiter.Type & DelimiterType.Close) != 0 && delimiter.DelimiterCount > 0)
                    {
                        var closeDelimiter = delimiter;
                        while (true)
                        {
                            // Now, look back in the stack (staying above stack_bottom and the openers_bottom for this delimiter type) 
                            // for the first matching potential opener (“matching” means same delimiter).
                            EmphasisDelimiterInline openDelimiter = null;
                            foreach (var parentDelimiter in delimiters)
                            {
                                if (parentDelimiter.DelimiterChar == closeDelimiter.DelimiterChar &&
                                    (parentDelimiter.Type & DelimiterType.Open) != 0 && parentDelimiter.DelimiterCount > 0)
                                {
                                    openDelimiter = parentDelimiter;
                                    break;
                                }
                            }

                            if (openDelimiter != null)
                            {
                                process_delims:
                                bool isStrong = openDelimiter.DelimiterCount >= 2 && closeDelimiter.DelimiterCount >= 2;

                                // Insert an emph or strong emph node accordingly, after the text node corresponding to the opener.

                                var emphasis = new EmphasisInline()
                                {
                                    DelimiterChar = closeDelimiter.DelimiterChar,
                                    Strong = isStrong
                                };
                                openDelimiter.FinalEmphasisInlines.Add(emphasis);

                                openDelimiter.DelimiterCount -= isStrong ? 2 : 1;
                                closeDelimiter.DelimiterCount -= isStrong ? 2 : 1;

                                if (closeDelimiter.DelimiterCount == 0)
                                {
                                    break;
                                }
                                if (openDelimiter.DelimiterCount > 0)
                                {
                                    goto process_delims;
                                }
                                else
                                {
                                    // TODO ?
                                    // openDelimiter
                                }
                            }
                            else
                            {
                                // TODO ?
                                break;
                            }
                        }
                    }

                    delimiters.Push(delimiter);
                }

                if (child is ContainerInline)
                {
                    ProcessEmphasis((ContainerInline) child, delimiters);
                }

                if (delimiter != null)
                {
                    delimiters.Pop();
                }

                child = next;
            }
        }






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