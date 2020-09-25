using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using NUnit.Framework;
using System.IO;

/// <summary>
/// General notes
/// - whitespace can occur before, between and after symbols
/// </summary>
namespace Markdig.Tests
{
    [TestFixture]
    public class TestCst
    {
        [Test]
        public void TestWhitespaceBefore()
        {
            var markdown = " This is a paragraph.  ";

            var pipelineBuilder = new MarkdownPipelineBuilder();
            MarkdownPipeline pipeline = pipelineBuilder.Build();

            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);
            var paragraphBlock = markdownDocument[0] as ParagraphBlock;

            var sw = new StringWriter();
            var nr = new NormalizeRenderer(sw);
            nr.Write(markdownDocument);

            Assert.AreEqual(markdown, sw.ToString());
        }

        [Test]
        public void TestNewLinesBeforeAndAfter()
        {
            var markdown = " \n  \nLine2\n\nLine1\n\n";
            //var markdown = "\r\nLine2\r\n\r\nLine1\r\n\r\n";

            var pipelineBuilder = new MarkdownPipelineBuilder();
            MarkdownPipeline pipeline = pipelineBuilder.Build();

            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);
            var paragraphBlock = markdownDocument[0] as ParagraphBlock;

            var sw = new StringWriter();
            var nr = new NormalizeRenderer(sw);
            nr.Write(markdownDocument);

