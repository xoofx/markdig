// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.
using System.Text;
using System.Text.RegularExpressions;

using Markdig.Extensions.JiraLinks;
using Markdig.Renderers.Roundtrip;
using Markdig.Syntax;

namespace Markdig.Tests;

public class TestParser
{
    [Test]
    public void EnsureSpecsAreUpToDate()
    {
        // In CI, SpecFileGen is guaranteed to run
        if (IsContinuousIntegration)
            return;

        var specsFilePaths = Directory.GetDirectories(TestsDirectory)
            .Where(dir => dir.EndsWith("Specs"))
            .SelectMany(dir => Directory.GetFiles(dir)
                .Where(file => file.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                .Where(file => file.IndexOf("readme", StringComparison.OrdinalIgnoreCase) == -1))
            .ToArray();

        var specsMarkdown = new string[specsFilePaths.Length];
        var specsSyntaxTrees = new MarkdownDocument[specsFilePaths.Length];

        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        for (int i = 0; i < specsFilePaths.Length; i++)
        {
            string markdown = specsMarkdown[i] = File.ReadAllText(specsFilePaths[i]);
            specsSyntaxTrees[i] = Markdown.Parse(markdown, pipeline);
        }

        foreach (var specFilePath in specsFilePaths)
        {
            string testFilePath = Path.ChangeExtension(specFilePath, ".generated.cs");

            Assert.True(File.Exists(testFilePath),
                "A new specification file has been added. Add the spec to the list in SpecFileGen and regenerate the tests.");

            DateTime specTime = File.GetLastWriteTimeUtc(specFilePath);
            DateTime testTime = File.GetLastWriteTimeUtc(testFilePath);

            // If file creation times aren't preserved by git, add some leeway
            // If specs have come from git, assume that they were regenerated since CI would fail otherwise
            testTime = testTime.AddMinutes(3);

            // This might not catch a changed spec every time, but should at least sometimes. Otherwise CI will catch it

            // This could also trigger, if a user has modified the spec file but reverted the change - can't think of a good workaround
            Assert.Less(specTime, testTime,
                $"{Path.GetFileName(specFilePath)} has been modified. Run SpecFileGen to regenerate the tests. " +
                "If you have modified a specification file, but reverted all changes, ignore this error or revert the 'changed' timestamp metadata on the file.");
        }

        TestDescendantsOrder.TestSchemas(specsSyntaxTrees);
    }

    [Test]
    public void ParseEmptyDocumentWithTrackTriviaEnabled()
    {
        var document = Markdown.Parse("", trackTrivia: true);
        using var sw = new StringWriter();
        new RoundtripRenderer(sw).Render(document);
        Assert.AreEqual("", sw.ToString());
    }

    public static void TestSpec(string inputText, string expectedOutputText, string extensions = null, bool plainText = false, string context = null)
    {
        context ??= string.Empty;
        if (!string.IsNullOrEmpty(context))
        {
            context += "\n";
        }
        foreach (var pipeline in GetPipeline(extensions))
        {
            TestSpec(inputText, expectedOutputText, pipeline.Value, plainText, context: context + $"Pipeline configured with extensions: {pipeline.Key}");
        }
    }

    public static void TestSpec(string inputText, string expectedOutputText, MarkdownPipeline pipeline, bool plainText = false, string context = null)
    {
        // Uncomment this line to get more debug information for process inlines.
        //pipeline.DebugLog = Console.Out;
        var result = plainText ? Markdown.ToPlainText(inputText, pipeline) : Markdown.ToHtml(inputText, pipeline);

        result = Compact(result);
        expectedOutputText = Compact(expectedOutputText);

        PrintAssertExpected(inputText, result, expectedOutputText, context);
    }

    public static void PrintAssertExpected(string source, string result, string expected, string context = null)
    {
        if (expected != result)
        {
            if (context != null)
            {
                Console.WriteLine(context);
            }
            Console.WriteLine("```````````````````Source");
            Console.WriteLine(DisplaySpaceAndTabs(source));
            Console.WriteLine("```````````````````Result");
            Console.WriteLine(DisplaySpaceAndTabs(result));
            Console.WriteLine("```````````````````Expected");
            Console.WriteLine(DisplaySpaceAndTabs(expected));
            Console.WriteLine("```````````````````");
            Console.WriteLine();
            TextAssert.AreEqual(expected, result);
        }
    }

    public static IEnumerable<KeyValuePair<string, MarkdownPipeline>> GetPipeline(string extensionsGroupText)
    {
        // For the standard case, we make sure that both the CommmonMark core and Extra/Advanced are CommonMark compliant!
        if (string.IsNullOrEmpty(extensionsGroupText))
        {
            yield return new KeyValuePair<string, MarkdownPipeline>("default", new MarkdownPipelineBuilder().Build());

            //yield return new KeyValuePair<string, MarkdownPipeline>("default-trivia", new MarkdownPipelineBuilder().EnableTrackTrivia().Build());

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

    public static readonly string TestsDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(typeof(TestParser).Assembly.Location), "../../.."));

    static TestParser()
    {
        const string RunningInsideVisualStudioPath = "\\src\\.vs\\markdig\\";
        int index = TestsDirectory.IndexOf(RunningInsideVisualStudioPath);
        if (index != -1)
        {
            TestsDirectory = TestsDirectory.Substring(0, index) + "\\src\\Markdig.Tests";
        }
    }
}