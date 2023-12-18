// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.SmartyPants;

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
        OpeningCharacters = ['\'', '"', '<', '>', '.', '-'];
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
                if (slice.PeekChar() == '\'')
                {
                    slice.SkipChar();
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

        CharHelper.CheckOpenCloseDelimiter(pc, c, false, out bool canOpen, out bool canClose);

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
            Span = { Start = processor.GetSourcePosition(startingPosition, out int line, out int column) },
            Line = line,
            Column = column,
            OpeningCharacter = openingChar,
            Type = type
        };
        pant.Span.End = pant.Span.Start + slice.Start - startingPosition - 1;

        // We will check in a post-process step for balanced open/close quotes
        if (postProcess)
        {
            var quotePants = GetOrCreateState(processor);

            // Register only if we don't have yet any quotes
            if (quotePants.Count is 0)
            {
                processor.Block!.ProcessInlinesEnd += BlockOnProcessInlinesEnd;
            }
            quotePants.Add(pant);
        }

        processor.Inline = pant;
        return true;
    }

    private ListSmartyPants GetOrCreateState(InlineProcessor processor)
    {
        if (!(processor.ParserStates[Index] is ListSmartyPants quotePants))
        {
            processor.ParserStates[Index] = quotePants = new ListSmartyPants();
        }
        return quotePants;
    }

    private readonly struct Opener
    {
        public readonly int Type;
        public readonly int Index;

        public Opener(int type, int index)
        {
            Type = type;
            Index = index;
        }
    }

    private void BlockOnProcessInlinesEnd(InlineProcessor processor, Inline? inline)
    {
        processor.Block!.ProcessInlinesEnd -= BlockOnProcessInlinesEnd;

        var pants = (ListSmartyPants) processor.ParserStates[Index];

        var openers = new Stack<Opener>(4);

        for (int i = 0; i < pants.Count; i++)
        {
            var quote = pants[i];
            var quoteType = quote.Type;

            int type;
            bool isLeft;

            if (quoteType is SmartyPantType.LeftQuote or SmartyPantType.RightQuote)
            {
                type = 0;
                isLeft = quoteType == SmartyPantType.LeftQuote;
            }
            else if (quoteType is SmartyPantType.LeftDoubleQuote or SmartyPantType.RightDoubleQuote)
            {
                type = 1;
                isLeft = quoteType == SmartyPantType.LeftDoubleQuote;
            }
            else if (quoteType is SmartyPantType.LeftAngleQuote or SmartyPantType.RightAngleQuote)
            {
                type = 2;
                isLeft = quoteType == SmartyPantType.LeftAngleQuote;
            }
            else
            {
                quote.ReplaceBy(quote.AsLiteralInline());
                continue;
            }

            if (isLeft)
            {
                openers.Push(new Opener(type, i));
            }
            else
            {
                bool found = false;

                while (openers.Count > 0)
                {
                    Opener opener = openers.Pop();
                    var previousQuote = pants[opener.Index];

                    if (opener.Type == type)
                    {
                        found = true;
                        break;
                    }
                    else
                    {
                        previousQuote.ReplaceBy(previousQuote.AsLiteralInline());
                    }
                }

                if (!found)
                {
                    quote.ReplaceBy(quote.AsLiteralInline());
                }
            }
        }

        foreach (var opener in openers)
        {
            var quote = pants[opener.Index];
            quote.ReplaceBy(quote.AsLiteralInline());
        }

        pants.Clear();
    }

    bool IPostInlineProcessor.PostProcess(
        InlineProcessor state,
        Inline? root,
        Inline? lastChild,
        int postInlineProcessorIndex,
        bool isFinalProcessing)
    {
        // Don't try to process anything if there are no dash
        if (!(state.ParserStates[Index] is ListSmartyPants quotePants) || !quotePants.HasDash)
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

                if (child is LiteralInline literal)
                {
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
                else if (child is ContainerInline childContainer)
                {
                    pendingContainers.Push(childContainer.FirstChild!);
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


    private sealed class ListSmartyPants : List<SmartyPant>
    {
        public bool HasDash { get; set; }
    }
}