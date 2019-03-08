// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Markdig.Extensions.JiraLinks;
using NUnit.Framework;

namespace Markdig.Tests
{
    public class TestParser
    {
        [Test]
        public void TestFixHang()
        {
            var input = File.ReadAllText(Path.Combine(Path.GetDirectoryName(typeof(TestParser).Assembly.Location), "hang.md"));
            var html = Markdown.ToHtml(input);
        }

        [Test]
        public void TestInvalidHtmlEntity()
        {
            var input = "9&ddr;&*&ddr;&de��__";
            TestSpec(input, "<p>9&amp;ddr;&amp;*&amp;ddr;&amp;de��__</p>");
        }

        [Test]
        public void TestInvalidCharacterHandling()
        {
            var input = File.ReadAllText(Path.Combine(Path.GetDirectoryName(typeof(TestParser).Assembly.Location), "ArgumentOutOfRangeException.md"));
            var html = Markdown.ToHtml(input);
        }

        [Test]
        public void TestInvalidCodeEscape()
        {
            var input = "```**Header**	";
            var html = Markdown.ToHtml(input);
        }

        [Test]
        public void TestEmphasisAndHtmlEntity()
        {
            var markdownText = "*Unlimited-Fun&#174;*&#174;";
            TestSpec(markdownText, "<p><em>Unlimited-Fun®</em>®</p>");
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
            TestSpec(input, @"<ol>
<li><p>In the :</p>
<pre><code>Id                                   DisplayName         Description
--                                   -----------         -----------
62375ab9-6b52-47ed-826b-58e47e0e304b Group.Unified       ...
</code></pre></li>
</ol>");
        }

        [Test]
        public void EnsureSpecsAreUpToDate()
        {
            // In CI, SpecFileGen is guaranteed to run
            if (IsContinuousIntegration)
                return;

            foreach (var specFilePath in SpecsFilePaths)
            {
                string testFilePath = Path.ChangeExtension(specFilePath, ".cs");

                Assert.True(File.Exists(testFilePath),
                    "A new specification file has been added. Add the spec to the list in SpecFileGen and regenerate the tests.");

                DateTime specTime = File.GetLastWriteTimeUtc(specFilePath);
                DateTime testTime = File.GetLastWriteTimeUtc(testFilePath);

                // If file creation times aren't preserved by git, add some leeway
                // If specs have come from git, assume that they were regenerated since CI would fail otherwise
                testTime = testTime.AddSeconds(2);

                // This might not catch a changed spec every time, but should most of the time. Otherwise CI will catch it

                // This could also trigger, if a user has modified the spec file but reverted the change - can't think of a good workaround
                Assert.Less(specTime, testTime,
                    $"{Path.GetFileName(specFilePath)} has been modified. Run SpecFileGen to regenerate the tests. " +
                    "If you have modified a specification file, but reverted all changes, ignore this error or revert the 'changed' timestamp metadata on the file.");
            }
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


        public static void TestSpec(string inputText, string expectedOutputText, string extensions = null, bool plainText = false)
        {
            foreach (var pipeline in GetPipeline(extensions))
            {
                Console.WriteLine($"Pipeline configured with extensions: {pipeline.Key}");
                TestSpec(inputText, expectedOutputText, pipeline.Value, plainText);
            }
        }

        public static void TestSpec(string inputText, string expectedOutputText, MarkdownPipeline pipeline, bool plainText = false)
        {
            // Uncomment this line to get more debug information for process inlines.
            //pipeline.DebugLog = Console.Out;
            var result = plainText ? Markdown.ToPlainText(inputText, pipeline) : Markdown.ToHtml(inputText, pipeline);

            result = Compact(result);
            expectedOutputText = Compact(expectedOutputText);

            Console.WriteLine("```````````````````Source");
            Console.WriteLine(DisplaySpaceAndTabs(inputText));
            Console.WriteLine("```````````````````Result");
            Console.WriteLine(DisplaySpaceAndTabs(result));
            Console.WriteLine("```````````````````Expected");
            Console.WriteLine(DisplaySpaceAndTabs(expectedOutputText));
            Console.WriteLine("```````````````````");
            Console.WriteLine();
            TextAssert.AreEqual(expectedOutputText, result);
        }

        private static IEnumerable<KeyValuePair<string, MarkdownPipeline>> GetPipeline(string extensionsGroupText)
        {
            // For the standard case, we make sure that both the CommmonMark core and Extra/Advanced are CommonMark compliant!
            if (string.IsNullOrEmpty(extensionsGroupText))
            {
                yield return new KeyValuePair<string, MarkdownPipeline>("default", new MarkdownPipelineBuilder().Build());

                yield return new KeyValuePair<string, MarkdownPipeline>("advanced", new MarkdownPipelineBuilder()  // Use similar to advanced extension without auto-identifier
                 .UseAbbreviations()
                //.UseAutoIdentifiers()
                .UseCitations()
                .UseCustomContainers()
                .UseDefinitionLists()
                .UseEmphasisExtras()
                .UseFigures()
                .UseFooters()
                .UseFootnotes()
                .UseGridTables()
                .UseMathematics()
                .UseMediaLinks()
                .UsePipeTables()
                .UseListExtras()
                .UseGenericAttributes().Build());

                yield break;
            }

            var extensionGroups = extensionsGroupText.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var extensionsText in extensionGroups)
            {
                var builder = new MarkdownPipelineBuilder();
                builder.DebugLog = Console.Out;
                if (extensionsText == "jiralinks")
                {
                    builder.UseJiraLinks(new JiraLinkOptions("http://your.company.abc"));
                }
                else
                {
                    builder = extensionsText == "self" ? builder.UseSelfPipeline() : builder.Configure(extensionsText);
                }
                yield return new KeyValuePair<string, MarkdownPipeline>(extensionsText, builder.Build());
            }
        }

        public static string DisplaySpaceAndTabs(string text)
        {
            // Output special characters to check correctly the results
            return text.Replace('\t', '→').Replace(' ', '·');
        }

        private static string Compact(string html)
        {
            // Normalize the output to make it compatible with CommonMark specs
            html = html.Replace("\r\n", "\n").Replace(@"\r", @"\n").Trim();
            html = Regex.Replace(html, @"\s+</li>", "</li>");
            html = Regex.Replace(html, @"<li>\s+", "<li>");
            html = html.Normalize(NormalizationForm.FormKD);
            return html;
        }

        public static readonly bool IsContinuousIntegration = Environment.GetEnvironmentVariable("CI") != null;

        /// <summary>
        /// Contains absolute paths to specification markdown files (order is the same as in <see cref="SpecsMarkdown"/>)
        /// </summary>
        public static readonly string[] SpecsFilePaths;
        /// <summary>
        /// Contains the markdown source for specification files (order is the same as in <see cref="SpecsFilePaths"/>)
        /// </summary>
        public static readonly string[] SpecsMarkdown;
        static TestParser()
        {
            string assemblyDir = Path.GetDirectoryName(typeof(TestParser).Assembly.Location);
            string specsDir = Path.GetFullPath(Path.Combine(assemblyDir, "../../Specs"));

            SpecsFilePaths = Directory.GetFiles(specsDir)
                .Where(file => file.EndsWith(".md", StringComparison.Ordinal) && !file.Contains("readme"))
                .ToArray();

            SpecsMarkdown = new string[SpecsFilePaths.Length];

            for (int i = 0; i < SpecsFilePaths.Length; i++)
            {
                SpecsMarkdown[i] = File.ReadAllText(SpecsFilePaths[i]);
            }
        }
    }
}