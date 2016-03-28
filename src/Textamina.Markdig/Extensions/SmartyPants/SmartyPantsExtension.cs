// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.SmartyPants
{
    public class SmartyPantsExtension
    {
         
    }

    public enum SmartyPantType
    {
        Quote,

        LeftQuote,

        RightQuote,

        DoubleQuote,

        LeftDoubleQuote,

        RightDoubleQuote,

        LeftAngleQuote,

        RightAngleQuote,

        Ellipsis,

        Dash2,

        Dash3,
    }

    public class SmartyPantOptions
    {
        public SmartyPantOptions()
        {
            TextMap = new Dictionary<SmartyPantType, string>();
        }

        public Dictionary<SmartyPantType, string> TextMap { get; }
    }

    public class HtmlSmartyPantRenderer : HtmlObjectRenderer<SmartyPant>
    {
        private readonly SmartyPantOptions options;

        public HtmlSmartyPantRenderer(SmartyPantOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            this.options = options;
        }

        protected override void Write(HtmlRenderer renderer, SmartyPant obj)
        {
            
        }
    }




    public class SmartyPant : LeafInline
    {
        public char OpeningCharacter { get; set; }

        public SmartyPantType Type { get; set; }
    }

    public class SmaryPantsInlineParser : InlineParser
    {
        public SmaryPantsInlineParser()
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
            // -- 	– 	&ndash; 	'ndash'
            // --- 	— 	&mdash; 	'mdash'

            var c = slice.CurrentChar;

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
            slice.NextChar();

            // Create the SmartyPant inline
            var pant = new SmartyPant()
            {
                OpeningCharacter = c,
                Type = type
            };

            // If we have an undetermined element (quote or double quote), we will resolve it at the end of the block
            if (type == SmartyPantType.Quote || type == SmartyPantType.DoubleQuote)
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

            SmartyPant simpleQuote = null;
            SmartyPant doubleQuote = null;

            // We only change quote into left or right quotes if we find proper balancing
            for (int i = 0; i < pants.Count; i++)
            {
                var quote = pants[i];

                if (quote.Type == SmartyPantType.Quote)
                {
                    if (simpleQuote == null)
                    {
                        simpleQuote = quote;
                    }
                    else
                    {
                        simpleQuote.Type = SmartyPantType.LeftQuote;
                        quote.Type = SmartyPantType.RightQuote;
                        simpleQuote = null;
                    }
                }
                else if (quote.Type == SmartyPantType.DoubleQuote)
                {
                    if (doubleQuote == null)
                    {
                        doubleQuote = quote;
                    }
                    else
                    {
                        doubleQuote.Type = SmartyPantType.LeftDoubleQuote;
                        quote.Type = SmartyPantType.RightDoubleQuote;
                        doubleQuote = null;
                    }
                }
            }

            pants.Clear();
        }
    }
}