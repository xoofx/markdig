
// --------------------------------
//         Table of Contents
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.TableOfContents
{
    [TestFixture]
    public class TestExtensionsTableOfContents
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## Table of Contents
        // 
        // The table of contents extension replaces a paragraph that consists of `[TOC]` alone with a nested list of the headings of the document. It is enabled with `UseTableOfContents()`. The examples also enable `UseAutoIdentifiers()`, which gives every heading an ID.
        // 
        // The list is wrapped in a `nav` element with the `doc-toc` role. An entry contains the text of its heading, and the entries of the headings below it form a nested list:
        [Test]
        public void ExtensionsTableOfContents_Example001()
        {
            // Example 1
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     [TOC]
            //     
            //     # Foo
            //     ## Bar
            //     ## Baz
            //
            // Should be rendered as:
            //     <nav class="toc" role="doc-toc">
            //     <ul>
            //     <li>Foo
            //     <ul>
            //     <li>Bar</li>
            //     <li>Baz</li>
            //     </ul>
            //     </li>
            //     </ul>
            //     </nav>
            //     <h1 id="foo">Foo</h1>
            //     <h2 id="bar">Bar</h2>
            //     <h2 id="baz">Baz</h2>

            TestParser.TestSpec("[TOC]\n\n# Foo\n## Bar\n## Baz", "<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>Foo\n<ul>\n<li>Bar</li>\n<li>Baz</li>\n</ul>\n</li>\n</ul>\n</nav>\n<h1 id=\"foo\">Foo</h1>\n<h2 id=\"bar\">Bar</h2>\n<h2 id=\"baz\">Baz</h2>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 1\nSection Extensions / Table of Contents\n");
        }

        // Every heading of the document is listed, including the headings before the marker:
        [Test]
        public void ExtensionsTableOfContents_Example002()
        {
            // Example 2
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     # Intro
            //     
            //     [TOC]
            //     
            //     ## Usage
            //
            // Should be rendered as:
            //     <h1 id="intro">Intro</h1>
            //     <nav class="toc" role="doc-toc">
            //     <ul>
            //     <li>Intro
            //     <ul>
            //     <li>Usage</li>
            //     </ul>
            //     </li>
            //     </ul>
            //     </nav>
            //     <h2 id="usage">Usage</h2>

            TestParser.TestSpec("# Intro\n\n[TOC]\n\n## Usage", "<h1 id=\"intro\">Intro</h1>\n<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>Intro\n<ul>\n<li>Usage</li>\n</ul>\n</li>\n</ul>\n</nav>\n<h2 id=\"usage\">Usage</h2>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 2\nSection Extensions / Table of Contents\n");
        }

        // A heading is nested below the closest preceding heading with a lower level, so a skipped level produces no empty entry:
        [Test]
        public void ExtensionsTableOfContents_Example003()
        {
            // Example 3
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     [TOC]
            //     
            //     # Foo
            //     ### Bar
            //     ## Baz
            //
            // Should be rendered as:
            //     <nav class="toc" role="doc-toc">
            //     <ul>
            //     <li>Foo
            //     <ul>
            //     <li>Bar</li>
            //     <li>Baz</li>
            //     </ul>
            //     </li>
            //     </ul>
            //     </nav>
            //     <h1 id="foo">Foo</h1>
            //     <h3 id="bar">Bar</h3>
            //     <h2 id="baz">Baz</h2>

            TestParser.TestSpec("[TOC]\n\n# Foo\n### Bar\n## Baz", "<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>Foo\n<ul>\n<li>Bar</li>\n<li>Baz</li>\n</ul>\n</li>\n</ul>\n</nav>\n<h1 id=\"foo\">Foo</h1>\n<h3 id=\"bar\">Bar</h3>\n<h2 id=\"baz\">Baz</h2>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 3\nSection Extensions / Table of Contents\n");
        }

        // A heading without such a preceding heading is a top-level entry, even if its level is higher than the level of a later heading:
        [Test]
        public void ExtensionsTableOfContents_Example004()
        {
            // Example 4
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     [TOC]
            //     
            //     ### Foo
            //     # Bar
            //     ## Baz
            //
            // Should be rendered as:
            //     <nav class="toc" role="doc-toc">
            //     <ul>
            //     <li>Foo</li>
            //     <li>Bar
            //     <ul>
            //     <li>Baz</li>
            //     </ul>
            //     </li>
            //     </ul>
            //     </nav>
            //     <h3 id="foo">Foo</h3>
            //     <h1 id="bar">Bar</h1>
            //     <h2 id="baz">Baz</h2>

            TestParser.TestSpec("[TOC]\n\n### Foo\n# Bar\n## Baz", "<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>Foo</li>\n<li>Bar\n<ul>\n<li>Baz</li>\n</ul>\n</li>\n</ul>\n</nav>\n<h3 id=\"foo\">Foo</h3>\n<h1 id=\"bar\">Bar</h1>\n<h2 id=\"baz\">Baz</h2>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 4\nSection Extensions / Table of Contents\n");
        }

        // Headings inside container blocks are listed too. Headings without content are left out:
        [Test]
        public void ExtensionsTableOfContents_Example005()
        {
            // Example 5
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     [TOC]
            //     
            //     > # Foo
            //     
            //     - ## Bar
            //     
            //     #
            //
            // Should be rendered as:
            //     <nav class="toc" role="doc-toc">
            //     <ul>
            //     <li>Foo
            //     <ul>
            //     <li>Bar</li>
            //     </ul>
            //     </li>
            //     </ul>
            //     </nav>
            //     <blockquote>
            //     <h1 id="foo">Foo</h1>
            //     </blockquote>
            //     <ul>
            //     <li>
            //     <h2 id="bar">Bar</h2>
            //     </li>
            //     </ul>
            //     <h1 id="section"></h1>

            TestParser.TestSpec("[TOC]\n\n> # Foo\n\n- ## Bar\n\n#", "<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>Foo\n<ul>\n<li>Bar</li>\n</ul>\n</li>\n</ul>\n</nav>\n<blockquote>\n<h1 id=\"foo\">Foo</h1>\n</blockquote>\n<ul>\n<li>\n<h2 id=\"bar\">Bar</h2>\n</li>\n</ul>\n<h1 id=\"section\"></h1>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 5\nSection Extensions / Table of Contents\n");
        }

        // An entry contains the plain text of its heading. The formatting is dropped, so that all entries look the same:
        [Test]
        public void ExtensionsTableOfContents_Example006()
        {
            // Example 6
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     [TOC]
            //     
            //     # Use *the* `<T>` & co
            //
            // Should be rendered as:
            //     <nav class="toc" role="doc-toc">
            //     <ul>
            //     <li>Use the &lt;T&gt; &amp; co</li>
            //     </ul>
            //     </nav>
            //     <h1 id="use-the-t-co">Use <em>the</em> <code>&lt;T&gt;</code> &amp; co</h1>

            TestParser.TestSpec("[TOC]\n\n# Use *the* `<T>` & co", "<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>Use the &lt;T&gt; &amp; co</li>\n</ul>\n</nav>\n<h1 id=\"use-the-t-co\">Use <em>the</em> <code>&lt;T&gt;</code> &amp; co</h1>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 6\nSection Extensions / Table of Contents\n");
        }

        // Headings with the same text get an entry each:
        [Test]
        public void ExtensionsTableOfContents_Example007()
        {
            // Example 7
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     [TOC]
            //     
            //     # Foo
            //     # Foo
            //
            // Should be rendered as:
            //     <nav class="toc" role="doc-toc">
            //     <ul>
            //     <li>Foo</li>
            //     <li>Foo</li>
            //     </ul>
            //     </nav>
            //     <h1 id="foo">Foo</h1>
            //     <h1 id="foo-1">Foo</h1>

            TestParser.TestSpec("[TOC]\n\n# Foo\n# Foo", "<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>Foo</li>\n<li>Foo</li>\n</ul>\n</nav>\n<h1 id=\"foo\">Foo</h1>\n<h1 id=\"foo-1\">Foo</h1>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 7\nSection Extensions / Table of Contents\n");
        }

        // Every marker of a document renders the same list:
        [Test]
        public void ExtensionsTableOfContents_Example008()
        {
            // Example 8
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     [TOC]
            //     
            //     # Foo
            //     
            //     [TOC]
            //
            // Should be rendered as:
            //     <nav class="toc" role="doc-toc">
            //     <ul>
            //     <li>Foo</li>
            //     </ul>
            //     </nav>
            //     <h1 id="foo">Foo</h1>
            //     <nav class="toc" role="doc-toc">
            //     <ul>
            //     <li>Foo</li>
            //     </ul>
            //     </nav>

            TestParser.TestSpec("[TOC]\n\n# Foo\n\n[TOC]", "<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>Foo</li>\n</ul>\n</nav>\n<h1 id=\"foo\">Foo</h1>\n<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>Foo</li>\n</ul>\n</nav>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 8\nSection Extensions / Table of Contents\n");
        }

        // A marker inside a container block is recognized as well:
        [Test]
        public void ExtensionsTableOfContents_Example009()
        {
            // Example 9
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     - [TOC]
            //     
            //     > [TOC]
            //     
            //     # Foo
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <nav class="toc" role="doc-toc">
            //     <ul>
            //     <li>Foo</li>
            //     </ul>
            //     </nav>
            //     </li>
            //     </ul>
            //     <blockquote>
            //     <nav class="toc" role="doc-toc">
            //     <ul>
            //     <li>Foo</li>
            //     </ul>
            //     </nav>
            //     </blockquote>
            //     <h1 id="foo">Foo</h1>

            TestParser.TestSpec("- [TOC]\n\n> [TOC]\n\n# Foo", "<ul>\n<li>\n<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>Foo</li>\n</ul>\n</nav>\n</li>\n</ul>\n<blockquote>\n<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>Foo</li>\n</ul>\n</nav>\n</blockquote>\n<h1 id=\"foo\">Foo</h1>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 9\nSection Extensions / Table of Contents\n");
        }

        // The marker needs no blank line after it when the next line starts a block that interrupts a paragraph:
        [Test]
        public void ExtensionsTableOfContents_Example010()
        {
            // Example 10
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     [TOC]
            //     # Foo
            //
            // Should be rendered as:
            //     <nav class="toc" role="doc-toc">
            //     <ul>
            //     <li>Foo</li>
            //     </ul>
            //     </nav>
            //     <h1 id="foo">Foo</h1>

            TestParser.TestSpec("[TOC]\n# Foo", "<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>Foo</li>\n</ul>\n</nav>\n<h1 id=\"foo\">Foo</h1>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 10\nSection Extensions / Table of Contents\n");
        }

        // A document without headings renders no table of contents:
        [Test]
        public void ExtensionsTableOfContents_Example011()
        {
            // Example 11
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     [TOC]
            //     
            //     Text
            //
            // Should be rendered as:
            //     <p>Text</p>

            TestParser.TestSpec("[TOC]\n\nText", "<p>Text</p>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 11\nSection Extensions / Table of Contents\n");
        }

        // The marker takes precedence over a link to a heading with the text `TOC`:
        [Test]
        public void ExtensionsTableOfContents_Example012()
        {
            // Example 12
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     [TOC]
            //     
            //     # TOC
            //
            // Should be rendered as:
            //     <nav class="toc" role="doc-toc">
            //     <ul>
            //     <li>TOC</li>
            //     </ul>
            //     </nav>
            //     <h1 id="toc">TOC</h1>

            TestParser.TestSpec("[TOC]\n\n# TOC", "<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>TOC</li>\n</ul>\n</nav>\n<h1 id=\"toc\">TOC</h1>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 12\nSection Extensions / Table of Contents\n");
        }

        // It also takes precedence over a link reference definition with the label `TOC`. An escaped bracket keeps the text, and an explicit collapsed reference keeps the link:
        [Test]
        public void ExtensionsTableOfContents_Example013()
        {
            // Example 13
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     [TOC]
            //     
            //     # Foo
            //     
            //     \[TOC]
            //     
            //     [TOC][]
            //     
            //     [TOC]: /url
            //
            // Should be rendered as:
            //     <nav class="toc" role="doc-toc">
            //     <ul>
            //     <li>Foo</li>
            //     </ul>
            //     </nav>
            //     <h1 id="foo">Foo</h1>
            //     <p>[TOC]</p>
            //     <p><a href="/url">TOC</a></p>

            TestParser.TestSpec("[TOC]\n\n# Foo\n\n\\[TOC]\n\n[TOC][]\n\n[TOC]: /url", "<nav class=\"toc\" role=\"doc-toc\">\n<ul>\n<li>Foo</li>\n</ul>\n</nav>\n<h1 id=\"foo\">Foo</h1>\n<p>[TOC]</p>\n<p><a href=\"/url\">TOC</a></p>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 13\nSection Extensions / Table of Contents\n");
        }

        // Only a paragraph that consists of `[TOC]` alone is a marker. More text in the paragraph, another casing or spacing, and a code span render as usual:
        [Test]
        public void ExtensionsTableOfContents_Example014()
        {
            // Example 14
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     [TOC] text
            //     
            //     [TOC]
            //     text
            //     
            //     [toc]
            //     
            //     [ TOC ]
            //     
            //     `[TOC]`
            //     
            //     # Foo
            //
            // Should be rendered as:
            //     <p>[TOC] text</p>
            //     <p>[TOC]
            //     text</p>
            //     <p>[toc]</p>
            //     <p>[ TOC ]</p>
            //     <p><code>[TOC]</code></p>
            //     <h1 id="foo">Foo</h1>

            TestParser.TestSpec("[TOC] text\n\n[TOC]\ntext\n\n[toc]\n\n[ TOC ]\n\n`[TOC]`\n\n# Foo", "<p>[TOC] text</p>\n<p>[TOC]\ntext</p>\n<p>[toc]</p>\n<p>[ TOC ]</p>\n<p><code>[TOC]</code></p>\n<h1 id=\"foo\">Foo</h1>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 14\nSection Extensions / Table of Contents\n");
        }

        // A lazy continuation line belongs to the paragraph before it:
        [Test]
        public void ExtensionsTableOfContents_Example015()
        {
            // Example 15
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //     > Foo
            //     [TOC]
            //     
            //     # Bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>Foo
            //     [TOC]</p>
            //     </blockquote>
            //     <h1 id="bar">Bar</h1>

            TestParser.TestSpec("> Foo\n[TOC]\n\n# Bar", "<blockquote>\n<p>Foo\n[TOC]</p>\n</blockquote>\n<h1 id=\"bar\">Bar</h1>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 15\nSection Extensions / Table of Contents\n");
        }

        // An indented code block and a setext heading are not markers either:
        [Test]
        public void ExtensionsTableOfContents_Example016()
        {
            // Example 16
            // Section: Extensions / Table of Contents
            //
            // The following Markdown:
            //         [TOC]
            //     
            //     [TOC]
            //     =====
            //
            // Should be rendered as:
            //     <pre><code>[TOC]
            //     </code></pre>
            //     <h1 id="toc">[TOC]</h1>

            TestParser.TestSpec("    [TOC]\n\n[TOC]\n=====", "<pre><code>[TOC]\n</code></pre>\n<h1 id=\"toc\">[TOC]</h1>", "autoidentifiers+tableofcontents|advanced+tableofcontents", context: "Example 16\nSection Extensions / Table of Contents\n");
        }
    }
}
