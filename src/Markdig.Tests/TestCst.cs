using Markdig.Renderers;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using NUnit.Framework;
using System.IO;

/// <summary>
/// General notes
/// - whitespace can occur before, between and after symbols
/// </summary>
/// TODO:
/// - \r\n, \r, \n
/// - \t and spaces
/// - html entities i.e. &gt;
/// 
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
            //var nr = new NormalizeRenderer(sw);
            var hr = new HtmlRenderer(sw);

            hr.Write(markdownDocument);

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

        // special cases
        [TestCase(" p \n\n\n\n p \n\n")]
        [TestCase("\np")]
        [TestCase("\n\np")]
        [TestCase("p\n")]
        [TestCase("p\n\n")]
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
        [TestCase("- i1\n")]
        [TestCase("- i1\n- i2")]
        [TestCase("- i1\n    - i2")]
        [TestCase("- i1\n    - i1.1\n    - i1.2")]
        [TestCase("- i1 \n- i2 \n")]
        [TestCase("- i1  \n- i2  \n")]
        [TestCase(" - i1")]
        [TestCase("  - i1")]
        [TestCase("   - i1")]
        [TestCase("\t- i1")]
        public void TestUnorderedList(string value)
        {
            RoundTrip(value);
        }

        [TestCase("-     i1\n\np\n")] // TODO: listblock should render newline, apparently last paragraph of last listitem dont have newline
        [TestCase("-     i1\n\n\np\n")]
        public void TestUnorderedList_Paragraph(string value)
        {
            RoundTrip(value);
        }

        [TestCase("1. i")]
        [TestCase("1.  i")]
        [TestCase("1. i ")]
        [TestCase("1.  i ")]
        public void TestOrderedList(string value)
        {
            RoundTrip(value);
        }

        [TestCase("   ![description](http://example.com)")]
        [TestCase("paragraph   ![description](http://example.com)")]
        public void TestImage2(string value)
        {
            RoundTrip(value);
        }

        [TestCase("# h")]
        [TestCase("# h ")]
        public void TestHeading(string value)
        {
            RoundTrip(value);
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
        public void TestUnorderedListItem_BlockQuote(string value)
        {
            RoundTrip(value);
        }

        [TestCase(">quote")]
        [TestCase("> quote")]
        [TestCase(">  quote")]
        [TestCase("   >  quote")]
        [TestCase(">q\n>q")]
        [TestCase(">q\n>q\n>q")]
        [TestCase(">q\n>\n>q")]
        [TestCase(">q\n>\n>\n>q")]
        [TestCase(">q\n>\n>\n>\n>q")]
        [TestCase(">q\n>\n>q\n>\n>q")]
        [TestCase(">**q**\n>p\n")]
        public void TestBlockQuote(string value)
        {
            RoundTrip(value);
        }

        //[TestCase("---")]
        [TestCase(" ---")]
        [TestCase("  ---")]
        [TestCase("   ---")]
        //[TestCase("--- ")]
        [TestCase(" --- ")]
        [TestCase("  --- ")]
        [TestCase("   --- ")]
        [TestCase("---\np")]
        [TestCase("---\n\np")]
        [TestCase("---\n# h")]
        //[TestCase("p\n\n---")]
        /// Note: "p\n---" is parsed as setext heading
        public void TestThematicBreak(string value)
        {
            RoundTrip(value);
        }

        [TestCase("\n> q")]
        [TestCase("\n> q\n")]
        [TestCase("\n> q\n\n")]
        [TestCase("> q\n\np")]
        [TestCase("p\n\n> q\n\n# h")]
        [TestCase(">**b**\n>\n>p\n>\np\n")]
        public void TestBlockQuote_Paragraph(string value)
        {
            RoundTrip(value);
        }

        [TestCase("> q\n\n# h\n")]
        public void TestBlockQuote_Header(string value)
        {
            RoundTrip(value);
        }

        [TestCase(">- i1\n>- i2\n")]
        public void TestBlockQuote_ListBlock(string value)
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

        [TestCase("[a]")] // TODO: this is not a link but a paragraph
        [TestCase("[a]()")]

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

        [TestCase("``a``")]
        [TestCase("- ```a```")]
        [TestCase("p ```a``` p")]

        // broken
        //[TestCase("```a```")]
        [TestCase("```a``` p")]
        [TestCase("```a`b`c```")]
        //[TestCase("p\n\n```a``` p")]
        //[TestCase("```a``` p\n```a``` p")]

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

        [TestCase("```\nc\n```")]
        [TestCase("\n```\nc\n```")]
        [TestCase("\n\n```\nc\n```")]
        [TestCase("```\nc\n```\n")]
        [TestCase("```\nc\n```\n\n")]
        [TestCase("\n```\nc\n```\n")]
        [TestCase("\n```\nc\n```\n\n")]
        [TestCase("\n\n```\nc\n```\n")]
        [TestCase("\n\n```\nc\n```\n\n")]

        [TestCase("```\nc\n````")]
        public void TestCodeBlock(string value)
        {
            RoundTrip(value);
        }

        [TestCase("p\n\n<div></div>\n")]
        [TestCase("<div></div>\n\n# h")]
        public void TestHtml(string value)
        {
            RoundTrip(value);
        }
    }
}
