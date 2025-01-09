using System.Text.RegularExpressions;

using Markdig.Extensions.AutoLinks;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using NUnit.Framework;

namespace Markdig.Tests;

public class MiscTests
{
    [Test]
    public void LinkWithInvalidNonAsciiDomainNameIsIgnored()
    {
        // Url from https://github.com/lunet-io/markdig/issues/438
        _ = Markdown.ToHtml("[minulém díle](http://V%20minulém%20díle%20jsme%20nainstalovali%20SQL%20Server,%20který%20je%20nutný%20pro%20běh%20Configuration%20Manageru.%20Dnes%20nás%20čeká%20instalace%20WSUS,%20což%20je%20produkt,%20jež%20je%20možné%20používat%20i%20jako%20samostatnou%20funkci%20ve%20Windows%20Serveru,%20který%20se%20stará%20o%20stažení%20a%20instalaci%20aktualizací%20z%20Microsoft%20Update%20na%20klientské%20počítače.%20Stejně%20jako%20v%20předchozích%20dílech,%20tak%20i%20v%20tomto%20si%20ukážeme%20obě%20varianty%20instalace%20–%20a%20to%20jak%20instalaci%20z%20PowerShellu,%20tak%20instalaci%20pomocí%20GUI.) ");

        // Valid IDN
        TestParser.TestSpec("[foo](http://ünicode.com)", "<p><a href=\"http://xn--nicode-2ya.com\">foo</a></p>");
        TestParser.TestSpec("[foo](http://ünicode.ünicode.com)", "<p><a href=\"http://xn--nicode-2ya.xn--nicode-2ya.com\">foo</a></p>");

        // Invalid IDN
        TestParser.TestSpec("[foo](http://ünicode..com)", "<p><a href=\"http://%C3%BCnicode..com\">foo</a></p>");
    }

    [TestCase("link [foo [bar]]")] // https://spec.commonmark.org/0.29/#example-508
    [TestCase("link [foo][bar]")]
    [TestCase("link [][foo][bar][]")]
    [TestCase("link [][foo][bar][[]]")]
    [TestCase("link [foo] [bar]")]
    [TestCase("link [[foo] [] [bar] [[abc]def]]")]
    [TestCase("[]")]
    [TestCase("[ ]")]
    [TestCase("[bar][]")]
    [TestCase("[bar][ foo]")]
    [TestCase("[bar][foo ][]")]
    [TestCase("[bar][fo[ ]o ][][]")]
    [TestCase("[a]b[c[d[e]f]g]h")]
    [TestCase("a[b[c[d]e]f[g]h]i foo [j]k[l[m]n]o")]
    [TestCase("a[b[c[d]e]f[g]h]i[] [][foo][bar][] foo [j]k[l[m]n]o")]
    [TestCase("a[b[c[d]e]f[g]h]i foo [j]k[l[m]n]o[][]")]
    public void LinkTextMayContainBalancedBrackets(string linkText)
    {
        string markdown = $"[{linkText}](/uri)";
        string expected = $@"<p><a href=""/uri"">{linkText}</a></p>";

        TestParser.TestSpec(markdown, expected);

        // Make the link text unbalanced
        foreach (var bracketIndex in linkText
            .Select((c, i) => new Tuple<char, int>(c, i))
            .Where(t => t.Item1 == '[' || t.Item1 == ']')
            .Select(t => t.Item2))
        {
            string brokenLinkText = linkText.Remove(bracketIndex, 1);

            markdown = $"[{brokenLinkText}](/uri)";
            expected = $@"<p><a href=""/uri"">{brokenLinkText}</a></p>";

            string actual = Markdown.ToHtml(markdown);
            Assert.AreNotEqual(expected, actual);
        }
    }

    [Theory]
    [TestCase('[', 9 * 1024, true, false)]
    [TestCase('[', 11 * 1024, true, true)]
    [TestCase('[', 100, false, false)]
    [TestCase('[', 150, false, true)]
    [TestCase('>', 100, true, false)]
    [TestCase('>', 150, true, true)]
    public void GuardsAgainstHighlyNestedNodes(char c, int count, bool parseOnly, bool shouldThrow)
    {
        var markdown = new string(c, count);
        TestDelegate test = parseOnly ? () => Markdown.Parse(markdown) : () => Markdown.ToHtml(markdown);

        if (shouldThrow)
        {
            Exception e = Assert.Throws<ArgumentException>(test);
            Assert.True(e.Message.Contains("depth limit"));
        }
        else
        {
            test();
        }
    }

    [Test]
    public void IsIssue356Corrected()
    {
        string input = @"https://foo.bar/path/\#m4mv5W0GYKZpGvfA.97";
        string expected = @"<p><a href=""https://foo.bar/path/%5C#m4mv5W0GYKZpGvfA.97"">https://foo.bar/path/\#m4mv5W0GYKZpGvfA.97</a></p>";

        TestParser.TestSpec($"<{input}>", expected);
        TestParser.TestSpec(input, expected, "autolinks|advanced");
    }

    [Test]
    public void IsIssue365Corrected()
    {
        // The scheme must be escaped too...
        string input = "![image](\"onclick=\"alert&amp;#40;'click'&amp;#41;\"://)";
        string expected = "<p><img src=\"%22onclick=%22alert&amp;#40;%27click%27&amp;#41;%22://\" alt=\"image\" /></p>";

        TestParser.TestSpec(input, expected);
    }