            Assert.AreEqual(markdown, sw.ToString());
        }

        private void RoundTrip(string markdown)
        {
            var pipelineBuilder = new MarkdownPipelineBuilder();
            MarkdownPipeline pipeline = pipelineBuilder.Build();
            MarkdownDocument markdownDocument = Markdown.Parse(markdown, pipeline);
            var sw = new StringWriter();
            var nr = new NormalizeRenderer(sw);

            nr.Write(markdownDocument);

            Assert.AreEqual(markdown, sw.ToString());
        }

        [TestCase("p")]
        [TestCase(" p")]
        [TestCase("p ")]
        [TestCase(" p ")]

        [TestCase("p\np")]
        [TestCase(" p\np")]
        [TestCase("p \np")]
        [TestCase(" p \np")]

        [TestCase("p\n p")]
        [TestCase(" p\n p")]
        [TestCase("p \n p")]
        [TestCase(" p \n p")]

        [TestCase("p\np ")]
        [TestCase(" p\np ")]
        [TestCase("p \np ")]
        [TestCase(" p \np ")]

        [TestCase("p\n\n p ")]
        [TestCase(" p\n\n p ")]
        [TestCase("p \n\n p ")]
        [TestCase(" p \n\n p ")]

        [TestCase("p\n\np")]
        [TestCase(" p\n\np")]
        [TestCase("p \n\np")]
        [TestCase(" p \n\np")]

        [TestCase("p\n\n p")]
        [TestCase(" p\n\n p")]
        [TestCase("p \n\n p")]
        [TestCase(" p \n\n p")]

        [TestCase("p\n\np ")]
        [TestCase(" p\n\np ")]
        [TestCase("p \n\np ")]
        [TestCase(" p \n\np ")]

        [TestCase("p\n\n p ")]
        [TestCase(" p\n\n p ")]
        [TestCase("p \n\n p ")]
        [TestCase(" p \n\n p ")]
        public void TestParagraph(string value)
        {
            RoundTrip(value);
        }

        [Test]
        public void TestNewLinesBeforeAndAfter2()
        {
            RoundTrip("\n# H1\n\nLine1");
            RoundTrip("\n# H1\n\nLine1\n");
            RoundTrip("\n# H1\n\nLine1\n\n");
            RoundTrip("\n\n# H1\n\nLine1\n\n");
            RoundTrip("\n\n# H1\nLine1\n\n");
            RoundTrip("\n\n# H1\nLine1\n\n");
        }

        // i = item
        [TestCase("- i1")]
        [TestCase("- i1 ")]
        [TestCase("- i1\n- i2")]
        [TestCase("- i1\n    - i2")]
        [TestCase("- i1\n    - i1.1\n    - i1.2")]
        public void TestList(string value)
        {
            RoundTrip(value);
        }


        [Test]
        public void TestImage()
        {
            RoundTrip("   ![description](http://example.com)");
            RoundTrip("paragraph   ![description](http://example.com)");
        }

        [TestCase("- > q")]
        [TestCase(" - > q")]
        [TestCase("  - > q")]
        [TestCase("   - > q")]
        [TestCase("-  > q")]
        [TestCase(" -  > q")]
        [TestCase("  -  > q")]
        [TestCase("   -  > q")]
        [TestCase("-   > q")]
        [TestCase(" -   > q")]
        [TestCase("  -   > q")]
        [TestCase("   -   > q")]
        [TestCase("-    > q")]
        [TestCase(" -    > q")]
        [TestCase("  -    > q")]
        [TestCase("   -    > q")]
        [TestCase("   -    > q1\n   -    > q2")]
        public void TestListItem_BlockQuote(string value)
        {
            RoundTrip(value);
        }

        [TestCase(">quote")]
        [TestCase("> quote")]
        [TestCase(">  quote")]
        [TestCase("   >  quote")]
        public void TestBlockQuote(string value)
        {
            RoundTrip(value);
        }

        [Test]
        public void TestBlockQuote()
        {
            //RoundTrip("- >quote"); // par in qb in l in li
            // 3ws? - ws 3ws? q ws? p
            // 3ws?     lb
            // -        li
            // ws       li
            // 3ws?     qb
            // q        qb
            // ws?      qb
            // p        p
            //RoundTrip("   -    > quote"); // par in qb in l in li
        }

        /// A codeblock is indented with 4 spaces. After the 4th space, whitespace is interpreted as content.
        [TestCase("    code")]
        [TestCase("     code")]
        public void TestImplicitCodeBlock(string value)
        {
            RoundTrip(value);
        }

        [TestCase("[](b)")]
        [TestCase(" [](b)")]
        [TestCase("[](b) ")]
        [TestCase(" [](b) ")]

        [TestCase("[a](b)")]
        [TestCase(" [a](b)")]
        [TestCase("[a](b) ")]
        [TestCase(" [a](b) ")]

        [TestCase("[ a](b)")]
        [TestCase(" [ a](b)")]
        [TestCase("[ a](b) ")]
        [TestCase(" [ a](b) ")]

        [TestCase("[a ](b)")]
        [TestCase(" [a ](b)")]
        [TestCase("[a ](b) ")]
        [TestCase(" [a ](b) ")]

        [TestCase("[ a ](b)")]
        [TestCase(" [ a ](b)")]
        [TestCase("[ a ](b) ")]
        [TestCase(" [ a ](b) ")]

        // below cases are required for a full CST but not have low prio for impl
        //[TestCase("[]( b)")]
        //[TestCase(" []( b)")]
        //[TestCase("[]( b) ")]
        //[TestCase(" []( b) ")]

        //[TestCase("[a]( b)")]
        //[TestCase(" [a]( b)")]
        //[TestCase("[a]( b) ")]
        //[TestCase(" [a]( b) ")]

        //[TestCase("[ a]( b)")]
        //[TestCase(" [ a]( b)")]
        //[TestCase("[ a]( b) ")]
        //[TestCase(" [ a]( b) ")]

        //[TestCase("[a ]( b)")]
        //[TestCase(" [a ]( b)")]
        //[TestCase("[a ]( b) ")]
        //[TestCase(" [a ]( b) ")]

        //[TestCase("[ a ]( b)")]
        //[TestCase(" [ a ]( b)")]
        //[TestCase("[ a ]( b) ")]
        //[TestCase(" [ a ]( b) ")]

        //[TestCase("[](b )")]
        //[TestCase(" [](b )")]
        //[TestCase("[](b ) ")]
        //[TestCase(" [](b ) ")]

        //[TestCase("[a](b )")]
        //[TestCase(" [a](b )")]
        //[TestCase("[a](b ) ")]
        //[TestCase(" [a](b ) ")]

        //[TestCase("[ a](b )")]
        //[TestCase(" [ a](b )")]
        //[TestCase("[ a](b ) ")]
        //[TestCase(" [ a](b ) ")]

        //[TestCase("[a ](b )")]
        //[TestCase(" [a ](b )")]
        //[TestCase("[a ](b ) ")]
        //[TestCase(" [a ](b ) ")]

        //[TestCase("[ a ](b )")]
        //[TestCase(" [ a ](b )")]
        //[TestCase("[ a ](b ) ")]
        //[TestCase(" [ a ](b ) ")]

        //[TestCase("[]( b )")]
        //[TestCase(" []( b )")]
        //[TestCase("[]( b ) ")]
        //[TestCase(" []( b ) ")]

        //[TestCase("[a]( b )")]
        //[TestCase(" [a]( b )")]
        //[TestCase("[a]( b ) ")]
        //[TestCase(" [a]( b ) ")]

        //[TestCase("[ a]( b )")]
        //[TestCase(" [ a]( b )")]
        //[TestCase("[ a]( b ) ")]
        //[TestCase(" [ a]( b ) ")]

        //[TestCase("[a ]( b )")]
        //[TestCase(" [a ]( b )")]
        //[TestCase("[a ]( b ) ")]
        //[TestCase(" [a ]( b ) ")]

        //[TestCase("[ a ]( b )")]
        //[TestCase(" [ a ]( b )")]
        //[TestCase("[ a ]( b ) ")]
        //[TestCase(" [ a ]( b ) ")]
        public void TestInlineLink(string value)
        {
            RoundTrip(value);
        }

        [TestCase("![](a)")]
        [TestCase(" ![](a)")]
        [TestCase("![](a) ")]
        [TestCase(" ![](a) ")]
        public void TestImage(string value)
        {
            RoundTrip(value);
        }

        [TestCase("``")]
        [TestCase(" ``")]
        [TestCase("`` ")]
        [TestCase(" `` ")]

        [TestCase("`a`")]
        [TestCase(" `a`")]
        [TestCase("`a` ")]
        [TestCase(" `a` ")]

        [TestCase("` a`")]
        [TestCase(" ` a`")]
        [TestCase("` a` ")]
        [TestCase(" ` a` ")]

        [TestCase("`a `")]
        [TestCase(" `a `")]
        [TestCase("`a ` ")]
        [TestCase(" `a ` ")]

        /// <see cref="CodeInlineParser"/>: intentionally trimmed. TODO: decide on how to handle
        //[TestCase("` a `")]
        //[TestCase(" ` a `")]
        //[TestCase("` a ` ")]
        //[TestCase(" ` a ` ")]
        public void TestCodeInline(string value)
        {
            RoundTrip(value);
        }

        [TestCase("<http://a>")]
        [TestCase(" <http://a>")]
        [TestCase("<http://a> ")]
        [TestCase(" <http://a> ")]

        [TestCase("< http://a>")]
        [TestCase(" < http://a>")]
        [TestCase("< http://a> ")]
        [TestCase(" < http://a> ")]

        [TestCase("<http://a >")]
        [TestCase(" <http://a >")]
        [TestCase("<http://a > ")]
        [TestCase(" <http://a > ")]

        [TestCase("< http://a >")]
        [TestCase(" < http://a >")]
        [TestCase("< http://a > ")]
        [TestCase(" < http://a > ")]
        public void TestAutolinkInline(string value)
        {
            RoundTrip(value);
        }
    }
}
