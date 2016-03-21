// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Parsers.Inlines
{
    /// <summary>
    /// An inline parser for <see cref="EmphasisInline"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Parsers.InlineParser" />
    /// <seealso cref="Textamina.Markdig.Parsers.IDelimiterProcessor" />
    public class EmphasisInlineParser : InlineParser, IDelimiterProcessor
    {
        private CharacterMap<EmphasisDescriptor> emphasisMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmphasisInlineParser"/> class.
        /// </summary>
        public EmphasisInlineParser()
        {
            EmphasisDescriptors = new List<EmphasisDescriptor>()
            {
                new EmphasisDescriptor('*', 1, 2, true),
                new EmphasisDescriptor('_', 1, 2, false)
            };
        }

        public List<EmphasisDescriptor> EmphasisDescriptors { get; }

        public override void Initialize(InlineProcessor processor)
        {
            OpeningCharacters = new char[EmphasisDescriptors.Count];

            var tempMap = new List<KeyValuePair<char, EmphasisDescriptor>>();
            for (int i = 0; i < EmphasisDescriptors.Count; i++)
            {
                var emphasis = EmphasisDescriptors[i];
                if (Array.IndexOf(OpeningCharacters, emphasis.Character) >= 0)
                {
                    throw new InvalidOperationException(
                        $"The character `{emphasis.Character}` is already used by another emphasis descriptor");
                }

                OpeningCharacters[i] = emphasis.Character;

                tempMap.Add(new KeyValuePair<char, EmphasisDescriptor>(emphasis.Character, emphasis));
            }

            emphasisMap = new CharacterMap<EmphasisDescriptor>(tempMap);
        }

        public bool ProcessDelimiters(InlineProcessor state, Inline root, Inline lastChild, int delimiterProcessorIndex, bool isFinalProcessing)
        {
            var container = root as ContainerInline;
            if (container == null)
            {
                return true;
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
                if (child == lastChild)
                {
                    break;
                }
                var delimiter = child as EmphasisDelimiterInline;
                if (delimiter != null)
                {
                    delimiters.Add(delimiter);
                }
                var subContainer = child as ContainerInline;
                child = subContainer?.LastChild;
                //if (delimiter == null && subContainer is DelimiterInline)
                //{
                //    subContainer.ReplaceBy(new LiteralInline() { Content = new StringSlice(((DelimiterInline)subContainer).ToLiteral()), IsClosed = true });
                //}
            }

            ProcessEmphasis(delimiters);
            return true;
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            // First, some definitions. A delimiter run is either a sequence of one or more * characters that 
            // is not preceded or followed by a * character, or a sequence of one or more _ characters that 
            // is not preceded or followed by a _ character.

            var delimiterChar = slice.CurrentChar;
            var emphasisDesc = emphasisMap[delimiterChar];
            var pc = slice.PeekCharExtra(-1);
            if (pc == delimiterChar && slice.PeekCharExtra(-2) != '\\')
            {
                return false;
            }

            int delimiterCount = 0;
            char c;
            do
            {
                delimiterCount++;
                c = slice.NextChar();
            } while (c == delimiterChar);


            // If the emphasis doesn't have the minimum required character
            if (delimiterCount < emphasisDesc.MinimumCount)
            {
                return false;
            }

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

            if (!emphasisDesc.EnableWithinWord)
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

            // We have potentially an open or close emphasis
            if (canOpen || canClose)
            {
                var delimiterType = DelimiterType.Undefined;
                if (canOpen)
                {
                    delimiterType |= DelimiterType.Open;
                }
                if (canClose)
                {
                    delimiterType |= DelimiterType.Close;
                }

                var delimiter = new EmphasisDelimiterInline(this, emphasisDesc)
                {
                    DelimiterCount = delimiterCount,
                    Type = delimiterType,
                };

                processor.Inline = delimiter;
                return true;
            }

            // We don't have an emphasis
            return false;
        }

        private static void ProcessEmphasis(List<EmphasisDelimiterInline> delimiters)
        {
            // The following method is inspired by the "An algorithm for parsing nested emphasis and links"
            // at the end of the CommonMark specs.

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
                                IsDouble = isStrong
                            };


                            var embracer = (ContainerInline)openDelimiter;

                            // Go down to the first emphasis with a lower level
                            while (true)
                            {
                                var previousEmphasis = embracer.FirstChild as EmphasisInline;
                                if (previousEmphasis != null && previousEmphasis.IsDouble && !isStrong && embracer.FirstChild == embracer.LastChild)
                                {
                                    embracer = previousEmphasis;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // Embrace all delimiters
                            embracer.EmbraceChildrenBy(emphasis);

                            openDelimiter.DelimiterCount -= isStrong ? 2 : 1;
                            closeDelimiter.DelimiterCount -= isStrong ? 2 : 1;

                            // Remove any intermediate emphasis
                            for (int k = i - 1; k >= openDelimiterIndex + 1; k--)
                            {
                                var literalDelimiter = delimiters[k];
                                var literal = new LiteralInline()
                                {
                                    Content = new StringSlice(literalDelimiter.ToLiteral()),
                                    IsClosed = true
                                };

                                literalDelimiter.ReplaceBy(literal);
                                delimiters.RemoveAt(k);
                                i--;
                            }

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
                                var firstChild = openDelimiter.FirstChild;
                                firstChild.Remove();
                                openDelimiter.ReplaceBy(firstChild);
                                firstChild.IsClosed = true;
                                closeDelimiter.Remove();
                                firstChild.InsertAfter(closeDelimiter);
                                delimiters.RemoveAt(openDelimiterIndex);
                                i--;
                            }
                        }
                        else if ((closeDelimiter.Type & DelimiterType.Open) == 0)
                        {
                            var literal = new LiteralInline()
                            {
                                Content = new StringSlice(closeDelimiter.ToLiteral()),
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
                    Content = new StringSlice(delimiter.ToLiteral()),
                    IsClosed = true
                };

                delimiter.ReplaceBy(literal);
            }
            delimiters.Clear();
        }
   }
}