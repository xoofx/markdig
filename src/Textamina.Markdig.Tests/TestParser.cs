// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Tests
{
    public class TestParser
    {
        public static void TestSpec(string inputText, string expectedOutputText, string extensions = null)
        {
            MarkdownParser.Log = Console.Out;
            var reader = new StringReader(inputText);
            var output = new StringWriter();
            Markdown.ConvertToHtml(reader, output, GetPipeline(extensions));

            var result = Compact(output.ToString());
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

        private static MarkdownPipeline GetPipeline(string extensionsStr)
        {
            if (extensionsStr == null)
            {
                return new MarkdownPipeline();
            }

            var pipeline = new MarkdownPipeline();
            foreach (var extension in extensionsStr.Split(new[] {'+'}, StringSplitOptions.RemoveEmptyEntries))
            {
                switch (extension.ToLowerInvariant())
                {
                    case "pipetables":
                        pipeline.UsePipeTable();
                        break;
                    case "strike":
                        pipeline.UseStrikethroughSuperAndSubScript();
                        break;
                    case "hardlinebreak":
                        pipeline.UseSoftlineBreakAsHardlineBreak();
                        break;
                    case "footnotes":
                        pipeline.UseFootnoteExtensions();
                        break;
                    case "attributes":
                        pipeline.UseFootnoteExtensions();
                        break;
                    default:
                        Console.WriteLine($"Unsupported extension: {extension}");
                        break;
                }
            }
            return pipeline;
        }

        private static string DisplaySpaceAndTabs(string text)
        {
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