// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.SmartyPants
{
    /// <summary>
    /// The inline parser for SmartyPants.
    /// </summary>
    public class SmartyPantsInlineParser : InlineParser, IPostInlineProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SmartyPantsInlineParser"/> class.
        /// </summary>
        public SmartyPantsInlineParser()
        {
            OpeningCharacters = new[] {'\'', '"', '<', '>', '.', '-'};
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            // We are matching the following characters:
            //
            // ' 	‘ ’ 	&lsquo; &rsquo; 	'left-single-quote', 'right-single-quote'
            // '' 	“ ” 	&ldquo; &rdquo; 	'left-double-quote', 'right-double-quote'
            // " 	“ ” 	&ldquo; &rdquo; 	'left-double-quote', 'right-double-quote'
            // << >> 	« » 	&laquo; &raquo; 	'left-angle-quote', 'right-angle-quote'
            // ... 	… 	&hellip; 	'ellipsis'

            // Special case: &ndash; and &mdash; are handle as a PostProcess step to avoid conflicts with pipetables header separator row
            // -- 	– 	&ndash; 	'ndash'
            // --- 	— 	&mdash; 	'mdash'

            var pc = slice.PeekCharExtra(-1);
            var c = slice.CurrentChar;
            var openingChar = c;

            var startingPosition = slice.Start;

            // undefined first
            var type = (SmartyPantType) 0;

            switch (c)
            {
                case '\'':
                    type = SmartyPantType.Quote; // We will resolve them at the end of parsing all inlines
                    if (slice.PeekChar(1) == '\'')
                    {
                        slice.NextChar();
                        type = SmartyPantType.DoubleQuote; // We will resolve them at the end of parsing all inlines
                    }
                    break;
                case '"':
                    type = SmartyPantType.DoubleQuote;
                    break;
                case '<':
                    if (slice.NextChar() == '<')
                    {
                        type = SmartyPantType.LeftAngleQuote;
                    }
                    break;
                case '>':
                    if (slice.NextChar() == '>')
                    {
                        type = SmartyPantType.RightAngleQuote;
                    }
                    break;
                case '.':
                    if (slice.NextChar() == '.' && slice.NextChar() == '.')
                    {
                        type = SmartyPantType.Ellipsis;
                    }
                    break;
                case '-':
                    if (slice.NextChar() == '-')
                    {
                        var quotePants = GetOrCreateState(processor);
                        quotePants.HasDash = true;
                        return false;
                    }
                    break;
            }

            // If it is not matched, early exit
            if (type == 0)
            {
                return false;
            }

            // Skip char
            c = slice.NextChar();

            bool canOpen;
            bool canClose;
            CharHelper.CheckOpenCloseDelimiter(pc, c, false, out canOpen, out canClose);

            bool postProcess = false;

            switch (type)
            {
                case SmartyPantType.Quote:
                    postProcess = true;
                    if (canOpen && !canClose)
                    {
                        type = SmartyPantType.LeftQuote;
                    }
                    else if (!canOpen && canClose)
                    {
                        type = SmartyPantType.RightQuote;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case SmartyPantType.DoubleQuote:
                    postProcess = true;
                    if (canOpen && !canClose)
                    {
                        type = SmartyPantType.LeftDoubleQuote;
                    }
                    else if (!canOpen && canClose)
                    {
                        type = SmartyPantType.RightDoubleQuote;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case SmartyPantType.LeftAngleQuote:
                    postProcess = true;
                    if (!canOpen || canClose)
                    {
                        return false;
                    }
                    break;
                case SmartyPantType.RightAngleQuote:
                    postProcess = true;
                    if (canOpen || !canClose)
                    {
                        return false;
                    }
                    break;
                case SmartyPantType.Ellipsis:
                    if (canOpen || !canClose)
                    {
                        return false;
                    }
                    break;
            }

            // Create the SmartyPant inline
            int line;
            int column;
            var pant = new SmartyPant()
            {
                Span = {Start = processor.GetSourcePosition(startingPosition, out line, out column)},
                Line = line,
                Column = column,
                OpeningCharacter = openingChar,
                Type = type
            };
            pant.Span.End = pant.Span.Start + slice.Start - startingPosition - 1;

            // We will check in a post-process step for balanaced open/close quotes
            if (postProcess)
            {
                var quotePants = GetOrCreateState(processor);

                // Register only if we don't have yet any quotes
                if (quotePants.Count == 0)
                {
                    processor.Block.ProcessInlinesEnd += BlockOnProcessInlinesEnd;
                }
                quotePants.Add(pant);
            }

            processor.Inline = pant;
            return true;
        }

        private ListSmartyPants GetOrCreateState(InlineProcessor processor)
        {
            var quotePants = processor.ParserStates[Index] as ListSmartyPants;
            if (quotePants == null)
            {
                processor.ParserStates[Index] = quotePants = new ListSmartyPants();
            }
            return quotePants;
        }

        private void BlockOnProcessInlinesEnd(InlineProcessor processor, Inline inline)
        {
            processor.Block.ProcessInlinesEnd -= BlockOnProcessInlinesEnd;

            var pants = (ListSmartyPants) processor.ParserStates[Index];

            // We only change quote into left or right quotes if we find proper balancing
            var previousIndices = new int[3] {-1, -1, -1};

            for (int i = 0; i < pants.Count; i++)
            {
                var quote = pants[i];

                int currentTypeIndex = -1;
                SmartyPantType expectedLeftQuote = 0;
                SmartyPantType expectedRightQuote = 0;

                if (quote.Type == SmartyPantType.LeftQuote || quote.Type == SmartyPantType.RightQuote)
                {
                    currentTypeIndex = 0;
                    expectedLeftQuote = SmartyPantType.LeftQuote;
                    expectedRightQuote = SmartyPantType.RightQuote;
                }
                else if (quote.Type == SmartyPantType.LeftDoubleQuote || quote.Type == SmartyPantType.RightDoubleQuote)
                {
                    currentTypeIndex = 1;
                    expectedLeftQuote = SmartyPantType.LeftDoubleQuote;
                    expectedRightQuote = SmartyPantType.RightDoubleQuote;
                }
                else if (quote.Type == SmartyPantType.LeftAngleQuote || quote.Type == SmartyPantType.RightAngleQuote)
                {
                    currentTypeIndex = 2;
                    expectedLeftQuote = SmartyPantType.LeftAngleQuote;
                    expectedRightQuote = SmartyPantType.RightAngleQuote;
                }

                if (currentTypeIndex < 0)
                {
                    continue;
                }

                int previousIndex = previousIndices[currentTypeIndex];
                var previousQuote = previousIndex >= 0 ? pants[previousIndex] : null;
                if (previousQuote == null)
                {
                    if (quote.Type == expectedLeftQuote)
                    {
                        previousIndices[currentTypeIndex] = i;
                    }
                }
                else
                {
                    if (quote.Type == expectedRightQuote)
                    {
                        // Replace all intermediate unmatched left or right SmartyPants to their literal equivalent
                        pants.RemoveAt(i);
                        i--;
                        for (int j = i; j > previousIndex; j--)
                        {
                            var toReplace = pants[j];
                            pants.RemoveAt(j);
                            toReplace.ReplaceBy(new LiteralInline(toReplace.ToString())
                            {
                                Span = toReplace.Span,
                                Line = toReplace.Line,
                                Column = toReplace.Column,
                            });
                            i--;
                        }

                        // If we matched, we remove left/right quotes from the list
                        pants.RemoveAt(previousIndex);
                        previousIndices[currentTypeIndex] = -1;
                    }
                    else
                    {
                        previousIndices[currentTypeIndex] = i;
                    }
                }
            }

            // If we have any quotes lefts, replace them by there literal equivalent
            foreach (var quote in pants)
            {
                quote.ReplaceBy(new LiteralInline(quote.ToString())
                {
                    Span = quote.Span,
                    Line = quote.Line,
                    Column = quote.Column,
                });
            }

            pants.Clear();
        }

        bool IPostInlineProcessor.PostProcess(InlineProcessor state, Inline root, Inline lastChild, int postInlineProcessorIndex,
            bool isFinalProcessing)
        {
            // Don't try to process anything if there are no dash
            var quotePants = state.ParserStates[Index] as ListSmartyPants;
            if (quotePants == null || !quotePants.HasDash)
            {
                return true;
            }

            var child = root;
            var pendingContainers = new Stack<Inline>();

            while (true)
            {
                while (child != null)
                {
                    var next = child.NextSibling;

                    if (child is LiteralInline)
                    {
                        var literal = (LiteralInline) child;

                        var startIndex = 0;

                        var indexOfDash = literal.Content.IndexOf("--", startIndex);
                        if (indexOfDash >= 0)
                        {
                            var type = SmartyPantType.Dash2;
                            if (literal.Content.PeekCharAbsolute(indexOfDash + 2) == '-')
                            {
                                type = SmartyPantType.Dash3;
                            }
                            var nextContent = literal.Content;
                            var originalSpan = literal.Span;
                            literal.Span.End -= literal.Content.End - indexOfDash + 1;
                            literal.Content.End = indexOfDash - 1;
                            nextContent.Start = indexOfDash + (type == SmartyPantType.Dash2 ? 2 : 3);

                            var pant = new SmartyPant()
                            {
                                Span = new SourceSpan(literal.Content.End + 1, nextContent.Start - 1),
                                Line = literal.Line,
                                Column = literal.Column,
                                OpeningCharacter = '-',
                                Type = type
                            };
                            literal.InsertAfter(pant);

                            var postLiteral = new LiteralInline()
                            {
                                Span = new SourceSpan(pant.Span.End + 1, originalSpan.End),
                                Line = literal.Line,
                                Column = literal.Column,
                                Content = nextContent
                            };
                            pant.InsertAfter(postLiteral);

                            // Use the pending literal to proceed further
                            next = postLiteral;
                        }
                    }
                    else if (child is ContainerInline)
                    {
                        pendingContainers.Push(((ContainerInline)child).FirstChild);
                    }

                    child = next;
                }
                if (pendingContainers.Count > 0)
                {
                    child = pendingContainers.Pop();
                }
                else
                {
                    break;
                }
            }
            return true;
        }


        private class ListSmartyPants : List<SmartyPant>
        {
            public bool HasDash { get; set; }
        }
    }
}