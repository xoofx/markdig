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

            var delimiters = new List<EmphasisDelimiterInline>();

            if (container is EmphasisDelimiterInline)
            {
                delimiters.Add((EmphasisDelimiterInline)container);
            }

            // Move current_position forward in the delimiter stack (if needed) until 
            // we find the first potential closer with delimiter * or _. (This will be the potential closer closest to the beginning of the input – the first one in parse order.)
            var child = container.LastChild;
            while (child != null)
            {
                var delimiter = child as EmphasisDelimiterInline;
                if (delimiter != null)
                {
                    delimiters.Add(delimiter);
                }
                child = (child as ContainerInline)?.LastChild;
            }

            ProcessEmphasis(delimiters);
        }

        private static void ProcessEmphasis(List<EmphasisDelimiterInline> delimiters)
        {
            // Move current_position forward in the delimiter stack (if needed) until 
            // we find the first potential closer with delimiter * or _. (This will be the potential closer closest to the beginning of the input – the first one in parse order.)
            for (int i = 0; i < delimiters.Count; i++)
            {
                var closeDelimiter = delimiters[i];
                if ((closeDelimiter.Type & DelimiterType.Close) != 0 && closeDelimiter.DelimiterCount > 0)
                {
                    while (true)
                    {
                        // Now, look back in the stack (staying above stack_bottom and the openers_bottom for this delimiter type) 
                        // for the first matching potential opener (“matching” means same delimiter).
                        EmphasisDelimiterInline openDelimiter = null;
                        int openDelimiterIndex = -1;
                        for (int j = i - 1; j >= 0; j--)
                        {
                            var previousOpenDelimiter = delimiters[j];
                            if (previousOpenDelimiter.DelimiterChar == closeDelimiter.DelimiterChar &&
                                (previousOpenDelimiter.Type & DelimiterType.Open) != 0 &&
                                previousOpenDelimiter.DelimiterCount > 0)
                            {
                                openDelimiter = previousOpenDelimiter;
                                openDelimiterIndex = j;
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

                            // Embrace all delimiters
                            openDelimiter.EmbraceChildrenBy(emphasis);

                            openDelimiter.DelimiterCount -= isStrong ? 2 : 1;
                            closeDelimiter.DelimiterCount -= isStrong ? 2 : 1;

                            if (closeDelimiter.DelimiterCount == 0)
                            {
                                closeDelimiter.MoveChildrenAfter(emphasis);
                                closeDelimiter.Remove();
                                delimiters.RemoveAt(i);
                                i--;

                                // Remove the open delimiter if it is also empty
                                if (openDelimiter.DelimiterCount == 0)
                                {
                                    openDelimiter.MoveChildrenAfter(openDelimiter);
                                    openDelimiter.Remove();
                                    delimiters.RemoveAt(openDelimiterIndex);
                                    i--;
                                }
                                break;
                            }

                            // The current delimiters are matching
                            if (openDelimiter.DelimiterCount > 0)
                            {
                                goto process_delims;
                            }
                            else
                            {
                                // Remove the open delimiter if it is also empty
                                openDelimiter.MoveChildrenAfter(openDelimiter.NextSibling ?? openDelimiter.Parent);
                                openDelimiter.Remove();
                                delimiters.RemoveAt(openDelimiterIndex);
                                i--;
                            }
                        }
                        else if ((closeDelimiter.Type & DelimiterType.Open) == 0)
                        {
                            var literal = new LiteralInline()
                            {
                                Content = closeDelimiter.ToLiteral(),
                                IsClosed = true
                            };

                            closeDelimiter.ReplaceBy(literal);
                            delimiters.RemoveAt(i);
                            i--;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Any delimiters left must be literal
            for (int i = 0; i < delimiters.Count; i++)
            {
                var delimiter = delimiters[i];
                var literal = new LiteralInline()
                {
                    Content = delimiter.ToLiteral(),
                    IsClosed = true
                };

                delimiter.ReplaceBy(literal);
            }
            delimiters.Clear();
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
                if (FirstChars.Contains(pc))
                {
                    return false;
                }

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
                bool nextIsPunctuation;
                bool nextIsWhiteSpace;
                bool prevIsPunctuation;
                bool prevIsWhiteSpace;
                pc.CheckUnicodeCategory(out prevIsWhiteSpace, out prevIsPunctuation);
                c.CheckUnicodeCategory(out nextIsWhiteSpace, out nextIsPunctuation);

                bool canOpen = !nextIsWhiteSpace &&
                               (!nextIsPunctuation || prevIsWhiteSpace || prevIsPunctuation);


                // A right-flanking delimiter run is a delimiter run that is 
                // (a) not preceded by Unicode whitespace, and 
                // (b) either not preceded by a punctuation character, or followed by Unicode whitespace 
                // or a punctuation character. 
                // For purposes of this definition, the beginning and the end of the line count as Unicode whitespace.
                bool canClose = !prevIsWhiteSpace &&
                                (!prevIsPunctuation || nextIsWhiteSpace || nextIsPunctuation);

                if (delimiterChar == '_')
                {
                    var temp = canOpen;
                    // A single _ character can open emphasis iff it is part of a left-flanking delimiter run and either 
                    // (a) not part of a right-flanking delimiter run or 
                    // (b) part of a right-flanking delimiter run preceded by punctuation.
                    canOpen = canOpen && (!canClose || prevIsPunctuation);

                    // A single _ character can close emphasis iff it is part of a right-flanking delimiter run and either
                    // (a) not part of a left-flanking delimiter run or 
                    // (b) part of a left-flanking delimiter run followed by punctuation.
                    canClose = canClose && (!temp || nextIsPunctuation);
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