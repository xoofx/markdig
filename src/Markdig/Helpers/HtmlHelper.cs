// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Text;

namespace Markdig.Helpers
{
    /// <summary>
    /// Helper to parse several HTML tags.
    /// </summary>
    public static class HtmlHelper
    {
        private static readonly char[] SearchBackAndAmp = { '\\', '&' };
        private static readonly char[] SearchAmp = { '&' };
        private static readonly string[] EscapeUrlsForAscii = new string[128];

        static HtmlHelper()
        {
            for (int i = 0; i < EscapeUrlsForAscii.Length; i++)
            {
                if (i <= 32 || @"""'<>[\]^`{|}~".IndexOf((char)i) >= 0 || i == 127)
                {
                    EscapeUrlsForAscii[i] = $"%{i:X2}";
                }
                else if ((char) i == '&')
                {
                    EscapeUrlsForAscii[i] = "&amp;";
                }
            }
        }
        public static string EscapeUrlCharacter(char c)
        {
            return c < 128 ? EscapeUrlsForAscii[c] : null;
        }

        public static bool TryParseHtmlTag(StringSlice text, out string htmlTag)
        {
            return TryParseHtmlTag(ref text, out htmlTag);
        }

        public static bool TryParseHtmlTag(ref StringSlice text, out string htmlTag)
        {
            var builder = StringBuilderCache.Local();
            var result = TryParseHtmlTag(ref text, builder);
            htmlTag = builder.ToString();
            builder.Length = 0;
            return result;
        }

        public static bool TryParseHtmlTag(ref StringSlice text, StringBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var c = text.CurrentChar;
            if (c != '<')
            {
                return false;
            }
            c = text.NextChar();

            builder.Append('<');

            switch (c)
            {
                case '/':
                    return TryParseHtmlCloseTag(ref text, builder);
                case '?':
                    return TryParseHtmlTagProcessingInstruction(ref text, builder);
                case '!':
                    builder.Append(c);
                    c = text.NextChar();
                    if (c == '-')
                    {
                        return TryParseHtmlTagHtmlComment(ref text, builder);
                    }

                    if (c == '[')
                    {
                        return TryParseHtmlTagCData(ref text, builder);
                    }

                    return TryParseHtmlTagDeclaration(ref text, builder);
            }

            return TryParseHtmlTagOpenTag(ref text, builder);
        }

        internal static bool TryParseHtmlTagOpenTag(ref StringSlice text, StringBuilder builder)
        {
            var c = text.CurrentChar;

            // Parse the tagname
            if (!c.IsAlpha())
            {
                return false;
            }
            builder.Append(c);

            while (true)
            {
                c = text.NextChar();
                if (c.IsAlphaNumeric() || c == '-')
                {
                    builder.Append(c);
                }
                else
                {
                    break;
                }
            }

            bool hasAttribute = false;
            while (true)
            {
                var hasWhitespaces = false;
                // Skip any whitespaces
                while (c.IsWhitespace())
                {
                    builder.Append(c);
                    c = text.NextChar();
                    hasWhitespaces = true;
                }

                switch (c)
                {
                    case '\0':
                        return false;
                    case '>':
                        text.NextChar();
                        builder.Append(c);
                        return true;
                    case '/':
                        builder.Append('/');
                        c = text.NextChar();
                        if (c != '>')
                        {
                            return false;
                        }
                        text.NextChar();
                        builder.Append('>');
                        return true;
                    case '=':

                        if (!hasAttribute)
                        {
                            return false;
                        }

                        builder.Append('=');

                        // Skip any spaces after
                        c = text.NextChar();
                        while (c.IsWhitespace())
                        {
                            builder.Append(c);
                            c = text.NextChar();
                        }

                        // Parse a quoted string
                        if (c == '\'' || c == '\"')
                        {
                            builder.Append(c);
                            char openingStringChar = c;
                            while (true)
                            {
                                c = text.NextChar();
                                if (c == '\0')
                                {
                                    return false;
                                }
                                if (c != openingStringChar)
                                {
                                    builder.Append(c);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            builder.Append(c);
                            c = text.NextChar();
                        }
                        else
                        {
                            // Parse until we match a space or a special html character
                            int matchCount = 0;
                            while (true)
                            {
                                if (c == '\0')
                                {
                                    return false;
                                }
                                if (c == ' ' || c == '\n' || c == '"' || c == '\'' || c == '=' || c == '<' || c == '>' || c == '`')
                                {
                                    break;
                                }
                                matchCount++;
                                builder.Append(c);
                                c = text.NextChar();
                            }

                            // We need at least one char after '='
                            if (matchCount == 0)
                            {
                                return false;
                            }
                        }

                        hasAttribute = false;
                        continue;
                    default:
                        if (!hasWhitespaces)
                        {
                            return false;
                        }

                        // Parse the attribute name
                        if (!(c.IsAlpha() || c == '_' || c == ':'))
                        {
                            return false;
                        }
                        builder.Append(c);

                        while (true)
                        {
                            c = text.NextChar();
                            if (c.IsAlphaNumeric() || c == '_' || c == ':' || c == '.' || c == '-')
                            {
                                builder.Append(c);
                            }
                            else
                            {
                                break;
                            }
                        }

                        hasAttribute = true;
                        break;
                }
            }
        }

        private static bool TryParseHtmlTagDeclaration(ref StringSlice text, StringBuilder builder)
        {
            var c = text.CurrentChar;
            bool hasAlpha = false;
            while (c.IsAlphaUpper())
            {
                builder.Append(c);
                c = text.NextChar();
                hasAlpha = true;
            }

            if (!hasAlpha || !c.IsWhitespace())
            {
                return false;
            }

            // Regexp: "\\![A-Z]+\\s+[^>\\x00]*>"
            while (true)
            {
                builder.Append(c);
                c = text.NextChar();
                if (c == '\0')
                {
                    return false;
                }

                if (c == '>')
                {
                    text.NextChar();
                    builder.Append('>');
                    return true;
                }
            }
        }

        private static bool TryParseHtmlTagCData(ref StringSlice text, StringBuilder builder)
        {
            builder.Append('[');
            var c = text.NextChar();
            if (c == 'C' &&
                text.NextChar() == 'D' &&
                text.NextChar() == 'A' &&
                text.NextChar() == 'T' &&
                text.NextChar() == 'A' && 
                (c = text.NextChar()) == '[')
            {
                builder.Append("CDATA[");
                while (true)
                {
                    var pc = c;
                    c = text.NextChar();
                    if (c == '\0')
                    {
                        return false;
                    }

                    if (c == ']' && pc == ']')
                    {
                        builder.Append(']');
                        c = text.NextChar();
                        if (c == '>')
                        {
                            builder.Append('>');
                            text.NextChar();
                            return true;
                        }

                        if (c == '\0')
                        {
                            return false;
                        }
                    }
                    builder.Append(c);
                }
            }
            return false;
        }

        internal static bool TryParseHtmlCloseTag(ref StringSlice text, StringBuilder builder)
        {
            // </[A-Za-z][A-Za-z0-9]+\s*>
            builder.Append('/');

            var c = text.NextChar();
            if (!c.IsAlpha())
            {
                return false;
            }
            builder.Append(c);

            bool skipSpaces = false;
            while (true)
            {
                c = text.NextChar();
                if (c == '>')
                {
                    text.NextChar();
                    builder.Append('>');
                    return true;
                }

                if (skipSpaces)
                {
                    if (c != ' ')
                    {
                        break;
                    }
                }
                else if (c == ' ')
                {
                    skipSpaces = true;
                }
                else if (!(c.IsAlphaNumeric() || c == '-'))
                {
                    break;
                }

                builder.Append(c);
            }
            return false;
        }


        private static bool TryParseHtmlTagHtmlComment(ref StringSlice text, StringBuilder builder)
        {
            var c = text.NextChar();
            if (c != '-')
            {
                return false;
            }
            builder.Append('-');
            builder.Append('-');
            if (text.PeekChar(1) == '>')
            {
                return false;
            }

            var countHyphen = 0;
            while (true)
            {
                c = text.NextChar();
                if (c == '\0')
                {
                    return false;
                }

                if (countHyphen == 2)
                {
                    if (c == '>')
                    {
                        builder.Append('>');
                        text.NextChar();
                        return true;
                    }
                    return false;
                }
                countHyphen = c == '-' ? countHyphen + 1 : 0;
                builder.Append(c);
            }
        }

        private static bool TryParseHtmlTagProcessingInstruction(ref StringSlice text, StringBuilder builder)
        {
            builder.Append('?');
            var prevChar = '\0';
            while (true)
            {
                var c = text.NextChar();
                if (c == '\0')
                {
                    return false;
                }

                if (c == '>' && prevChar == '?')
                {
                    builder.Append('>');
                    text.NextChar();
                    return true;
                }
                prevChar = c;
                builder.Append(c);
            }
        }

        /// <summary>
        /// Destructively unescape a string: remove backslashes before punctuation or symbol characters.
        /// </summary>
        /// <param name="text">The string data that will be changed by unescaping any punctuation or symbol characters.</param>
        /// <param name="removeBackSlash">if set to <c>true</c> [remove back slash].</param>
        /// <returns></returns>
        public static string Unescape(string text, bool removeBackSlash = true)
        {
            // Credits: code from CommonMark.NET
            // Copyright (c) 2014, Kārlis Gaņģis All rights reserved. 
            // See license for details:  https://github.com/Knagis/CommonMark.NET/blob/master/LICENSE.md
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // remove backslashes before punctuation chars:
            int searchPos = 0;
            int lastPos = 0;
            char c;
            char[] search = removeBackSlash ? SearchBackAndAmp : SearchAmp;
            StringBuilder sb = null;

            while ((searchPos = text.IndexOfAny(search, searchPos)) != -1)
            {
                if (sb == null)
                {
                    sb = StringBuilderCache.Local();
                    sb.Length = 0;
                }
                c = text[searchPos];
                if (removeBackSlash && c == '\\')
                {
                    searchPos++;

                    if (text.Length == searchPos)
                        break;

                    c = text[searchPos];
                    if (c.IsEscapableSymbol())
                    {
                        sb.Append(text, lastPos, searchPos - lastPos - 1);
                        lastPos = searchPos;
                    }
                }
                else if (c == '&')
                {
                    string namedEntity;
                    int numericEntity;
                    var match = ScanEntity(text, searchPos, text.Length - searchPos, out namedEntity,
                        out numericEntity);
                    if (match == 0)
                    {
                        searchPos++;
                    }
                    else
                    {
                        searchPos += match;

                        if (namedEntity != null)
                        {
                            var decoded = EntityHelper.DecodeEntity(namedEntity);
                            if (decoded != null)
                            {
                                sb.Append(text, lastPos, searchPos - match - lastPos);
                                sb.Append(decoded);
                                lastPos = searchPos;
                            }
                        }
                        else if (numericEntity >= 0)
                        {
                            sb.Append(text, lastPos, searchPos - match - lastPos);
                            if (numericEntity == 0)
                            {
                                sb.Append('\0'.EscapeInsecure());
                            }
                            else
                            {
                                var decoded = EntityHelper.DecodeEntity(numericEntity);
                                if (decoded != null)
                                {
                                    sb.Append(decoded);
                                }
                                else
                                {
                                    sb.Append('\uFFFD');
                                }
                            }

                            lastPos = searchPos;
                        }
                    }
                }
            }

            if (sb == null)
                return text;

            sb.Append(text, lastPos, text.Length - lastPos);
            var result = sb.ToString();
            sb.Length = 0;
            return result;
        }

        /// <summary>
        /// Scans an entity.
        /// Returns number of chars matched.
        /// </summary>
        public static int ScanEntity(string s, int pos, int length, out string namedEntity, out int numericEntity)
        {
            // Credits: code from CommonMark.NET
            // Copyright (c) 2014, Kārlis Gaņģis All rights reserved. 
            // See license for details:  https://github.com/Knagis/CommonMark.NET/blob/master/LICENSE.md

            /*!re2c
                  [&] ([#] ([Xx][A-Fa-f0-9]{1,8}|[0-9]{1,8}) |[A-Za-z][A-Za-z0-9]{1,31} ) [;]
                     { return (p - start); }
                  .? { return 0; }
                */

            var lastPos = pos + length;

            namedEntity = null;
            numericEntity = 0;

            if (pos + 3 >= lastPos)
                return 0;

            if (s[pos] != '&')
                return 0;

            char c;
            int i;
            int counter = 0;
            if (s[pos + 1] == '#')
            {
                c = s[pos + 2];
                if (c == 'x' || c == 'X')
                {
                    // expect 1-8 hex digits starting from pos+3
                    for (i = pos + 3; i < lastPos; i++)
                    {
                        c = s[i];
                        if (c >= '0' && c <= '9')
                        {
                            if (++counter == 9) return 0;
                            numericEntity = numericEntity*16 + (c - '0');
                            continue;
                        }
                        else if (c >= 'A' && c <= 'F')
                        {
                            if (++counter == 9) return 0;
                            numericEntity = numericEntity*16 + (c - 'A' + 10);
                            continue;
                        }
                        else if (c >= 'a' && c <= 'f')
                        {
                            if (++counter == 9) return 0;
                            numericEntity = numericEntity*16 + (c - 'a' + 10);
                            continue;
                        }

                        if (c == ';')
                            return counter == 0 ? 0 : i - pos + 1;

                        return 0;
                    }
                }
                else
                {
                    // expect 1-8 digits starting from pos+2
                    for (i = pos + 2; i < lastPos; i++)
                    {
                        c = s[i];
                        if (c >= '0' && c <= '9')
                        {
                            if (++counter == 9) return 0;
                            numericEntity = numericEntity*10 + (c - '0');
                            continue;
                        }

                        if (c == ';')
                            return counter == 0 ? 0 : i - pos + 1;

                        return 0;
                    }
                }
            }
            else
            {
                // expect a letter and 1-31 letters or digits
                c = s[pos + 1];
                if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')))
                    return 0;

                for (i = pos + 2; i < lastPos; i++)
                {
                    c = s[i];
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    {
                        if (++counter == 32)
                            return 0;

                        continue;
                    }

                    if (c == ';')
                    {
                        namedEntity = s.Substring(pos + 1, counter + 1);
                        return counter == 0 ? 0 : i - pos + 1;
                    }

                    return 0;
                }
            }

            return 0;
        }
    }
}