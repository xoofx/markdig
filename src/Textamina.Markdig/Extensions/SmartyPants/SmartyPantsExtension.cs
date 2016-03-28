// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Parsers.Inlines;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.SmartyPants
{
    public class SmartyPantsExtension : IMarkdownExtension
    {
        public SmartyPantsExtension(SmartyPantOptions options)
        {
            Options = options ?? new SmartyPantOptions();
        }

        public SmartyPantOptions Options { get; }

        public void Setup(MarkdownPipeline pipeline)
        {
            if (!pipeline.InlineParsers.Contains<SmaryPantsInlineParser>())
            {
                // Insert the parser after the code span parser
                pipeline.InlineParsers.InsertAfter<CodeInlineParser>(new SmaryPantsInlineParser());
            }

            var htmlRenderer = pipeline.Renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<HtmlSmartyPantRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Add(new HtmlSmartyPantRenderer(Options));
                }
            }
        }
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
            Mapping = new Dictionary<SmartyPantType, string>()
            {
                {SmartyPantType.Quote, "'"},
                {SmartyPantType.DoubleQuote, "\""},
                {SmartyPantType.LeftQuote, "&lsquo;"},
                {SmartyPantType.RightQuote, "&rsquo;"},
                {SmartyPantType.LeftDoubleQuote, "&ldquo;"},
                {SmartyPantType.RightDoubleQuote, "&rdquo;"},
                {SmartyPantType.LeftAngleQuote, "&laquo;"},
                {SmartyPantType.RightAngleQuote, "&raquo;"},
                {SmartyPantType.Ellipsis, "&hellip;"},
                {SmartyPantType.Dash2, "&ndash;"},
                {SmartyPantType.Dash3, "&mdash;"},
            };
        }

        public Dictionary<SmartyPantType, string> Mapping { get; }
    }

    public class HtmlSmartyPantRenderer : HtmlObjectRenderer<SmartyPant>
    {
        private static readonly SmartyPantOptions DefaultOptions = new SmartyPantOptions();

        private readonly SmartyPantOptions options;

        public HtmlSmartyPantRenderer(SmartyPantOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            this.options = options;
        }

        protected override void Write(HtmlRenderer renderer, SmartyPant obj)
        {
            string text;
            if (!options.Mapping.TryGetValue(obj.Type, out text))
            {
                DefaultOptions.Mapping.TryGetValue(obj.Type, out text);
            }
            renderer.Write(text);
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

            var pc = slice.PeekCharExtra(-1);
            var c = slice.CurrentChar;
            var openingChar = c;

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
                case SmartyPantType.Dash2:
                case SmartyPantType.Dash3:
                    if (!canOpen && !canClose)
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
            SmartyPant previousSimpleQuote = null;
            SmartyPant previousDoubleQuote = null;
            SmartyPant previousAngleQuote = null;
            for (int i = 0; i < pants.Count; i++)
            {
                var quote = pants[i];

                if (quote.Type == SmartyPantType.LeftQuote || quote.Type == SmartyPantType.RightQuote)
                {
                    HandleLeftRight(quote, ref previousSimpleQuote, SmartyPantType.LeftQuote, SmartyPantType.RightQuote,
                        "'", "'");
                }
                else if (quote.Type == SmartyPantType.LeftDoubleQuote || quote.Type == SmartyPantType.RightDoubleQuote)
                {
                    var isRegularDoubleQuote = quote.OpeningCharacter == '"';

                    HandleLeftRight(quote, ref previousDoubleQuote, SmartyPantType.LeftDoubleQuote, SmartyPantType.RightDoubleQuote,
                        isRegularDoubleQuote ? "\"" : "``", isRegularDoubleQuote ? "\"" : "''");
                }
                else if (quote.Type == SmartyPantType.LeftAngleQuote || quote.Type == SmartyPantType.RightAngleQuote)
                {
                    HandleLeftRight(quote, ref previousAngleQuote, SmartyPantType.LeftAngleQuote, SmartyPantType.RightAngleQuote,
                        "<<", ">>");
                }
            }

            pants.Clear();
        }

        private void HandleLeftRight(SmartyPant quote, ref SmartyPant previousQuote, SmartyPantType left, SmartyPantType right, string leftText, string rightText)
        {
            if (previousQuote == null)
            {
                if (quote.Type == left)
                {
                    previousQuote = quote;
                }
                else
                {
                    quote.ReplaceBy(new LiteralInline(rightText));
                }
            }
            else
            {
                if (quote.Type == left)
                {
                    previousQuote.ReplaceBy(new LiteralInline(leftText));
                    previousQuote = quote;
                }
                else
                {
                    previousQuote = null;
                }
            }
        }
    }
}