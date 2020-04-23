// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;
using NUnit.Framework;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestPragmaLines
    {
        [Test]
        public void TestFindClosest()
        {
            var doc = Markdown.Parse(
"test1\n" +                      // 0
"\n" +                           // 1
"test2\n" +                      // 2
"\n" +                           // 3
"test3\n" +                      // 4
"\n" +                           // 5
"test4\n" +                      // 6
"\n" +                           // 7
"# Heading\n" +                  // 8
"\n" +                           // 9
"Long para\n" +                  // 10
"on multiple\n" +                // 11
"lines\n" +                      // 12
"to check that\n" +              // 13
"lines are\n" +                  // 14
"correctly \n" +                 // 15
"found\n" +                      // 16
"\n" +                           // 17
"- item1\n" +                    // 18
"- item2\n" +                    // 19
"- item3\n" +                    // 20
"\n" +                           // 21
"This is a last paragraph\n"     // 22
                , new MarkdownPipelineBuilder().UsePragmaLines().Build());

            foreach (var exact in new int[] {0, 2, 4, 6, 8, 10, 18, 19, 20, 22})
            {
                Assert.AreEqual(exact, doc.FindClosestLine(exact));
            }

            Assert.AreEqual(22, doc.FindClosestLine(23));

            Assert.AreEqual(10, doc.FindClosestLine(11));
            Assert.AreEqual(10, doc.FindClosestLine(12));
            Assert.AreEqual(10, doc.FindClosestLine(13));
            Assert.AreEqual(18, doc.FindClosestLine(14)); // > 50% of the paragraph, we switch to next
            Assert.AreEqual(18, doc.FindClosestLine(15));
            Assert.AreEqual(18, doc.FindClosestLine(16));
        }

        [Test]
        public void TestFindClosest1()
        {
            var text = 
"- item1\n" +                    // 0
"  - item11\n" +                 // 1
"  - item12\n" +                 // 2
"    - item121\n" +              // 3
"  - item13\n" +                 // 4
"    - item131\n" +              // 5
"      - item1311\n";            // 6

            var pipeline = new MarkdownPipelineBuilder().UsePragmaLines().Build();
            var doc = Markdown.Parse(text, pipeline);

            for (int exact = 0; exact < 7; exact++)
            {
                Assert.AreEqual(exact, doc.FindClosestLine(exact));
            }

            Assert.AreEqual(6, doc.FindClosestLine(50));
        }
    }
}