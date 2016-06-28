// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using NUnit.Framework;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestNormalize
    {

        [Test]
        public void TestParagraph()
        {
            AssertNormalize("This is a plain paragraph");
            AssertNormalize(@"This
is
a
plain
paragraph");
        }

        [Test]
        public void TestParagraphMulti()
        {
            AssertNormalize(@"line1

line2

line3");
        }

        [Test]
        public void TestListUnordered()
        {
            AssertNormalize(@"
- a
- b
- c
");
        }

        [Test]
        public void TestListOrdered()
        {
            AssertNormalize(@"
1. a
2. b
3. c
");
        }

        [Test]
        public void TestHeaderAndParagraph()
        {
            AssertNormalize(@"
# heading

paragraph
");
        }


        [Test]
        public void TestQuote()
        {
            AssertNormalize(@"
> test1
> 
> test2
");
        }


        [Test]
        public void TestEmphasis()
        {
            AssertNormalize("This is a plain **paragraph**");
            AssertNormalize("This is a plain *paragraph*");
            AssertNormalize("This is a plain _paragraph_");
            AssertNormalize("This is a plain __paragraph__");
            AssertNormalize("This is a pl*ai*n **paragraph**");
        }

        [Test]
        public void TestLinks()
        {
            AssertNormalize("This is a [link](http://company.com)");
            AssertNormalize("This is an ![image](http://company.com)");
        }


        public void AssertNormalize(string input, string expected = null)
        {
            expected = expected ?? input;
            input = NormText(input);
            expected = NormText(expected);

            var result = Markdown.Normalize(input);
            result = NormText(result);

            Console.WriteLine("```````````````````Source");
            Console.WriteLine(TestParser.DisplaySpaceAndTabs(input));
            Console.WriteLine("```````````````````Result");
            Console.WriteLine(TestParser.DisplaySpaceAndTabs(result));
            Console.WriteLine("```````````````````Expected");
            Console.WriteLine(TestParser.DisplaySpaceAndTabs(expected));
            Console.WriteLine("```````````````````");
            Console.WriteLine();

            TextAssert.AreEqual(expected, result);
        }

        private static string NormText(string text)
        {
            return text.Trim().Replace("\r\n", "\n").Replace('\r', '\n');
        }
    }
}