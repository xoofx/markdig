
// --------------------------------
//               Yaml
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.Yaml
{
    [TestFixture]
    public class TestExtensionsYAMLFrontmatterDiscard
    {
        // # Extensions
        // 
        // Adds support for YAML frontmatter parsing:
        // 
        // ## YAML frontmatter discard
        //  
        // If a frontmatter is present, it will not be rendered:
        [Test]
        public void ExtensionsYAMLFrontmatterDiscard_Example001()
        {
            // Example 1
            // Section: Extensions / YAML frontmatter discard
            //
            // The following Markdown:
            //     ---
            //     this: is a frontmatter
            //     ---
            //     This is a text
            //
            // Should be rendered as:
            //     <p>This is a text</p>

            TestParser.TestSpec("---\nthis: is a frontmatter\n---\nThis is a text", "<p>This is a text</p>", "yaml", context: "Example 1\nSection Extensions / YAML frontmatter discard\n");
        }

        // But if a frontmatter doesn't happen on the first line, it will be parse as regular Markdown content
        [Test]
        public void ExtensionsYAMLFrontmatterDiscard_Example002()
        {
            // Example 2
            // Section: Extensions / YAML frontmatter discard
            //
            // The following Markdown:
            //     This is a text1
            //     ---
            //     this: is a frontmatter
            //     ---
            //     This is a text2
            //
            // Should be rendered as:
            //     <h2>This is a text1</h2>
            //     <h2>this: is a frontmatter</h2>
            //     <p>This is a text2</p>

            TestParser.TestSpec("This is a text1\n---\nthis: is a frontmatter\n---\nThis is a text2", "<h2>This is a text1</h2>\n<h2>this: is a frontmatter</h2>\n<p>This is a text2</p>", "yaml", context: "Example 2\nSection Extensions / YAML frontmatter discard\n");
        }

        // It expects an exact 3 dashes `---`:
        [Test]
        public void ExtensionsYAMLFrontmatterDiscard_Example003()
        {
            // Example 3
            // Section: Extensions / YAML frontmatter discard
            //
            // The following Markdown:
            //     ----
            //     this: is a frontmatter
            //     ----
            //     This is a text
            //
            // Should be rendered as:
            //     <hr />
            //     <h2>this: is a frontmatter</h2>
            //     <p>This is a text</p>

            TestParser.TestSpec("----\nthis: is a frontmatter\n----\nThis is a text", "<hr />\n<h2>this: is a frontmatter</h2>\n<p>This is a text</p>", "yaml", context: "Example 3\nSection Extensions / YAML frontmatter discard\n");
        }

        // It can end with three dots `...`:
        [Test]
        public void ExtensionsYAMLFrontmatterDiscard_Example004()
        {
            // Example 4
            // Section: Extensions / YAML frontmatter discard
            //
            // The following Markdown:
            //     ---
            //     this: is a frontmatter
            //     
            //     ...
            //     This is a text
            //
            // Should be rendered as:
            //     <p>This is a text</p>

            TestParser.TestSpec("---\nthis: is a frontmatter\n\n...\nThis is a text", "<p>This is a text</p>", "yaml", context: "Example 4\nSection Extensions / YAML frontmatter discard\n");
        }

        // If the end front matter marker (`...` or `---`) is not present, it will render the `---` has a `<hr>`:
        [Test]
        public void ExtensionsYAMLFrontmatterDiscard_Example005()
        {
            // Example 5
            // Section: Extensions / YAML frontmatter discard
            //
            // The following Markdown:
            //     ---
            //     this: is a frontmatter
            //     This is a text
            //
            // Should be rendered as:
            //     <hr />
            //     <p>this: is a frontmatter
            //     This is a text</p>

            TestParser.TestSpec("---\nthis: is a frontmatter\nThis is a text", "<hr />\n<p>this: is a frontmatter\nThis is a text</p>", "yaml", context: "Example 5\nSection Extensions / YAML frontmatter discard\n");
        }

        // It expects exactly three dots `...`:
        [Test]
        public void ExtensionsYAMLFrontmatterDiscard_Example006()
        {
            // Example 6
            // Section: Extensions / YAML frontmatter discard
            //
            // The following Markdown:
            //     ---
            //     this: is a frontmatter
            //     ....
            //     This is a text
            //
            // Should be rendered as:
            //     <hr />
            //     <p>this: is a frontmatter
            //     ....
            //     This is a text</p>

            TestParser.TestSpec("---\nthis: is a frontmatter\n....\nThis is a text", "<hr />\n<p>this: is a frontmatter\n....\nThis is a text</p>", "yaml", context: "Example 6\nSection Extensions / YAML frontmatter discard\n");
        }

        // Front matter ends with the first line containing three dots `...` or three dashes `---`:
        [Test]
        public void ExtensionsYAMLFrontmatterDiscard_Example007()
        {
            // Example 7
            // Section: Extensions / YAML frontmatter discard
            //
            // The following Markdown:
            //     ---
            //     this: is a frontmatter
            //     ....
            //     
            //     Hello
            //     ---
            //     This is a text
            //
            // Should be rendered as:
            //     <p>This is a text</p>

            TestParser.TestSpec("---\nthis: is a frontmatter\n....\n\nHello\n---\nThis is a text", "<p>This is a text</p>", "yaml", context: "Example 7\nSection Extensions / YAML frontmatter discard\n");
        }

        // It expects whitespace can exist after the leading characters
        [Test]
        public void ExtensionsYAMLFrontmatterDiscard_Example008()
        {
            // Example 8
            // Section: Extensions / YAML frontmatter discard
            //
            // The following Markdown:
            //     ---   
            //     this: is a frontmatter
            //     ...
            //     This is a text
            //
            // Should be rendered as:
            //     <p>This is a text</p>

            TestParser.TestSpec("---   \nthis: is a frontmatter\n...\nThis is a text", "<p>This is a text</p>", "yaml", context: "Example 8\nSection Extensions / YAML frontmatter discard\n");
        }

        // It expects whitespace can exist after the trailing characters
        [Test]
        public void ExtensionsYAMLFrontmatterDiscard_Example009()
        {
            // Example 9
            // Section: Extensions / YAML frontmatter discard
            //
            // The following Markdown:
            //     ---
            //     this: is a frontmatter
            //     ...     
            //     This is a text
            //
            // Should be rendered as:
            //     <p>This is a text</p>

            TestParser.TestSpec("---\nthis: is a frontmatter\n...     \nThis is a text", "<p>This is a text</p>", "yaml", context: "Example 9\nSection Extensions / YAML frontmatter discard\n");
        }
    }
}
