using Markdig.Extensions.Tables;
using Markdig.Syntax;
using NUnit.Framework;
using System.Linq;

namespace Markdig.Tests
{
    [TestFixture]
    public sealed class TestPipeTable
    {
        [TestCase("| S | T |\r\n|---|---| \r\n| G | H |")]
        [TestCase("| S | T |\r\n|---|---|\t\r\n| G | H |")]
        [TestCase("| S | T |\r\n|---|---|\f\r\n| G | H |")]
        [TestCase("| S | \r\n|---|\r\n| G |\r\n\r\n| D | D |\r\n| ---| ---| \r\n| V | V |", 2)]
        public void TestTableBug(string markdown, int tableCount = 1)
        {
            MarkdownDocument document = Markdown.Parse(markdown, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

            Table[] tables = document.Descendants().OfType<Table>().ToArray();

            Assert.AreEqual(tableCount, tables.Length);
        }
    }
}
