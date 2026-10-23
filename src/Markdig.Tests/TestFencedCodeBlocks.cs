using Markdig.Syntax;

namespace Markdig.Tests;

public class TestFencedCodeBlocks
{
    [Test]
    [TestCase("c#", "c#", "")]
    [TestCase("C#", "C#", "")]
    [TestCase(" c#", "c#", "")]
    [TestCase(" c# ", "c#", "")]
    [TestCase(" \tc# ", "c#", "")]
    [TestCase("\t c# \t", "c#", "")]
    [TestCase(" c# ", "c#", "")]
    [TestCase(" c# foo", "c#", "foo")]
    [TestCase(" c# \t  fOo \t", "c#", "fOo")]
    [TestCase("in\\%fo arg\\%ument", "in%fo", "arg%ument")]
    [TestCase("info&#9; arg&acute;ument", "info\t", "arg\u00B4ument")]
    [TestCase("\u00A0c#\u00A0", "\u00A0c#\u00A0", "")]
    [TestCase(" c# \u00A0foo\u00A0 ", "c#", "\u00A0foo\u00A0")]
    [TestCase("\\\\*", "\\*", "")]
    [TestCase("a\\\\_b c\\\\\\*", "a\\_b", "c\\*")]
    [TestCase("\\&amp;", "&amp;", "")]
    [TestCase("\\\\&amp;", "\\&", "")]
    public void TestInfoAndArguments(string infoString, string expectedInfo, string expectedArguments)
    {
        Test('`');
        Test('~');

        void Test(char fencedChar)
        {
            const string Contents = "Foo\nBar\n";

            var fence = new string(fencedChar, 3);
            string markdownText = $"{fence}{infoString}\n{Contents}\n{fence}\n";

            MarkdownDocument document = Markdown.Parse(markdownText);

            FencedCodeBlock codeBlock = document.Descendants<FencedCodeBlock>().Single();

            Assert.AreEqual(fencedChar, codeBlock.FencedChar);
            Assert.AreEqual(3, codeBlock.OpeningFencedCharCount);
            Assert.AreEqual(3, codeBlock.ClosingFencedCharCount);
            Assert.AreEqual(expectedInfo, codeBlock.Info);
            Assert.AreEqual(expectedArguments, codeBlock.Arguments);
            Assert.AreEqual(Contents, codeBlock.Lines.ToString());
        }
    }
}
