
// --------------------------------
//            List Extras
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.ListExtras
{
    [TestFixture]
    public class TestExtensionsOrderedListWithAlphaLetter
    {
        // # Extensions
        // 
        // The following additional list items are supported:
        // 
        // ## Ordered list with alpha letter
        //  
        // Allows to use a list using an alpha letter instead of a number
        [Test]
        public void ExtensionsOrderedListWithAlphaLetter_Example001()
        {
            // Example 1
            // Section: Extensions / Ordered list with alpha letter
            //
            // The following Markdown:
            //     a. First item
            //     b. Second item
            //     c. Last item
            //
            // Should be rendered as:
            //     <ol type="a">
            //     <li>First item</li>
            //     <li>Second item</li>
            //     <li>Last item</li>
            //     </ol>

            TestParser.TestSpec("a. First item\nb. Second item\nc. Last item", "<ol type=\"a\">\n<li>First item</li>\n<li>Second item</li>\n<li>Last item</li>\n</ol>", "listextras|advanced", context: "Example 1\nSection Extensions / Ordered list with alpha letter\n");
        }

        // It works also for uppercase alpha: 
        [Test]
        public void ExtensionsOrderedListWithAlphaLetter_Example002()
        {
            // Example 2
            // Section: Extensions / Ordered list with alpha letter
            //
            // The following Markdown:
            //     A. First item
            //     B. Second item
            //     C. Last item
            //
            // Should be rendered as:
            //     <ol type="A">
            //     <li>First item</li>
            //     <li>Second item</li>
            //     <li>Last item</li>
            //     </ol>

            TestParser.TestSpec("A. First item\nB. Second item\nC. Last item", "<ol type=\"A\">\n<li>First item</li>\n<li>Second item</li>\n<li>Last item</li>\n</ol>", "listextras|advanced", context: "Example 2\nSection Extensions / Ordered list with alpha letter\n");
        }

        // Like for numbered list, a list can start with a different letter
        [Test]
        public void ExtensionsOrderedListWithAlphaLetter_Example003()
        {
            // Example 3
            // Section: Extensions / Ordered list with alpha letter
            //
            // The following Markdown:
            //     b. First item
            //     c. Second item
            //
            // Should be rendered as:
            //     <ol type="a" start="2">
            //     <li>First item</li>
            //     <li>Second item</li>
            //     </ol>

            TestParser.TestSpec("b. First item\nc. Second item", "<ol type=\"a\" start=\"2\">\n<li>First item</li>\n<li>Second item</li>\n</ol>", "listextras|advanced", context: "Example 3\nSection Extensions / Ordered list with alpha letter\n");
        }

        // A different type of list will break the existing list:
        [Test]
        public void ExtensionsOrderedListWithAlphaLetter_Example004()
        {
            // Example 4
            // Section: Extensions / Ordered list with alpha letter
            //
            // The following Markdown:
            //     a. First item1
            //     b. Second item
            //     A. First item2
            //
            // Should be rendered as:
            //     <ol type="a">
            //     <li>First item1</li>
            //     <li>Second item</li>
            //     </ol>
            //     <ol type="A">
            //     <li>First item2</li>
            //     </ol>

            TestParser.TestSpec("a. First item1\nb. Second item\nA. First item2", "<ol type=\"a\">\n<li>First item1</li>\n<li>Second item</li>\n</ol>\n<ol type=\"A\">\n<li>First item2</li>\n</ol>", "listextras|advanced", context: "Example 4\nSection Extensions / Ordered list with alpha letter\n");
        }
    }

    [TestFixture]
    public class TestExtensionsOrderedListWithRomanLetter
    {
        // ## Ordered list with roman letter
        // 
        // Allows to use a list using a roman number instead of a number.
        [Test]
        public void ExtensionsOrderedListWithRomanLetter_Example005()
        {
            // Example 5
            // Section: Extensions / Ordered list with roman letter
            //
            // The following Markdown:
            //     i. First item
            //     ii. Second item
            //     iii. Third item
            //     iv. Last item
            //
            // Should be rendered as:
            //     <ol type="i">
            //     <li>First item</li>
            //     <li>Second item</li>
            //     <li>Third item</li>
            //     <li>Last item</li>
            //     </ol>

            TestParser.TestSpec("i. First item\nii. Second item\niii. Third item\niv. Last item", "<ol type=\"i\">\n<li>First item</li>\n<li>Second item</li>\n<li>Third item</li>\n<li>Last item</li>\n</ol>", "listextras|advanced", context: "Example 5\nSection Extensions / Ordered list with roman letter\n");
        }

        // It works also for uppercase alpha: 
        [Test]
        public void ExtensionsOrderedListWithRomanLetter_Example006()
        {
            // Example 6
            // Section: Extensions / Ordered list with roman letter
            //
            // The following Markdown:
            //     I. First item
            //     II. Second item
            //     III. Third item
            //     IV. Last item
            //
            // Should be rendered as:
            //     <ol type="I">
            //     <li>First item</li>
            //     <li>Second item</li>
            //     <li>Third item</li>
            //     <li>Last item</li>
            //     </ol>

            TestParser.TestSpec("I. First item\nII. Second item\nIII. Third item\nIV. Last item", "<ol type=\"I\">\n<li>First item</li>\n<li>Second item</li>\n<li>Third item</li>\n<li>Last item</li>\n</ol>", "listextras|advanced", context: "Example 6\nSection Extensions / Ordered list with roman letter\n");
        }

        // Like for numbered list, a list can start with a different letter
        [Test]
        public void ExtensionsOrderedListWithRomanLetter_Example007()
        {
            // Example 7
            // Section: Extensions / Ordered list with roman letter
            //
            // The following Markdown:
            //     ii. First item
            //     iii. Second item
            //
            // Should be rendered as:
            //     <ol type="i" start="2">
            //     <li>First item</li>
            //     <li>Second item</li>
            //     </ol>

            TestParser.TestSpec("ii. First item\niii. Second item", "<ol type=\"i\" start=\"2\">\n<li>First item</li>\n<li>Second item</li>\n</ol>", "listextras|advanced", context: "Example 7\nSection Extensions / Ordered list with roman letter\n");
        }

        // Lists can be restarted, specifying the start point.
        [Test]
        public void ExtensionsOrderedListWithRomanLetter_Example008()
        {
            // Example 8
            // Section: Extensions / Ordered list with roman letter
            //
            // The following Markdown:
            //     1.   First item
            //     
            //     Some text
            //     
            //     2.   Second item
            //
            // Should be rendered as:
            //     <ol>
            //     <li>First item</li>
            //     </ol>
            //     <p>Some text</p>
            //     <ol start="2">
            //     <li>Second item</li>
            //     </ol>

            TestParser.TestSpec("1.   First item\n\nSome text\n\n2.   Second item", "<ol>\n<li>First item</li>\n</ol>\n<p>Some text</p>\n<ol start=\"2\">\n<li>Second item</li>\n</ol>", "listextras|advanced", context: "Example 8\nSection Extensions / Ordered list with roman letter\n");
        }
    }
}
