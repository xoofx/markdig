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
        public void TestParagraphNewLineAndSpaces()
        {
            //     0123 45678
            Check("012\n  345", @"
paragraph    ( 0, 0)  0-8
literal      ( 0, 0)  0-2
linebreak    ( 0, 3)  3-3
literal      ( 1, 2)  6-8
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

        [Test]
        public void TestEmphasis()
        {
            Check("012**3456789**", @"
paragraph    ( 0, 0)  0-13
literal      ( 0, 0)  0-2
emphasis     ( 0, 3)  3-13
literal      ( 0, 5)  5-11
");
        }

        [Test]
        public void TestEmphasis2()
        {
            //     01234567
            Check("01*2**3*", @"
paragraph    ( 0, 0)  0-7
literal      ( 0, 0)  0-1
emphasis     ( 0, 2)  2-4
literal      ( 0, 3)  3-3
emphasis     ( 0, 5)  5-7
literal      ( 0, 6)  6-6
");
        }

        [Test]
        public void TestEmphasis3()
        {
            //     0123456789
            Check("01**2***3*", @"
paragraph    ( 0, 0)  0-9
literal      ( 0, 0)  0-1
emphasis     ( 0, 2)  2-6
literal      ( 0, 4)  4-4
emphasis     ( 0, 7)  7-9
literal      ( 0, 8)  8-8
");
        }

        [Test]
        public void TestEmphasisFalse()
        {
            Check("0123456789**0123", @"
paragraph    ( 0, 0)  0-15
literal      ( 0, 0)  0-9
literal      ( 0,10) 10-11
literal      ( 0,12) 12-15
");
        }

        [Test]
        public void TestHeading()
        {
            //     012345
            Check("# 2345", @"
heading      ( 0, 0)  0-5
literal      ( 0, 2)  2-5
");
        }

        [Test]
        public void TestHeadingWithEmphasis()
        {
            //     0123456789
            Check("# 23**45**", @"
heading      ( 0, 0)  0-9
literal      ( 0, 2)  2-3
emphasis     ( 0, 4)  4-9
literal      ( 0, 6)  6-7
");
        }

        [Test]
        public void TestCodeSpan()
        {
            //     012345678
            Check("0123`456`", @"
paragraph    ( 0, 0)  0-8
literal      ( 0, 0)  0-3
code         ( 0, 4)  4-8
");
        }

        [Test]
        public void TestLink()
        {
            //     0123456789
            Check("012[45](#)", @"
paragraph    ( 0, 0)  0-9
literal      ( 0, 0)  0-2
link         ( 0, 3)  3-9
literal      ( 0, 4)  4-5
");
        }

        [Test]
        public void TestHtmlInline()
        {
            //     0123456789
            Check("01<b>4</b>", @"
paragraph    ( 0, 0)  0-9
literal      ( 0, 0)  0-1
html         ( 0, 2)  2-4
literal      ( 0, 5)  5-5
html         ( 0, 6)  6-9
");
        }

        [Test]
        public void TestAutolinkInline()
        {
            //     0123456789ABCD
            Check("01<http://yes>", @"
paragraph    ( 0, 0)  0-13
literal      ( 0, 0)  0-1
autolink     ( 0, 2)  2-13
");
        }

        [Test]
        public void TestFencedCodeBlock()
        {
            //     012 3456 78 9ABC
            Check("01\n```\n3\n```\n", @"
paragraph    ( 0, 0)  0-1
literal      ( 0, 0)  0-1
fencedcode   ( 1, 0)  3-11
");
        }

        [Test]
        public void TestHtmlBlock()
        {
            //     012345 67 89ABCDE F 0
            Check("<div>\n0\n</div>\n\n1", @"
html         ( 0, 0)  0-13
paragraph    ( 4, 0) 16-16
literal      ( 4, 0) 16-16
");
        }

        [Test]
        public void TestThematicBreak()
        {
            //     0123 4567
            Check("---\n---\n", @"
thematicbreak ( 0, 0)  0-2
thematicbreak ( 1, 0)  4-6
");
        }

        [Test]
        public void TestQuoteBlock()
        {
            //     0123456
            Check("> 2345\n", @"
quote        ( 0, 0)  0-5
paragraph    ( 0, 2)  2-5
literal      ( 0, 2)  2-5
");
        }

        [Test]
        public void TestQuoteBlockWithLines()
        {
            //     01234 56789A
            Check("> 01\n>  23\n", @"
quote        ( 0, 0)  0-9
paragraph    ( 0, 2)  2-9
literal      ( 0, 2)  2-3
linebreak    ( 0, 4)  4-4
literal      ( 1, 3)  8-9
");
        }

        [Test]
        public void TestQuoteBlockWithLazyContinuation()
        {
            //     01234 56
            Check("> 01\n23\n", @"
quote        ( 0, 0)  0-6
paragraph    ( 0, 2)  2-6
literal      ( 0, 2)  2-3
linebreak    ( 0, 4)  4-4
literal      ( 1, 0)  5-6
");
        }

        [Test]
        public void TestListBlock()
        {
            //     0123 4567
            Check("- 0\n- 1\n", @"
list         ( 0, 0)  0-6
listitem     ( 0, 0)  0-2
paragraph    ( 0, 2)  2-2
literal      ( 0, 2)  2-2
listitem     ( 1, 0)  4-6
paragraph    ( 1, 2)  6-6
literal      ( 1, 2)  6-6
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