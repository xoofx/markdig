// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Parsers.Inlines
{
    /// <summary>
    /// An inline parser for <see cref="EmphasisInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.InlineParser" />
    /// <seealso cref="IPostInlineProcessor" />
    public class EmphasisInlineParser : InlineParser, IPostInlineProcessor
    {
        private CharacterMap<EmphasisDescriptor> emphasisMap;
        private readonly DelimitersObjectCache inlinesCache;

        public delegate EmphasisInline CreateEmphasisInlineDelegate(char emphasisChar, bool isStrong);

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
            inlinesCache = new DelimitersObjectCache();
        }

        /// <summary>
        /// Gets the emphasis descriptors.
        /// </summary>
        public List<EmphasisDescriptor> EmphasisDescriptors { get; }

        /// <summary>
        /// Determines whether this parser is using the specified character as an emphasis delimiter.
        /// </summary>
        /// <param name="c">The character to look for.</param>
        /// <returns><c>true</c> if this parser is using the specified character as an emphasis delimiter; otherwise <c>false</c></returns>
        public bool HasEmphasisChar(char c)
        {
            foreach (var emphasis in EmphasisDescriptors)
            {
                if (emphasis.Character == c)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets or sets the create emphasis inline delegate (allowing to create a different emphasis inline class)
        /// </summary>
        public CreateEmphasisInlineDelegate CreateEmphasisInline { get; set; }

        public override void Initialize()
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

        public bool PostProcess(InlineProcessor state, Inline root, Inline lastChild, int postInlineProcessorIndex, bool isFinalProcessing)
        {
            var container = root as ContainerInline;
            if (container == null)
            {
                return true;
            }

            List<EmphasisDelimiterInline> delimiters = null;
            if (container is EmphasisDelimiterInline)
            {
                delimiters = inlinesCache.Get();
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
                    if (delimiters == null)
                    {
                        delimiters = inlinesCache.Get();
                    }
                    delimiters.Add(delimiter);
                }
                var subContainer = child as ContainerInline;
                child = subContainer?.LastChild;
            }

            if (delimiters != null)
            {
                ProcessEmphasis(state, delimiters);
                inlinesCache.Release(delimiters);
            }
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

            var startPosition = slice.Start;

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

            // Calculate Open-Close for current character
            bool canOpen;
            bool canClose;
            CharHelper.CheckOpenCloseDelimiter(pc, c, emphasisDesc.EnableWithinWord, out canOpen, out canClose);

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

                int line;
                int column;
                var delimiter = new EmphasisDelimiterInline(this, emphasisDesc)
                {
                    DelimiterCount = delimiterCount,
                    Type = delimiterType,
                    Span = new SourceSpan(processor.GetSourcePosition(startPosition, out line, out column), processor.GetSourcePosition(slice.Start - 1)),
                    Column = column,
                    Line = line,
                };

                processor.Inline = delimiter;
                return true;
            }

            // We don't have an emphasis
            return false;
        }

        private void ProcessEmphasis(InlineProcessor processor, List<EmphasisDelimiterInline> delimiters)
        {
            // The following method is inspired by the "An algorithm for parsing nested emphasis and links"
            // at the end of the CommonMark specs.

            // Move current_position forward in the delimiter stack (if needed) until 
            // we find the first potential closer with delimiter * or _. (This will be the potential closer closest to the beginning of the input – the first one in parse order.)
            for (int i = 0; i < delimiters.Count; i++)
            {
                var closeDelimiter = delimiters[i];
                // Skip delimiters not supported by this instance
                if (emphasisMap[closeDelimiter.DelimiterChar] == null)
                {
                    continue;
                }

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

                            var isOddMatch = ((closeDelimiter.Type & DelimiterType.Open) != 0 ||
                                             (previousOpenDelimiter.Type & DelimiterType.Close) != 0) &&
                                             previousOpenDelimiter.DelimiterCount != closeDelimiter.DelimiterCount &&
                                             (previousOpenDelimiter.DelimiterCount + closeDelimiter.DelimiterCount) % 3 == 0;

                            if (previousOpenDelimiter.DelimiterChar == closeDelimiter.DelimiterChar &&
                                (previousOpenDelimiter.Type & DelimiterType.Open) != 0 &&
                                previousOpenDelimiter.DelimiterCount > 0 && !isOddMatch)
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
                            var emphasis = CreateEmphasisInline?.Invoke(closeDelimiter.DelimiterChar, isStrong)
                                ?? new EmphasisInline()
                                {
                                    DelimiterChar = closeDelimiter.DelimiterChar,
                                    IsDouble = isStrong
                                };

                            // Update position for emphasis
                            var openDelimitercount = openDelimiter.DelimiterCount;
                            var closeDelimitercount = closeDelimiter.DelimiterCount;
                            var delimiterDelta = isStrong ? 2 : 1;

                            emphasis.Span.Start = openDelimiter.Span.Start;
                            emphasis.Line = openDelimiter.Line;
                            emphasis.Column = openDelimiter.Column;
                            emphasis.Span.End = closeDelimiter.Span.End - closeDelimitercount + delimiterDelta;

                            openDelimiter.Span.Start += delimiterDelta;
                            openDelimiter.Column += delimiterDelta;
                            closeDelimiter.Span.Start += delimiterDelta;
                            closeDelimiter.Column += delimiterDelta;

                            openDelimiter.DelimiterCount -= delimiterDelta;
                            closeDelimiter.DelimiterCount -= delimiterDelta;

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

                            // Copy attributes attached to delimiter to the emphasis
                            var attributes = closeDelimiter.TryGetAttributes();
                            if (attributes != null)
                            {
                                emphasis.SetAttributes(attributes);
                            }

                            // Embrace all delimiters
                            embracer.EmbraceChildrenBy(emphasis);

                            // Remove any intermediate emphasis
                            for (int k = i - 1; k >= openDelimiterIndex + 1; k--)
                            {
                                var literalDelimiter = delimiters[k];
                                var literal = new LiteralInline()
                                {
                                    Content = new StringSlice(literalDelimiter.ToLiteral()),
                                    IsClosed = true,
                                    Span = literalDelimiter.Span,
                                    Line = literalDelimiter.Line,
                                    Column = literalDelimiter.Column
                                };

                                literalDelimiter.ReplaceBy(literal);
                                
                                delimiters.RemoveAt(k);
                                i--;
                            }

                            if (closeDelimiter.DelimiterCount == 0)
                            {
                                var newParent = openDelimiter.DelimiterCount > 0 ? emphasis : emphasis.Parent;
                                closeDelimiter.MoveChildrenAfter(newParent);
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
                                IsClosed = true,
                                Span = closeDelimiter.Span,
                                Line = closeDelimiter.Line,
                                Column = closeDelimiter.Column
                            };

                            closeDelimiter.ReplaceBy(literal);
                            // Notifies processor as we are creating an inline locally
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
                    IsClosed = true,
                    Span = delimiter.Span,
                    Line = delimiter.Line,
                    Column = delimiter.Column
                };

                delimiter.ReplaceBy(literal);
            }
            delimiters.Clear();
        }

        public class DelimitersObjectCache : ObjectCache<List<EmphasisDelimiterInline>>
        {
            protected override List<EmphasisDelimiterInline> NewInstance()
            {
                return new List<EmphasisDelimiterInline>(4);
            }

            protected override void Reset(List<EmphasisDelimiterInline> instance)
            {
                instance.Clear();
            }
        }
    }
}