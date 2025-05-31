// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Markdig.Syntax;

namespace Markdig.Helpers;

/// <summary>
/// Helpers to parse Markdown links.
/// </summary>
public static class LinkHelper
{
    public static bool TryParseAutolink(StringSlice text, [NotNullWhen(true)] out string? link, out bool isEmail)
    {
        return TryParseAutolink(ref text, out link, out isEmail);
    }

    public static string Urilize(string headingText, bool allowOnlyAscii, bool keepOpeningDigits = false)
    {
        return Urilize(headingText.AsSpan(), allowOnlyAscii, keepOpeningDigits);
    }

    public static string Urilize(ReadOnlySpan<char> headingText, bool allowOnlyAscii, bool keepOpeningDigits = false)
    {
        var headingBuffer = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);
        bool hasLetter = keepOpeningDigits && headingText.Length > 0 && char.IsLetterOrDigit(headingText[0]);
        bool previousIsSpace = false;
        for (int i = 0; i < headingText.Length; i++)
        {
            var c = headingText[i];
            var normalized = allowOnlyAscii ? CharNormalizer.ConvertToAscii(c) : null;
            for (int j = 0; j < (normalized?.Length ?? 1); j++)
            {
                if (normalized != null)
                {
                    c = normalized[j];
                }

                if (char.IsLetter(c))
                {
                    if (allowOnlyAscii && (c < ' ' || c >= 127))
                    {
                        continue;
                    }
                    c = char.IsUpper(c) ? char.ToLowerInvariant(c) : c;
                    headingBuffer.Append(c);
                    hasLetter = true;
                    previousIsSpace = false;
                }
                else if (hasLetter)
                {
                    if (IsReservedPunctuation(c))
                    {
                        if (previousIsSpace)
                        {
                            headingBuffer.Length--;
                        }
                        if (headingBuffer[headingBuffer.Length - 1] != c)
                        {
                            headingBuffer.Append(c);
                        }
                        previousIsSpace = false;
                    }
                    else if (c.IsDigit())
                    {
                        headingBuffer.Append(c);
                        previousIsSpace = false;
                    }
                    else if (!previousIsSpace && c.IsWhitespace())
                    {
                        var pc = headingBuffer[headingBuffer.Length - 1];
                        if (!IsReservedPunctuation(pc))
                        {
                            headingBuffer.Append('-');
                        }
                        previousIsSpace = true;
                    }
                }
            }
        }

        // Trim trailing _ - .
        while (headingBuffer.Length > 0)
        {
            var c = headingBuffer[headingBuffer.Length - 1];
            if (IsReservedPunctuation(c))
            {
                headingBuffer.Length--;
            }
            else
            {
                break;
            }
        }

