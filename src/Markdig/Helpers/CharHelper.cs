// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Markdig.Helpers
{
    /// <summary>
    /// Helper class for handling characters.
    /// </summary>
    public static class CharHelper
    {
        public const int TabSize = 4;

        public const char ReplacementChar = '\uFFFD';

        public const string ReplacementCharString = "\uFFFD";

        private const char HighSurrogateStart = '\ud800';
        private const char HighSurrogateEnd = '\udbff';
        private const char LowSurrogateStart = '\udc00';
        private const char LowSurrogateEnd = '\udfff';

        // We don't support LCDM
        private static readonly Dictionary<char, int> romanMap = new Dictionary<char, int>(6) {
            { 'i', 1 }, { 'v', 5 }, { 'x', 10 },
            { 'I', 1 }, { 'V', 5 }, { 'X', 10 }
        };

        private static readonly char[] punctuationExceptions = { '−', '-', '†', '‡' };

        public static void CheckOpenCloseDelimiter(char pc, char c, bool enableWithinWord, out bool canOpen, out bool canClose)
        {
            pc.CheckUnicodeCategory(out bool prevIsWhiteSpace, out bool prevIsPunctuation);
            c.CheckUnicodeCategory(out bool nextIsWhiteSpace, out bool nextIsPunctuation);

            var prevIsExcepted = prevIsPunctuation && punctuationExceptions.Contains(pc);
            var nextIsExcepted = nextIsPunctuation && punctuationExceptions.Contains(c);

            // A left-flanking delimiter run is a delimiter run that is
            // (1) not followed by Unicode whitespace, and either
            // (2a) not followed by a punctuation character or
            // (2b) followed by a punctuation character and preceded by Unicode whitespace or a punctuation character.
            // For purposes of this definition, the beginning and the end of the line count as Unicode whitespace.
            canOpen = !nextIsWhiteSpace &&
                           ((!nextIsPunctuation || nextIsExcepted) || prevIsWhiteSpace || prevIsPunctuation);


            // A right-flanking delimiter run is a delimiter run that is
            // (1) not preceded by Unicode whitespace, and either
            // (1a) not preceded by a punctuation character, or
            // (2b) preceded by a punctuation character and followed by Unicode whitespace or a punctuation character.
            // For purposes of this definition, the beginning and the end of the line count as Unicode whitespace.
            canClose = !prevIsWhiteSpace &&
                            ((!prevIsPunctuation || prevIsExcepted) || nextIsWhiteSpace || nextIsPunctuation);

            if (!enableWithinWord)
            {
                var temp = canOpen;
                // A single _ character can open emphasis iff it is part of a left-flanking delimiter run and either 
                // (a) not part of a right-flanking delimiter run or 
                // (b) part of a right-flanking delimiter run preceded by punctuation.
                canOpen = canOpen && (!canClose || prevIsPunctuation);

                // A single _ character can close emphasis iff it is part of a right-flanking delimiter run and either
                // (a) not part of a left-flanking delimiter run or 
                // (b) part of a left-flanking delimiter run followed by punctuation.
                canClose = canClose && (!temp || nextIsPunctuation);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRomanLetterPartial(char c)
        {
            // We don't support LCDM
            return IsRomanLetterLowerPartial(c) || IsRomanLetterUpperPartial(c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRomanLetterLowerPartial(char c)
        {
            // We don't support LCDM
            return c == 'i' || c == 'v' || c == 'x';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRomanLetterUpperPartial(char c)
        {
            // We don't support LCDM
            return c == 'I' || c == 'V' || c == 'X';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RomanToArabic(ReadOnlySpan<char> text)
        {
            int result = 0;
            for (int i = 0; i < text.Length; i++)
            {
                var candidate = romanMap[text[i]];
                if ((uint)(i + 1) < text.Length && candidate < romanMap[text[i + 1]])
                {
                    result -= candidate;
                }
                else
                {
                    result += candidate;
                }
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AddTab(int column)
        {
            // return ((column + TabSize) / TabSize) * TabSize;
            Debug.Assert(TabSize == 4, "Change the AddTab implementation if TabSize is no longer a power of 2");
            return TabSize + (column & ~(TabSize - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAcrossTab(int column)
        {
            return (column & (TabSize - 1)) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this char[] charList, char c)
        {
            foreach (char ch in charList)
            {
                if (ch == c)
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhitespace(this char c)
        {
            // 2.1 Characters and lines 
            // A whitespace character is a space(U + 0020), tab(U + 0009), newline(U + 000A), line tabulation (U + 000B), form feed (U + 000C), or carriage return (U + 000D).
            return c <= ' ' && (c == ' ' || c == '\t' || c == '\n' || c == '\v' || c == '\f' || c == '\r');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsControl(this char c)
        {
            return c < ' ' || char.IsControl(c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEscapableSymbol(this char c)
        {
            // char.IsSymbol also works with Unicode symbols that cannot be escaped based on the specification.
            return (c > ' ' && c < '0') || (c > '9' && c < 'A') || (c > 'Z' && c < 'a') || (c > 'z' && c < 127) || c == '•';
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpaceOrZero(this char c)
        {
            return IsZero(c) || IsWhitespace(c);
        }

        // Note that we are not considering the character & as a punctuation in HTML
        // as it is used for HTML entities, print unicode, so we assume that when we have a `&` 
        // it is more likely followed by a valid HTML Entity that represents a non punctuation
        public static void CheckUnicodeCategory(this char c, out bool space, out bool punctuation)
        {
            // Credits: code from CommonMark.NET
            // Copyright (c) 2014, Kārlis Gaņģis All rights reserved. 
            // See license for details:  https://github.com/Knagis/CommonMark.NET/blob/master/LICENSE.md
            if (c <= 'ÿ')
            {
                space = c == '\0' || c == ' ' || (c >= '\t' && c <= '\r') || c == '\u00a0' || c == '\u0085';
                punctuation = c == '\0' || (c >= 33 && c <= 47 && c != 38) || (c >= 58 && c <= 64) || (c >= 91 && c <= 96) || (c >= 123 && c <= 126);
            }
            else
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                space = category == UnicodeCategory.SpaceSeparator
                    || category == UnicodeCategory.LineSeparator
                    || category == UnicodeCategory.ParagraphSeparator;
                punctuation = !space &&
                    (category == UnicodeCategory.ConnectorPunctuation
                    || category == UnicodeCategory.DashPunctuation
                    || category == UnicodeCategory.OpenPunctuation
                    || category == UnicodeCategory.ClosePunctuation
                    || category == UnicodeCategory.InitialQuotePunctuation
                    || category == UnicodeCategory.FinalQuotePunctuation
                    || category == UnicodeCategory.OtherPunctuation);
            }
        }

        // Same as CheckUnicodeCategory
        internal static bool IsSpaceOrPunctuation(this char c)
        {
            if (c <= 'ÿ')
            {
                return c == '\0' || c == ' ' || (c >= '\t' && c <= '\r') || c == '\u00a0' || c == '\u0085' ||
                    (c >= 33 && c <= 47 && c != 38) || (c >= 58 && c <= 64) || (c >= 91 && c <= 96) || (c >= 123 && c <= 126);
            }
            else
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                return category == UnicodeCategory.SpaceSeparator
                    || category == UnicodeCategory.LineSeparator
                    || category == UnicodeCategory.ParagraphSeparator
                    || category == UnicodeCategory.ConnectorPunctuation
                    || category == UnicodeCategory.DashPunctuation
                    || category == UnicodeCategory.OpenPunctuation
                    || category == UnicodeCategory.ClosePunctuation
                    || category == UnicodeCategory.InitialQuotePunctuation
                    || category == UnicodeCategory.FinalQuotePunctuation
                    || category == UnicodeCategory.OtherPunctuation;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNewLine(this char c)
        {
            return c == '\n';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this char c)
        {
            return c == '\0';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSpace(this char c)
        {
            // 2.1 Characters and lines 
            // A space is U+0020.
            return c == ' ';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTab(this char c)
        {
            // 2.1 Characters and lines 
            // A space is U+0009.
            return c == '\t';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSpaceOrTab(this char c)
        {
            return IsSpace(c) || IsTab(c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char EscapeInsecure(this char c)
        {
            // 2.3 Insecure characters
            // For security reasons, the Unicode character U+0000 must be replaced with the REPLACEMENT CHARACTER (U+FFFD).
            return c == '\0' ? '\ufffd' : c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAlphaUpper(this char c)
        {
            return (uint)(c - 'A') <= ('Z' - 'A');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAlpha(this char c)
        {
            return (uint)((c - 'A') & ~0x20) <= ('Z' - 'A');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAlphaNumeric(this char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDigit(this char c)
        {
            return (uint)(c - '0') <= ('9' - '0');
        }

        public static bool IsAsciiPunctuation(this char c)
        {
            // 2.1 Characters and lines 
            // An ASCII punctuation character is !, ", #, $, %, &, ', (, ), *, +, ,, -, ., /, :, ;, <, =, >, ?, @, [, \, ], ^, _, `, {, |, }, or ~.
            switch (c)
            {
                case '!':
                case '"':
                case '#':
                case '$':
                case '%':
                case '&':
                case '\'':
                case '(':
                case ')':
                case '*':
                case '+':
                case ',':
                case '-':
                case '.':
                case '/':
                case ':':
                case ';':
                case '<':
                case '=':
                case '>':
                case '?':
                case '@':
                case '[':
                case '\\':
                case ']':
                case '^':
                case '_':
                case '`':
                case '{':
                case '|':
                case '}':
                case '~':
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmailUsernameSpecialChar(char c)
        {
            return ".!#$%&'*+/=?^_`{|}~-+.~".IndexOf(c) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHighSurrogate(char c)
        {
            return IsInInclusiveRange(c, HighSurrogateStart, HighSurrogateEnd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowSurrogate(char c)
        {
            return IsInInclusiveRange(c, LowSurrogateStart, LowSurrogateEnd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInInclusiveRange(char c, char min, char max)
            => (uint)(c - min) <= (uint)(max - min);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsInInclusiveRange(int value, uint min, uint max)
            => ((uint)value - min) <= (max - min);

        public static IEnumerable<int> ToUtf32(StringSlice text)
        {
            for (int i = text.Start; i <= text.End; i++)
            {
                if (IsHighSurrogate(text[i]) && i < text.End && IsLowSurrogate(text[i + 1]))
                {
                    Debug.Assert(char.IsSurrogatePair(text[i], text[i + 1]));
                    yield return char.ConvertToUtf32(text[i], text[i + 1]);
                }
                else
                {
                    yield return text[i];
                }
            }
        }

        public static bool IsRightToLeft(int c)
        {
            // Generated from Table D.1 of RFC3454
            // http://www.ietf.org/rfc/rfc3454.txt

            // Probably should use a binary search approach

            return c >= 0x0005D0 && c <= 0x0005EA ||
                   c >= 0x0005F0 && c <= 0x0005F4 ||
                   c >= 0x000621 && c <= 0x00063A ||
                   c >= 0x000640 && c <= 0x00064A ||
                   c >= 0x00066D && c <= 0x00066F ||
                   c >= 0x000671 && c <= 0x0006D5 ||
                   c >= 0x0006E5 && c <= 0x0006E6 ||
                   c >= 0x0006FA && c <= 0x0006FE ||
                   c >= 0x000700 && c <= 0x00070D ||
                   c >= 0x000712 && c <= 0x00072C ||
                   c >= 0x000780 && c <= 0x0007A5 ||
                   c >= 0x00FB1F && c <= 0x00FB28 ||
                   c >= 0x00FB2A && c <= 0x00FB36 ||
                   c >= 0x00FB38 && c <= 0x00FB3C ||
                   c >= 0x00FB40 && c <= 0x00FB41 ||
                   c >= 0x00FB43 && c <= 0x00FB44 ||
                   c >= 0x00FB46 && c <= 0x00FBB1 ||
                   c >= 0x00FBD3 && c <= 0x00FD3D ||
                   c >= 0x00FD50 && c <= 0x00FD8F ||
                   c >= 0x00FD92 && c <= 0x00FDC7 ||
                   c >= 0x00FDF0 && c <= 0x00FDFC ||
                   c >= 0x00FE70 && c <= 0x00FE74 ||
                   c >= 0x00FE76 && c <= 0x00FEFC ||
                   c == 0x0005BE || c == 0x0005C0 ||
                   c == 0x0005C3 || c == 0x00061B ||
                   c == 0x00061F || c == 0x0006DD ||
                   c == 0x000710 || c == 0x0007B1 ||
                   c == 0x00200F || c == 0x00FB1D ||
                   c == 0x00FB3E;
        }

        public static bool IsLeftToRight(int c)
        {
            // Generated from Table D.2 of RFC3454
            // http://www.ietf.org/rfc/rfc3454.txt

            // Probably should use a binary search approach

            return c >= 0x000041 && c <= 0x00005A ||
                   c >= 0x000061 && c <= 0x00007A ||
                   c >= 0x0000C0 && c <= 0x0000D6 ||
                   c >= 0x0000D8 && c <= 0x0000F6 ||
                   c >= 0x0000F8 && c <= 0x000220 ||
                   c >= 0x000222 && c <= 0x000233 ||
                   c >= 0x000250 && c <= 0x0002AD ||
                   c >= 0x0002B0 && c <= 0x0002B8 ||
                   c >= 0x0002BB && c <= 0x0002C1 ||
                   c >= 0x0002D0 && c <= 0x0002D1 ||
                   c >= 0x0002E0 && c <= 0x0002E4 ||
                   c >= 0x000388 && c <= 0x00038A ||
                   c >= 0x00038E && c <= 0x0003A1 ||
                   c >= 0x0003A3 && c <= 0x0003CE ||
                   c >= 0x0003D0 && c <= 0x0003F5 ||
                   c >= 0x000400 && c <= 0x000482 ||
                   c >= 0x00048A && c <= 0x0004CE ||
                   c >= 0x0004D0 && c <= 0x0004F5 ||
                   c >= 0x0004F8 && c <= 0x0004F9 ||
                   c >= 0x000500 && c <= 0x00050F ||
                   c >= 0x000531 && c <= 0x000556 ||
                   c >= 0x000559 && c <= 0x00055F ||
                   c >= 0x000561 && c <= 0x000587 ||
                   c >= 0x000905 && c <= 0x000939 ||
                   c >= 0x00093D && c <= 0x000940 ||
                   c >= 0x000949 && c <= 0x00094C ||
                   c >= 0x000958 && c <= 0x000961 ||
                   c >= 0x000964 && c <= 0x000970 ||
                   c >= 0x000982 && c <= 0x000983 ||
                   c >= 0x000985 && c <= 0x00098C ||
                   c >= 0x00098F && c <= 0x000990 ||
                   c >= 0x000993 && c <= 0x0009A8 ||
                   c >= 0x0009AA && c <= 0x0009B0 ||
                   c >= 0x0009B6 && c <= 0x0009B9 ||
                   c >= 0x0009BE && c <= 0x0009C0 ||
                   c >= 0x0009C7 && c <= 0x0009C8 ||
                   c >= 0x0009CB && c <= 0x0009CC ||
                   c >= 0x0009DC && c <= 0x0009DD ||
                   c >= 0x0009DF && c <= 0x0009E1 ||
                   c >= 0x0009E6 && c <= 0x0009F1 ||
                   c >= 0x0009F4 && c <= 0x0009FA ||
                   c >= 0x000A05 && c <= 0x000A0A ||
                   c >= 0x000A0F && c <= 0x000A10 ||
                   c >= 0x000A13 && c <= 0x000A28 ||
                   c >= 0x000A2A && c <= 0x000A30 ||
                   c >= 0x000A32 && c <= 0x000A33 ||
                   c >= 0x000A35 && c <= 0x000A36 ||
                   c >= 0x000A38 && c <= 0x000A39 ||
                   c >= 0x000A3E && c <= 0x000A40 ||
                   c >= 0x000A59 && c <= 0x000A5C ||
                   c >= 0x000A66 && c <= 0x000A6F ||
                   c >= 0x000A72 && c <= 0x000A74 ||
                   c >= 0x000A85 && c <= 0x000A8B ||
                   c >= 0x000A8F && c <= 0x000A91 ||
                   c >= 0x000A93 && c <= 0x000AA8 ||
                   c >= 0x000AAA && c <= 0x000AB0 ||
                   c >= 0x000AB2 && c <= 0x000AB3 ||
                   c >= 0x000AB5 && c <= 0x000AB9 ||
                   c >= 0x000ABD && c <= 0x000AC0 ||
                   c >= 0x000ACB && c <= 0x000ACC ||
                   c >= 0x000AE6 && c <= 0x000AEF ||
                   c >= 0x000B02 && c <= 0x000B03 ||
                   c >= 0x000B05 && c <= 0x000B0C ||
                   c >= 0x000B0F && c <= 0x000B10 ||
                   c >= 0x000B13 && c <= 0x000B28 ||
                   c >= 0x000B2A && c <= 0x000B30 ||
                   c >= 0x000B32 && c <= 0x000B33 ||
                   c >= 0x000B36 && c <= 0x000B39 ||
                   c >= 0x000B3D && c <= 0x000B3E ||
                   c >= 0x000B47 && c <= 0x000B48 ||
                   c >= 0x000B4B && c <= 0x000B4C ||
                   c >= 0x000B5C && c <= 0x000B5D ||
                   c >= 0x000B5F && c <= 0x000B61 ||
                   c >= 0x000B66 && c <= 0x000B70 ||
                   c >= 0x000B85 && c <= 0x000B8A ||
                   c >= 0x000B8E && c <= 0x000B90 ||
                   c >= 0x000B92 && c <= 0x000B95 ||
                   c >= 0x000B99 && c <= 0x000B9A ||
                   c >= 0x000B9E && c <= 0x000B9F ||
                   c >= 0x000BA3 && c <= 0x000BA4 ||
                   c >= 0x000BA8 && c <= 0x000BAA ||
                   c >= 0x000BAE && c <= 0x000BB5 ||
                   c >= 0x000BB7 && c <= 0x000BB9 ||
                   c >= 0x000BBE && c <= 0x000BBF ||
                   c >= 0x000BC1 && c <= 0x000BC2 ||
                   c >= 0x000BC6 && c <= 0x000BC8 ||
                   c >= 0x000BCA && c <= 0x000BCC ||
                   c >= 0x000BE7 && c <= 0x000BF2 ||
                   c >= 0x000C01 && c <= 0x000C03 ||
                   c >= 0x000C05 && c <= 0x000C0C ||
                   c >= 0x000C0E && c <= 0x000C10 ||
                   c >= 0x000C12 && c <= 0x000C28 ||
                   c >= 0x000C2A && c <= 0x000C33 ||
                   c >= 0x000C35 && c <= 0x000C39 ||
                   c >= 0x000C41 && c <= 0x000C44 ||
                   c >= 0x000C60 && c <= 0x000C61 ||
                   c >= 0x000C66 && c <= 0x000C6F ||
                   c >= 0x000C82 && c <= 0x000C83 ||
                   c >= 0x000C85 && c <= 0x000C8C ||
                   c >= 0x000C8E && c <= 0x000C90 ||
                   c >= 0x000C92 && c <= 0x000CA8 ||
                   c >= 0x000CAA && c <= 0x000CB3 ||
                   c >= 0x000CB5 && c <= 0x000CB9 ||
                   c >= 0x000CC0 && c <= 0x000CC4 ||
                   c >= 0x000CC7 && c <= 0x000CC8 ||
                   c >= 0x000CCA && c <= 0x000CCB ||
                   c >= 0x000CD5 && c <= 0x000CD6 ||
                   c >= 0x000CE0 && c <= 0x000CE1 ||
                   c >= 0x000CE6 && c <= 0x000CEF ||
                   c >= 0x000D02 && c <= 0x000D03 ||
                   c >= 0x000D05 && c <= 0x000D0C ||
                   c >= 0x000D0E && c <= 0x000D10 ||
                   c >= 0x000D12 && c <= 0x000D28 ||
                   c >= 0x000D2A && c <= 0x000D39 ||
                   c >= 0x000D3E && c <= 0x000D40 ||
                   c >= 0x000D46 && c <= 0x000D48 ||
                   c >= 0x000D4A && c <= 0x000D4C ||
                   c >= 0x000D60 && c <= 0x000D61 ||
                   c >= 0x000D66 && c <= 0x000D6F ||
                   c >= 0x000D82 && c <= 0x000D83 ||
                   c >= 0x000D85 && c <= 0x000D96 ||
                   c >= 0x000D9A && c <= 0x000DB1 ||
                   c >= 0x000DB3 && c <= 0x000DBB ||
                   c >= 0x000DC0 && c <= 0x000DC6 ||
                   c >= 0x000DCF && c <= 0x000DD1 ||
                   c >= 0x000DD8 && c <= 0x000DDF ||
                   c >= 0x000DF2 && c <= 0x000DF4 ||
                   c >= 0x000E01 && c <= 0x000E30 ||
                   c >= 0x000E32 && c <= 0x000E33 ||
                   c >= 0x000E40 && c <= 0x000E46 ||
                   c >= 0x000E4F && c <= 0x000E5B ||
                   c >= 0x000E81 && c <= 0x000E82 ||
                   c >= 0x000E87 && c <= 0x000E88 ||
                   c >= 0x000E94 && c <= 0x000E97 ||
                   c >= 0x000E99 && c <= 0x000E9F ||
                   c >= 0x000EA1 && c <= 0x000EA3 ||
                   c >= 0x000EAA && c <= 0x000EAB ||
                   c >= 0x000EAD && c <= 0x000EB0 ||
                   c >= 0x000EB2 && c <= 0x000EB3 ||
                   c >= 0x000EC0 && c <= 0x000EC4 ||
                   c >= 0x000ED0 && c <= 0x000ED9 ||
                   c >= 0x000EDC && c <= 0x000EDD ||
                   c >= 0x000F00 && c <= 0x000F17 ||
                   c >= 0x000F1A && c <= 0x000F34 ||
                   c >= 0x000F3E && c <= 0x000F47 ||
                   c >= 0x000F49 && c <= 0x000F6A ||
                   c >= 0x000F88 && c <= 0x000F8B ||
                   c >= 0x000FBE && c <= 0x000FC5 ||
                   c >= 0x000FC7 && c <= 0x000FCC ||
                   c >= 0x001000 && c <= 0x001021 ||
                   c >= 0x001023 && c <= 0x001027 ||
                   c >= 0x001029 && c <= 0x00102A ||
                   c >= 0x001040 && c <= 0x001057 ||
                   c >= 0x0010A0 && c <= 0x0010C5 ||
                   c >= 0x0010D0 && c <= 0x0010F8 ||
                   c >= 0x001100 && c <= 0x001159 ||
                   c >= 0x00115F && c <= 0x0011A2 ||
                   c >= 0x0011A8 && c <= 0x0011F9 ||
                   c >= 0x001200 && c <= 0x001206 ||
                   c >= 0x001208 && c <= 0x001246 ||
                   c >= 0x00124A && c <= 0x00124D ||
                   c >= 0x001250 && c <= 0x001256 ||
                   c >= 0x00125A && c <= 0x00125D ||
                   c >= 0x001260 && c <= 0x001286 ||
                   c >= 0x00128A && c <= 0x00128D ||
                   c >= 0x001290 && c <= 0x0012AE ||
                   c >= 0x0012B2 && c <= 0x0012B5 ||
                   c >= 0x0012B8 && c <= 0x0012BE ||
                   c >= 0x0012C2 && c <= 0x0012C5 ||
                   c >= 0x0012C8 && c <= 0x0012CE ||
                   c >= 0x0012D0 && c <= 0x0012D6 ||
                   c >= 0x0012D8 && c <= 0x0012EE ||
                   c >= 0x0012F0 && c <= 0x00130E ||
                   c >= 0x001312 && c <= 0x001315 ||
                   c >= 0x001318 && c <= 0x00131E ||
                   c >= 0x001320 && c <= 0x001346 ||
                   c >= 0x001348 && c <= 0x00135A ||
                   c >= 0x001361 && c <= 0x00137C ||
                   c >= 0x0013A0 && c <= 0x0013F4 ||
                   c >= 0x001401 && c <= 0x001676 ||
                   c >= 0x001681 && c <= 0x00169A ||
                   c >= 0x0016A0 && c <= 0x0016F0 ||
                   c >= 0x001700 && c <= 0x00170C ||
                   c >= 0x00170E && c <= 0x001711 ||
                   c >= 0x001720 && c <= 0x001731 ||
                   c >= 0x001735 && c <= 0x001736 ||
                   c >= 0x001740 && c <= 0x001751 ||
                   c >= 0x001760 && c <= 0x00176C ||
                   c >= 0x00176E && c <= 0x001770 ||
                   c >= 0x001780 && c <= 0x0017B6 ||
                   c >= 0x0017BE && c <= 0x0017C5 ||
                   c >= 0x0017C7 && c <= 0x0017C8 ||
                   c >= 0x0017D4 && c <= 0x0017DA ||
                   c >= 0x0017E0 && c <= 0x0017E9 ||
                   c >= 0x001810 && c <= 0x001819 ||
                   c >= 0x001820 && c <= 0x001877 ||
                   c >= 0x001880 && c <= 0x0018A8 ||
                   c >= 0x001E00 && c <= 0x001E9B ||
                   c >= 0x001EA0 && c <= 0x001EF9 ||
                   c >= 0x001F00 && c <= 0x001F15 ||
                   c >= 0x001F18 && c <= 0x001F1D ||
                   c >= 0x001F20 && c <= 0x001F45 ||
                   c >= 0x001F48 && c <= 0x001F4D ||
                   c >= 0x001F50 && c <= 0x001F57 ||
                   c >= 0x001F5F && c <= 0x001F7D ||
                   c >= 0x001F80 && c <= 0x001FB4 ||
                   c >= 0x001FB6 && c <= 0x001FBC ||
                   c >= 0x001FC2 && c <= 0x001FC4 ||
                   c >= 0x001FC6 && c <= 0x001FCC ||
                   c >= 0x001FD0 && c <= 0x001FD3 ||
                   c >= 0x001FD6 && c <= 0x001FDB ||
                   c >= 0x001FE0 && c <= 0x001FEC ||
                   c >= 0x001FF2 && c <= 0x001FF4 ||
                   c >= 0x001FF6 && c <= 0x001FFC ||
                   c >= 0x00210A && c <= 0x002113 ||
                   c >= 0x002119 && c <= 0x00211D ||
                   c >= 0x00212A && c <= 0x00212D ||
                   c >= 0x00212F && c <= 0x002131 ||
                   c >= 0x002133 && c <= 0x002139 ||
                   c >= 0x00213D && c <= 0x00213F ||
                   c >= 0x002145 && c <= 0x002149 ||
                   c >= 0x002160 && c <= 0x002183 ||
                   c >= 0x002336 && c <= 0x00237A ||
                   c >= 0x00249C && c <= 0x0024E9 ||
                   c >= 0x003005 && c <= 0x003007 ||
                   c >= 0x003021 && c <= 0x003029 ||
                   c >= 0x003031 && c <= 0x003035 ||
                   c >= 0x003038 && c <= 0x00303C ||
                   c >= 0x003041 && c <= 0x003096 ||
                   c >= 0x00309D && c <= 0x00309F ||
                   c >= 0x0030A1 && c <= 0x0030FA ||
                   c >= 0x0030FC && c <= 0x0030FF ||
                   c >= 0x003105 && c <= 0x00312C ||
                   c >= 0x003131 && c <= 0x00318E ||
                   c >= 0x003190 && c <= 0x0031B7 ||
                   c >= 0x0031F0 && c <= 0x00321C ||
                   c >= 0x003220 && c <= 0x003243 ||
                   c >= 0x003260 && c <= 0x00327B ||
                   c >= 0x00327F && c <= 0x0032B0 ||
                   c >= 0x0032C0 && c <= 0x0032CB ||
                   c >= 0x0032D0 && c <= 0x0032FE ||
                   c >= 0x003300 && c <= 0x003376 ||
                   c >= 0x00337B && c <= 0x0033DD ||
                   c >= 0x0033E0 && c <= 0x0033FE ||
                   c >= 0x003400 && c <= 0x004DB5 ||
                   c >= 0x004E00 && c <= 0x009FA5 ||
                   c >= 0x00A000 && c <= 0x00A48C ||
                   c >= 0x00AC00 && c <= 0x00D7A3 ||
                   c >= 0x00D800 && c <= 0x00FA2D ||
                   c >= 0x00FA30 && c <= 0x00FA6A ||
                   c >= 0x00FB00 && c <= 0x00FB06 ||
                   c >= 0x00FB13 && c <= 0x00FB17 ||
                   c >= 0x00FF21 && c <= 0x00FF3A ||
                   c >= 0x00FF41 && c <= 0x00FF5A ||
                   c >= 0x00FF66 && c <= 0x00FFBE ||
                   c >= 0x00FFC2 && c <= 0x00FFC7 ||
                   c >= 0x00FFCA && c <= 0x00FFCF ||
                   c >= 0x00FFD2 && c <= 0x00FFD7 ||
                   c >= 0x00FFDA && c <= 0x00FFDC ||
                   c >= 0x010300 && c <= 0x01031E ||
                   c >= 0x010320 && c <= 0x010323 ||
                   c >= 0x010330 && c <= 0x01034A ||
                   c >= 0x010400 && c <= 0x010425 ||
                   c >= 0x010428 && c <= 0x01044D ||
                   c >= 0x01D000 && c <= 0x01D0F5 ||
                   c >= 0x01D100 && c <= 0x01D126 ||
                   c >= 0x01D12A && c <= 0x01D166 ||
                   c >= 0x01D16A && c <= 0x01D172 ||
                   c >= 0x01D183 && c <= 0x01D184 ||
                   c >= 0x01D18C && c <= 0x01D1A9 ||
                   c >= 0x01D1AE && c <= 0x01D1DD ||
                   c >= 0x01D400 && c <= 0x01D454 ||
                   c >= 0x01D456 && c <= 0x01D49C ||
                   c >= 0x01D49E && c <= 0x01D49F ||
                   c >= 0x01D4A5 && c <= 0x01D4A6 ||
                   c >= 0x01D4A9 && c <= 0x01D4AC ||
                   c >= 0x01D4AE && c <= 0x01D4B9 ||
                   c >= 0x01D4BD && c <= 0x01D4C0 ||
                   c >= 0x01D4C2 && c <= 0x01D4C3 ||
                   c >= 0x01D4C5 && c <= 0x01D505 ||
                   c >= 0x01D507 && c <= 0x01D50A ||
                   c >= 0x01D50D && c <= 0x01D514 ||
                   c >= 0x01D516 && c <= 0x01D51C ||
                   c >= 0x01D51E && c <= 0x01D539 ||
                   c >= 0x01D53B && c <= 0x01D53E ||
                   c >= 0x01D540 && c <= 0x01D544 ||
                   c >= 0x01D54A && c <= 0x01D550 ||
                   c >= 0x01D552 && c <= 0x01D6A3 ||
                   c >= 0x01D6A8 && c <= 0x01D7C9 ||
                   c >= 0x020000 && c <= 0x02A6D6 ||
                   c >= 0x02F800 && c <= 0x02FA1D ||
                   c >= 0x0F0000 && c <= 0x0FFFFD ||
                   c >= 0x100000 && c <= 0x10FFFD ||
                   c == 0x0000AA || c == 0x0000B5 ||
                   c == 0x0000BA || c == 0x0002EE ||
                   c == 0x00037A || c == 0x000386 ||
                   c == 0x00038C || c == 0x000589 ||
                   c == 0x000903 || c == 0x000950 ||
                   c == 0x0009B2 || c == 0x0009D7 ||
                   c == 0x000A5E || c == 0x000A83 ||
                   c == 0x000A8D || c == 0x000AC9 ||
                   c == 0x000AD0 || c == 0x000AE0 ||
                   c == 0x000B40 || c == 0x000B57 ||
                   c == 0x000B83 || c == 0x000B9C ||
                   c == 0x000BD7 || c == 0x000CBE ||
                   c == 0x000CDE || c == 0x000D57 ||
                   c == 0x000DBD || c == 0x000E84 ||
                   c == 0x000E8A || c == 0x000E8D ||
                   c == 0x000EA5 || c == 0x000EA7 ||
                   c == 0x000EBD || c == 0x000EC6 ||
                   c == 0x000F36 || c == 0x000F38 ||
                   c == 0x000F7F || c == 0x000F85 ||
                   c == 0x000FCF || c == 0x00102C ||
                   c == 0x001031 || c == 0x001038 ||
                   c == 0x0010FB || c == 0x001248 ||
                   c == 0x001258 || c == 0x001288 ||
                   c == 0x0012B0 || c == 0x0012C0 ||
                   c == 0x001310 || c == 0x0017DC ||
                   c == 0x001F59 || c == 0x001F5B ||
                   c == 0x001F5D || c == 0x001FBE ||
                   c == 0x00200E || c == 0x002071 ||
                   c == 0x00207F || c == 0x002102 ||
                   c == 0x002107 || c == 0x002115 ||
                   c == 0x002124 || c == 0x002126 ||
                   c == 0x002128 || c == 0x002395 ||
                   c == 0x01D4A2 || c == 0x01D4BB ||
                   c == 0x01D546;
        }

        // Used by ListExtraItemParser to format numbers from 1 - 26
        private static readonly string[] smallNumberStringCache = {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
            "20", "21", "22", "23", "24", "25", "26",
        };

        internal static string SmallNumberToString(int number)
        {
            string[] cache = smallNumberStringCache;
            if ((uint)number < (uint)cache.Length)
            {
                return cache[number];
            }

            return number.ToString();
        }
    }
}