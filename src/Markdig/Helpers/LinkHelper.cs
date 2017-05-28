// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Runtime.CompilerServices;
using System.Text;
using Markdig.Syntax;

namespace Markdig.Helpers
{
    /// <summary>
    /// Helpers to parse Markdown links.
    /// </summary>
    public static class LinkHelper
    {
        public static bool TryParseAutolink(StringSlice text, out string link, out bool isEmail)
        {
            return TryParseAutolink(ref text, out link, out isEmail);
        }

        public static string Urilize(string headingText, bool allowOnlyAscii)
        {
            var headingBuffer = StringBuilderCache.Local();
            bool hasLetter = false;
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

            var text = headingBuffer.ToString();
            headingBuffer.Length = 0;
            return text;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        private static bool IsReservedPunctuation(char c)
        {
            return c == '_' || c == '-' || c == '.';
        }

        public static bool TryParseAutolink(ref StringSlice text, out string link, out bool isEmail)
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
                if (c.IsDigit() || CharHelper.IsEmailUsernameSpecialChar(c))
                {
                    state = -1;
                }
                else
                {
                    return false;
                }
            }

            var builder = StringBuilderCache.Local();

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
                        builder.Length = 0;
                        return false;
                    }
                    builder.Append(c);
                }
                else if (c == ':')
                {
                    if (state < 0 || builder.Length <= 2)
                    {
                        builder.Length = 0;
                        return false;
                    }
                    state = 1;
                    break;
                } else if (c == '@')
                {
                    if (state > 0)
                    {
                        builder.Length = 0;
                        return false;
                    }
                    state = -1;
                    break;
                }
                else
                {
                    builder.Length = 0;
                    return false;
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

                        text.NextChar();
                        link = builder.ToString();
                        builder.Length = 0;
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
                // scan an uri            
                // An absolute URI, for these purposes, consists of a scheme followed by a colon (:) 
                // followed by zero or more characters other than ASCII whitespace and control characters, <, and >. 
                // If the URI includes these characters, they must be percent-encoded (e.g. %20 for a space).

                while (true)
                {
                    c = text.NextChar();
                    if (c == '\0')
                    {
                        break;
                    }

                    if (c == '>')
                    {
                        text.NextChar();
                        link = builder.ToString();
                        builder.Length = 0;
                        return true;
                    }

                    // Chars valid for both scheme and email
                    if (c > ' ' && c < 127 && c != '<')
                    {
                        builder.Append(c);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            builder.Length = 0;
            return false;
        }

        public static bool TryParseInlineLink(StringSlice text, out string link, out string title)
        {
            SourceSpan linkSpan;
            SourceSpan titleSpan;
            return TryParseInlineLink(ref text, out link, out title, out linkSpan, out titleSpan);
        }

        public static bool TryParseInlineLink(StringSlice text, out string link, out string title, out SourceSpan linkSpan, out SourceSpan titleSpan)
        {
            return TryParseInlineLink(ref text, out link, out title, out linkSpan, out titleSpan);
        }

        public static bool TryParseInlineLink(ref StringSlice text, out string link, out string title)
        {
            SourceSpan linkSpan;
            SourceSpan titleSpan;
            return TryParseInlineLink(ref text, out link, out title, out linkSpan, out titleSpan);
        }

        public static bool TryParseInlineLink(ref StringSlice text, out string link, out string title, out SourceSpan linkSpan, out SourceSpan titleSpan)
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
                text.NextChar();
                text.TrimStart();

                var pos = text.Start;
                if (TryParseUrl(ref text, out link))
                {
                    linkSpan.Start = pos;
                    linkSpan.End = text.Start - 1;
                    if (linkSpan.End < linkSpan.Start)
                    {
                        linkSpan = SourceSpan.Empty;
                    }

                    int spaceCount;
                    text.TrimStart(out spaceCount);
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
                        else if (TryParseTitle(ref text, out title))
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
                text.NextChar();
                title = title ?? String.Empty;
            }

            return isValid;
        }

        public static bool TryParseTitle<T>(T text, out string title) where T : ICharIterator
        {
            return TryParseTitle(ref text, out title);
        }

        public static bool TryParseTitle<T>(ref T text, out string title) where T : ICharIterator
        {
            bool isValid = false;
            var buffer = StringBuilderCache.Local();
            buffer.Length = 0;

            // a sequence of zero or more characters between straight double-quote characters ("), including a " character only if it is backslash-escaped, or
            // a sequence of zero or more characters between straight single-quote characters ('), including a ' character only if it is backslash-escaped, or
            var c = text.CurrentChar;
            if (c == '\'' || c == '"' || c == '(')
            {
                var closingQuote = c == '(' ? ')' : c;
                bool hasEscape = false;
                // -1: undefined
                //  0: has only spaces
                //  1: has other characters
                int hasOnlyWhiteSpacesSinceLastLine = -1;
                while (true)
                {
                    c = text.NextChar();

                    if (c == '\n')
                    {
                        if (hasOnlyWhiteSpacesSinceLastLine >= 0)
                        {
                            if (hasOnlyWhiteSpacesSinceLastLine == 1)
                            {
                                break;
                            }
                            hasOnlyWhiteSpacesSinceLastLine = -1;
                        }
                    }

                    if (c == '\0')
                    {
                        break;
                    }

                    if (c == closingQuote)
                    {
                        if (hasEscape)
                        {
                            buffer.Append(closingQuote);
                            hasEscape = false;
                            continue;
                        }

                        // Skip last quote
                        text.NextChar();
                        isValid = true;
                        break;
                    }

                    if (hasEscape && !c.IsAsciiPunctuation())
                    {
                        buffer.Append('\\');
                    }

                    if (c == '\\')
                    {
                        hasEscape = true;
                        continue;
                    }

                    hasEscape = false;

                    if (c.IsSpaceOrTab())
                    {
                        if (hasOnlyWhiteSpacesSinceLastLine < 0)
                        {
                            hasOnlyWhiteSpacesSinceLastLine = 1;
                        }
                    }
                    else if (c != '\n')
                    {
                        hasOnlyWhiteSpacesSinceLastLine = 0;
                    }

                    buffer.Append(c);
                }
            }

            title = isValid ? buffer.ToString() : null;
            buffer.Length = 0;
            return isValid;
        }

        public static bool TryParseUrl<T>(T text, out string link) where T : ICharIterator
        {
            return TryParseUrl(ref text, out link);
        }

        public static bool TryParseUrl<T>(ref T text, out string link) where T : ICharIterator
        {
            bool isValid = false;
            var buffer = StringBuilderCache.Local();
            buffer.Length = 0;

            var c = text.CurrentChar;

            // a sequence of zero or more characters between an opening < and a closing > 
            // that contains no spaces, line breaks, or unescaped < or > characters, or
            if (c == '<')
            {
                bool hasEscape = false;
                do
                {
                    c = text.NextChar();
                    if (!hasEscape && c == '>')
                    {
                        text.NextChar();
                        isValid = true;
                        break;
                    }

                    if (!hasEscape && c == '<')
                    {
                        break;
                    }

                    if (hasEscape && !c.IsAsciiPunctuation())
                    {
                        buffer.Append('\\');
                    }

                    if (c == '\\')
                    {
                        hasEscape = true;
                        continue;
                    }

                    hasEscape = false;

                    if (c.IsWhitespace()) // TODO: specs unclear. space is strict or relaxed? (includes tabs?)
                    {
                        break;
                    }

                    buffer.Append(c);

                } while (c != '\0');
            }
            else
            {
                // a nonempty sequence of characters that does not include ASCII space or control characters, 
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
                            if (openedParent > 0)
                            {
                                break;
                            }
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

                    if (hasEscape && !c.IsAsciiPunctuation())
                    {
                        buffer.Append('\\');
                    }

                    // If we have an escape
                    if (c == '\\')
                    {
                        hasEscape = true;
                        c = text.NextChar();
                        continue;
                    }

                    hasEscape = false;

                    if (IsEndOfUri(c))
                    {
                        isValid = true;
                        break;
                    }

                    if (c == '.' && IsEndOfUri(text.PeekChar()))
                    {
                        isValid = true;
                        break;
                    }

                    buffer.Append(c);

                    c = text.NextChar();
                }
            }

            link = isValid ? buffer.ToString() : null;
            buffer.Length = 0;
            return isValid;
        }

        private static bool IsEndOfUri(char c)
        {
            return c == '\0' || c.IsSpaceOrTab() || c.IsControl(); // TODO: specs unclear. space is strict or relaxed? (includes tabs?)
        }

        public static bool TryParseLinkReferenceDefinition<T>(T text, out string label, out string url,
            out string title) where T : ICharIterator
        {
            return TryParseLinkReferenceDefinition(ref text, out label, out url, out title);
        }

        public static bool TryParseLinkReferenceDefinition<T>(ref T text, out string label, out string url, out string title)
            where T : ICharIterator
        {
            SourceSpan labelSpan;
            SourceSpan urlSpan;
            SourceSpan titleSpan;
            return TryParseLinkReferenceDefinition(ref text, out label, out url, out title, out labelSpan, out urlSpan,
                out titleSpan);
        }

        public static bool TryParseLinkReferenceDefinition<T>(ref T text, out string label, out string url, out string title, out SourceSpan labelSpan, out SourceSpan urlSpan, out SourceSpan titleSpan) where T : ICharIterator
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
            text.NextChar(); // Skip ':'

            // Skip any whitespace before the url
            text.TrimStart();

            urlSpan.Start = text.Start;
            if (!TryParseUrl(ref text, out url) || string.IsNullOrEmpty(url))
            {
                return false;
            }
            urlSpan.End = text.Start - 1;

            var saved = text;
            int newLineCount;
            var hasWhiteSpaces = CharIteratorHelper.TrimStartAndCountNewLines(ref text, out newLineCount);
            var c = text.CurrentChar;
            if (c == '\'' || c == '"' || c == '(')
            {
                titleSpan.Start = text.Start;
                if (TryParseTitle(ref text, out title))
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
                if (text.CurrentChar == '\0' || newLineCount > 0)
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

            if (c != '\0' && c != '\n')
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

            return true;
        }

        public static bool TryParseLabel<T>(T lines, out string label) where T : ICharIterator
        {
            SourceSpan labelSpan;
            return TryParseLabel(ref lines, false, out label, out labelSpan);
        }

        public static bool TryParseLabel<T>(T lines, out string label, out SourceSpan labelSpan) where T : ICharIterator
        {
            return TryParseLabel(ref lines, false, out label, out labelSpan);
        }

        public static bool TryParseLabel<T>(ref T lines, out string label) where T : ICharIterator
        {
            SourceSpan labelSpan;
            return TryParseLabel(ref lines, false, out label, out labelSpan);
        }


        public static bool TryParseLabel<T>(ref T lines, out string label, out SourceSpan labelSpan) where T : ICharIterator
        {
            return TryParseLabel(ref lines, false, out label, out labelSpan);
        }

        public static bool TryParseLabel<T>(ref T lines, bool allowEmpty, out string label, out SourceSpan labelSpan) where T : ICharIterator
        {
            label = null;
            char c = lines.CurrentChar;
            labelSpan = SourceSpan.Empty;
            if (c != '[')
            {
                return false;
            }
            var buffer = StringBuilderCache.Local();

            var startLabel = -1;
            var endLabel = -1;

            bool hasEscape = false;
            bool previousWhitespace = true;
            bool hasNonWhiteSpace = false;
            bool isValid = false;
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
                        lines.NextChar(); // Skip ]
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

                                label = buffer.ToString();
                                isValid = true;
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

            buffer.Length = 0;

            return isValid;
        }
    }
}