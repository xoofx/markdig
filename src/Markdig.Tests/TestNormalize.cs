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
        public void TestCodeBlock()
        {
            AssertNormalizeNoTrim("    public void HelloWorld();\n    {\n    }\n\n");
            AssertNormalizeNoTrim("```` \npublic void HelloWorld();\n{\n}\n````\n"); //TODO: Bug? Space after the fence
            AssertNormalizeNoTrim("````csharp  \npublic void HelloWorld();\n{\n}\n````\n"); //TODO: Bug? two spaces after the language
            AssertNormalizeNoTrim("````csharp hideNewKeyword=true \npublic void HelloWorld();\n{\n}\n````\n"); //TODO: Bug? space after the attributes
        }

        [Test]
        public void TestHeading()
        {
            AssertNormalizeNoTrim("# Heading\n\n");
            AssertNormalizeNoTrim("## Heading\n\n");
            AssertNormalizeNoTrim("### Heading\n\n");
            AssertNormalizeNoTrim("#### Heading\n\n");
            AssertNormalizeNoTrim("##### Heading\n\n");
            AssertNormalizeNoTrim("###### Heading\n\n");

            AssertNormalizeNoTrim("Heading\n=======\n\n", "# Heading\n\n");
        }

        [Test]
        public void TestHtml()
        {
            /*AssertNormalizeNoTrim(@"<div id=""foo"" class=""bar
  baz"">
</ div >");*/ // TODO: Bug: Throws Exception during emit 
        }

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
        public void TestListOrderedAndIntended()
        {
            AssertNormalize(@"
1. a
2. b
   - foo
   - bar
3. c
", @"
1. a
2. b
  - foo
  - bar
3. c
"); // TODO: Bug: intend is always stripped a space.
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
        public void TestThematicBreak()
        {
            AssertNormalizeNoTrim("***\n");

            AssertNormalizeNoTrim("* * *\n", "***\n");
        }

        [Test]
        public void TestAutolinkInline()
        {
            AssertNormalizeNoTrim("This has a <auto.link.com>");
        }

        [Test]
        public void TestCodeInline()
        {
            AssertNormalizeNoTrim("This has a `HelloWorld()` in it");
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

        [Test]
        public void TestHtmlEntity()
        {
            AssertNormalizeNoTrim("This is a &auml; blank", "This is a ä blank"); //TODO: bug?
        }

        [Test]
        public void TestHtmlInline()
        {
            AssertNormalizeNoTrim("foo <hr/> bar");
        }

        public void AssertNormalizeNoTrim(string input, string expected = null)
            => AssertNormalize(input, expected, false);

        public void AssertNormalize(string input, string expected = null, bool trim = true)
        {
            expected = expected ?? input;
            input = NormText(input, trim);
            expected = NormText(expected, trim);

            var result = Markdown.Normalize(input);
            result = NormText(result, trim);

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

        private static string NormText(string text, bool trim)
        {
            if (trim)
            {
                text = text.Trim();
            }
            return text.Replace("\r\n", "\n").Replace('\r', '\n');
        }
    }
}