// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Tests;

[TestFixture]
public class TestLinkHelper
{
    [Test]
    public void TestUrlSimple()
    {
        var text = new StringSlice("toto tutu");
        Assert.True(LinkHelper.TryParseUrl(ref text, out string link, out _));
        Assert.AreEqual("toto", link);
        Assert.AreEqual(' ', text.CurrentChar);
    }

    [Test]
    public void TestUrlUrl()
    {
        var text = new StringSlice("http://google.com)");
        Assert.True(LinkHelper.TryParseUrl(ref text, out string link, out _));
        Assert.AreEqual("http://google.com", link);
        Assert.AreEqual(')', text.CurrentChar);
    }

    [Test]
    [TestCase("http://google.com.")]
    [TestCase("http://google.com. ")]
    public void TestUrlTrailingFullStop(string uri)
    {
        var text = new StringSlice(uri);
        Assert.True(LinkHelper.TryParseUrl(ref text, out string link, out _, true));
        Assert.AreEqual("http://google.com", link);
        Assert.AreEqual('.', text.CurrentChar);
    }

    [Test]
    public void TestUrlNestedParenthesis()
    {
        var text = new StringSlice("(toto)tutu(tata) nooo");
        Assert.True(LinkHelper.TryParseUrl(ref text, out string link, out _));
        Assert.AreEqual("(toto)tutu(tata)", link);
        Assert.AreEqual(' ', text.CurrentChar);
    }

    [Test]
    public void TestUrlAlternate()
    {
        var text = new StringSlice("<toto_tata_tutu> nooo");
        Assert.True(LinkHelper.TryParseUrl(ref text, out string link, out _));
        Assert.AreEqual("toto_tata_tutu", link);
        Assert.AreEqual(' ', text.CurrentChar);
    }

    [Test]
    public void TestUrlAlternateInvalid()
    {
        var text = new StringSlice("<toto_tata_tutu");
        Assert.False(LinkHelper.TryParseUrl(ref text, out string link, out _));
    }

    [Test]
    public void TestTitleSimple()
    {
        var text = new StringSlice(@"'tata\tutu\''");
        Assert.True(LinkHelper.TryParseTitle(ref text, out string title, out _));
        Assert.AreEqual(@"tata\tutu'", title);
    }

