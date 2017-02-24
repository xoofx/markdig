// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Markdig.Helpers
{
    using System.Collections.Generic;

    /// <summary>
    /// Helper class for handling characters.
    /// </summary>
    public static class CharHelper
    {
        public const int TabSize = 4;

        public const char ZeroSafeChar = '\uFFFD';

        public const string ZeroSafeString = "\uFFFD";

        // We don't support LCDM
        private static IDictionary<char, int> romanMap = new Dictionary<char, int> { { 'I', 1 }, { 'V', 5 }, { 'X', 10 } };

        private static readonly char[] punctuationExceptions = { '−', '-', '†', '‡' };

        public static void CheckOpenCloseDelimiter(char pc, char c, bool enableWithinWord, out bool canOpen, out bool canClose)
        {
            // A left-flanking delimiter run is a delimiter run that is 
            // (a) not followed by Unicode whitespace, and
            // (b) either not followed by a punctuation character, or preceded by Unicode whitespace 
            // or a punctuation character. 
            // For purposes of this definition, the beginning and the end of the line count as Unicode whitespace.
            bool nextIsPunctuation;
            bool nextIsWhiteSpace;
            bool prevIsPunctuation;
            bool prevIsWhiteSpace;
            pc.CheckUnicodeCategory(out prevIsWhiteSpace, out prevIsPunctuation);
            c.CheckUnicodeCategory(out nextIsWhiteSpace, out nextIsPunctuation);

            var prevIsExcepted = prevIsPunctuation && punctuationExceptions.Contains(pc);
            var nextIsExcepted = nextIsPunctuation && punctuationExceptions.Contains(c);

            canOpen = !nextIsWhiteSpace &&
                           ((!nextIsPunctuation || nextIsExcepted) || prevIsWhiteSpace || prevIsPunctuation);


            // A right-flanking delimiter run is a delimiter run that is 
            // (a) not preceded by Unicode whitespace, and 
            // (b) either not preceded by a punctuation character, or followed by Unicode whitespace 
            // or a punctuation character. 
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

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsRomanLetterPartial(char c)
        {
            // We don't support LCDM
            return IsRomanLetterLowerPartial(c) || IsRomanLetterUpperPartial(c);
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsRomanLetterLowerPartial(char c)
        {
            // We don't support LCDM
            return c == 'i' || c == 'v' || c == 'x';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsRomanLetterUpperPartial(char c)
        {
            // We don't support LCDM
            return c == 'I' || c == 'V' || c == 'X';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static int RomanToArabic(string text)
        {
            int result = 0;
            for (int i = 0; i < text.Length; i++)
            {
                var character = Char.ToUpperInvariant(text[i]);
                var candidate = romanMap[character];
                if (i + 1 < text.Length && candidate < romanMap[Char.ToUpperInvariant(text[i + 1])])
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

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static int AddTab(int column)
        {
            return ((column + TabSize) / TabSize) * TabSize;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsAcrossTab(int column)
        {
            return (column & (TabSize - 1)) != 0;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool Contains(this char[] charList, char c)
        {
            for (int i = 0; i < charList.Length; i++)
            {
                if (charList[i] == c)
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsWhitespace(this char c)
        {
            // 2.1 Characters and lines 
            // A whitespace character is a space(U + 0020), tab(U + 0009), newline(U + 000A), line tabulation (U + 000B), form feed (U + 000C), or carriage return (U + 000D).
            return c == ' ' || c == '\t' || c == '\n' || c == '\v' || c == '\f' || c == '\r';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsControl(this char c)
        {
            return c < ' ' || Char.IsControl(c);
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsEscapableSymbol(this char c)
        {
            // char.IsSymbol also works with Unicode symbols that cannot be escaped based on the specification.
            return (c > ' ' && c < '0') || (c > '9' && c < 'A') || (c > 'Z' && c < 'a') || (c > 'z' && c < 127) || c == '•';
        }

        //[MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsWhiteSpaceOrZero(this char c)
        {
            return IsWhitespace(c) || IsZero(c);
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

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsNewLine(this char c)
        {
            return c == '\n';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsZero(this char c)
        {
            return c == '\0';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsSpace(this char c)
        {
            // 2.1 Characters and lines 
            // A space is U+0020.
            return c == ' ';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsTab(this char c)
        {
            // 2.1 Characters and lines 
            // A space is U+0009.
            return c == '\t';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsSpaceOrTab(this char c)
        {
            return IsSpace(c) || IsTab(c);
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static char EscapeInsecure(this char c)
        {
            // 2.3 Insecure characters
            // For security reasons, the Unicode character U+0000 must be replaced with the REPLACEMENT CHARACTER (U+FFFD).
            return c == '\0' ? '\ufffd' : c;
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsAlphaUpper(this char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsAlpha(this char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsAlphaNumeric(this char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsDigit(this char c)
        {
            return c >= '0' && c <= '9';
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

        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public static bool IsEmailUsernameSpecialChar(char c)
        {
            return ".!#$%&'*+/=?^_`{|}~-+.~".IndexOf(c) >= 0;
        }
    }
}