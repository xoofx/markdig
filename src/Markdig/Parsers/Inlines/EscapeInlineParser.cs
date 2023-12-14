// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax.Inlines;

namespace Markdig.Parsers.Inlines;

/// <summary>
/// An inline parser for escape characters.
/// </summary>
/// <seealso cref="InlineParser" />
public class EscapeInlineParser : InlineParser
{
    public EscapeInlineParser()
    {
        OpeningCharacters = ['\\'];
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        var startPosition = slice.Start;
        // Go to escape character
        var c = slice.NextChar();
        int line;
        int column;
        if (c.IsAsciiPunctuation())
        {
            processor.Inline = new LiteralInline()
            {
                Content = new StringSlice(slice.Text, slice.Start, slice.Start),
                Span = { Start = processor.GetSourcePosition(startPosition, out line, out column) },
                Line = line,
                Column = column,
                IsFirstCharacterEscaped = true,
            };
            processor.Inline.Span.End = processor.Inline.Span.Start + 1;
            slice.SkipChar();
            return true;
        }

        // A backslash at the end of the line is a [hard line break]:
        if (c == '\n' || c == '\r')
        {
            var newLine = c == '\n' ? NewLine.LineFeed : NewLine.CarriageReturn;
            if (c == '\r' && slice.PeekChar() == '\n')
            {
                newLine = NewLine.CarriageReturnLineFeed;
            }
            var inline = new LineBreakInline()
            {
                IsHard = true,
                IsBackslash = true,
                Span = { Start = processor.GetSourcePosition(startPosition, out line, out column) },
                Line = line,
                Column = column,
            };
            processor.Inline = inline;

            if (processor.TrackTrivia)
            {
                inline.NewLine = newLine;
            }
            
            inline.Span.End = inline.Span.Start + 1;
            slice.SkipChar(); // Skip \n or \r alone
            if (newLine == NewLine.CarriageReturnLineFeed)
            {
                slice.SkipChar(); // Skip \r\n
            }
            return true;
        }

        return false;
    }
}