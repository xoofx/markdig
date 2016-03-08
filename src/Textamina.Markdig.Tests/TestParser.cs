// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Textamina.Markdig.Tests
{
    public class TestParser
    {
        public static void TestSpec(string inputText, string expectedOutputText, string extensions = null)
        {
            foreach (var pipeline in GetPipeline(extensions))
            {
                Console.WriteLine($"Pipeline configured with extensions: {extensions}");
                // Uncomment this line to get more debug information for process inlines.
                //pipeline.DebugLog = Console.Out;
                var result = Markdown.ToHtml(inputText, pipeline);

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

        private static IEnumerable<MarkdownPipeline> GetPipeline(string extensionsGroupText)
        {
            if (string.IsNullOrEmpty(extensionsGroupText))
            {
                yield return new MarkdownPipeline();
                yield break;
            }

            var extensionGroups = extensionsGroupText.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var extensionsText in extensionGroups)
            {
                var pipeline = new MarkdownPipeline();
                foreach (var extension in extensionsText.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    switch (extension.ToLowerInvariant())
                    {
                        case "pipetables":
                            pipeline.UsePipeTable();
                            break;
                        case "extra_emphasis":
                            pipeline.UseStrikeoutSuperAndSubScript();
                            break;
                        case "hardlinebreak":
                            pipeline.UseSoftlineBreakAsHardlineBreak();
                            break;
                        case "footnotes":
                            pipeline.UseFootnotes();
                            break;
                        case "attributes":
                            pipeline.UseAttributes();
                            break;
                        case "gridtables":
                            pipeline.UseGridTable();
                            break;
                        case "abbreviations":
                            pipeline.UseAbbreviation();
                            break;
                        case "emojis":
                            pipeline.UseEmojiAndSmiley();
                            break;
                        case "definitionlists":
                            pipeline.UseDefinitionList();
                            break;
                        case "customcontainers":
                            pipeline.UseCustomContainer();
                            break;
                        default:
                            Console.WriteLine($"Unsupported extension: {extension}");
                            break;
                    }
                }

                yield return pipeline;
            }
        }

        private static string DisplaySpaceAndTabs(string text)
        {
            // Output special characters to check correctly the results
            return text.Replace('\t', '→').Replace(' ', '·');
        }

        private static string Compact(string html)
        {
            // Normalize the output to make it compatible with CommonMark specs
            html = html.Replace("\r", "").Trim();
            html = Regex.Replace(html, @"\s+</li>", "</li>");
            html = Regex.Replace(html, @"<li>\s+", "<li>");
            html = html.Normalize(NormalizationForm.FormKD);
            return html;
        }
    }
}