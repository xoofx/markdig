// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.SmartyPants
{
    /// <summary>
    /// The inline parser for SmartyPants.
    /// </summary>
    public class SmaryPantsInlineParser : InlineParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SmaryPantsInlineParser"/> class.
        /// </summary>
        public SmaryPantsInlineParser()
        {
            OpeningCharacters = new[] {'`', '\'', '"', '<', '>', '.', '-'};
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
            // -- 	– 	&ndash; 	'ndash'
            // --- 	— 	&mdash; 	'mdash'

            var pc = slice.PeekCharExtra(-1);
            var c = slice.CurrentChar;
            var openingChar = c;

            // undefined first
            var type = (SmartyPantType) 0;

            switch (c)
            {
                case '`':
                    if (slice.PeekChar(1) == '`')
                    {
                        slice.NextChar();
                        type = SmartyPantType.DoubleQuote; // We will resolve them at the end of parsing all inlines
                    }
                    break;
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
                        type = SmartyPantType.Dash2;
                        if (slice.PeekChar(1) == '-')
                        {
                            slice.NextChar();
                            type = SmartyPantType.Dash3;
                        }
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
            var pant = new SmartyPant()
            {
                OpeningCharacter = openingChar,
                Type = type
            };

            // We will check in a post-process step for balanaced open/close quotes
            if (postProcess)
            {
                var quotePants = processor.ParserStates[Index] as List<SmartyPant>;
                if (quotePants == null)
                {
                    processor.ParserStates[Index] = quotePants = new List<SmartyPant>();
                }
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

        private void BlockOnProcessInlinesEnd(InlineProcessor processor, Inline inline)
        {
            processor.Block.ProcessInlinesEnd -= BlockOnProcessInlinesEnd;

            var pants = (List<SmartyPant>) processor.ParserStates[Index];

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
                        // Replace all intermediate unmatched left or right SmartyPants to there literal equivalent
                        pants.RemoveAt(i);
                        i--;
                        for (int j = i; j > previousIndex; j--)
                        {
                            var toReplace = pants[j];
                            pants.RemoveAt(j);
                            toReplace.ReplaceBy(new LiteralInline(toReplace.ToString()));
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
                quote.ReplaceBy(new LiteralInline(quote.ToString()));
            }

            pants.Clear();
        }
    }
}