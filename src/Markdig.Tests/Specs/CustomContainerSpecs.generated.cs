
// --------------------------------
//         Custom Containers
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.CustomContainers
{
    [TestFixture]
    public class TestExtensionsCustomContainer
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## Custom Container
        // 
        // A custom container is similar to a fenced code block, but it is using the character `:` to declare a block (with at least 3 characters), and instead of generating a `<pre><code>...</code></pre>` it will generate a `<div>...</div>` block.
        [Test]
        public void ExtensionsCustomContainer_Example001()
        {
            // Example 1
            // Section: Extensions / Custom Container
            //
            // The following Markdown:
            //     :::spoiler
            //     This is a *spoiler*
            //     :::
            //
            // Should be rendered as:
            //     <div class="spoiler"><p>This is a <em>spoiler</em></p>
            //     </div>

            TestParser.TestSpec(":::spoiler\nThis is a *spoiler*\n:::", "<div class=\"spoiler\"><p>This is a <em>spoiler</em></p>\n</div>", "customcontainers+attributes|advanced", context: "Example 1\nSection Extensions / Custom Container\n");
        }

        // The text following the opened custom container is optional:
        [Test]
        public void ExtensionsCustomContainer_Example002()
        {
            // Example 2
            // Section: Extensions / Custom Container
            //
            // The following Markdown:
            //     :::
            //     This is a regular div
            //     :::
            //
            // Should be rendered as:
            //     <div><p>This is a regular div</p>
            //     </div>

            TestParser.TestSpec(":::\nThis is a regular div\n:::", "<div><p>This is a regular div</p>\n</div>", "customcontainers+attributes|advanced", context: "Example 2\nSection Extensions / Custom Container\n");
        }

        // Like for fenced code block, you can use more than 3 `:` characters as long as the closing has the same number of characters:
        [Test]
        public void ExtensionsCustomContainer_Example003()
        {
            // Example 3
            // Section: Extensions / Custom Container
            //
            // The following Markdown:
            //     ::::::::::::spoiler
            //     This is a spoiler
            //     ::::::::::::
            //
            // Should be rendered as:
            //     <div class="spoiler"><p>This is a spoiler</p>
            //     </div>

            TestParser.TestSpec("::::::::::::spoiler\nThis is a spoiler\n::::::::::::", "<div class=\"spoiler\"><p>This is a spoiler</p>\n</div>", "customcontainers+attributes|advanced", context: "Example 3\nSection Extensions / Custom Container\n");
        }

        // Like for fenced code block, a custom container can span over multiple empty lines in a list block:
        [Test]
        public void ExtensionsCustomContainer_Example004()
        {
            // Example 4
            // Section: Extensions / Custom Container
            //
            // The following Markdown:
            //     - This is a list
            //       :::spoiler
            //       This is a spoiler
            //       - item1
            //       - item2
            //       :::
            //     - A second item in the list
            //
            // Should be rendered as:
            //     <ul>
            //     <li>This is a list
            //     <div class="spoiler">This is a spoiler
            //     <ul>
            //     <li>item1</li>
            //     <li>item2</li>
            //     </ul>
            //     </div>
            //     </li>
            //     <li>A second item in the list</li>
            //     </ul>

            TestParser.TestSpec("- This is a list\n  :::spoiler\n  This is a spoiler\n  - item1\n  - item2\n  :::\n- A second item in the list", "<ul>\n<li>This is a list\n<div class=\"spoiler\">This is a spoiler\n<ul>\n<li>item1</li>\n<li>item2</li>\n</ul>\n</div>\n</li>\n<li>A second item in the list</li>\n</ul>", "customcontainers+attributes|advanced", context: "Example 4\nSection Extensions / Custom Container\n");
        }

        // Attributes extension is also supported for Custom Container, as long as the Attributes extension is activated after the CustomContainer extension (`.UseCustomContainer().UseAttributes()`)
        [Test]
        public void ExtensionsCustomContainer_Example005()
        {
            // Example 5
            // Section: Extensions / Custom Container
            //
            // The following Markdown:
            //     :::spoiler {#myspoiler myprop=yes}
            //     This is a spoiler
            //     :::
            //
            // Should be rendered as:
            //     <div id="myspoiler" class="spoiler" myprop="yes"><p>This is a spoiler</p>
            //     </div>

            TestParser.TestSpec(":::spoiler {#myspoiler myprop=yes}\nThis is a spoiler\n:::", "<div id=\"myspoiler\" class=\"spoiler\" myprop=\"yes\"><p>This is a spoiler</p>\n</div>", "customcontainers+attributes|advanced", context: "Example 5\nSection Extensions / Custom Container\n");
        }

        // The content of a custom container can contain any blocks:
        [Test]
        public void ExtensionsCustomContainer_Example006()
        {
            // Example 6
            // Section: Extensions / Custom Container
            //
            // The following Markdown:
            //     :::mycontainer
            //     <p>This is a raw spoiler</p>
            //     :::
            //
            // Should be rendered as:
            //     <div class="mycontainer"><p>This is a raw spoiler</p>
            //     </div>

            TestParser.TestSpec(":::mycontainer\n<p>This is a raw spoiler</p>\n:::", "<div class=\"mycontainer\"><p>This is a raw spoiler</p>\n</div>", "customcontainers+attributes|advanced", context: "Example 6\nSection Extensions / Custom Container\n");
        }
    }

    [TestFixture]
    public class TestExtensionsInlineCustomContainer
    {
        // ## Inline Custom Container 
        // 
        // A custom container can also be used within an inline container (e.g: paragraph, heading...) by enclosing a text by a new emphasis `::`
        [Test]
        public void ExtensionsInlineCustomContainer_Example007()
        {
            // Example 7
            // Section: Extensions / Inline Custom Container 
            //
            // The following Markdown:
            //     This is a text ::with special emphasis::
            //
            // Should be rendered as:
            //     <p>This is a text <span>with special emphasis</span></p>

            TestParser.TestSpec("This is a text ::with special emphasis::", "<p>This is a text <span>with special emphasis</span></p>", "customcontainers+attributes|advanced", context: "Example 7\nSection Extensions / Inline Custom Container \n");
        }

        // Any other emphasis inline can be used within this emphasis inline container:
        [Test]
        public void ExtensionsInlineCustomContainer_Example008()
        {
            // Example 8
            // Section: Extensions / Inline Custom Container 
            //
            // The following Markdown:
            //     This is a text ::with special *emphasis*::
            //
            // Should be rendered as:
            //     <p>This is a text <span>with special <em>emphasis</em></span></p>

            TestParser.TestSpec("This is a text ::with special *emphasis*::", "<p>This is a text <span>with special <em>emphasis</em></span></p>", "customcontainers+attributes|advanced", context: "Example 8\nSection Extensions / Inline Custom Container \n");
        }

        // Attributes can be attached to a inline custom container:
        [Test]
        public void ExtensionsInlineCustomContainer_Example009()
        {
            // Example 9
            // Section: Extensions / Inline Custom Container 
            //
            // The following Markdown:
            //     This is a text ::with special emphasis::{#myId .myemphasis}
            //
            // Should be rendered as:
            //     <p>This is a text <span id="myId" class="myemphasis">with special emphasis</span></p>

            TestParser.TestSpec("This is a text ::with special emphasis::{#myId .myemphasis}", "<p>This is a text <span id=\"myId\" class=\"myemphasis\">with special emphasis</span></p>", "customcontainers+attributes|advanced", context: "Example 9\nSection Extensions / Inline Custom Container \n");
        }
    }
}
