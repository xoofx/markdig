using NUnit.Framework;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Tests
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
            var text = new StringSlice(@"(http://google.com 'this is a title')ABC");
            string link;
            string title;
            Assert.True(LinkHelper.TryParseInlineLink(ref text, out link, out title));
            Assert.AreEqual("http://google.com", link);
            Assert.AreEqual("this is a title", title);
            Assert.AreEqual('A', text.CurrentChar);
        }

        [Test]
        public void TestUrlAndTitleEmpty()
        {
            var text = new StringSlice(@"(<>)A");
            string link;
            string title;
            Assert.True(LinkHelper.TryParseInlineLink(ref text, out link, out title));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual(string.Empty, title);
            Assert.AreEqual('A', text.CurrentChar);
        }

        [Test]
        public void TestUrlAndTitleEmpty2()
        {
            var text = new StringSlice(@"( <> )A");
            string link;
            string title;
            Assert.True(LinkHelper.TryParseInlineLink(ref text, out link, out title));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual(string.Empty, title);
            Assert.AreEqual('A', text.CurrentChar);
        }


        [Test]
        public void TestUrlEmptyWithTitleWithMultipleSpaces()
        {
            var text = new StringSlice(@"(   <>      'toto'       )A");
            string link;
            string title;
            Assert.True(LinkHelper.TryParseInlineLink(ref text, out link, out title));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual("toto", title);
            Assert.AreEqual('A', text.CurrentChar);
        }

        [Test]
        public void TestUrlEmpty()
        {
            var text = new StringSlice(@"()A");
            string link;
            string title;
            Assert.True(LinkHelper.TryParseInlineLink(ref text, out link, out title));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual(string.Empty, title);
            Assert.AreEqual('A', text.CurrentChar);
        }

        [Test]
        public void TestMultipleLines()
        {
            var text = new StringSlice(@"(
                   <http://google.com>  
                 'toto' )A");
            string link;
            string title;
            Assert.True(LinkHelper.TryParseInlineLink(ref text, out link, out title));
            Assert.AreEqual("http://google.com", link);
            Assert.AreEqual("toto", title);
            Assert.AreEqual('A', text.CurrentChar);
        }

        [Test]
        public void TestLabelSimple()
        {
            var text = new StringSlice("[foo]");
            string label;
            Assert.True(LinkHelper.TryParseLabel(ref text, out label));
            Assert.AreEqual("foo", label);
        }

        [Test]
        public void TestLabelEscape()
        {
            var text = new StringSlice(@"[fo\[\]o]");
            string label;
            Assert.True(LinkHelper.TryParseLabel(ref text, out label));
            Assert.AreEqual(@"fo[]o", label);
        }

        [Test]
        public void TestLabelEscape2()
        {
            var text = new StringSlice(@"[\]]");
            string label;
            Assert.True(LinkHelper.TryParseLabel(ref text, out label));
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
            var text = new StringSlice(@"[     fo    o    z     ]");
            string label;
            Assert.True(LinkHelper.TryParseLabel(ref text, out label));
            Assert.AreEqual(@"fo o z", label);
        }

        [Test]
        public void TestlLinkReferenceDefinitionSimple()
        {
            var text = new StringSlice(@"[foo]: /toto 'title'");
            string label;
            string url;
            string title;
            Assert.True(LinkHelper.TryParseLinkReferenceDefinition(ref text, out label, out url, out title));
            Assert.AreEqual(@"foo", label);
            Assert.AreEqual(@"/toto", url);
            Assert.AreEqual(@"title", title);
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
    }
}