using System.Collections.Generic;
using System.Globalization;
using Markdig.Helpers;
using NUnit.Framework;

namespace Markdig.Tests
{
    public class TestCharHelper
    {
        // An ASCII punctuation character is
        // !, ", #, $, %, &, ', (, ), *, +, ,, -, ., / (U+0021–2F),
        // :, ;, <, =, >, ?, @ (U+003A–0040),
        // [, \, ], ^, _, ` (U+005B–0060),
        // {, |, }, or ~ (U+007B–007E).
        private static readonly HashSet<char> s_asciiPunctuation = new()
        {
            '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/',
            ':', ';', '<', '=', '>', '?', '@',
            '[', '\\', ']', '^', '_', '`',
            '{', '|', '}', '~'
        };

        // A Unicode punctuation character is an ASCII punctuation character or anything in the general Unicode categories
        // Pc, Pd, Pe, Pf, Pi, Po, or Ps.
        private static readonly HashSet<UnicodeCategory> s_punctuationCategories = new()
        {
            UnicodeCategory.ConnectorPunctuation,
            UnicodeCategory.DashPunctuation,
            UnicodeCategory.ClosePunctuation,
            UnicodeCategory.FinalQuotePunctuation,
            UnicodeCategory.InitialQuotePunctuation,
            UnicodeCategory.OtherPunctuation,
            UnicodeCategory.OpenPunctuation
        };

        private static bool ExpectedIsPunctuation(char c)
        {
            return c <= 127
                ? s_asciiPunctuation.Contains(c)
                : s_punctuationCategories.Contains(CharUnicodeInfo.GetUnicodeCategory(c));
        }

        private static bool ExpectedIsWhitespace(char c)
        {
            // A Unicode whitespace character is any code point in the Unicode Zs general category,
            // or a tab (U+0009), line feed (U+000A), form feed (U+000C), or carriage return (U+000D).
            return c == '\t' || c == '\n' || c == '\u000C' || c == '\r' ||
                CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator;
        }

        [Test]
        public void IsWhitespace()
        {
            for (int i = char.MinValue; i <= char.MaxValue; i++)
            {
                char c = (char)i;

                Assert.AreEqual(ExpectedIsWhitespace(c), CharHelper.IsWhitespace(c));
            }
        }

        [Test]
        public void CheckUnicodeCategory()
        {
            for (int i = char.MinValue; i <= char.MaxValue; i++)
            {
                char c = (char)i;

                bool expectedSpace = c == 0 || ExpectedIsWhitespace(c);
                bool expectedPunctuation = c == 0 || ExpectedIsPunctuation(c);

                CharHelper.CheckUnicodeCategory(c, out bool spaceActual, out bool punctuationActual);

                Assert.AreEqual(expectedSpace, spaceActual);
                Assert.AreEqual(expectedPunctuation, punctuationActual);
            }
        }

        [Test]
        public void IsSpaceOrPunctuation()
        {
            for (int i = char.MinValue; i <= char.MaxValue; i++)
            {
                char c = (char)i;

                bool expected = c == 0 || ExpectedIsWhitespace(c) || ExpectedIsPunctuation(c);

                Assert.AreEqual(expected, CharHelper.IsSpaceOrPunctuation(c));
            }
        }
    }
}
