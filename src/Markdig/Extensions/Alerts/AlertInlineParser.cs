// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.Alerts;

/// <summary>
/// An inline parser for an alert inline (e.g. `[!NOTE]`).
/// </summary>
/// <seealso cref="InlineParser" />
public class AlertInlineParser : InlineParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlertInlineParser"/> class.
    /// </summary>
    public AlertInlineParser()
    {
        OpeningCharacters = ['['];
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        // We expect the alert to be the first child of a quote block. Example:
        // > [!NOTE]
        // > This is a note
        if (processor.Block is not ParagraphBlock paragraphBlock || paragraphBlock.Parent is not QuoteBlock quoteBlock || paragraphBlock.Inline?.FirstChild != null
            || quoteBlock is AlertBlock || quoteBlock.Parent is not MarkdownDocument)
        {
            return false;
        }

        var saved = slice;
        var c = slice.NextChar();
        if (c != '!')
        {
            slice = saved;
            return false;
        }

        c = slice.NextChar(); // Skip !

        var start = slice.Start;
        var end = start;
        while (c.IsAlpha())
        {
            end = slice.Start;
            c = slice.NextChar();
        }

        // We need at least one character
        if (c != ']' || start == end)
        {
            slice = saved;
            return false;
        }

        var alertType = new StringSlice(slice.Text, start, end);
        c = slice.NextChar(); // Skip ]

        start = slice.Start;
        while (true)
        {
            if (c == '\0' || c == '\n' || c == '\r')
            {
                end = slice.Start;
                if (c == '\r')
                {
                    c = slice.NextChar(); // Skip \r
                    if (c == '\0' || c == '\n')
                    {
                        end = slice.Start;
                        if (c == '\n')
                        {
                            slice.NextChar(); // Skip \n
                        }
                    }
                }
                else if (c == '\n')
                {
                    slice.NextChar(); // Skip \n
                }
                break;
            }
            else if (!c.IsSpaceOrTab())
            {
                slice = saved;
                return false;
            }

            c = slice.NextChar();
        }

        var alertBlock = new AlertBlock(alertType)
        {
            Span = quoteBlock.Span,
            TriviaSpaceAfterKind = new StringSlice(slice.Text, start, end),
            Line = quoteBlock.Line,
            Column = quoteBlock.Column,
        };

        alertBlock.GetAttributes().AddClass("markdown-alert");
        alertBlock.GetAttributes().AddClass($"markdown-alert-{alertType.ToString().ToLowerInvariant()}");

        // Replace the quote block with the alert block
        var parentQuoteBlock = quoteBlock.Parent!;
        var indexOfQuoteBlock = parentQuoteBlock.IndexOf(quoteBlock);
        parentQuoteBlock[indexOfQuoteBlock] = alertBlock;

        while (quoteBlock.Count > 0)
        {
            var block = quoteBlock[0];
            quoteBlock.RemoveAt(0);
            alertBlock.Add(block);
        }

        // Workaround to replace the parent container
        // Experimental API, so we are keeping it internal for now until we are sure it's the way we want to go
        processor.ReplaceParentContainer(quoteBlock, alertBlock);

        return true;
    }
}
