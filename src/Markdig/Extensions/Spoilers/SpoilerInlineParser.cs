// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Markdig.Extensions.Spoilers;

/// <summary>
///     The parser for inline spoilers.
/// </summary>
/// <seealso cref="SpoilerExtension"/>
internal sealed class SpoilerInlineParser : InlineParser
{
    public SpoilerInlineParser() => OpeningCharacters = ['|'];

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        char match = slice.CurrentChar;
        if (slice.PeekCharExtra(-1) == match)
        {
            return false;
        }

        slice.SkipChar();

        int openPipeCount = 1;
        while (slice.CurrentChar == match)
        {
            ++openPipeCount;
            slice.SkipChar();
        }

        ReadOnlySpan<char> span = slice.AsSpan();
        int i = span.IndexOfAny('\r', '\n', match);
        if (i < 0 || (uint)i >= (uint)span.Length)
        {
            return false;
        }

        int closePipeCount = 0;
        while ((uint)i < (uint)span.Length && span[i] == match)
        {
            closePipeCount++;
            i++;
        }

        span = span.Slice(i);
        if (openPipeCount != closePipeCount)
        {
            return false;
        }

        ReadOnlySpan<char> rawContent = slice.AsSpan().Slice(0, slice.Length - span.Length - openPipeCount);

        StringSlice content = new(slice.Text, slice.Start, slice.Start + rawContent.Length - 1);

        int startPosition = slice.Start;
        slice.Start = startPosition + rawContent.Length + openPipeCount;

        startPosition -= openPipeCount;

        SpoilerInline inline = new(content)
        {
            Span = new SourceSpan(processor.GetSourcePosition(startPosition, out int line, out int column), processor.GetSourcePosition(slice.Start - 1)),
            Line = line,
            Column = column
        };
        HtmlAttributes attributes = inline.GetAttributes();
        attributes.AddClass("spoiler");

        processor.Inline = inline;
        return true;
    }
}