    [Test]
    public void TestTitleSimpleAlternate()
    {
        var text = new StringSlice(@"""tata\tutu\"""" ");
        Assert.True(LinkHelper.TryParseTitle(ref text, out string title, out _));
        Assert.AreEqual(@"tata\tutu""", title);
        Assert.AreEqual(' ', text.CurrentChar);
    }

    [Test]
    public void TestTitleMultiline()
    {
        var text = new StringSlice("'this\ris\r\na\ntitle'");
        Assert.True(LinkHelper.TryParseTitle(ref text, out string title, out _));
        Assert.AreEqual("this\ris\r\na\ntitle", title);
    }

    [Test]
    public void TestTitleMultilineWithSpaceAndBackslash()
    {
        var text = new StringSlice("'a\n\\ \\\ntitle'");
        Assert.True(LinkHelper.TryParseTitle(ref text, out string title, out _));
        Assert.AreEqual("a\n\\ \\\ntitle", title);
    }

    [Test]
    public void TestUrlAndTitle()
    {
        //                           0         1         2         3
        //                           0123456789012345678901234567890123456789
        var text = new StringSlice(@"(http://google.com 'this is a title')ABC");
        Assert.True(LinkHelper.TryParseInlineLink(ref text, out string link, out string title, out SourceSpan linkSpan, out SourceSpan titleSpan));
        Assert.AreEqual("http://google.com", link);
        Assert.AreEqual("this is a title", title);
        Assert.AreEqual(new SourceSpan(1, 17), linkSpan);
        Assert.AreEqual(new SourceSpan(19, 35), titleSpan);
        Assert.AreEqual('A', text.CurrentChar);
    }

    [Test]
    public void TestUrlEmptyAndTitleNull()
    {
        //                           01234
        var text = new StringSlice(@"(<>)A");
        Assert.True(LinkHelper.TryParseInlineLink(ref text, out string link, out string title, out SourceSpan linkSpan, out SourceSpan titleSpan));
        Assert.AreEqual(string.Empty, link);
        Assert.AreEqual(null, title);
        Assert.AreEqual(new SourceSpan(1, 2), linkSpan);
        Assert.AreEqual(SourceSpan.Empty, titleSpan);
        Assert.AreEqual('A', text.CurrentChar);
    }

    [Test]
    public void TestUrlEmptyAndTitleNull2()
    {
        //                           012345
        var text = new StringSlice(@"( <> )A");
        Assert.True(LinkHelper.TryParseInlineLink(ref text, out string link, out string title, out SourceSpan linkSpan, out SourceSpan titleSpan));
        Assert.AreEqual(string.Empty, link);
        Assert.AreEqual(null, title);
        Assert.AreEqual(new SourceSpan(2, 3), linkSpan);
        Assert.AreEqual(SourceSpan.Empty, titleSpan);
        Assert.AreEqual('A', text.CurrentChar);
    }


    [Test]
    public void TestUrlEmptyWithTitleWithMultipleSpaces()
    {
        //                           0         1         2
        //                           0123456789012345678901234567
        var text = new StringSlice(@"(   <>      'toto'       )A");
        Assert.True(LinkHelper.TryParseInlineLink(ref text, out string link, out string title, out SourceSpan linkSpan, out SourceSpan titleSpan));
        Assert.AreEqual(string.Empty, link);
        Assert.AreEqual("toto", title);
        Assert.AreEqual(new SourceSpan(4, 5), linkSpan);
        Assert.AreEqual(new SourceSpan(12, 17), titleSpan);
        Assert.AreEqual('A', text.CurrentChar);
    }

    [Test]
    public void TestUrlEmpty()
    {
        var text = new StringSlice(@"()A");
        Assert.True(LinkHelper.TryParseInlineLink(ref text, out string link, out string title, out SourceSpan linkSpan, out SourceSpan titleSpan));
        Assert.AreEqual(string.Empty, link);
        Assert.AreEqual(null, title);
        Assert.AreEqual(SourceSpan.Empty, linkSpan);
        Assert.AreEqual(SourceSpan.Empty, titleSpan);
        Assert.AreEqual('A', text.CurrentChar);
    }

    [Test]
    public void TestMultipleLines()
    {
        //                          0          1         2          3
        //                          01 2345678901234567890 1234567890123456789
        var text = new StringSlice("(\n<http://google.com>\n    'toto' )A");
        Assert.True(LinkHelper.TryParseInlineLink(ref text, out string link, out string title, out SourceSpan linkSpan, out SourceSpan titleSpan));
        Assert.AreEqual("http://google.com", link);
        Assert.AreEqual("toto", title);
        Assert.AreEqual(new SourceSpan(2, 20), linkSpan);
        Assert.AreEqual(new SourceSpan(26, 31), titleSpan);
        Assert.AreEqual('A', text.CurrentChar);
    }

    [Test]
    public void TestLabelSimple()
    {
        //                          01234
        var text = new StringSlice("[foo]");
        Assert.True(LinkHelper.TryParseLabel(ref text, out string label, out SourceSpan labelSpan));
        Assert.AreEqual(new SourceSpan(1, 3), labelSpan);
        Assert.AreEqual("foo", label);
    }

    [Test]
    public void TestLabelEscape()
    {
        //                           012345678
        var text = new StringSlice(@"[fo\[\]o]");
        Assert.True(LinkHelper.TryParseLabel(ref text, out string label, out SourceSpan labelSpan));
        Assert.AreEqual(new SourceSpan(1, 7), labelSpan);
        Assert.AreEqual(@"fo[]o", label);
    }

    [Test]
    public void TestLabelEscape2()
    {
        //                           0123
        var text = new StringSlice(@"[\]]");
        Assert.True(LinkHelper.TryParseLabel(ref text, out string label, out SourceSpan labelSpan));
        Assert.AreEqual(new SourceSpan(1, 2), labelSpan);
        Assert.AreEqual(@"]", label);
    }

    [Test]
    public void TestLabelInvalids()
    {
        Assert.False(LinkHelper.TryParseLabel(new StringSlice(@"a"), out string label));
        Assert.False(LinkHelper.TryParseLabel(new StringSlice(@"["), out label));
        Assert.False(LinkHelper.TryParseLabel(new StringSlice(@"[\x]"), out label));
        Assert.False(LinkHelper.TryParseLabel(new StringSlice(@"[[]"), out label));
        Assert.False(LinkHelper.TryParseLabel(new StringSlice(@"[     ]"), out label));
        Assert.False(LinkHelper.TryParseLabel(new StringSlice(@"[  \t \n  ]"), out label));
    }

    [Test]
    public void TestLabelWhitespaceCollapsedAndTrim()
    {
        //                           0         1         2         3
        //                           0123456789012345678901234567890123456789
        var text = new StringSlice(@"[     fo    o    z     ]");
        Assert.True(LinkHelper.TryParseLabel(ref text, out string label, out SourceSpan labelSpan));
        Assert.AreEqual(new SourceSpan(6, 17), labelSpan);
        Assert.AreEqual(@"fo o z", label);
    }

    [Test]
    public void TestlLinkReferenceDefinitionSimple()
    {
        //                           0         1         2         3
        //                           0123456789012345678901234567890123456789
        var text = new StringSlice(@"[foo]: /toto 'title'");
        Assert.True(LinkHelper.TryParseLinkReferenceDefinition(ref text, out string label, out string url, out string title, out SourceSpan labelSpan, out SourceSpan urlSpan, out SourceSpan titleSpan));
        Assert.AreEqual(@"foo", label);
        Assert.AreEqual(@"/toto", url);
        Assert.AreEqual(@"title", title);
        Assert.AreEqual(new SourceSpan(1, 3), labelSpan);
        Assert.AreEqual(new SourceSpan(7, 11), urlSpan);
        Assert.AreEqual(new SourceSpan(13, 19), titleSpan);

    }

    [Test]
    public void TestlLinkReferenceDefinitionInvalid()
    {
        var text = new StringSlice("[foo]: /url (title) x\n");
        Assert.False(LinkHelper.TryParseLinkReferenceDefinition(ref text, out _, out _, out _, out _, out _, out _));
    }

    [Test]
    public void TestAutoLinkUrlSimple()
    {
        var text = new StringSlice(@"<http://google.com>");
        Assert.True(LinkHelper.TryParseAutolink(ref text, out string url, out bool isEmail));
        Assert.False(isEmail);
        Assert.AreEqual("http://google.com", url);
    }

    [Test]
    public void TestAutoLinkEmailSimple()
    {
        var text = new StringSlice(@"<user@host.com>");
        Assert.True(LinkHelper.TryParseAutolink(ref text, out string email, out bool isEmail));
        Assert.True(isEmail);
        Assert.AreEqual("user@host.com", email);
    }

    [Test]
    public void TestAutolinkInvalid()
    {
        Assert.False(LinkHelper.TryParseAutolink(new StringSlice(@""), out string text, out bool isEmail));
        Assert.False(LinkHelper.TryParseAutolink(new StringSlice(@"<"), out text, out isEmail));
        Assert.False(LinkHelper.TryParseAutolink(new StringSlice(@"<ab"), out text, out isEmail));
        Assert.False(LinkHelper.TryParseAutolink(new StringSlice(@"<user@>"), out text, out isEmail));
    }

    [TestCase("Header identifiers in HTML", "header-identifiers-in-html")]
    [TestCase("* Dogs*?--in *my* house?", "dogs-in-my-house")] // Not Pandoc equivalent: dogs--in...
    [TestCase("[HTML], [S5], or [RTF]?", "html-s5-or-rtf")]
    [TestCase("3. Applications", "applications")]
    [TestCase("33", "")]
    public void TestUrilizeNonAscii_Pandoc(string input, string expectedResult)
    {
        Assert.AreEqual(expectedResult, LinkHelper.Urilize(input, false));
    }

    [TestCase("Header identifiers in HTML", "header-identifiers-in-html")]
    [TestCase("* Dogs*?--in *my* house?", "-dogs--in-my-house")]
    [TestCase("[HTML], [S5], or [RTF]?", "html-s5-or-rtf")]
    [TestCase("3. Applications", "3-applications")]
    [TestCase("33", "33")]
    public void TestUrilizeGfm(string input, string expectedResult)
    {
        Assert.AreEqual(expectedResult, LinkHelper.UrilizeAsGfm(input));
    }

    [TestCase("abc", "abc")]
    [TestCase("a-c", "a-c")]
    [TestCase("a c", "a-c")]
    [TestCase("a_c", "a_c")]
    [TestCase("a.c", "a.c")]
    [TestCase("a,c", "ac")]
    [TestCase("a--", "a")] // Not Pandoc-equivalent: a--
    [TestCase("a__", "a")] // Not Pandoc-equivalent: a__
    [TestCase("a..", "a")] // Not Pandoc-equivalent: a..
    [TestCase("a??", "a")]
    [TestCase("a  ", "a")]
    [TestCase("a--d", "a-d")]
    [TestCase("a__d", "a_d")]
    [TestCase("a??d", "ad")]
    [TestCase("a  d", "a-d")]
    [TestCase("a..d", "a.d")]
    [TestCase("-bc", "bc")]
    [TestCase("_bc", "bc")]
    [TestCase(" bc", "bc")]
    [TestCase("?bc", "bc")]
    [TestCase(".bc", "bc")]
    [TestCase("a-.-", "a")] // Not Pandoc equivalent: a-.-
    public void TestUrilizeOnlyAscii_Simple(string input, string expectedResult)
    {
        Assert.AreEqual(expectedResult, LinkHelper.Urilize(input, true));
    }

    [TestCase("bær", "baer")]
    [TestCase("bør", "boer")]
    [TestCase("bΘr", "br")]
    [TestCase("四五", "")]
    public void TestUrilizeOnlyAscii_NonAscii(string input, string expectedResult)
    {
        Assert.AreEqual(expectedResult, LinkHelper.Urilize(input, true));
    }

    [TestCase("bár", "bar")]
    [TestCase("àrrivé", "arrive")]
    public void TestUrilizeOnlyAscii_Normalization(string input, string expectedResult)
    {
        Assert.AreEqual(expectedResult, LinkHelper.Urilize(input, true));
    }

    // Tests for NormalizeScandinavianOrGermanChar method mappings
    // These special characters are always normalized (both allowOnlyAscii=true and false)
    // 
    // Note: When allowOnlyAscii=true, NFD (Canonical Decomposition) is applied first:
    // - German umlauts ä,ö,ü decompose to base letter + combining mark (ü -> u + ¨)
    //   The combining mark is then stripped, leaving just the base letter (ü -> u)
    // - å decomposes similarly (å -> a + ˚ -> a)
    // - But ø, æ, ß, þ, ð do NOT decompose, so they use NormalizeScandinavianOrGermanChar
    //
    // When allowOnlyAscii=false, NormalizeScandinavianOrGermanChar is used for ALL special chars
    
    // German ß (Eszett/sharp s) - does NOT decompose with NFD
    [TestCase("Straße", "strasse")]    // ß -> ss (both allowOnlyAscii=true and false)
    
    // Scandinavian æ, ø - do NOT decompose with NFD
    [TestCase("æble", "aeble")]        // æ -> ae (both modes)
    [TestCase("Ærø", "aeroe")]         // Æ -> Ae, ø -> oe (both modes, then lowercase)
    [TestCase("København", "koebenhavn")] // ø -> oe (both modes)
    [TestCase("Øresund", "oeresund")]  // Ø -> Oe (both modes, then lowercase)
    
    // Icelandic þ, ð - do NOT decompose with NFD
    [TestCase("þing", "thing")]        // þ (thorn) -> th (both modes)
    [TestCase("bað", "bad")]           // ð (eth) -> d (both modes)
    
    // Mixed special characters (only chars that behave same in both modes)
    [TestCase("øst-æble", "oest-aeble")] // ø->oe, æ->ae (both modes)
    public void TestUrilizeScandinavianGermanChars(string input, string expectedResult)
    {
        // These transformations apply regardless of allowOnlyAscii flag
        Assert.AreEqual(expectedResult, LinkHelper.Urilize(input, true));
        Assert.AreEqual(expectedResult, LinkHelper.Urilize(input, false));
    }
    
    // Tests specific to allowOnlyAscii=true behavior
    // German umlauts (ä, ö, ü) and å decompose with NFD, so they become base letter only
    [TestCase("schön", "schon")]       // ö decomposes to o (NFD strips combining mark)
    [TestCase("Mädchen", "madchen")]   // ä decomposes to a
    [TestCase("Übung", "ubung")]       // Ü decomposes to U (then lowercase to u)
    [TestCase("Düsseldorf", "dusseldorf")] // ü decomposes to u
    [TestCase("Käse", "kase")]         // ä decomposes to a
    [TestCase("gå", "ga")]             // å decomposes to a
    [TestCase("Ålesund", "alesund")]   // Å decomposes to A (then lowercase)
    [TestCase("grüßen", "grussen")]    // ü decomposes to u, ß -> ss
    [TestCase("Þór", "thor")]          // Þ -> Th, ó decomposes to o (then lowercase)
    [TestCase("Íslandsbanki", "islandsbanki")] // Í decomposes to I (then lowercase)
    public void TestUrilizeOnlyAscii_GermanUmlautsDecompose(string input, string expectedResult)
    {
        // With allowOnlyAscii=true, these characters decompose via NFD and lose their diacritics
        Assert.AreEqual(expectedResult, LinkHelper.Urilize(input, true));
    }
    
    // Tests specific to allowOnlyAscii=false behavior
    // All special chars use NormalizeScandinavianOrGermanChar (including ä, ö, ü, å)
    [TestCase("schön", "schoen")]      // ö -> oe (NormalizeScandinavianOrGermanChar)
    [TestCase("Mädchen", "maedchen")]  // ä -> ae
    [TestCase("Übung", "uebung")]      // Ü -> Ue (then lowercase)
    [TestCase("Düsseldorf", "duesseldorf")] // ü -> ue
    [TestCase("Käse", "kaese")]        // ä -> ae
    [TestCase("gå", "gaa")]            // å -> aa
    [TestCase("Ålesund", "aalesund")]  // Å -> Aa (then lowercase)
    [TestCase("grüßen", "gruessen")]   // ü -> ue, ß -> ss
    [TestCase("Þór", "thór")]          // Þ -> Th (then lowercase 'th'), ó is kept as-is
    [TestCase("Íslandsbanki", "íslandsbanki")] // í is kept as-is when allowOnlyAscii=false
    public void TestUrilizeNonAscii_GermanUmlautsExpanded(string input, string expectedResult)
    {
        // With allowOnlyAscii=false, these characters use NormalizeScandinavianOrGermanChar
        Assert.AreEqual(expectedResult, LinkHelper.Urilize(input, false));
    }
    
    [TestCase("123", "")]
    [TestCase("1,-b", "b")]
    [TestCase("b1,-", "b1")] // Not Pandoc equivalent: b1-
    [TestCase("ab3", "ab3")]
    [TestCase("ab3de", "ab3de")]
    public void TestUrilizeOnlyAscii_Numeric(string input, string expectedResult)
    {
        Assert.AreEqual(expectedResult, LinkHelper.Urilize(input, true));
    }

    [TestCase("一二三四五", "一二三四五")]
    [TestCase("一,-b", "一-b")]
    public void TestUrilizeNonAscii_NonAsciiNumeric(string input, string expectedResult)
    {
        Assert.AreEqual(expectedResult, LinkHelper.Urilize(input, false));
    }

    [TestCase("bær", "baer")]
    [TestCase("æ5el", "ae5el")]
    [TestCase("-æ5el", "ae5el")]
    [TestCase("-frø-", "froe")]
    [TestCase("-fr-ø", "fr-oe")]
    public void TestUrilizeNonAscii_Simple(string input, string expectedResult)
    {
        Assert.AreEqual(expectedResult, LinkHelper.Urilize(input, false));
    }

    // Just to be sure, test for characters expressly forbidden in URI fragments:
    [TestCase("b#r", "br")]
    [TestCase("b%r", "br")] // Invalid except as an escape character
    [TestCase("b^r", "br")]
    [TestCase("b[r", "br")]
    [TestCase("b]r", "br")]
    [TestCase("b{r", "br")]
    [TestCase("b}r", "br")]
    [TestCase("b<r", "br")]
    [TestCase("b>r", "br")]
    [TestCase(@"b\r", "br")]
    [TestCase(@"b""r", "br")]
    [TestCase(@"Requirement 😀", "requirement")]
    public void TestUrilizeNonAscii_NonValidCharactersForFragments(string input, string expectedResult)
    {
        Assert.AreEqual(expectedResult, LinkHelper.Urilize(input, false));
    }

    [Test]
    public void TestUnicodeInDomainNameOfLinkReferenceDefinition()
    {
        TestParser.TestSpec("[Foo]\n\n[Foo]: http://ünicode.com", "<p><a href=\"http://xn--nicode-2ya.com\">Foo</a></p>");
    }
}
