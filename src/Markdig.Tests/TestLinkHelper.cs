// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using NUnit.Framework;
using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestLinkHelper
    {
        [Test]
        public void TestUrlSimple()
        {
            var text = new StringSlice("toto tutu");
            string link;
            Assert.True(LinkHelper.TryParseUrl(ref text, out link));
            Assert.AreEqual("toto", link);
            Assert.AreEqual(' ', text.CurrentChar);
        }

        [Test]
        public void TestUrlUrl()
        {
            var text = new StringSlice("http://google.com)");
            string link;
            Assert.True(LinkHelper.TryParseUrl(ref text, out link));
            Assert.AreEqual("http://google.com", link);
            Assert.AreEqual(')', text.CurrentChar);
        }

        [Test]
        public void TestUrlNestedParenthesis()
        {
            var text = new StringSlice("(toto)tutu(tata) nooo");
            string link;
            Assert.True(LinkHelper.TryParseUrl(ref text, out link));
            Assert.AreEqual("(toto)tutu(tata)", link);
            Assert.AreEqual(' ', text.CurrentChar);
        }

        [Test]
        public void TestUrlAlternate()
        {
            var text = new StringSlice("<toto_tata_tutu> nooo");
            string link;
            Assert.True(LinkHelper.TryParseUrl(ref text, out link));
            Assert.AreEqual("toto_tata_tutu", link);
            Assert.AreEqual(' ', text.CurrentChar);
        }

        [Test]
        public void TestUrlAlternateInvalid()
        {
            var text = new StringSlice("<toto_tata_tutu");
            string link;
            Assert.False(LinkHelper.TryParseUrl(ref text, out link));
        }

        [Test]
        public void TestTitleSimple()
        {
            var text = new StringSlice(@"'tata\tutu\''");
            string title;
            Assert.True(LinkHelper.TryParseTitle(ref text, out title));
            Assert.AreEqual(@"tata\tutu'", title);
        }

        [Test]
        public void TestTitleSimpleAlternate()
        {
            var text = new StringSlice(@"""tata\tutu\"""" ");
            string title;
            Assert.True(LinkHelper.TryParseTitle(ref text, out title));
            Assert.AreEqual(@"tata\tutu""", title);
            Assert.AreEqual(' ', text.CurrentChar);
        }

        [Test]
        public void TestUrlAndTitle()
        {
            //                           0         1         2         3
            //                           0123456789012345678901234567890123456789
            var text = new StringSlice(@"(http://google.com 'this is a title')ABC");
            string link;
            string title;
            SourceSpan linkSpan;
            SourceSpan titleSpan;
            Assert.True(LinkHelper.TryParseInlineLink(ref text, out link, out title, out linkSpan, out titleSpan));
            Assert.AreEqual("http://google.com", link);
            Assert.AreEqual("this is a title", title);
            Assert.AreEqual(new SourceSpan(1, 17), linkSpan);
            Assert.AreEqual(new SourceSpan(19, 35), titleSpan);
            Assert.AreEqual('A', text.CurrentChar);
        }

        [Test]
        public void TestUrlAndTitleEmpty()
        {
            //                           01234
            var text = new StringSlice(@"(<>)A");
            string link;
            string title;
            SourceSpan linkSpan;
            SourceSpan titleSpan;
            Assert.True(LinkHelper.TryParseInlineLink(ref text, out link, out title, out linkSpan, out titleSpan));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual(string.Empty, title);
            Assert.AreEqual(new SourceSpan(1, 2), linkSpan);
            Assert.AreEqual(SourceSpan.Empty, titleSpan);
            Assert.AreEqual('A', text.CurrentChar);
        }

        [Test]
        public void TestUrlAndTitleEmpty2()
        {
            //                           012345
            var text = new StringSlice(@"( <> )A");
            string link;
            string title;
            SourceSpan linkSpan;
            SourceSpan titleSpan;
            Assert.True(LinkHelper.TryParseInlineLink(ref text, out link, out title, out linkSpan, out titleSpan));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual(string.Empty, title);
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
            string link;
            string title;
            SourceSpan linkSpan;
            SourceSpan titleSpan;
            Assert.True(LinkHelper.TryParseInlineLink(ref text, out link, out title, out linkSpan, out titleSpan));
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
            string link;
            string title;
            SourceSpan linkSpan;
            SourceSpan titleSpan;
            Assert.True(LinkHelper.TryParseInlineLink(ref text, out link, out title, out linkSpan, out titleSpan));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual(string.Empty, title);
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
            string link;
            string title;
            SourceSpan linkSpan;
            SourceSpan titleSpan;
            Assert.True(LinkHelper.TryParseInlineLink(ref text, out link, out title, out linkSpan, out titleSpan));
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
            string label;
            SourceSpan labelSpan;
            Assert.True(LinkHelper.TryParseLabel(ref text, out label, out labelSpan));
            Assert.AreEqual(new SourceSpan(1, 3), labelSpan);
            Assert.AreEqual("foo", label);
        }

        [Test]
        public void TestLabelEscape()
        {
            //                           012345678
            var text = new StringSlice(@"[fo\[\]o]");
            string label;
            SourceSpan labelSpan;
            Assert.True(LinkHelper.TryParseLabel(ref text, out label, out labelSpan));
            Assert.AreEqual(new SourceSpan(1, 7), labelSpan);
            Assert.AreEqual(@"fo[]o", label);
        }

        [Test]
        public void TestLabelEscape2()
        {
            //                           0123
            var text = new StringSlice(@"[\]]");
            string label;
            SourceSpan labelSpan;
            Assert.True(LinkHelper.TryParseLabel(ref text, out label, out labelSpan));
            Assert.AreEqual(new SourceSpan(1, 2), labelSpan);
            Assert.AreEqual(@"]", label);
        }

        [Test]
        public void TestLabelInvalids()
        {
            string label;
            Assert.False(LinkHelper.TryParseLabel(new StringSlice(@"a"), out label));
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
            string label;
            SourceSpan labelSpan;
            Assert.True(LinkHelper.TryParseLabel(ref text, out label, out labelSpan));
            Assert.AreEqual(new SourceSpan(6, 17), labelSpan);
            Assert.AreEqual(@"fo o z", label);
        }

        [Test]
        public void TestlLinkReferenceDefinitionSimple()
        {
            //                           0         1         2         3
            //                           0123456789012345678901234567890123456789
            var text = new StringSlice(@"[foo]: /toto 'title'");
            string label;
            string url;
            string title;
            SourceSpan labelSpan;
            SourceSpan urlSpan;
            SourceSpan titleSpan;
            Assert.True(LinkHelper.TryParseLinkReferenceDefinition(ref text, out label, out url, out title, out labelSpan, out urlSpan, out titleSpan));
            Assert.AreEqual(@"foo", label);
            Assert.AreEqual(@"/toto", url);
            Assert.AreEqual(@"title", title);
            Assert.AreEqual(new SourceSpan(1, 3), labelSpan);
            Assert.AreEqual(new SourceSpan(7, 11), urlSpan);
            Assert.AreEqual(new SourceSpan(13, 19), titleSpan);

        }

        [Test]
        public void TestAutoLinkUrlSimple()
        {
            var text = new StringSlice(@"<http://google.com>");
            string url;
            bool isEmail;
            Assert.True(LinkHelper.TryParseAutolink(ref text, out url, out isEmail));
            Assert.False(isEmail);
            Assert.AreEqual("http://google.com", url);
        }

        [Test]
        public void TestAutoLinkEmailSimple()
        {
            var text = new StringSlice(@"<user@host.com>");
            string email;
            bool isEmail;
            Assert.True(LinkHelper.TryParseAutolink(ref text, out email, out isEmail));
            Assert.True(isEmail);
            Assert.AreEqual("user@host.com", email);
        }

        [Test]
        public void TestAutolinkInvalid()
        {
            string text;
            bool isEmail;
            Assert.False(LinkHelper.TryParseAutolink(new StringSlice(@""), out text, out isEmail));
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

        [TestCase("bær", "br")]
        [TestCase("bør", "br")]
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

        [TestCase("bær", "bær")]
        [TestCase("æ5el", "æ5el")]
        [TestCase("-æ5el", "æ5el")]
        [TestCase("-frø-", "frø")]
        [TestCase("-fr-ø", "fr-ø")]
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
    }
}