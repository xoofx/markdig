// Generated: 2019-05-15 02:46:55

// --------------------------------
//           Abbreviations
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.Abbreviations
{
    [TestFixture]
    public class TestExtensionsAbbreviation
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## Abbreviation
        // 
        // Abbreviation can be declared by using the `*[Abbreviation Label]: Abbreviation description`
        // 
        // Abbreviation definition will be removed from the original document. Any Abbreviation label found in literals will be replaced by the abbreviation description:
        [Test]
        public void ExtensionsAbbreviation_Example001()
        {
            // Example 1
            // Section: Extensions / Abbreviation
            //
            // The following Markdown:
            //     *[HTML]: Hypertext Markup Language
            //     
            //     Later in a text we are using HTML and it becomes an abbr tag HTML
            //
            // Should be rendered as:
            //     <p>Later in a text we are using <abbr title="Hypertext Markup Language">HTML</abbr> and it becomes an abbr tag <abbr title="Hypertext Markup Language">HTML</abbr></p>

            Console.WriteLine("Example 1\nSection Extensions / Abbreviation\n");
            TestParser.TestSpec("*[HTML]: Hypertext Markup Language\n\nLater in a text we are using HTML and it becomes an abbr tag HTML", "<p>Later in a text we are using <abbr title=\"Hypertext Markup Language\">HTML</abbr> and it becomes an abbr tag <abbr title=\"Hypertext Markup Language\">HTML</abbr></p>", "abbreviations|advanced");
        }

        // An abbreviation definition can be indented at most 3 spaces
        [Test]
        public void ExtensionsAbbreviation_Example002()
        {
            // Example 2
            // Section: Extensions / Abbreviation
            //
            // The following Markdown:
            //     *[HTML]: Hypertext Markup Language
            //         *[This]: is not an abbreviation
            //
            // Should be rendered as:
            //     <pre><code>*[This]: is not an abbreviation
            //     </code></pre>

            Console.WriteLine("Example 2\nSection Extensions / Abbreviation\n");
            TestParser.TestSpec("*[HTML]: Hypertext Markup Language\n    *[This]: is not an abbreviation", "<pre><code>*[This]: is not an abbreviation\n</code></pre>", "abbreviations|advanced");
        }

        // An abbreviation may contain spaces:
        [Test]
        public void ExtensionsAbbreviation_Example003()
        {
            // Example 3
            // Section: Extensions / Abbreviation
            //
            // The following Markdown:
            //     *[SUPER HTML]: Super Hypertext Markup Language
            //     
            //     This is a SUPER HTML document    
            //
            // Should be rendered as:
            //     <p>This is a <abbr title="Super Hypertext Markup Language">SUPER HTML</abbr> document</p>

            Console.WriteLine("Example 3\nSection Extensions / Abbreviation\n");
            TestParser.TestSpec("*[SUPER HTML]: Super Hypertext Markup Language\n\nThis is a SUPER HTML document    ", "<p>This is a <abbr title=\"Super Hypertext Markup Language\">SUPER HTML</abbr> document</p>", "abbreviations|advanced");
        }

        // Abbreviation may contain any unicode characters:
        [Test]
        public void ExtensionsAbbreviation_Example004()
        {
            // Example 4
            // Section: Extensions / Abbreviation
            //
            // The following Markdown:
            //     *[😃 HTML]: Hypertext Markup Language
            //     
            //     This is a 😃 HTML document    
            //
            // Should be rendered as:
            //     <p>This is a <abbr title="Hypertext Markup Language">😃 HTML</abbr> document</p>

            Console.WriteLine("Example 4\nSection Extensions / Abbreviation\n");
            TestParser.TestSpec("*[😃 HTML]: Hypertext Markup Language\n\nThis is a 😃 HTML document    ", "<p>This is a <abbr title=\"Hypertext Markup Language\">😃 HTML</abbr> document</p>", "abbreviations|advanced");
        }

        // Abbreviations may be similar:
        [Test]
        public void ExtensionsAbbreviation_Example005()
        {
            // Example 5
            // Section: Extensions / Abbreviation
            //
            // The following Markdown:
            //     *[1A]: First
            //     *[1A1]: Second
            //     *[1A2]: Third
            //     
            //     We can abbreviate 1A, 1A1 and 1A2!
            //
            // Should be rendered as:
            //     <p>We can abbreviate <abbr title="First">1A</abbr>, <abbr title="Second">1A1</abbr> and <abbr title="Third">1A2</abbr>!</p>

            Console.WriteLine("Example 5\nSection Extensions / Abbreviation\n");
            TestParser.TestSpec("*[1A]: First\n*[1A1]: Second\n*[1A2]: Third\n\nWe can abbreviate 1A, 1A1 and 1A2!", "<p>We can abbreviate <abbr title=\"First\">1A</abbr>, <abbr title=\"Second\">1A1</abbr> and <abbr title=\"Third\">1A2</abbr>!</p>", "abbreviations|advanced");
        }

        // Abbreviations should match whole word only:
        [Test]
        public void ExtensionsAbbreviation_Example006()
        {
            // Example 6
            // Section: Extensions / Abbreviation
            //
            // The following Markdown:
            //     *[1A]: First
            //     
            //     We should not abbreviate 1.1A or 11A!
            //
            // Should be rendered as:
            //     <p>We should not abbreviate 1.1A or 11A!</p>

            Console.WriteLine("Example 6\nSection Extensions / Abbreviation\n");
            TestParser.TestSpec("*[1A]: First\n\nWe should not abbreviate 1.1A or 11A!", "<p>We should not abbreviate 1.1A or 11A!</p>", "abbreviations|advanced");
        }

        // Abbreviations should match whole word only, even if the word is the entire content:
        [Test]
        public void ExtensionsAbbreviation_Example007()
        {
            // Example 7
            // Section: Extensions / Abbreviation
            //
            // The following Markdown:
            //     *[1A]: First
            //     
            //     1.1A
            //
            // Should be rendered as:
            //     <p>1.1A</p>

            Console.WriteLine("Example 7\nSection Extensions / Abbreviation\n");
            TestParser.TestSpec("*[1A]: First\n\n1.1A", "<p>1.1A</p>", "abbreviations|advanced");
        }

        // Abbreviations should match whole word only, even if there is another glossary term:
        [Test]
        public void ExtensionsAbbreviation_Example008()
        {
            // Example 8
            // Section: Extensions / Abbreviation
            //
            // The following Markdown:
            //     *[SCO]: First
            //     *[SCOM]: Second
            //     
            //     SCOM
            //
            // Should be rendered as:
            //     <p><abbr title="Second">SCOM</abbr></p>

            Console.WriteLine("Example 8\nSection Extensions / Abbreviation\n");
            TestParser.TestSpec("*[SCO]: First\n*[SCOM]: Second\n\nSCOM", "<p><abbr title=\"Second\">SCOM</abbr></p>", "abbreviations|advanced");
        }

        // Abbreviations should only match when surrounded by whitespace:
        [Test]
        public void ExtensionsAbbreviation_Example009()
        {
            // Example 9
            // Section: Extensions / Abbreviation
            //
            // The following Markdown:
            //     *[PR]: Pull Request
            //     
            //     PRAA
            //
            // Should be rendered as:
            //     <p>PRAA</p>

            Console.WriteLine("Example 9\nSection Extensions / Abbreviation\n");
            TestParser.TestSpec("*[PR]: Pull Request\n\nPRAA", "<p>PRAA</p>", "abbreviations|advanced");
        }

        // Single character abbreviations should be matched
        [Test]
        public void ExtensionsAbbreviation_Example010()
        {
            // Example 10
            // Section: Extensions / Abbreviation
            //
            // The following Markdown:
            //     *[A]: Foo
            //     
            //     A
            //
            // Should be rendered as:
            //     <p><abbr title="Foo">A</abbr></p>

            Console.WriteLine("Example 10\nSection Extensions / Abbreviation\n");
            TestParser.TestSpec("*[A]: Foo\n\nA", "<p><abbr title=\"Foo\">A</abbr></p>", "abbreviations|advanced");
        }

        // The longest matching abbreviation should be used
        [Test]
        public void ExtensionsAbbreviation_Example011()
        {
            // Example 11
            // Section: Extensions / Abbreviation
            //
            // The following Markdown:
            //     *[Foo]: foo
            //     *[Foo Bar]: foobar
            //     
            //     Foo B
            //
            // Should be rendered as:
            //     <p><abbr title="foo">Foo</abbr> B</p>

            Console.WriteLine("Example 11\nSection Extensions / Abbreviation\n");
            TestParser.TestSpec("*[Foo]: foo\n*[Foo Bar]: foobar\n\nFoo B", "<p><abbr title=\"foo\">Foo</abbr> B</p>", "abbreviations|advanced");
        }
    }
}