    [Test]
    public void TestAltTextIsCorrectlyEscaped()
    {
        TestParser.TestSpec(
            @"![This is image alt text with quotation ' and double quotation ""hello"" world](girl.png)",
            @"<p><img src=""girl.png"" alt=""This is image alt text with quotation ' and double quotation &quot;hello&quot; world"" /></p>");
    }

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
        //Console.WriteLine("Math Expressions:\n");
        var pl = new MarkdownPipelineBuilder().UseMathematics().Build(); // UseEmphasisExtras(EmphasisExtraOptions.Subscript).Build()
        var html = Markdown.ToHtml(math, pl);
        //Console.WriteLine(html);
    }

    [Test]
    public void InlineMathExpression()
    {
        string math = @"Math expressions

$\frac{n!}{k!(n-k)!} = \binom{n}{k}$
";
        var pl = new MarkdownPipelineBuilder().UseMathematics().Build(); // UseEmphasisExtras(EmphasisExtraOptions.Subscript).Build()

        var html = Markdown.ToHtml(math, pl);

        var test1 = html.Contains("<p><span class=\"math\">\\(");
        var test2 = html.Contains("\\)</span></p>");
        if (!test1 || !test2)
        {
            Console.WriteLine(html);
        }

        Assert.IsTrue(test1, "Leading bracket missing");
        Assert.IsTrue(test2, "Trailing bracket missing");
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
        var test1 = html.Contains("<div class=\"math\">\n\\[");
        var test2 = html.Contains("\\]</div>");
        if (!test1 || !test2)
        {
            Console.WriteLine(html);
        }

        Assert.IsTrue(test1, "Leading bracket missing");
        Assert.IsTrue(test2, "Trailing bracket missing");
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
        TestParser.TestSpec("www.foo.bar", "<p><a href=\"http://www.foo.bar\" target=\"_blank\">www.foo.bar</a></p>", newWindowPipeline);
    }

    [Test]
    public void CanUseHttpsPrefixForWWWAutoLinks()
    {
        var pipeline = new MarkdownPipelineBuilder().UseAutoLinks().Build();
        var httpsPipeline = new MarkdownPipelineBuilder().UseAutoLinks(new AutoLinkOptions() { UseHttpsForWWWLinks = true }).Build();

        TestParser.TestSpec("www.foo.bar", "<p><a href=\"http://www.foo.bar\">www.foo.bar</a></p>", pipeline);
        TestParser.TestSpec("www.foo.bar", "<p><a href=\"https://www.foo.bar\">www.foo.bar</a></p>", httpsPipeline);
    }

    [Test]
    public void RootInlineHasCorrectSourceSpan()
    {
        var pipeline = new MarkdownPipelineBuilder().UsePreciseSourceLocation().Build();
        pipeline.TrackTrivia = true;

        var document = Markdown.Parse("0123456789\n", pipeline);

        var expectedSourceSpan = new SourceSpan(0, 10);
        Assert.That(((LeafBlock)document.LastChild).Inline.Span == expectedSourceSpan);
    }

    [Test]
    public void RootInlineInTableCellHasCorrectSourceSpan()
    {
        var pipeline = new MarkdownPipelineBuilder().UsePreciseSourceLocation().UseAdvancedExtensions().Build();
        pipeline.TrackTrivia = true;

        var document = Markdown.Parse("| a | b |\n| --- | --- |\n| <span id=\"dest\"></span><span id=\"DEST\"></span>*dest*<br/> | \\[in\\] The address of the result of the operation.<br/> |", pipeline);

        var paragraph = (ParagraphBlock)((TableCell)((TableRow)((Table)document.LastChild).LastChild).First()).LastChild;
        Assert.That(paragraph.Inline.Span.Start == paragraph.Inline.FirstChild.Span.Start);
        Assert.That(paragraph.Inline.Span.End == paragraph.Inline.LastChild.Span.End);
    }

    [Test]
    public void TestGridTableShortLine()
    {
        var input = @"
+--+
|  |
+-";

        var expected = @"<table>
<col style=""width:100%"" />
<tbody>
<tr>
<td></td>
</tr>
</tbody>
</table>
";
        TestParser.TestSpec(input, expected, new MarkdownPipelineBuilder().UseGridTables().Build());
    }
  
    [Test]
    public void TestDefinitionListInListItemWithBlankLine()
    {
        var input = @"
- 

  term
  :   definition
";

        var expected = @"<ul>
<li>
<dl>
<dt>term</dt>
<dd>definition</dd>
</dl>
</li>
</ul>
";
        TestParser.TestSpec(input, expected, new MarkdownPipelineBuilder().UseDefinitionLists().Build());
    }

    [Test]
    public void TestAlertWithinAlertOrNestedBlock()
    {
        var input = @"
>[!NOTE]
[!NOTE]
The second one is not a note.

>>[!NOTE]
Also not a note.
";

        var expected = @"<div class=""markdown-alert markdown-alert-note"">
<p class=""markdown-alert-title""><svg viewBox=""0 0 16 16"" version=""1.1"" width=""16"" height=""16"" aria-hidden=""true""><path d=""M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8Zm8-6.5a6.5 6.5 0 1 0 0 13 6.5 6.5 0 0 0 0-13ZM6.5 7.75A.75.75 0 0 1 7.25 7h1a.75.75 0 0 1 .75.75v2.75h.25a.75.75 0 0 1 0 1.5h-2a.75.75 0 0 1 0-1.5h.25v-2h-.25a.75.75 0 0 1-.75-.75ZM8 6a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z""></path></svg>Note</p>
<p>[!NOTE]
The second one is not a note.</p>
</div>
<blockquote>
<blockquote>
<p>[!NOTE]
Also not a note.</p>
</blockquote>
</blockquote>
";
        TestParser.TestSpec(input, expected, new MarkdownPipelineBuilder().UseAlertBlocks().Build());
    }
}
