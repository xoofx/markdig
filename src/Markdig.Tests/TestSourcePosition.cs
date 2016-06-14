// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Text;
using Markdig.Helpers;
using Markdig.Syntax;
using NUnit.Framework;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestSourcePosition
    {
        [Test]
        public void TestParagraph()
        {
            Check("0123456789", @"
paragraph    ( 0, 0)  0-9
literal      ( 0, 0)  0-9
");
        }

        [Test]
        public void TestParagraphFalseEmphasis()
        {
            Check("0123456789**0123", @"
paragraph    ( 0, 0)  0-15
literal      ( 0, 0)  0-9
literal      ( 0, 0) 10-11
literal      ( 0,12) 12-15
");
        }

        [Test]
        public void TestParagraphAndNewLine()
        {
            Check("0123456789\n0123456789", @"
paragraph    ( 0, 0)  0-20
literal      ( 0, 0)  0-9
linebreak    ( 0,10) 10-10
literal      ( 1, 0) 11-20
");

            Check("0123456789\r\n0123456789", @"
paragraph    ( 0, 0)  0-21
literal      ( 0, 0)  0-9
linebreak    ( 0,10) 10-10
literal      ( 1, 0) 12-21
");
        }

        [Test]
        public void TestParagraph2()
        {
            Check("0123456789\n\n0123456789", @"
paragraph    ( 0, 0)  0-9
literal      ( 0, 0)  0-9
paragraph    ( 2, 0) 12-21
literal      ( 2, 0) 12-21
");
        }

        private static void Check(string text, string expectedResult)
        {
            var pipeline = new MarkdownPipelineBuilder().UsePreciseSourceLocation().Build();
            var document = Markdown.Parse(text, pipeline);

            var build = new StringBuilder();
            foreach (var val in document.Descendants())
            {
                var name = GetTypeName(val.GetType());
                build.Append($"{name,-12} ({val.Line,2},{val.Column,2}) {val.SourceStartPosition,2}-{val.SourceEndPosition}\n");
            }
            var result = build.ToString().Trim();

            expectedResult = expectedResult.Trim();
            expectedResult = expectedResult.Replace("\r\n", "\n").Replace("\r", "\n");

            if (expectedResult != result)
            {
                Console.WriteLine("```````````````````Source");
                Console.WriteLine(TestParser.DisplaySpaceAndTabs(text));
                Console.WriteLine("```````````````````Result");
                Console.WriteLine(result);
                Console.WriteLine("```````````````````Expected");
                Console.WriteLine(expectedResult);
                Console.WriteLine("```````````````````");
                Console.WriteLine();
            }

            TextAssert.AreEqual(expectedResult, result);
        }

        private static string GetTypeName(Type type)
        {
            return type.Name.ToLowerInvariant()
                .Replace("block", string.Empty)
                .Replace("inline", string.Empty);
        }
    }
}