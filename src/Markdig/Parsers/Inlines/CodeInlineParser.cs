// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Diagnostics;

using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Parsers.Inlines;

/// <summary>
/// An inline parser for a <see cref="CodeInline"/>.
/// </summary>
/// <seealso cref="InlineParser" />
public class CodeInlineParser : InlineParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeInlineParser"/> class.
    /// </summary>
    public CodeInlineParser()
    {
        OpeningCharacters = ['`'];
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        char match = slice.CurrentChar;
        if (slice.PeekCharExtra(-1) == match)
        {
            return false;
        }

        Debug.Assert(match is not ('\r' or '\n'));

        // Match the opened sticks
        int openSticks = slice.CountAndSkipChar(match);

        // A backtick string is a string of one or more backtick characters (`) that is neither preceded nor followed by a backtick.
        // A code span begins with a backtick string and ends with a backtick string of equal length.
        // The contents of the code span are the characters between the two backtick strings, normalized in the following ways:

        // 1. line endings are converted to spaces.

        // 2. If the resulting string both begins AND ends with a space character, but does not consist entirely
        // of space characters, a single space character is removed from the front and back.
        // This allows you to include code that begins or ends with backtick characters, which must be separated by
        // whitespace from the opening or closing backtick strings.

        ReadOnlySpan<char> span = slice.AsSpan();
        bool containsNewLines = false;

        while (true)
        {
            int i = span.IndexOfAny('\r', '\n', match);

            if ((uint)i >= (uint)span.Length)
            {
                // We got to the end of the input before seeing the match character. CodeInline can't match here.
                return false;
            }

            int closeSticks = 0;

            while ((uint)i < (uint)span.Length && span[i] == match)
            {
                closeSticks++;
                i++;
            }

            span = span.Slice(i);

            if (openSticks == closeSticks)
            {
                break;
            }
            else if (closeSticks == 0)
            {
                containsNewLines = true;
                span = span.Slice(1);
            }
        }

        ReadOnlySpan<char> rawContent = slice.AsSpan().Slice(0, slice.Length - span.Length - openSticks);

        var content = containsNewLines
            ? new LazySubstring(ReplaceNewLines(rawContent)) // Should be the rare path.
            : new LazySubstring(slice.Text, slice.Start, rawContent.Length);

        // Remove one space from front and back if the string is not all spaces
        if (rawContent.Length > 2 &&
            rawContent[0] is ' ' or '\n' &&
            rawContent[rawContent.Length - 1] is ' ' or '\n' &&
            rawContent.ContainsAnyExcept(' ', '\r', '\n'))
        {
            content.Offset++;
            content.Length -= 2;
        }

        int startPosition = slice.Start;
        slice.Start = startPosition + rawContent.Length + openSticks;

        // We've already skipped the opening sticks. Account for that here.
        startPosition -= openSticks;

        var codeInline = new CodeInline(content)
        {
            Delimiter = slice.Text[startPosition],
            Span = new SourceSpan(processor.GetSourcePosition(startPosition, out int line, out int column), processor.GetSourcePosition(slice.Start - 1)),
            Line = line,
            Column = column,
            DelimiterCount = openSticks,
        };

        if (processor.TrackTrivia)
        {
            // startPosition and slice.Start include the opening/closing sticks.
            codeInline.ContentWithTrivia = new StringSlice(slice.Text, startPosition + openSticks, slice.Start - openSticks - 1);
        }

        processor.Inline = codeInline;
        return true;
    }

    private static string ReplaceNewLines(ReadOnlySpan<char> content)
    {
        var builder = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);

        while (true)
        {
            int i = content.IndexOfAny('\r', '\n');

            if ((uint)i >= (uint)content.Length)
            {
                builder.Append(content);
                break;
            }

            builder.Append(content.Slice(0, i));

            if (content[i] == '\n')
            {
                // Transform '\n' into a single space
                builder.Append(' ');
            }

            content = content.Slice(i + 1);
        }

        return builder.ToString();
    }
}