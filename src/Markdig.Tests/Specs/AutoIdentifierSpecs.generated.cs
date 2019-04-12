// Generated: 2019-04-05 16:06:14

// --------------------------------
//         Auto Identifiers
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.AutoIdentifiers
{
    [TestFixture]
    public class TestExtensionsHeadingAutoIdentifiers
    {
        // # Extensions
        // 
        // This section describes the auto identifier extension
        // 
        // ## Heading Auto Identifiers
        // 
        // Allows to automatically creates an identifier for a heading:
        [Test]
        public void ExtensionsHeadingAutoIdentifiers_Example001()
        {
            // Example 1
            // Section: Extensions / Heading Auto Identifiers
            //
            // The following Markdown:
            //     # This is a heading
            //
            // Should be rendered as:
            //     <h1 id="this-is-a-heading">This is a heading</h1>

            Console.WriteLine("Example 1\nSection Extensions / Heading Auto Identifiers\n");
            TestParser.TestSpec("# This is a heading", "<h1 id=\"this-is-a-heading\">This is a heading</h1>", "autoidentifiers|advanced");
        }

        // Only punctuation `-`, `_` and `.` is kept, all other non letter characters are discarded.
        // Consecutive same character `-`, `_` or `.` are rendered into a single one
        // Characters `-`, `_` and `.` at the end of the string are also discarded.
        [Test]
        public void ExtensionsHeadingAutoIdentifiers_Example002()
        {
            // Example 2
            // Section: Extensions / Heading Auto Identifiers
            //
            // The following Markdown:
            //     # This - is a &@! heading _ with . and ! -
            //
            // Should be rendered as:
            //     <h1 id="this-is-a-heading_with.and">This - is a &amp;@! heading _ with . and ! -</h1>

            Console.WriteLine("Example 2\nSection Extensions / Heading Auto Identifiers\n");
            TestParser.TestSpec("# This - is a &@! heading _ with . and ! -", "<h1 id=\"this-is-a-heading_with.and\">This - is a &amp;@! heading _ with . and ! -</h1>", "autoidentifiers|advanced");
        }

        // Formatting (emphasis) are also discarded:
        [Test]
        public void ExtensionsHeadingAutoIdentifiers_Example003()
        {
            // Example 3
            // Section: Extensions / Heading Auto Identifiers
            //
            // The following Markdown:
            //     # This is a *heading*
            //
            // Should be rendered as:
            //     <h1 id="this-is-a-heading">This is a <em>heading</em></h1>

            Console.WriteLine("Example 3\nSection Extensions / Heading Auto Identifiers\n");
            TestParser.TestSpec("# This is a *heading*", "<h1 id=\"this-is-a-heading\">This is a <em>heading</em></h1>", "autoidentifiers|advanced");
        }

        // Links are also removed:
        [Test]
        public void ExtensionsHeadingAutoIdentifiers_Example004()
        {
            // Example 4
            // Section: Extensions / Heading Auto Identifiers
            //
            // The following Markdown:
            //     # This is a [heading](/url)
            //
            // Should be rendered as:
            //     <h1 id="this-is-a-heading">This is a <a href="/url">heading</a></h1>

            Console.WriteLine("Example 4\nSection Extensions / Heading Auto Identifiers\n");
            TestParser.TestSpec("# This is a [heading](/url)", "<h1 id=\"this-is-a-heading\">This is a <a href=\"/url\">heading</a></h1>", "autoidentifiers|advanced");
        }

        // If multiple heading have the same text, -1, -2...-n will be postfix to the header id.
        [Test]
        public void ExtensionsHeadingAutoIdentifiers_Example005()
        {
            // Example 5
            // Section: Extensions / Heading Auto Identifiers
            //
            // The following Markdown:
            //     # This is a heading
            //     # This is a heading
            //
            // Should be rendered as:
            //     <h1 id="this-is-a-heading">This is a heading</h1>
            //     <h1 id="this-is-a-heading-1">This is a heading</h1>

            Console.WriteLine("Example 5\nSection Extensions / Heading Auto Identifiers\n");
            TestParser.TestSpec("# This is a heading\n# This is a heading", "<h1 id=\"this-is-a-heading\">This is a heading</h1>\n<h1 id=\"this-is-a-heading-1\">This is a heading</h1>", "autoidentifiers|advanced");
        }

        // The heading Id will start on the first letter character of the heading, all previous characters will be discarded:
        [Test]
        public void ExtensionsHeadingAutoIdentifiers_Example006()
        {
            // Example 6
            // Section: Extensions / Heading Auto Identifiers
            //
            // The following Markdown:
            //     # 1.0 This is a heading
            //
            // Should be rendered as:
            //     <h1 id="this-is-a-heading">1.0 This is a heading</h1>

            Console.WriteLine("Example 6\nSection Extensions / Heading Auto Identifiers\n");
            TestParser.TestSpec("# 1.0 This is a heading", "<h1 id=\"this-is-a-heading\">1.0 This is a heading</h1>", "autoidentifiers|advanced");
        }

        // If the heading is all stripped by the previous rules, the id `section` will be used instead:
        [Test]
        public void ExtensionsHeadingAutoIdentifiers_Example007()
        {
            // Example 7
            // Section: Extensions / Heading Auto Identifiers
            //
            // The following Markdown:
            //     # 1.0 & ^ % *
            //     # 1.0 & ^ % *
            //
            // Should be rendered as:
            //     <h1 id="section">1.0 &amp; ^ % *</h1>
            //     <h1 id="section-1">1.0 &amp; ^ % *</h1>

            Console.WriteLine("Example 7\nSection Extensions / Heading Auto Identifiers\n");
            TestParser.TestSpec("# 1.0 & ^ % *\n# 1.0 & ^ % *", "<h1 id=\"section\">1.0 &amp; ^ % *</h1>\n<h1 id=\"section-1\">1.0 &amp; ^ % *</h1>", "autoidentifiers|advanced");
        }

        // When the options "AutoLink" is setup, it is possible to link to an existing heading by using the 
        // exact same Label text as the heading:
        [Test]
        public void ExtensionsHeadingAutoIdentifiers_Example008()
        {
            // Example 8
            // Section: Extensions / Heading Auto Identifiers
            //
            // The following Markdown:
            //     # This is a heading
            //     [This is a heading]
            //
            // Should be rendered as:
            //     <h1 id="this-is-a-heading">This is a heading</h1>
            //     <p><a href="#this-is-a-heading">This is a heading</a></p>

            Console.WriteLine("Example 8\nSection Extensions / Heading Auto Identifiers\n");
            TestParser.TestSpec("# This is a heading\n[This is a heading]", "<h1 id=\"this-is-a-heading\">This is a heading</h1>\n<p><a href=\"#this-is-a-heading\">This is a heading</a></p>", "autoidentifiers|advanced");
        }

        // Links before the heading are also working:
        [Test]
        public void ExtensionsHeadingAutoIdentifiers_Example009()
        {
            // Example 9
            // Section: Extensions / Heading Auto Identifiers
            //
            // The following Markdown:
            //     [This is a heading]
            //     # This is a heading
            //
            // Should be rendered as:
            //     <p><a href="#this-is-a-heading">This is a heading</a></p>
            //     <h1 id="this-is-a-heading">This is a heading</h1>

            Console.WriteLine("Example 9\nSection Extensions / Heading Auto Identifiers\n");
            TestParser.TestSpec("[This is a heading]\n# This is a heading", "<p><a href=\"#this-is-a-heading\">This is a heading</a></p>\n<h1 id=\"this-is-a-heading\">This is a heading</h1>", "autoidentifiers|advanced");
        }

        // The text of the link can be changed:
        [Test]
        public void ExtensionsHeadingAutoIdentifiers_Example010()
        {
            // Example 10
            // Section: Extensions / Heading Auto Identifiers
            //
            // The following Markdown:
            //     [With a new text][This is a heading]
            //     # This is a heading
            //
            // Should be rendered as:
            //     <p><a href="#this-is-a-heading">With a new text</a></p>
            //     <h1 id="this-is-a-heading">This is a heading</h1>

            Console.WriteLine("Example 10\nSection Extensions / Heading Auto Identifiers\n");
            TestParser.TestSpec("[With a new text][This is a heading]\n# This is a heading", "<p><a href=\"#this-is-a-heading\">With a new text</a></p>\n<h1 id=\"this-is-a-heading\">This is a heading</h1>", "autoidentifiers|advanced");
        }

        // An autoidentifier should not conflict with an existing link:
        [Test]
        public void ExtensionsHeadingAutoIdentifiers_Example011()
        {
            // Example 11
            // Section: Extensions / Heading Auto Identifiers
            //
            // The following Markdown:
            //     ![scenario image][scenario]
            //     ## Scenario
            //     [scenario]: ./scenario.png
            //
            // Should be rendered as:
            //     <p><img src="./scenario.png" alt="scenario image" /></p>
            //     <h2 id="scenario">Scenario</h2>

            Console.WriteLine("Example 11\nSection Extensions / Heading Auto Identifiers\n");
            TestParser.TestSpec("![scenario image][scenario]\n## Scenario\n[scenario]: ./scenario.png", "<p><img src=\"./scenario.png\" alt=\"scenario image\" /></p>\n<h2 id=\"scenario\">Scenario</h2>", "autoidentifiers|advanced");
        }
    }
}
