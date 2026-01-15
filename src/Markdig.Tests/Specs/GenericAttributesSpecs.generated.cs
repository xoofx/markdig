
// --------------------------------
//        Generic Attributes
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.GenericAttributes
{
    [TestFixture]
    public class TestExtensionsGenericAttributes
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## Generic Attributes
        // 
        // Attributes can be attached to:
        // - The previous inline element if the previous element is not a literal
        // - The next block if the current block is a paragraph and the attributes is the only inline present in the paragraph
        // - Or the current block
        // 
        // Attributes can be of 3 kinds:
        // 
        // - An id element, starting by `#` that will be used to set the `id` property of the HTML element
        // - A class element, starting by `.` that will be appended to the CSS class property of the HTML element
        // - a `name=value` or `name="value"` that will be appended as an attribute of the HTML element
        // 
        // The following shows that attributes is attached to the current block or the previous inline:
        [Test]
        public void ExtensionsGenericAttributes_Example001()
        {
            // Example 1
            // Section: Extensions / Generic Attributes
            //
            // The following Markdown:
            //     # This is a heading with an an attribute{#heading-link}
            //     
            //     # This is a heading # {#heading-link2}
            //     
            //     [This is a link](http://google.com){#a-link .myclass data-lang=fr data-value="This is a value"}
            //     
            //     This is a heading{#heading-link2}
            //     -----------------
            //     
            //     This is a paragraph with an attached attributes {#myparagraph attached-bool-property attached-bool-property2}
            //
            // Should be rendered as:
            //     <h1 id="heading-link">This is a heading with an an attribute</h1>
            //     <h1 id="heading-link2">This is a heading</h1>
            //     <p><a href="http://google.com" id="a-link" class="myclass" data-lang="fr" data-value="This is a value">This is a link</a></p>
            //     <h2 id="heading-link2">This is a heading</h2>
            //     <p id="myparagraph" attached-bool-property="" attached-bool-property2="">This is a paragraph with an attached attributes </p>

            TestParser.TestSpec("# This is a heading with an an attribute{#heading-link}\n\n# This is a heading # {#heading-link2}\n\n[This is a link](http://google.com){#a-link .myclass data-lang=fr data-value=\"This is a value\"}\n\nThis is a heading{#heading-link2}\n-----------------\n\nThis is a paragraph with an attached attributes {#myparagraph attached-bool-property attached-bool-property2}", "<h1 id=\"heading-link\">This is a heading with an an attribute</h1>\n<h1 id=\"heading-link2\">This is a heading</h1>\n<p><a href=\"http://google.com\" id=\"a-link\" class=\"myclass\" data-lang=\"fr\" data-value=\"This is a value\">This is a link</a></p>\n<h2 id=\"heading-link2\">This is a heading</h2>\n<p id=\"myparagraph\" attached-bool-property=\"\" attached-bool-property2=\"\">This is a paragraph with an attached attributes </p>", "attributes|advanced", context: "Example 1\nSection Extensions / Generic Attributes\n");
        }

        // The following shows that attributes can be attached to the next block if they are used inside a single line just preceding the block (and preceded by a blank line or beginning of a block container):
        [Test]
        public void ExtensionsGenericAttributes_Example002()
        {
            // Example 2
            // Section: Extensions / Generic Attributes
            //
            // The following Markdown:
            //     {#fenced-id .fenced-class}
            //     ~~~
            //     This is a fenced with attached attributes
            //     ~~~ 
            //
            // Should be rendered as:
            //     <pre><code id="fenced-id" class="fenced-class">This is a fenced with attached attributes
            //     </code></pre>

            TestParser.TestSpec("{#fenced-id .fenced-class}\n~~~\nThis is a fenced with attached attributes\n~~~ ", "<pre><code id=\"fenced-id\" class=\"fenced-class\">This is a fenced with attached attributes\n</code></pre>", "attributes|advanced", context: "Example 2\nSection Extensions / Generic Attributes\n");
        }

        // Attribute values can be one character long
        [Test]
        public void ExtensionsGenericAttributes_Example003()
        {
            // Example 3
            // Section: Extensions / Generic Attributes
            //
            // The following Markdown:
            //     [Foo](url){data-x=1}
            //     
            //     [Foo](url){data-x='1'}
            //     
            //     [Foo](url){data-x=11}
            //
            // Should be rendered as:
            //     <p><a href="url" data-x="1">Foo</a></p>
            //     <p><a href="url" data-x="1">Foo</a></p>
            //     <p><a href="url" data-x="11">Foo</a></p>

            TestParser.TestSpec("[Foo](url){data-x=1}\n\n[Foo](url){data-x='1'}\n\n[Foo](url){data-x=11}", "<p><a href=\"url\" data-x=\"1\">Foo</a></p>\n<p><a href=\"url\" data-x=\"1\">Foo</a></p>\n<p><a href=\"url\" data-x=\"11\">Foo</a></p>", "attributes|advanced", context: "Example 3\nSection Extensions / Generic Attributes\n");
        }

        // Attributes that occur immediately before a block element, on a line by themselves, affect that element
        [Test]
        public void ExtensionsGenericAttributes_Example004()
        {
            // Example 4
            // Section: Extensions / Generic Attributes
            //
            // The following Markdown:
            //     {.center}
            //     A paragraph
            //
            // Should be rendered as:
            //     <p class="center">A paragraph</p>

            TestParser.TestSpec("{.center}\nA paragraph", "<p class=\"center\">A paragraph</p>", "attributes|advanced", context: "Example 4\nSection Extensions / Generic Attributes\n");
        }
    }

    [TestFixture]
    public class TestExtensionsEscapedQuotesInAttributeValues
    {
        // ## Escaped Quotes in Attribute Values
        // 
        // Backslash escapes are supported in quoted attribute values, following CommonMark conventions.
        // Escaped quotes (\") allow including quote characters within quoted values.
        // 
        // Escaped double quotes inside double-quoted value:
        [Test]
        public void ExtensionsEscapedQuotesInAttributeValues_Example005()
        {
            // Example 5
            // Section: Extensions / Escaped Quotes in Attribute Values
            //
            // The following Markdown:
            //     [Link](url){data-value="hello \"world\""}
            //
            // Should be rendered as:
            //     <p><a href="url" data-value="hello &quot;world&quot;">Link</a></p>

            TestParser.TestSpec("[Link](url){data-value=\"hello \\\"world\\\"\"}", "<p><a href=\"url\" data-value=\"hello &quot;world&quot;\">Link</a></p>", "attributes|advanced", context: "Example 5\nSection Extensions / Escaped Quotes in Attribute Values\n");
        }

        // Escaped single quotes inside single-quoted value:
        [Test]
        public void ExtensionsEscapedQuotesInAttributeValues_Example006()
        {
            // Example 6
            // Section: Extensions / Escaped Quotes in Attribute Values
            //
            // The following Markdown:
            //     [Link](url){data-value='hello \'world\''}
            //
            // Should be rendered as:
            //     <p><a href="url" data-value="hello 'world'">Link</a></p>

            TestParser.TestSpec("[Link](url){data-value='hello \\'world\\''}", "<p><a href=\"url\" data-value=\"hello 'world'\">Link</a></p>", "attributes|advanced", context: "Example 6\nSection Extensions / Escaped Quotes in Attribute Values\n");
        }

        // Mixed quotes (no escaping needed, existing behavior):
        [Test]
        public void ExtensionsEscapedQuotesInAttributeValues_Example007()
        {
            // Example 7
            // Section: Extensions / Escaped Quotes in Attribute Values
            //
            // The following Markdown:
            //     [Link](url){data-value='hello "world"'}
            //
            // Should be rendered as:
            //     <p><a href="url" data-value="hello &quot;world&quot;">Link</a></p>

            TestParser.TestSpec("[Link](url){data-value='hello \"world\"'}", "<p><a href=\"url\" data-value=\"hello &quot;world&quot;\">Link</a></p>", "attributes|advanced", context: "Example 7\nSection Extensions / Escaped Quotes in Attribute Values\n");
        }

        // Escaped backslash followed by quote (backslash is escaped, quote closes the value):
        [Test]
        public void ExtensionsEscapedQuotesInAttributeValues_Example008()
        {
            // Example 8
            // Section: Extensions / Escaped Quotes in Attribute Values
            //
            // The following Markdown:
            //     [Link](url){data-value="path\\"}
            //
            // Should be rendered as:
            //     <p><a href="url" data-value="path\">Link</a></p>

            TestParser.TestSpec("[Link](url){data-value=\"path\\\\\"}", "<p><a href=\"url\" data-value=\"path\\\">Link</a></p>", "attributes|advanced", context: "Example 8\nSection Extensions / Escaped Quotes in Attribute Values\n");
        }

        // Backslash not followed by escapable character (preserved as-is):
        [Test]
        public void ExtensionsEscapedQuotesInAttributeValues_Example009()
        {
            // Example 9
            // Section: Extensions / Escaped Quotes in Attribute Values
            //
            // The following Markdown:
            //     [Link](url){data-value="hello\nworld"}
            //
            // Should be rendered as:
            //     <p><a href="url" data-value="hello\nworld">Link</a></p>

            TestParser.TestSpec("[Link](url){data-value=\"hello\\nworld\"}", "<p><a href=\"url\" data-value=\"hello\\nworld\">Link</a></p>", "attributes|advanced", context: "Example 9\nSection Extensions / Escaped Quotes in Attribute Values\n");
        }

        // Complex escape sequence (Word field code use case):
        [Test]
        public void ExtensionsEscapedQuotesInAttributeValues_Example010()
        {
            // Example 10
            // Section: Extensions / Escaped Quotes in Attribute Values
            //
            // The following Markdown:
            //     [Date](){field="DATE \\@ \"MMMM d, yyyy\""}
            //
            // Should be rendered as:
            //     <p><a href="" field="DATE \@ &quot;MMMM d, yyyy&quot;">Date</a></p>

            TestParser.TestSpec("[Date](){field=\"DATE \\\\@ \\\"MMMM d, yyyy\\\"\"}", "<p><a href=\"\" field=\"DATE \\@ &quot;MMMM d, yyyy&quot;\">Date</a></p>", "attributes|advanced", context: "Example 10\nSection Extensions / Escaped Quotes in Attribute Values\n");
        }
    }
}