        return headingBuffer.ToString();
    }

    public static string UrilizeAsGfm(string headingText)
    {
        return UrilizeAsGfm(headingText.AsSpan());
    }

    public static string UrilizeAsGfm(ReadOnlySpan<char> headingText)
    {
        // Following https://github.com/jch/html-pipeline/blob/master/lib/html/pipeline/toc_filter.rb
        var headingBuffer = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);
        for (int i = 0; i < headingText.Length; i++)
        {
            var c = headingText[i];
            if (char.IsLetterOrDigit(c) || c == '-' || c == '_')
            {
                headingBuffer.Append(char.ToLowerInvariant(c));
            }
            else if (c == ' ')
            {
                headingBuffer.Append('-');
            }
        }
        return headingBuffer.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsReservedPunctuation(char c)
    {
        return c == '_' || c == '-' || c == '.';
    }

    public static bool TryParseAutolink(ref StringSlice text, [NotNullWhen(true)] out string? link, out bool isEmail)
    {
        link = null;
        isEmail = false;

        var c = text.CurrentChar;
        if (c != '<')
        {
            return false;
        }

        // An absolute URI, for these purposes, consists of a scheme followed by a colon (:) 
        // followed by zero or more characters other than ASCII whitespace and control characters, <, and >. 
        // If the URI includes these characters, they must be percent-encoded (e.g. %20 for a space).
        // A URI that would end with a full stop (.) is treated instead as ending immediately before the full stop.

        // a scheme is any sequence of 2–32 characters 
        // beginning with an ASCII letter 
        // and followed by any combination of ASCII letters, digits, or the symbols plus (”+”), period (”.”), or hyphen (”-”).

        // An email address, for these purposes, is anything that matches the non-normative regex from the HTML5 spec:
        // /^
        // [a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+
        // @
        // [a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?
        // (?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/

        c = text.NextChar();

        // -1: scan email
        //  0: scan uri or email
        // +1: scan uri
        int state = 0;

        if (!c.IsAlpha())
        {
            // We may have an email char?
            if (CharHelper.IsEmailUsernameSpecialCharOrDigit(c))
            {
                state = -1;
            }
            else
            {
                return false;
            }
        }

        var builder = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);

        // ****************************
        // 1. Scan scheme or user email
        // ****************************
        builder.Append(c);
        while (true)
        {
            c = text.NextChar();

            // Chars valid for both scheme and email
            var isSpecialChar = c == '+' || c == '.' || c == '-';
            var isValidChar = c.IsAlphaNumeric() || isSpecialChar;
            if (state <= 0 && CharHelper.IsEmailUsernameSpecialChar(c))
            {
                isValidChar = true;
                // If this is not a special char valid also for url scheme, then we have an email
                if (!isSpecialChar)
                {
                    state = -1;
                }
            }

            if (isValidChar)
            {
                // a scheme is any sequence of 2–32 characters 
                if (state > 0 && builder.Length >= 32)
                {
                    goto ReturnFalse;
                }
                builder.Append(c);
            }
            else if (c == ':')
            {
                if (state < 0 || builder.Length <= 2)
                {
                    goto ReturnFalse;
                }
                state = 1;
                break;
            } else if (c == '@')
            {
                if (state > 0)
                {
                    goto ReturnFalse;
                }
                state = -1;
                break;
            }
            else
            {
                goto ReturnFalse;
            }
        }

        // append ':' or '@' 
        builder.Append(c); 

        if (state < 0)
        {
            isEmail = true;

            // scan an email
            // [a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?
            // (?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/
            bool hasMinus = false;
            int domainCharCount = 0;
            char pc = '\0';
            while (true)
            {
                c = text.NextChar();
                if (c == '>')
                {
                    if (domainCharCount == 0 || hasMinus)
                    {
                        break;
                    }

                    text.SkipChar();
                    link = builder.ToString();
                    return true;
                }

                if (c.IsAlphaNumeric() || (domainCharCount > 0 && (hasMinus = c == '-')))
                {
                    domainCharCount++;
                    if (domainCharCount > 63)
                    {
                        break;
                    }
                }
                else if (c == '.')
                {
                    if (pc == '.' || pc == '-')
                    {
                        break;
                    }
                    domainCharCount = 0;
                    hasMinus = false;
                }
                else
                {
                    break;
                }
                builder.Append(c);
                pc = c;
            }
        }
        else
        {
            // 6.5 Autolinks - https://spec.commonmark.org/0.31.2/#autolinks
            // An absolute URI, for these purposes, consists of a scheme followed by a colon (:) followed by
            // zero or more characters other than ASCII control characters, space, <, and >.
            // If the URI includes these characters, they must be percent-encoded (e.g. %20 for a space).
            //
            // 2.1 Characters and lines
            // An ASCII control character is a character between U+0000–1F (both including) or U+007F.

            text.SkipChar();
            ReadOnlySpan<char> slice = text.AsSpan();

            Debug.Assert(!slice.Contains('\0'));

            // This set of invalid characters includes '>'.
            int end = slice.IndexOfAny(CharHelper.InvalidAutoLinkCharacters);

            if ((uint)end < (uint)slice.Length && slice[end] == '>')
            {
                // We've found '>' and all characters before it are valid.
#if NET
                link = string.Concat(builder.AsSpan(), slice.Slice(0, end));
                builder.Dispose();
#else
                builder.Append(slice.Slice(0, end));
                link = builder.ToString();
#endif
                text.Start += end + 1; // +1 to skip '>'
                return true;
            }
        }

    ReturnFalse:
        builder.Dispose();
        return false;
    }

    public static bool TryParseInlineLink(StringSlice text, out string? link, out string? title)
    {
        return TryParseInlineLink(ref text, out link, out title, out _, out _);
    }

    public static bool TryParseInlineLink(StringSlice text, out string? link, out string? title, out SourceSpan linkSpan, out SourceSpan titleSpan)
    {
        return TryParseInlineLink(ref text, out link, out title, out linkSpan, out titleSpan);
    }

    public static bool TryParseInlineLink(ref StringSlice text, out string? link, out string? title)
    {
        return TryParseInlineLink(ref text, out link, out title, out SourceSpan linkSpan, out SourceSpan titleSpan);
    }

    public static bool TryParseInlineLink(ref StringSlice text, out string? link, out string? title, out SourceSpan linkSpan, out SourceSpan titleSpan)
    {
        // 1. An inline link consists of a link text followed immediately by a left parenthesis (, 
        // 2. optional whitespace,  TODO: specs: is it whitespace or multiple whitespaces?
        // 3. an optional link destination, 
        // 4. an optional link title separated from the link destination by whitespace, 
        // 5. optional whitespace,  TODO: specs: is it whitespace or multiple whitespaces?
        // 6. and a right parenthesis )
        bool isValid = false;
        var c = text.CurrentChar;
        link = null;
        title = null;

        linkSpan = SourceSpan.Empty;
        titleSpan = SourceSpan.Empty;

        // 1. An inline link consists of a link text followed immediately by a left parenthesis (, 
        if (c == '(')
        {
            text.SkipChar();
            text.TrimStart(); // this breaks whitespace before an uri

            var pos = text.Start;
            if (TryParseUrl(ref text, out link, out _))
            {
                linkSpan.Start = pos;
                linkSpan.End = text.Start - 1;
                if (linkSpan.End < linkSpan.Start)
                {
                    linkSpan = SourceSpan.Empty;
                }

                text.TrimStart(out int spaceCount);
                var hasWhiteSpaces = spaceCount > 0;

                c = text.CurrentChar;
                if (c == ')')
                {
                    isValid = true;
                }
                else if (hasWhiteSpaces)
                {
                    c = text.CurrentChar;
                    pos = text.Start;
                    if (c == ')')
                    {
                        isValid = true;
                    }
                    else if (TryParseTitle(ref text, out title, out char enclosingCharacter))
                    {
                        titleSpan.Start = pos;
                        titleSpan.End = text.Start - 1;
                        if (titleSpan.End < titleSpan.Start)
                        {
                            titleSpan = SourceSpan.Empty;
                        }
                        text.TrimStart();
                        c = text.CurrentChar;

                        if (c == ')')
                        {
                            isValid = true;
                        }
                    }
                }
            }
        }

        if (isValid)
        {
            // Skip ')'
            text.SkipChar();
            // not to normalize nulls into empty strings, since LinkInline.Title property is nullable.
        }

        return isValid;
    }

    public static bool TryParseInlineLinkTrivia(
        ref StringSlice text,
        [NotNullWhen(true)] out string? link,
        out SourceSpan unescapedLink,
        out string? title,
        out SourceSpan unescapedTitle,
        out char titleEnclosingCharacter,
        out SourceSpan linkSpan,
        out SourceSpan titleSpan,
        out SourceSpan triviaBeforeLink,
        out SourceSpan triviaAfterLink,
        out SourceSpan triviaAfterTitle,
        out bool urlHasPointyBrackets)
    {
        // 1. An inline link consists of a link text followed immediately by a left parenthesis (, 
        // 2. optional whitespace,  TODO: specs: is it whitespace or multiple whitespaces?
        // 3. an optional link destination, 
        // 4. an optional link title separated from the link destination by whitespace, 
        // 5. optional whitespace,  TODO: specs: is it whitespace or multiple whitespaces?
        // 6. and a right parenthesis )
        bool isValid = false;
        var c = text.CurrentChar;
        link = null;
        unescapedLink = SourceSpan.Empty;
        title = null;
        unescapedTitle = SourceSpan.Empty;

        linkSpan = SourceSpan.Empty;
        titleSpan = SourceSpan.Empty;
        triviaBeforeLink = SourceSpan.Empty;
        triviaAfterLink = SourceSpan.Empty;
        triviaAfterTitle = SourceSpan.Empty;
        urlHasPointyBrackets = false;
        titleEnclosingCharacter = '\0';

        // 1. An inline link consists of a link text followed immediately by a left parenthesis (, 
        if (c == '(')
        {
            text.SkipChar();
            var sourcePosition = text.Start;
            text.TrimStart();
            triviaBeforeLink = new SourceSpan(sourcePosition, text.Start - 1);
            var pos = text.Start;
            if (TryParseUrlTrivia(ref text, out link, out urlHasPointyBrackets))
            {
                linkSpan.Start = pos;
                linkSpan.End = text.Start - 1;
                unescapedLink.Start = pos + (urlHasPointyBrackets ? 1 : 0);
                unescapedLink.End = text.Start - 1 - (urlHasPointyBrackets ? 1 : 0);
                if (linkSpan.End < linkSpan.Start)
                {
                    linkSpan = SourceSpan.Empty;
                }

                int triviaStart = text.Start;
                text.TrimStart(out int spaceCount);

                triviaAfterLink = new SourceSpan(triviaStart, text.Start - 1);
                var hasWhiteSpaces = spaceCount > 0;

                c = text.CurrentChar;
                if (c == ')')
                {
                    isValid = true;
                }
                else if (hasWhiteSpaces)
                {
                    c = text.CurrentChar;
                    pos = text.Start;
                    if (c == ')')
                    {
                        isValid = true;
                    }
                    else if (TryParseTitleTrivia(ref text, out title, out titleEnclosingCharacter))
                    {
                        titleSpan.Start = pos;
                        titleSpan.End = text.Start - 1;
                        unescapedTitle.Start = pos + 1; // skip opening character
                        unescapedTitle.End = text.Start - 1 - 1; // skip closing character
                        if (titleSpan.End < titleSpan.Start)
                        {
                            titleSpan = SourceSpan.Empty;
                        }
                        var startTrivia = text.Start;
                        text.TrimStart();
                        triviaAfterTitle = new SourceSpan(startTrivia, text.Start - 1);
                        c = text.CurrentChar;

                        if (c == ')')
                        {
                            isValid = true;
                        }
                    }
                }
            }
        }

        if (isValid)
        {
            // Skip ')'
            text.SkipChar();
            title ??= string.Empty;
        }
        return isValid;
    }

    public static bool TryParseTitle<T>(T text, out string? title) where T : ICharIterator
    {
        return TryParseTitle(ref text, out title, out _);
    }

    public static bool TryParseTitle<T>(ref T text, out string? title, out char enclosingCharacter) where T : ICharIterator
    {
        var buffer = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);
        enclosingCharacter = '\0';

        // a sequence of zero or more characters between straight double-quote characters ("), including a " character only if it is backslash-escaped, or
        // a sequence of zero or more characters between straight single-quote characters ('), including a ' character only if it is backslash-escaped, or
        var c = text.CurrentChar;
        if (c == '\'' || c == '"' || c == '(')
        {
            enclosingCharacter = c;
            var closingQuote = c == '(' ? ')' : c;
            bool hasEscape = false;
            bool isLineBlank = false; // the first line is never blank
            while ((c = text.NextChar()) != '\0')
            {
                if (c == '\r' || c == '\n')
                {
                    if (isLineBlank)
                    {
                        break;
                    }

                    if (hasEscape)
                    {
                        hasEscape = false;
                        buffer.Append('\\');
                    }

                    buffer.Append(c);

                    if (c == '\r' && text.PeekChar() == '\n')
                    {
                        buffer.Append('\n');
                        text.SkipChar();
                    }

                    isLineBlank = true;
                }
                else if (hasEscape)
                {
                    hasEscape = false;

                    if (!c.IsAsciiPunctuation())
                    {
                        buffer.Append('\\');
                    }

                    buffer.Append(c);
                }
                else if (c == closingQuote)
                {
                    // Skip last quote
                    text.SkipChar();
                    title = buffer.ToString();
                    return true;
                }
                else if (c == '\\')
                {
                    hasEscape = true;
                    isLineBlank = false;
                }
                else
                {
                    if (isLineBlank && !c.IsSpaceOrTab())
                    {
                        isLineBlank = false;
                    }

                    buffer.Append(c);
                }
            }
        }

        buffer.Dispose();
        title = null;
        return false;
    }

    public static bool TryParseTitleTrivia<T>(ref T text, out string? title, out char enclosingCharacter) where T : ICharIterator
    {
        var buffer = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);
        enclosingCharacter = '\0';

        // a sequence of zero or more characters between straight double-quote characters ("), including a " character only if it is backslash-escaped, or
        // a sequence of zero or more characters between straight single-quote characters ('), including a ' character only if it is backslash-escaped, or
        var c = text.CurrentChar;
        if (c == '\'' || c == '"' || c == '(')
        {
            enclosingCharacter = c;
            var closingQuote = c == '(' ? ')' : c;
            bool hasEscape = false;
            bool isLineBlank = false; // the first line is never blank
            while ((c = text.NextChar()) != '\0')
            {
                if (c == '\r' || c == '\n')
                {
                    if (isLineBlank)
                    {
                        break;
                    }

                    if (hasEscape)
                    {
                        hasEscape = false;
                        buffer.Append('\\');
                    }

                    buffer.Append(c);

                    if (c == '\r' && text.PeekChar() == '\n')
                    {
                        buffer.Append('\n');
                        text.SkipChar();
                    }

                    isLineBlank = true;
                }
                else if (hasEscape)
                {
                    hasEscape = false;

                    if (!c.IsAsciiPunctuation())
                    {
                        buffer.Append('\\');
                    }

                    buffer.Append(c);
                }
                else if (c == closingQuote)
                {
                    // Skip last quote
                    text.SkipChar();
                    title = buffer.ToString();
                    return true;
                }
                else if (c == '\\')
                {
                    hasEscape = true;
                    isLineBlank = false;
                }
                else
                {
                    if (isLineBlank && !c.IsSpaceOrTab())
                    {
                        isLineBlank = false;
                    }

                    buffer.Append(c);
                }
            }
        }

        buffer.Dispose();
        title = null;
        return false;
    }

    public static bool TryParseUrl<T>(T text, [NotNullWhen(true)] out string? link) where T : ICharIterator
    {
        return TryParseUrl(ref text, out link, out _);
    }

    public static bool TryParseUrl<T>(ref T text, [NotNullWhen(true)] out string? link, out bool hasPointyBrackets, bool isAutoLink = false) where T : ICharIterator
    {
        bool isValid = false;
        hasPointyBrackets = false;
        var buffer = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);

        var c = text.CurrentChar;

        // a sequence of zero or more characters between an opening < and a closing > 
        // that contains no line breaks, or unescaped < or > characters, or
        if (c == '<')
        {
            bool hasEscape = false;
            do
            {
                c = text.NextChar();
                if (!hasEscape && c == '>')
                {
                    text.SkipChar();
                    hasPointyBrackets = true;
                    isValid = true;
                    break;
                }

                if (!hasEscape && c == '<')
                {
                    break;
                }

                if (hasEscape)
                {
                    hasEscape = false;
                    if (!c.IsAsciiPunctuation())
                    {
                        buffer.Append('\\');
                    }
                }
                else if (c == '\\')
                {
                    hasEscape = true;
                    continue;
                }

                if (c.IsNewLineOrLineFeed())
                {
                    break;
                }

                buffer.Append(c);

            } while (c != '\0');
        }
        else
        {
            // a nonempty sequence of characters that does not start with <, does not include ASCII space or control characters,
            // and includes parentheses only if (a) they are backslash-escaped or (b) they are part of a 
            // balanced pair of unescaped parentheses that is not itself inside a balanced pair of unescaped 
            // parentheses. 
            bool hasEscape = false;
            int openedParent = 0;
            while (true)
            {
                // Match opening and closing parenthesis
                if (c == '(')
                {
                    if (!hasEscape)
                    {
                        openedParent++;
                    }
                }

                if (c == ')')
                {
                    if (!hasEscape)
                    {
                        openedParent--;
                        if (openedParent < 0)
                        {
                            isValid = true;
                            break;
                        }
                    }
                }

                if (!isAutoLink)
                {
                    if (hasEscape)
                    {
                        hasEscape = false;
                        if (!c.IsAsciiPunctuation())
                        {
                            buffer.Append('\\');
                        }
                    }
                    // If we have an escape
                    else if (c == '\\')
                    {
                        hasEscape = true;
                        c = text.NextChar();
                        continue;
                    }
                }

                if (IsEndOfUri(c, isAutoLink))
                {
                    isValid = true;
                    break;
                }

                if (isAutoLink)
                {
                    if (c == '&')
                    {
                        if (HtmlHelper.ScanEntity(text, out _, out _, out _) > 0)
                        {
                            isValid = true;
                            break;
                        }
                    }
                    if (IsTrailingUrlStopCharacter(c) && IsEndOfUri(text.PeekChar(), true))
                    {
                        isValid = true;
                        break;
                    }
                }

                buffer.Append(c);

                c = text.NextChar();
            }

            if (openedParent > 0)
            {
                isValid = false;
            }
        }

        if (isValid)
        {
            link = buffer.ToString();
        }
        else
        {
            buffer.Dispose();
            link = null;
        }
        return isValid;
    }

    public static bool TryParseUrlTrivia<T>(ref T text, out string? link, out bool hasPointyBrackets, bool isAutoLink = false) where T : ICharIterator
    {
        bool isValid = false;
        hasPointyBrackets = false;
        var buffer = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);

        var c = text.CurrentChar;

        // a sequence of zero or more characters between an opening < and a closing > 
        // that contains no line breaks, or unescaped < or > characters, or
        if (c == '<')
        {
            bool hasEscape = false;
            do
            {
                c = text.NextChar();
                if (!hasEscape && c == '>')
                {
                    text.SkipChar();
                    hasPointyBrackets = true;
                    isValid = true;
                    break;
                }

                if (!hasEscape && c == '<')
                {
                    break;
                }

                if (hasEscape)
                {
                    hasEscape = false;
                    if (!c.IsAsciiPunctuation())
                    {
                        buffer.Append('\\');
                    }
                }
                else if (c == '\\')
                {
                    hasEscape = true;
                    continue;
                }

                if (c.IsNewLineOrLineFeed())
                {
                    break;
                }

                buffer.Append(c);

            } while (c != '\0');
        }
        else
        {
            // a nonempty sequence of characters that does not start with <, does not include ASCII space or control characters,
            // and includes parentheses only if (a) they are backslash-escaped or (b) they are part of a 
            // balanced pair of unescaped parentheses that is not itself inside a balanced pair of unescaped 
            // parentheses. 
            bool hasEscape = false;
            int openedParent = 0;
            while (true)
            {
                // Match opening and closing parenthesis
                if (c == '(')
                {
                    if (!hasEscape)
                    {
                        openedParent++;
                    }
                }

                if (c == ')')
                {
                    if (!hasEscape)
                    {
                        openedParent--;
                        if (openedParent < 0)
                        {
                            isValid = true;
                            break;
                        }
                    }
                }

                if (!isAutoLink)
                {
                    if (hasEscape)
                    {
                        hasEscape = false;
                        if (!c.IsAsciiPunctuation())
                        {
                            buffer.Append('\\');
                        }
                    }
                    // If we have an escape
                    else if (c == '\\')
                    {
                        hasEscape = true;
                        c = text.NextChar();
                        continue;
                    }
                }

                if (IsEndOfUri(c, isAutoLink))
                {
                    isValid = true;
                    break;
                }

                if (isAutoLink)
                {
                    if (c == '&')
                    {
                        if (HtmlHelper.ScanEntity(text, out _, out _, out _) > 0)
                        {
                            isValid = true;
                            break;
                        }
                    }
                    if (IsTrailingUrlStopCharacter(c) && IsEndOfUri(text.PeekChar(), true))
                    {
                        isValid = true;
                        break;
                    }
                }

                buffer.Append(c);

                c = text.NextChar();
            }

            if (openedParent > 0)
            {
                isValid = false;
            }
        }

        if (isValid)
        {
            link = buffer.ToString();
        }
        else
        {
            buffer.Dispose();
            link = null;
        }
        return isValid;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsTrailingUrlStopCharacter(char c)
    {
        // Trailing punctuation (specifically, ?, !, ., ,, :, *, _, and ~) will not be considered part of the autolink, though they may be included in the interior of the link:
        return c == '?' || c == '!' || c == '.' || c == ',' || c == ':' || c == '*' || c == '*' || c == '_' || c == '~';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsEndOfUri(char c, bool isAutoLink)
    {
        return c == '\0' || c.IsSpaceOrTab() || c.IsControl() || (isAutoLink && c == '<'); // TODO: specs unclear. space is strict or relaxed? (includes tabs?)
    }

    public static bool IsValidDomain(string link, int prefixLength, bool allowDomainWithoutPeriod = false)
    {
        // https://github.github.com/gfm/#extended-www-autolink
        // A valid domain consists of alphanumeric characters, underscores (_), hyphens (-) and periods (.).
        // There must be at least one period, and no underscores may be present in the last two segments of the domain.

        // Extended as of https://github.com/lunet-io/markdig/issues/316 to accept non-ascii characters,
        // as long as they are not in the space or punctuation categories

        int segmentCount = 1;
        bool segmentHasCharacters = false;
        int lastUnderscoreSegment = -1;

        for (int i = prefixLength; (uint)i < (uint)link.Length; i++)
        {
            char c = link[i];

            if (!c.IsAlphaNumeric())
            {
                if (c == '.') // New segment
                {
                    if (!segmentHasCharacters)
                        return false;

                    segmentCount++;
                    segmentHasCharacters = false;
                    continue;
                }

                if (c == '/' || c == '?' || c == '#' || c == ':') // End of domain name
                    break;

                if (c == '_')
                {
                    lastUnderscoreSegment = segmentCount;
                }
                else if (c != '-' && CharHelper.IsSpaceOrPunctuationForGFMAutoLink(c))
                {
                    // An invalid character has been found
                    return false;
                }
            }

            segmentHasCharacters = true;
        }

        return (segmentCount != 1 || allowDomainWithoutPeriod) && // At least one dot was present
            segmentHasCharacters && // Last segment has valid characters
            segmentCount - lastUnderscoreSegment >= 2; // No underscores are present in the last two segments of the domain
    }

    public static bool TryParseLinkReferenceDefinition<T>(ref T text,
        out string? label,
        out string? url,
        out string? title,
        out SourceSpan labelSpan,
        out SourceSpan urlSpan,
        out SourceSpan titleSpan) where T : ICharIterator
    {
        url = null;
        title = null;

        urlSpan = SourceSpan.Empty;
        titleSpan = SourceSpan.Empty;

        if (!TryParseLabel(ref text, out label, out labelSpan))
        {
            return false;
        }

        if (text.CurrentChar != ':')
        {
            label = null;
            return false;
        }
        text.SkipChar(); // Skip ':'

        // Skip any whitespace before the url
        text.TrimStart();

        urlSpan.Start = text.Start;
        bool isAngleBracketsUrl = text.CurrentChar == '<';
        if (!TryParseUrl(ref text, out url, out _) || (!isAngleBracketsUrl && string.IsNullOrEmpty(url)))
        {
            return false;
        }
        urlSpan.End = text.Start - 1;

        var saved = text;
        var hasWhiteSpaces = CharIteratorHelper.TrimStartAndCountNewLines(ref text, out int newLineCount);
        var c = text.CurrentChar;
        if (c == '\'' || c == '"' || c == '(')
        {
            titleSpan.Start = text.Start;
            if (TryParseTitle(ref text, out title, out _))
            {
                titleSpan.End = text.Start - 1;
                // If we have a title, it requires a whitespace after the url
                if (!hasWhiteSpaces)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (text.IsEmpty || newLineCount > 0)
            {
                return true;
            }
        }

        // Check that the current line has only trailing spaces
        c = text.CurrentChar;
        while (c.IsSpaceOrTab())
        {
            c = text.NextChar();
        }

        if (c != '\0' && c != '\n' && c != '\r')
        {
            // If we were able to parse the url but the title doesn't end with space, 
            // we are still returning a valid definition
            if (newLineCount > 0 && title != null)
            {
                text = saved;
                title = null;
                return true;
            }

            label = null;
            url = null;
            title = null;
            return false;
        }

        if (c == '\r' && text.PeekChar() == '\n')
        {
            text.SkipChar();
        }

        return true;
    }

    public static bool TryParseLinkReferenceDefinitionTrivia<T>(
        ref T text,
        out SourceSpan triviaBeforeLabel,
        out string? label,
        out SourceSpan labelWithTrivia,
        out SourceSpan triviaBeforeUrl, // can contain newline
        out string? url,
        out SourceSpan unescapedUrl,
        out bool urlHasPointyBrackets,
        out SourceSpan triviaBeforeTitle, // can contain newline
        out string? title, // can contain non-consecutive newlines
        out SourceSpan unescapedTitle,
        out char titleEnclosingCharacter,
        out NewLine newLine,
        out SourceSpan triviaAfterTitle,
        out SourceSpan labelSpan,
        out SourceSpan urlSpan,
        out SourceSpan titleSpan) where T : ICharIterator
    {
        labelWithTrivia = SourceSpan.Empty;
        triviaBeforeUrl = SourceSpan.Empty;
        url = null;
        unescapedUrl = SourceSpan.Empty;
        triviaBeforeTitle = SourceSpan.Empty;
        title = null;
        unescapedTitle = SourceSpan.Empty;
        newLine = NewLine.None;

        urlSpan = SourceSpan.Empty;
        titleSpan = SourceSpan.Empty;

        text.TrimStart();
        triviaBeforeLabel = new SourceSpan(0, text.Start - 1);
        triviaAfterTitle = SourceSpan.Empty;
        urlHasPointyBrackets = false;
        titleEnclosingCharacter = '\0';

        labelWithTrivia.Start = text.Start + 1; // skip opening [
        if (!TryParseLabelTrivia(ref text, out label, out labelSpan))
        {
            return false;
        }
        labelWithTrivia.End = text.Start - 2; // skip closing ] and subsequent :

        if (text.CurrentChar != ':')
        {
            label = null;
            return false;
        }
        text.SkipChar(); // Skip ':'
        var triviaBeforeUrlStart = text.Start;

        // Skip any whitespace before the url
        text.TrimStart();
        triviaBeforeUrl = new SourceSpan(triviaBeforeUrlStart, text.Start - 1);

        urlSpan.Start = text.Start;
        bool isAngleBracketsUrl = text.CurrentChar == '<';
        unescapedUrl.Start = text.Start + (isAngleBracketsUrl ? 1 : 0);
        if (!TryParseUrlTrivia(ref text, out url, out urlHasPointyBrackets) || (!isAngleBracketsUrl && string.IsNullOrEmpty(url)))
        {
            return false;
        }
        urlSpan.End = text.Start - 1;
        unescapedUrl.End = text.Start - 1 - (isAngleBracketsUrl ? 1 : 0);
        int triviaBeforeTitleStart = text.Start;

        var saved = text;
        var hasWhiteSpaces = CharIteratorHelper.TrimStartAndCountNewLines(ref text, out int newLineCount, out newLine);

        // Remove the newline from the trivia (as it may have multiple lines)
        var triviaBeforeTitleEnd = text.Start - 1;
        triviaBeforeTitle = new SourceSpan(triviaBeforeTitleStart, triviaBeforeTitleEnd);
        var c = text.CurrentChar;
        if (c == '\'' || c == '"' || c == '(')
        {
            titleSpan.Start = text.Start;
            unescapedTitle.Start = text.Start + 1; // + 1; // skip opening enclosing character
            if (TryParseTitleTrivia(ref text, out title, out titleEnclosingCharacter))
            {
                titleSpan.End = text.Start - 1;
                unescapedTitle.End = text.Start - 1 - 1;  // skip closing enclosing character
                // If we have a title, it requires a whitespace after the url
                if (!hasWhiteSpaces)
                {
                    return false;
                }

                // Discard the newline if we have a title
                newLine = NewLine.None;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (text.IsEmpty || newLineCount > 0)
            {
                // If we have an end of line, we need to remove it from the trivia
                triviaBeforeTitle.End -= newLine.Length();
                triviaAfterTitle = new SourceSpan(text.Start, text.Start - 1);
                return true;
            }
        }

        // Check that the current line has only trailing spaces
        c = text.CurrentChar;
        int triviaAfterTitleStart = text.Start;
        while (c.IsSpaceOrTab())
        {
            c = text.NextChar();
        }

        if (c != '\0' && c != '\n' && c != '\r')
        {
            // If we were able to parse the url but the title doesn't end with space, 
            // we are still returning a valid definition
            if (newLineCount > 0 && title != null)
            {
                text = saved;
                title = null;
                newLine = NewLine.None;
                unescapedTitle = SourceSpan.Empty;
                triviaAfterTitle = SourceSpan.Empty;
                return true;
            }

            label = null;
            url = null;
            unescapedUrl = SourceSpan.Empty;
            title = null;
            unescapedTitle = SourceSpan.Empty;
            return false;
        }
        triviaAfterTitle = new SourceSpan(triviaAfterTitleStart, text.Start - 1);
        if (c != '\0')
        {
            if (c == '\n')
            {
                newLine = NewLine.LineFeed;
            }
            else if (c == '\r' && text.PeekChar() == '\n')
            {
                newLine = NewLine.CarriageReturnLineFeed;
                text.SkipChar();
            }
            else if (c == '\r')
            {
                newLine = NewLine.CarriageReturn;
            }
        }

        return true;
    }

    public static bool TryParseLabel<T>(T lines, [NotNullWhen(true)] out string? label) where T : ICharIterator
    {
        return TryParseLabel(ref lines, false, out label, out SourceSpan labelSpan);
    }

    public static bool TryParseLabel<T>(T lines, [NotNullWhen(true)] out string? label, out SourceSpan labelSpan) where T : ICharIterator
    {
        return TryParseLabel(ref lines, false, out label, out labelSpan);
    }

    public static bool TryParseLabel<T>(ref T lines, [NotNullWhen(true)] out string? label) where T : ICharIterator
    {
        return TryParseLabel(ref lines, false, out label, out SourceSpan labelSpan);
    }

    public static bool TryParseLabel<T>(ref T lines, [NotNullWhen(true)] out string? label, out SourceSpan labelSpan) where T : ICharIterator
    {
        return TryParseLabel(ref lines, false, out label, out labelSpan);
    }

    public static bool TryParseLabelTrivia<T>(ref T lines, [NotNullWhen(true)] out string? label, out SourceSpan labelSpan) where T : ICharIterator
    {
        return TryParseLabelTrivia(ref lines, false, out label, out labelSpan);
    }

    public static bool TryParseLabel<T>(ref T lines, bool allowEmpty, [NotNullWhen(true)] out string? label, out SourceSpan labelSpan) where T : ICharIterator
    {
        label = null;
        char c = lines.CurrentChar;
        labelSpan = SourceSpan.Empty;
        if (c != '[')
        {
            return false;
        }
        var buffer = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);

        var startLabel = -1;
        var endLabel = -1;

        bool hasEscape = false;
        bool previousWhitespace = true;
        bool hasNonWhiteSpace = false;
        while (true)
        {
            c = lines.NextChar();
            if (c == '\0')
            {
                break;
            }

            if (hasEscape)
            {
                if (c != '[' && c != ']' && c != '\\')
                {
                    break;
                }
            }
            else
            {
                if (c == '[')
                {
                    break;
                }

                if (c == ']')
                {
                    lines.SkipChar(); // Skip ]
                    if (allowEmpty || hasNonWhiteSpace)
                    {
                        // Remove trailing spaces
                        for (int i = buffer.Length - 1; i >= 0; i--)
                        {
                            if (!buffer[i].IsWhitespace())
                            {
                                break;
                            }
                            buffer.Length = i;
                            endLabel--;
                        }

                        // Only valid if buffer is less than 1000 characters
                        if (buffer.Length <= 999)
                        {
                            labelSpan.Start = startLabel;
                            labelSpan.End = endLabel;
                            if (labelSpan.Start > labelSpan.End)
                            {
                                labelSpan = SourceSpan.Empty;
                            }
                            goto ReturnValid;
                        }
                    }
                    break;
                }
            }

            var isWhitespace = c.IsWhitespace();
            if (isWhitespace)
            {
                // Replace any whitespace by a single ' '
                c = ' ';
            }

            if (!hasEscape && c == '\\')
            {
                if (startLabel < 0)
                {
                    startLabel = lines.Start;
                }
                hasEscape = true;
            }
            else
            {
                hasEscape = false;

                if (!previousWhitespace || !isWhitespace)
                {
                    if (startLabel < 0)
                    {
                        startLabel = lines.Start;
                    }
                    endLabel = lines.Start;
                    buffer.Append(c);
                    if (!isWhitespace)
                    {
                        hasNonWhiteSpace = true;
                    }
                }
            }
            previousWhitespace = isWhitespace;
        }

        buffer.Dispose();
        return false;

    ReturnValid:
        label = buffer.ToString();
        return true;
    }

    public static bool TryParseLabelTrivia<T>(ref T lines, bool allowEmpty, out string? label, out SourceSpan labelSpan) where T : ICharIterator
    {
        label = null;
        char c = lines.CurrentChar;
        labelSpan = SourceSpan.Empty;
        if (c != '[')
        {
            return false;
        }
        var buffer = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);

        var startLabel = -1;
        var endLabel = -1;

        bool hasEscape = false;
        bool previousWhitespace = true;
        bool hasNonWhiteSpace = false;
        while (true)
        {
            c = lines.NextChar();
            if (c == '\0')
            {
                break;
            }

            if (hasEscape)
            {
                if (c != '[' && c != ']' && c != '\\')
                {
                    break;
                }
            }
            else
            {
                if (c == '[')
                {
                    break;
                }

                if (c == ']')
                {
                    lines.SkipChar(); // Skip ]
                    if (allowEmpty || hasNonWhiteSpace)
                    {
                        // Remove trailing spaces
                        for (int i = buffer.Length - 1; i >= 0; i--)
                        {
                            if (!buffer[i].IsWhitespace())
                            {
                                break;
                            }
                            buffer.Length = i;
                            endLabel--;
                        }

                        // Only valid if buffer is less than 1000 characters
                        if (buffer.Length <= 999)
                        {
                            labelSpan.Start = startLabel;
                            labelSpan.End = endLabel;
                            if (labelSpan.Start > labelSpan.End)
                            {
                                labelSpan = SourceSpan.Empty;
                            }
                            goto ReturnValid;
                        }
                    }
                    break;
                }
            }

            var isWhitespace = c.IsWhitespace();


            if (!hasEscape && c == '\\')
            {
                if (startLabel < 0)
                {
                    startLabel = lines.Start;
                }
                hasEscape = true;
            }
            else
            {
                hasEscape = false;

                if (!previousWhitespace || !isWhitespace)
                {
                    if (startLabel < 0)
                    {
                        startLabel = lines.Start;
                    }
                    endLabel = lines.Start;
                    if (isWhitespace)
                    {
                        // Replace any whitespace by a single ' '
                        buffer.Append(' ');
                    }
                    else
                    {
                        buffer.Append(c);
                    }
                    if (!isWhitespace)
                    {
                        hasNonWhiteSpace = true;
                    }
                }
            }
            previousWhitespace = isWhitespace;
        }

        buffer.Dispose();
        return false;

    ReturnValid:
        label = buffer.ToString();
        return true;
    }
}
