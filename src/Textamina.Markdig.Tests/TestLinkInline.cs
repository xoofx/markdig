using NUnit.Framework;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Tests
{
    [TestFixture]
    public class TestLinkInline
    {

        [Test]
        public void TestLinkSimple()
        {
            var text = new StringLineGroup("toto tutu");
            string link;
            Assert.True(LinkInline.TryParseLink(text, out link));
            Assert.AreEqual("toto", link);
            Assert.AreEqual(' ', text.Current);
        }

        [Test]
        public void TestLinkUrl()
        {
            var text = new StringLineGroup("http://google.com)");
            string link;
            Assert.True(LinkInline.TryParseLink(text, out link));
            Assert.AreEqual("http://google.com", link);
            Assert.AreEqual(')', text.Current);
        }

        [Test]
        public void TestLinkNestedParenthesis()
        {
            var text = new StringLineGroup("(toto)tutu(tata) nooo");
            string link;
            Assert.True(LinkInline.TryParseLink(text, out link));
            Assert.AreEqual("(toto)tutu(tata)", link);
            Assert.AreEqual(' ', text.Current);
        }

        [Test]
        public void TestLinkAlternate()
        {
            var text = new StringLineGroup("<toto_tata_tutu> nooo");
            string link;
            Assert.True(LinkInline.TryParseLink(text, out link));
            Assert.AreEqual("toto_tata_tutu", link);
            Assert.AreEqual(' ', text.Current);
        }

        [Test]
        public void TestLinkAlternateInvalid()
        {
            var text = new StringLineGroup("<toto_tata_tutu");
            string link;
            Assert.False(LinkInline.TryParseLink(text, out link));
        }

        [Test]
        public void TestTitleSimple()
        {
            var text = new StringLineGroup(@"'tata\tutu\''");
            string title;
            Assert.True(LinkInline.TryParseTitle(text, out title));
            Assert.AreEqual(@"tata\tutu'", title);
        }

        [Test]
        public void TestTitleSimpleAlternate()
        {
            var text = new StringLineGroup(@"""tata\tutu\"""" ");
            string title;
            Assert.True(LinkInline.TryParseTitle(text, out title));
            Assert.AreEqual(@"tata\tutu""", title);
            Assert.AreEqual(' ', text.Current);
        }

        [Test]
        public void TestLinkAndTitle()
        {
            var text = new StringLineGroup(@"(http://google.com 'this is a title')ABC");
            string link;
            string title;
            Assert.True(LinkInline.TryParseUrlAndTitle(text, out link, out title));
            Assert.AreEqual("http://google.com", link);
            Assert.AreEqual("this is a title", title);
            Assert.AreEqual('A', text.Current);
        }

        [Test]
        public void TestLinkAndTitleEmpty()
        {
            var text = new StringLineGroup(@"(<>)A");
            string link;
            string title;
            Assert.True(LinkInline.TryParseUrlAndTitle(text, out link, out title));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual(string.Empty, title);
            Assert.AreEqual('A', text.Current);
        }

        [Test]
        public void TestLinkAndTitleEmpty2()
        {
            var text = new StringLineGroup(@"( <> )A");
            string link;
            string title;
            Assert.True(LinkInline.TryParseUrlAndTitle(text, out link, out title));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual(string.Empty, title);
            Assert.AreEqual('A', text.Current);
        }


        [Test]
        public void TestLinkEmptyWithTitleWithMultipleSpaces()
        {
            var text = new StringLineGroup(@"(   <>      'toto'       )A");
            string link;
            string title;
            Assert.True(LinkInline.TryParseUrlAndTitle(text, out link, out title));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual("toto", title);
            Assert.AreEqual('A', text.Current);
        }

        [Test]
        public void TestLinkEmpty()
        {
            var text = new StringLineGroup(@"()A");
            string link;
            string title;
            Assert.True(LinkInline.TryParseUrlAndTitle(text, out link, out title));
            Assert.AreEqual(string.Empty, link);
            Assert.AreEqual(string.Empty, title);
            Assert.AreEqual('A', text.Current);
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
            Assert.True(LinkInline.TryParseUrlAndTitle(text, out link, out title));
            Assert.AreEqual("http://google.com", link);
            Assert.AreEqual("toto", title);
            Assert.AreEqual('A', text.Current);
        }
    }
}