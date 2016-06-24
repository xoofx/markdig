// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Markdig.Tests
{
    public class TestParser
    {
        public static void TestSpec(string inputText, string expectedOutputText, string extensions = null)
        {
            foreach (var pipeline in GetPipeline(extensions))
            {
                Console.WriteLine($"Pipeline configured with extensions: {pipeline.Key}");
                // Uncomment this line to get more debug information for process inlines.
                //pipeline.DebugLog = Console.Out;
                var result = Markdown.ToHtml(inputText, pipeline.Value);

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
                var pipeline = extensionsText == "self" ? builder.UseSelfPipeline() : builder.Configure(extensionsText);
                yield return new KeyValuePair<string, MarkdownPipeline>(extensionsText, pipeline.Build());
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
    }
}