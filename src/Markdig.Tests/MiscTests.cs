using System;
using System.IO;
using System.Text.RegularExpressions;
using Markdig.Extensions.AutoLinks;
using NUnit.Framework;

namespace Markdig.Tests
{
    public class MiscTests
    {
        [Test]
        public void TestChangelogPRLinksMatchDescription()
        {
            string solutionFolder = Path.GetFullPath(Path.Combine(TestParser.TestsDirectory, "../.."));
            string changelogPath = Path.Combine(solutionFolder, "changelog.md");
            string changelog = File.ReadAllText(changelogPath);
            var matches = Regex.Matches(changelog, @"\(\[\(PR #(\d+)\)\]\(.*?pull\/(\d+)\)\)");
            Assert.Greater(matches.Count, 0);
            foreach (Match match in matches)
            {
                Assert.True(int.TryParse(match.Groups[1].Value, out int textNr));
                Assert.True(int.TryParse(match.Groups[2].Value, out int linkNr));
                Assert.AreEqual(textNr, linkNr);
            }
        }

        [Test]
        public void TestFixHang()
        {
            var input = File.ReadAllText(Path.Combine(TestParser.TestsDirectory, "hang.md"));
            _ = Markdown.ToHtml(input);
        }

        [Test]
        public void TestInvalidHtmlEntity()
        {
            var input = "9&ddr;&*&ddr;&de��__";
            TestParser.TestSpec(input, "<p>9&amp;ddr;&amp;*&amp;ddr;&amp;de��__</p>");
        }

        [Test]
        public void TestInvalidCharacterHandling()
        {
            var input = File.ReadAllText(Path.Combine(TestParser.TestsDirectory, "ArgumentOutOfRangeException.md"));
            _ = Markdown.ToHtml(input);
        }

        [Test]
        public void TestInvalidCodeEscape()
        {
            var input = "```**Header**	";
            _ = Markdown.ToHtml(input);
        }

        [Test]
        public void TestEmphasisAndHtmlEntity()
        {
            var markdownText = "*Unlimited-Fun&#174;*&#174;";
            TestParser.TestSpec(markdownText, "<p><em>Unlimited-Fun®</em>®</p>");
        }

        [Test]
        public void TestThematicInsideCodeBlockInsideList()
        {
            var input = @"1. In the :

   ```
   Id                                   DisplayName         Description
   --                                   -----------         -----------
   62375ab9-6b52-47ed-826b-58e47e0e304b Group.Unified       ...
   ```";
            TestParser.TestSpec(input, @"<ol>
<li><p>In the :</p>
<pre><code>Id                                   DisplayName         Description
--                                   -----------         -----------
62375ab9-6b52-47ed-826b-58e47e0e304b Group.Unified       ...
</code></pre></li>
</ol>");
        }

        [Test]
        public void VisualizeMathExpressions()
        {
            string math = @"Math expressions

$\frac{n!}{k!(n-k)!} = \binom{n}{k}$

$$\frac{n!}{k!(n-k)!} = \binom{n}{k}$$

$$
\frac{n!}{k!(n-k)!} = \binom{n}{k}
$$

<div class=""math"">
\begin{align}
\sqrt{37} & = \sqrt{\frac{73^2-1}{12^2}} \\
 & = \sqrt{\frac{73^2}{12^2}\cdot\frac{73^2-1}{73^2}} \\ 
 & = \sqrt{\frac{73^2}{12^2}}\sqrt{\frac{73^2-1}{73^2}} \\
 & = \frac{73}{12}\sqrt{1 - \frac{1}{73^2}} \\ 
 & \approx \frac{73}{12}\left(1 - \frac{1}{2\cdot73^2}\right)
\end{align}
</div>
";
            Console.WriteLine("Math Expressions:\n");

            var pl = new MarkdownPipelineBuilder().UseMathematics().Build(); // UseEmphasisExtras(EmphasisExtraOptions.Subscript).Build()


            var html = Markdown.ToHtml(math, pl);
            Console.WriteLine(html);
        }

        [Test]
        public void InlineMathExpression()
        {
            string math = @"Math expressions

$\frac{n!}{k!(n-k)!} = \binom{n}{k}$
";
            var pl = new MarkdownPipelineBuilder().UseMathematics().Build(); // UseEmphasisExtras(EmphasisExtraOptions.Subscript).Build()

            var html = Markdown.ToHtml(math, pl);
            Console.WriteLine(html);

            Assert.IsTrue(html.Contains("<p><span class=\"math\">\\("), "Leading bracket missing");
            Assert.IsTrue(html.Contains("\\)</span></p>"), "Trailing bracket missing");
        }

        [Test]
        public void BlockMathExpression()
        {
            string math = @"Math expressions

$$
\frac{n!}{k!(n-k)!} = \binom{n}{k}
$$
";
            var pl = new MarkdownPipelineBuilder().UseMathematics().Build(); // UseEmphasisExtras(EmphasisExtraOptions.Subscript).Build()

            var html = Markdown.ToHtml(math, pl);
            Console.WriteLine(html);

            Assert.IsTrue(html.Contains("<div class=\"math\">\n\\["), "Leading bracket missing");
            Assert.IsTrue(html.Contains("\\]</div>"), "Trailing bracket missing");
        }

        [Test]
        public void CanDisableParsingHeadings()
        {
            var noHeadingsPipeline = new MarkdownPipelineBuilder().DisableHeadings().Build();

            TestParser.TestSpec("Foo\n===", "<h1>Foo</h1>");
            TestParser.TestSpec("Foo\n===", "<p>Foo\n===</p>", noHeadingsPipeline);

            TestParser.TestSpec("# Heading 1", "<h1>Heading 1</h1>");
            TestParser.TestSpec("# Heading 1", "<p># Heading 1</p>", noHeadingsPipeline);

            // Does not also disable link reference definitions
            TestParser.TestSpec("[Foo]\n\n[Foo]: bar", "<p><a href=\"bar\">Foo</a></p>");
            TestParser.TestSpec("[Foo]\n\n[Foo]: bar", "<p><a href=\"bar\">Foo</a></p>", noHeadingsPipeline);
        }

        [Test]
        public void CanOpenAutoLinksInNewWindow()
        {
            var pipeline = new MarkdownPipelineBuilder().UseAutoLinks().Build();
            var newWindowPipeline = new MarkdownPipelineBuilder().UseAutoLinks(new AutoLinkOptions() { OpenInNewWindow = true }).Build();

            TestParser.TestSpec("www.foo.bar", "<p><a href=\"http://www.foo.bar\">www.foo.bar</a></p>", pipeline);
            TestParser.TestSpec("www.foo.bar", "<p><a href=\"http://www.foo.bar\" target=\"blank\">www.foo.bar</a></p>", newWindowPipeline);
        }

        [Test]
        public void CanUseHttpsPrefixForWWWAutoLinks()
        {
            var pipeline = new MarkdownPipelineBuilder().UseAutoLinks().Build();
            var httpsPipeline = new MarkdownPipelineBuilder().UseAutoLinks(new AutoLinkOptions() { UseHttpsForWWWLinks = true }).Build();

            TestParser.TestSpec("www.foo.bar", "<p><a href=\"http://www.foo.bar\">www.foo.bar</a></p>", pipeline);
            TestParser.TestSpec("www.foo.bar", "<p><a href=\"https://www.foo.bar\">www.foo.bar</a></p>", httpsPipeline);
        }
    }
}
