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
            var text = new StringLineGroup("toto tutu");
            string link;
            Assert.True(LinkHelper.TryParseUrl(text, out link));
            Assert.AreEqual("toto", link);
            Assert.AreEqual(' ', text.CurrentChar);
        }

        [Test]
        public void TestUrlUrl()
        {
            var text = new StringLineGroup("http://google.com)");
            string link;
            Assert.True(LinkHelper.TryParseUrl(text, out link));
            Assert.AreEqual("http://google.com", link);
            Assert.AreEqual(')', text.CurrentChar);
        }

        [Test]
        public void TestUrlNestedParenthesis()
        {
            var text = new StringLineGroup("(toto)tutu(tata) nooo");
            string link;
            Assert.True(LinkHelper.TryParseUrl(text, out link));
            Assert.AreEqual("(toto)tutu(tata)", link);
            Assert.AreEqual(' ', text.CurrentChar);
        }

        [Test]
        public void TestUrlAlternate()
        {
            var text = new StringLineGroup("<toto_tata_tutu> nooo");
            string link;
            Assert.True(LinkHelper.TryParseUrl(text, out link));
            Assert.AreEqual("toto_tata_tutu", link);
            Assert.AreEqual(' ', text.CurrentChar);
        }

        [Test]
        public void TestUrlAlternateInvalid()
        {
            var text = new StringLineGroup("<toto_tata_tutu");
            string link;
            Assert.False(LinkHelper.TryParseUrl(text, out link));
        }

        [Test]
        public void TestTitleSimple()
        {
            var text = new StringLineGroup(@"'tata\tutu\''");
            string title;
            Assert.True(LinkHelper.TryParseTitle(text, out title));
            Assert.AreEqual(@"tata\tutu'", title);
        }

        [Test]
        public void TestTitleSimpleAlternate()
        {
            var text = new StringLineGroup(@"""tata\tutu\"""" ");
            string title;
            Assert.True(LinkHelper.TryParseTitle(text, out title));
            Assert.AreEqual(@"tata\tutu""", title);
            Assert.AreEqual(' ', text.CurrentChar);
        }

        [Test]
        public void TestUrlAndTitle()
        {
            var text = new StringLineGroup(@"(http://google.com 'this is a title')ABC");
            string link;
            string title;
            Assert.True(LinkHelper.TryParseUrlAndTitle(text, out link, out title));
            Assert.AreEqual("http://google.com", link);
            Assert.AreEqual("this is a title", title);
            Assert.AreEqual('A', text.CurrentChar);
        }

        [Test]
        public void TestUrlAndTitleEmpty()
        {
            var text = new StringLineGroup(@"(<>)A");
            string link;
            string title;
            Assert.True(LinkHelper.TryParseUrlAndTitle(text, out link, out title));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual(string.Empty, title);
            Assert.AreEqual('A', text.CurrentChar);
        }

        [Test]
        public void TestUrlAndTitleEmpty2()
        {
            var text = new StringLineGroup(@"( <> )A");
            string link;
            string title;
            Assert.True(LinkHelper.TryParseUrlAndTitle(text, out link, out title));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual(string.Empty, title);
            Assert.AreEqual('A', text.CurrentChar);
        }


        [Test]
        public void TestUrlEmptyWithTitleWithMultipleSpaces()
        {
            var text = new StringLineGroup(@"(   <>      'toto'       )A");
            string link;
            string title;
            Assert.True(LinkHelper.TryParseUrlAndTitle(text, out link, out title));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual("toto", title);
            Assert.AreEqual('A', text.CurrentChar);
        }

        [Test]
        public void TestUrlEmpty()
        {
            var text = new StringLineGroup(@"()A");
            string link;
            string title;
            Assert.True(LinkHelper.TryParseUrlAndTitle(text, out link, out title));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual(string.Empty, title);
            Assert.AreEqual('A', text.CurrentChar);
        }

        [Test]
        public void TestMultipleLines()
        {
            var text = new StringLineGroup()
            {
                new StringLine("("),
                new StringLine("   <http://google.com>  "),
                new StringLine(" 'toto' )A")
            };
            string link;
            string title;
            Assert.True(LinkHelper.TryParseUrlAndTitle(text, out link, out title));
            Assert.AreEqual("http://google.com", link);
            Assert.AreEqual("toto", title);
            Assert.AreEqual('A', text.CurrentChar);
        }


        [Test]
        public void TestLabelSimple()
        {
            var text = new StringLineGroup("[foo]");
            string label;
            Assert.True(LinkHelper.TryParseLabel(text, out label));
            Assert.AreEqual("foo", label);
        }

        [Test]
        public void TestLabelEscape()
        {
            var text = new StringLineGroup(@"[fo\[\]o]");
            string label;
            Assert.True(LinkHelper.TryParseLabel(text, out label));
            Assert.AreEqual(@"fo[]o", label);
        }

        [Test]
        public void TestLabelEscape2()
        {
            var text = new StringLineGroup(@"[\]]");
            string label;
            Assert.True(LinkHelper.TryParseLabel(text, out label));
            Assert.AreEqual(@"]", label);
        }

        [Test]
        public void TestLabelInvalids()
        {
            string label;
            Assert.False(LinkHelper.TryParseLabel(new StringLineGroup(@"a"), out label));
            Assert.False(LinkHelper.TryParseLabel(new StringLineGroup(@"["), out label));
            Assert.False(LinkHelper.TryParseLabel(new StringLineGroup(@"[\x]"), out label));
            Assert.False(LinkHelper.TryParseLabel(new StringLineGroup(@"[[]"), out label));
            Assert.False(LinkHelper.TryParseLabel(new StringLineGroup(@"[     ]"), out label));
            Assert.False(LinkHelper.TryParseLabel(new StringLineGroup(@"[  \t \n  ]"), out label));
        }

        [Test]
        public void TestLabelWhitespaceCollapsedAndTrim()
        {
            var text = new StringLineGroup(@"[     fo    o    z     ]");
            string label;
            Assert.True(LinkHelper.TryParseLabel(text, out label));
            Assert.AreEqual(@"fo o z", label);
        }
    }
}