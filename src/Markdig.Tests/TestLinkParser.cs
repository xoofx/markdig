using System.Linq;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using NUnit.Framework;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestLinkParser
    {
        [Test]
        public void TestInlineLink()
        {
            var doc = Markdown.Parse("There is a [link](/yoyo)");
            var link = doc.Descendants<LinkInline>().First();
            Assert.AreEqual("link", link.Label);
            Assert.AreEqual("/yoyo", link.Url);
        }

        [Test]
        public void TestReferenceLink()
        {
            var doc = Markdown.Parse("There is a [link][foo]\n\n[foo]: /toto");
            var link = doc.Descendants<LinkInline>().First();
            Assert.AreEqual("link", link.Label);
            Assert.AreEqual("/toto", link.Url);
        }

        [Test]
        public void TestReferenceLinkSimple()
        {
            var doc = Markdown.Parse("There is a [link]\n\n[link]: /tutu");
            var link = doc.Descendants<LinkInline>().First();
            Assert.AreEqual("link", link.Label);
            Assert.AreEqual("/tutu", link.Url);
        }
    }
}