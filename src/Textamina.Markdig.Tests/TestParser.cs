// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license. See license.txt file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Tests
{

    [TestFixture]
    public class TestParser
    {
        private const string RelativeBasePath = @"..\..\TestFiles";
        private const string InputFilePattern = "*.txt";
        private const string OutputEndFileExtension = ".out.txt";

        [Test]
        public void TestSimple()
        {
            var reader = new StringReader(@"- toto
- tata");
//            var reader = new StringReader(@"> > toto tata
//> titi toto
//");
            var result = Markdown.Convert(reader);
            Console.WriteLine(result);
        }

        //[TestCaseSource("TestFiles")]
        public static void TestSpec(string inputText, string expectedOutputText)
        {
            MarkdownParser.Log = Console.Out;
            var reader = new StringReader(inputText);
            var output = new StringWriter();
            Markdown.Convert(reader, output);

            var result = Compact(output.ToString());
            expectedOutputText = Compact(expectedOutputText);

            Console.WriteLine("``````````````````` Source");
            Console.WriteLine(DisplaySpaceAndTabs(inputText));
            Console.WriteLine("``````````````````` Result");
            Console.WriteLine(DisplaySpaceAndTabs(result));
            Console.WriteLine("``````````````````` Expected");
            Console.WriteLine(DisplaySpaceAndTabs(expectedOutputText));
            Console.WriteLine("```````````````````");
            Console.WriteLine();
            TextAssert.AreEqual(expectedOutputText, result);
        }

        private static string DisplaySpaceAndTabs(string text)
        {
            return text.Replace('\t', '→').Replace(' ', '·');
        }

        private static string Compact(string html)
        {
            html = html.Replace("\r", "").Trim();

            // collapse spaces and newlines before </li> and after <li>
            html = Regex.Replace(html, @"\s+</li>", "</li>");
            html = Regex.Replace(html, @"<li>\s+", "<li>");

            // needed to compare UTF-32 characters
            html = html.Normalize(NormalizationForm.FormKD);
            return html;
        }





        public static IEnumerable<object[]> TestFiles
        {
            get
            {
                var baseDir = Path.GetFullPath(Path.Combine(BaseDirectory, RelativeBasePath));
                return
                    Directory.EnumerateFiles(baseDir, InputFilePattern, SearchOption.AllDirectories)
                        .Where(f => !f.EndsWith(OutputEndFileExtension))
                        .Select(f => f.StartsWith(baseDir) ? f.Substring(baseDir.Length + 1) : f)
                        .OrderBy(f => f)
                        .Select(x => new object[]
                        {
                            new TestFilePath(x)
                        });
            }
        }

        /// <summary>
        /// Use an internal class to have a better display of the filename in Resharper Unit Tests runner.
        /// </summary>
        public struct TestFilePath
        {
            public TestFilePath(string filePath)
            {
                FilePath = filePath;
            }

            public string FilePath { get; }

            public override string ToString()
            {
                return FilePath;
            }
        }

        private static string BaseDirectory
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var codebase = new Uri(assembly.CodeBase);
                var path = codebase.LocalPath;
                return Path.GetDirectoryName(path);
            }
        }
    }
}