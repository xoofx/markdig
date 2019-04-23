// Generated: 2019-04-15 05:06:35

// --------------------------------
//         Definition Lists
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.DefinitionLists
{
    [TestFixture]
    public class TestExtensionsDefinitionLists
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## Definition lists
        // 
        // A custom container is similar to a fenced code block, but it is using the character `:` to declare a block (with at least 3 characters), and instead of generating a `<pre><code>...</code></pre>` it will generate a `<div>...</div>` block.
        [Test]
        public void ExtensionsDefinitionLists_Example001()
        {
            // Example 1
            // Section: Extensions / Definition lists
            //
            // The following Markdown:
            //     
            //     Term 1
            //     :   This is a definition item
            //         With a paragraph
            //         > This is a block quote
            //     
            //         - This is a list
            //         - with an item2
            //     
            //         ```java
            //         Test
            //     
            //     
            //         ```
            //     
            //         And a last line
            //     :   This ia another definition item
            //     
            //     Term2
            //     Term3 *with some inline*
            //     :   This is another definition for term2
            //
            // Should be rendered as:
            //     <dl>
            //     <dt>Term 1</dt>
            //     <dd><p>This is a definition item
            //     With a paragraph</p>
            //     <blockquote>
            //     <p>This is a block quote</p>
            //     </blockquote>
            //     <ul>
            //     <li>This is a list</li>
            //     <li>with an item2</li>
            //     </ul>
            //     <pre><code class="language-java">Test
            //     
            //     
            //     </code></pre>
            //     <p>And a last line</p>
            //     </dd>
            //     <dd>This ia another definition item</dd>
            //     <dt>Term2</dt>
            //     <dt>Term3 <em>with some inline</em></dt>
            //     <dd>This is another definition for term2</dd>
            //     </dl>

            Console.WriteLine("Example 1\nSection Extensions / Definition lists\n");
            TestParser.TestSpec("\nTerm 1\n:   This is a definition item\n    With a paragraph\n    > This is a block quote\n\n    - This is a list\n    - with an item2\n\n    ```java\n    Test\n\n\n    ```\n\n    And a last line\n:   This ia another definition item\n\nTerm2\nTerm3 *with some inline*\n:   This is another definition for term2", "<dl>\n<dt>Term 1</dt>\n<dd><p>This is a definition item\nWith a paragraph</p>\n<blockquote>\n<p>This is a block quote</p>\n</blockquote>\n<ul>\n<li>This is a list</li>\n<li>with an item2</li>\n</ul>\n<pre><code class=\"language-java\">Test\n\n\n</code></pre>\n<p>And a last line</p>\n</dd>\n<dd>This ia another definition item</dd>\n<dt>Term2</dt>\n<dt>Term3 <em>with some inline</em></dt>\n<dd>This is another definition for term2</dd>\n</dl>", "definitionlists+attributes|advanced");
        }

        // A definition term can be followed at most by one blank line. Lazy continuations are supported for definitions:
        [Test]
        public void ExtensionsDefinitionLists_Example002()
        {
            // Example 2
            // Section: Extensions / Definition lists
            //
            // The following Markdown:
            //     Term 1
            //     
            //     :   Definition
            //     with lazy continuation.
            //     
            //         Second paragraph of the definition.
            //
            // Should be rendered as:
            //     <dl>
            //     <dt>Term 1</dt>
            //     <dd><p>Definition
            //     with lazy continuation.</p>
            //     <p>Second paragraph of the definition.</p>
            //     </dd>
            //     </dl>

            Console.WriteLine("Example 2\nSection Extensions / Definition lists\n");
            TestParser.TestSpec("Term 1\n\n:   Definition\nwith lazy continuation.\n\n    Second paragraph of the definition.", "<dl>\n<dt>Term 1</dt>\n<dd><p>Definition\nwith lazy continuation.</p>\n<p>Second paragraph of the definition.</p>\n</dd>\n</dl>", "definitionlists+attributes|advanced");
        }

        // The definition must be indented to 4 characters including the `:`. 
        [Test]
        public void ExtensionsDefinitionLists_Example003()
        {
            // Example 3
            // Section: Extensions / Definition lists
            //
            // The following Markdown:
            //     Term 1
            //     :  Invalid with less than 3 characters
            //
            // Should be rendered as:
            //     <p>Term 1
            //     :  Invalid with less than 3 characters</p>

            Console.WriteLine("Example 3\nSection Extensions / Definition lists\n");
            TestParser.TestSpec("Term 1\n:  Invalid with less than 3 characters", "<p>Term 1\n:  Invalid with less than 3 characters</p>", "definitionlists+attributes|advanced");
        }

        // The `:` can be indented up to 3 spaces:
        [Test]
        public void ExtensionsDefinitionLists_Example004()
        {
            // Example 4
            // Section: Extensions / Definition lists
            //
            // The following Markdown:
            //     Term 1
            //        : Valid even if `:` starts at most 3 spaces
            //
            // Should be rendered as:
            //     <dl>
            //     <dt>Term 1</dt>
            //     <dd>Valid even if <code>:</code> starts at most 3 spaces</dd>
            //     </dl>

            Console.WriteLine("Example 4\nSection Extensions / Definition lists\n");
            TestParser.TestSpec("Term 1\n   : Valid even if `:` starts at most 3 spaces", "<dl>\n<dt>Term 1</dt>\n<dd>Valid even if <code>:</code> starts at most 3 spaces</dd>\n</dl>", "definitionlists+attributes|advanced");
        }

        // But more than 3 spaces before `:` will trigger an indented code block:
        [Test]
        public void ExtensionsDefinitionLists_Example005()
        {
            // Example 5
            // Section: Extensions / Definition lists
            //
            // The following Markdown:
            //     Term 1
            //     
            //         : Not valid
            //
            // Should be rendered as:
            //     <p>Term 1</p>
            //     <pre><code>: Not valid
            //     </code></pre>

            Console.WriteLine("Example 5\nSection Extensions / Definition lists\n");
            TestParser.TestSpec("Term 1\n\n    : Not valid", "<p>Term 1</p>\n<pre><code>: Not valid\n</code></pre>", "definitionlists+attributes|advanced");
        }

        // Definition lists can be nested inside list items
        [Test]
        public void ExtensionsDefinitionLists_Example006()
        {
            // Example 6
            // Section: Extensions / Definition lists
            //
            // The following Markdown:
            //     1.  First
            //         
            //     2.  Second
            //         
            //         Term 1
            //         :   Definition
            //         
            //         Term 2
            //         :   Second Definition
            //
            // Should be rendered as:
            //     <ol>
            //     <li><p>First</p></li>
            //     <li><p>Second</p>
            //     <dl>
            //     <dt>Term 1</dt>
            //     <dd>Definition</dd>
            //     <dt>Term 2</dt>
            //     <dd>Second Definition</dd>
            //     </dl></li>
            //     </ol>

            Console.WriteLine("Example 6\nSection Extensions / Definition lists\n");
            TestParser.TestSpec("1.  First\n    \n2.  Second\n    \n    Term 1\n    :   Definition\n    \n    Term 2\n    :   Second Definition", "<ol>\n<li><p>First</p></li>\n<li><p>Second</p>\n<dl>\n<dt>Term 1</dt>\n<dd>Definition</dd>\n<dt>Term 2</dt>\n<dd>Second Definition</dd>\n</dl></li>\n</ol>", "definitionlists+attributes|advanced");
        }
    }
}
