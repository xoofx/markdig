using System.Globalization;

using Markdig.Helpers;

namespace Markdig.Tests;

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

    // A Unicode punctuation character is a character in the Unicode P (punctuation) or S (symbol) general categories.
    private static readonly HashSet<UnicodeCategory> s_punctuationCategories =
    [
        UnicodeCategory.ConnectorPunctuation,
        UnicodeCategory.DashPunctuation,
        UnicodeCategory.OpenPunctuation,
        UnicodeCategory.ClosePunctuation,
        UnicodeCategory.InitialQuotePunctuation,
        UnicodeCategory.FinalQuotePunctuation,
        UnicodeCategory.OtherPunctuation,
        UnicodeCategory.MathSymbol,
        UnicodeCategory.CurrencySymbol,
        UnicodeCategory.ModifierSymbol,
        UnicodeCategory.OtherSymbol,
    ];

    private static readonly HashSet<UnicodeCategory> s_punctuationWithoutSymbolsCategories =
    [
        UnicodeCategory.ConnectorPunctuation,
        UnicodeCategory.DashPunctuation,
        UnicodeCategory.OpenPunctuation,
        UnicodeCategory.ClosePunctuation,
        UnicodeCategory.InitialQuotePunctuation,
        UnicodeCategory.FinalQuotePunctuation,
        UnicodeCategory.OtherPunctuation,
    ];

    private static bool ExpectedIsPunctuation(char c)
    {
        return c <= 127
            ? s_asciiPunctuation.Contains(c)
            : s_punctuationCategories.Contains(CharUnicodeInfo.GetUnicodeCategory(c));
    }

    private static bool ExpectedIsPunctuationWithoutSymbols(char c)
    {
        return c <= 127
            ? s_asciiPunctuation.Contains(c)
            : s_punctuationWithoutSymbolsCategories.Contains(CharUnicodeInfo.GetUnicodeCategory(c));
    }

    private static bool ExpectedIsWhitespace(char c)
    {
        // A Unicode whitespace character is any code point in the Unicode Zs general category,
        // or a tab (U+0009), line feed (U+000A), form feed (U+000C), or carriage return (U+000D).
        return c == '\t' || c == '\n' || c == '\f' || c == '\r' ||
            CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator;
    }

    [Test]
    public void IsAcrossTab()
    {
        Assert.False(CharHelper.IsAcrossTab(0));
        Assert.True(CharHelper.IsAcrossTab(1));
        Assert.True(CharHelper.IsAcrossTab(2));
        Assert.True(CharHelper.IsAcrossTab(3));
        Assert.False(CharHelper.IsAcrossTab(4));
    }

    [Test]
    public void AddTab()
    {
        Assert.AreEqual(4, CharHelper.AddTab(0));
        Assert.AreEqual(4, CharHelper.AddTab(1));
        Assert.AreEqual(4, CharHelper.AddTab(2));
        Assert.AreEqual(4, CharHelper.AddTab(3));
        Assert.AreEqual(8, CharHelper.AddTab(4));
        Assert.AreEqual(8, CharHelper.AddTab(5));
    }

    [Test]
    public void IsWhitespace()
    {
        Test(
            ExpectedIsWhitespace,
            CharHelper.IsWhitespace);

        Test(
            ExpectedIsWhitespace,
            CharHelper.WhitespaceChars.Contains);
    }

    [Test]
    public void IsWhiteSpaceOrZero()
    {
        Test(
            c => ExpectedIsWhitespace(c) || c == 0,
            CharHelper.IsWhiteSpaceOrZero);
    }

    [Test]
    public void IsAsciiPunctuation()
    {
        Test(
            c => char.IsAscii(c) && ExpectedIsPunctuation(c),
            CharHelper.IsAsciiPunctuation);
    }

    [Test]
    public void IsAsciiPunctuationOrZero()
    {
        Test(
            c => char.IsAscii(c) && (ExpectedIsPunctuation(c) || c == 0),
            CharHelper.IsAsciiPunctuationOrZero);
    }

    [Test]
    public void IsSpaceOrPunctuationForGFMAutoLink()
    {
        Test(
            c => c == 0 || ExpectedIsWhitespace(c) || ExpectedIsPunctuationWithoutSymbols(c),
            CharHelper.IsSpaceOrPunctuationForGFMAutoLink);
    }

    [Test]
    public void InvalidAutoLinkCharacters()
    {
        // 6.5 Autolinks - https://spec.commonmark.org/0.31.2/#autolinks
        // An absolute URI, for these purposes, consists of a scheme followed by a colon (:) followed by
        // zero or more characters other than ASCII control characters, space, <, and >.
        //
        // 2.1 Characters and lines
        // An ASCII control character is a character between U+0000–1F (both including) or U+007F.
        Test(
            c => c != 0 && c is < (char)0x20 or ' ' or '<' or '>' or '\u007F',
            CharHelper.InvalidAutoLinkCharacters.Contains);
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
    public void IsControl()
    {
        Test(
            char.IsControl,
            CharHelper.IsControl);
    }

    [Test]
    public void IsAlpha()
    {
        Test(
            c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'),
            CharHelper.IsAlpha);
    }

    [Test]
    public void IsAlphaUpper()
    {
        Test(
            c => c >= 'A' && c <= 'Z',
            CharHelper.IsAlphaUpper);
    }

    [Test]
    public void IsAlphaNumeric()
    {
        Test(
            c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'),
            CharHelper.IsAlphaNumeric);
    }

    [Test]
    public void IsDigit()
    {
        Test(
            c => c >= '0' && c <= '9',
            CharHelper.IsDigit);
    }

    [Test]
    public void IsNewLineOrLineFeed()
    {
        Test(
            c => c is '\r' or '\n',
            CharHelper.IsNewLineOrLineFeed);
    }

    [Test]
    public void IsSpaceOrTab()
    {
        Test(
            c => c is ' ' or '\t',
            CharHelper.IsSpaceOrTab);
    }

    [Test]
    public void IsEscapableSymbol()
    {
        Test(
            "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~•".Contains,
            CharHelper.IsEscapableSymbol);
    }

    [Test]
    public void IsEmailUsernameSpecialChar()
    {
        Test(
            ".!#$%&'*+/=?^_`{|}~-+.~".Contains,
            CharHelper.IsEmailUsernameSpecialChar);
    }

    [Test]
    public void IsEmailUsernameSpecialCharOrDigit()
    {
        Test(
            c => CharHelper.IsDigit(c) || ".!#$%&'*+/=?^_`{|}~-+.~".Contains(c),
            CharHelper.IsEmailUsernameSpecialCharOrDigit);
    }

    private static void Test(Func<char, bool> expected, Func<char, bool> actual)
    {
        for (int i = char.MinValue; i <= char.MaxValue; i++)
        {
            char c = (char)i;

            bool expectedResult = expected(c);
            bool actualResult = actual(c);

            if (expectedResult != actualResult)
            {
                Assert.AreEqual(expectedResult, actualResult, $"Char: '{c}' ({i})");
            }
        }
    }
}
