
// --------------------------------
//             Footnotes
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.Footnotes
{
    [TestFixture]
    public class TestExtensionsFootnotes
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## Footnotes
        // 
        // Allows footnotes using the following syntax (taken from pandoc example):
        [Test]
        public void ExtensionsFootnotes_Example001()
        {
            // Example 1
            // Section: Extensions / Footnotes
            //
            // The following Markdown:
            //     Here is a footnote reference,[^1] and another.[^longnote]
            //     
            //     This is another reference to [^1]
            //     
            //     [^1]: Here is the footnote.
            //     
            //     And another reference to [^longnote]
            //     
            //     [^longnote]: Here's one with multiple blocks.
            //     
            //         Subsequent paragraphs are indented to show that they
            //     belong to the previous footnote.
            //     
            //         > This is a block quote
            //         > Inside a footnote
            //     
            //             { some.code }
            //     
            //         The whole paragraph can be indented, or just the first
            //         line.  In this way, multi-paragraph footnotes work like
            //         multi-paragraph list items.
            //     
            //     This paragraph won't be part of the note, because it
            //     isn't indented.
            //
            // Should be rendered as:
            //     <p>Here is a footnote reference,<a id="fnref:1" href="#fn:1" class="footnote-ref"><sup>1</sup></a> and another.<a id="fnref:3" href="#fn:2" class="footnote-ref"><sup>2</sup></a></p>
            //     <p>This is another reference to <a id="fnref:2" href="#fn:1" class="footnote-ref"><sup>1</sup></a></p>
            //     <p>And another reference to <a id="fnref:4" href="#fn:2" class="footnote-ref"><sup>2</sup></a></p>
            //     <p>This paragraph won't be part of the note, because it
            //     isn't indented.</p>
            //     <div class="footnotes">
            //     <hr />
            //     <ol>
            //     <li id="fn:1">
            //     <p>Here is the footnote.<a href="#fnref:1" class="footnote-back-ref">&#8617;</a><a href="#fnref:2" class="footnote-back-ref">&#8617;</a></p>
            //     </li>
            //     <li id="fn:2">
            //     <p>Here's one with multiple blocks.</p>
            //     <p>Subsequent paragraphs are indented to show that they
            //     belong to the previous footnote.</p>
            //     <blockquote>
            //     <p>This is a block quote
            //     Inside a footnote</p>
            //     </blockquote>
            //     <pre><code>{ some.code }
            //     </code></pre>
            //     <p>The whole paragraph can be indented, or just the first
            //     line.  In this way, multi-paragraph footnotes work like
            //     multi-paragraph list items.<a href="#fnref:3" class="footnote-back-ref">&#8617;</a><a href="#fnref:4" class="footnote-back-ref">&#8617;</a></p>
            //     </li>
            //     </ol>
            //     </div>

            TestParser.TestSpec("Here is a footnote reference,[^1] and another.[^longnote]\n\nThis is another reference to [^1]\n\n[^1]: Here is the footnote.\n\nAnd another reference to [^longnote]\n\n[^longnote]: Here's one with multiple blocks.\n\n    Subsequent paragraphs are indented to show that they\nbelong to the previous footnote.\n\n    > This is a block quote\n    > Inside a footnote\n\n        { some.code }\n\n    The whole paragraph can be indented, or just the first\n    line.  In this way, multi-paragraph footnotes work like\n    multi-paragraph list items.\n\nThis paragraph won't be part of the note, because it\nisn't indented.", "<p>Here is a footnote reference,<a id=\"fnref:1\" href=\"#fn:1\" class=\"footnote-ref\"><sup>1</sup></a> and another.<a id=\"fnref:3\" href=\"#fn:2\" class=\"footnote-ref\"><sup>2</sup></a></p>\n<p>This is another reference to <a id=\"fnref:2\" href=\"#fn:1\" class=\"footnote-ref\"><sup>1</sup></a></p>\n<p>And another reference to <a id=\"fnref:4\" href=\"#fn:2\" class=\"footnote-ref\"><sup>2</sup></a></p>\n<p>This paragraph won't be part of the note, because it\nisn't indented.</p>\n<div class=\"footnotes\">\n<hr />\n<ol>\n<li id=\"fn:1\">\n<p>Here is the footnote.<a href=\"#fnref:1\" class=\"footnote-back-ref\">&#8617;</a><a href=\"#fnref:2\" class=\"footnote-back-ref\">&#8617;</a></p>\n</li>\n<li id=\"fn:2\">\n<p>Here's one with multiple blocks.</p>\n<p>Subsequent paragraphs are indented to show that they\nbelong to the previous footnote.</p>\n<blockquote>\n<p>This is a block quote\nInside a footnote</p>\n</blockquote>\n<pre><code>{ some.code }\n</code></pre>\n<p>The whole paragraph can be indented, or just the first\nline.  In this way, multi-paragraph footnotes work like\nmulti-paragraph list items.<a href=\"#fnref:3\" class=\"footnote-back-ref\">&#8617;</a><a href=\"#fnref:4\" class=\"footnote-back-ref\">&#8617;</a></p>\n</li>\n</ol>\n</div>", "footnotes|advanced", context: "Example 1\nSection Extensions / Footnotes\n");
        }

        // Check with multiple consecutive footnotes:
        [Test]
        public void ExtensionsFootnotes_Example002()
        {
            // Example 2
            // Section: Extensions / Footnotes
            //
            // The following Markdown:
            //     Here is a footnote[^1]. And another one[^2]. And a third one[^3]. And a fourth[^4].
            //     
            //     [^1]: Footnote 1 text
            //     
            //     [^2]: Footnote 2 text
            //     
            //     a
            //     
            //     [^3]: Footnote 3 text
            //     
            //     [^4]: Footnote 4 text
            //
            // Should be rendered as:
            //     <p>Here is a footnote<a id="fnref:1" href="#fn:1" class="footnote-ref"><sup>1</sup></a>. And another one<a id="fnref:2" href="#fn:2" class="footnote-ref"><sup>2</sup></a>. And a third one<a id="fnref:3" href="#fn:3" class="footnote-ref"><sup>3</sup></a>. And a fourth<a id="fnref:4" href="#fn:4" class="footnote-ref"><sup>4</sup></a>.</p>
            //     <p>a</p>
            //     <div class="footnotes">
            //     <hr />
            //     <ol>
            //     <li id="fn:1">
            //     <p>Footnote 1 text<a href="#fnref:1" class="footnote-back-ref">&#8617;</a></p></li>
            //     <li id="fn:2">
            //     <p>Footnote 2 text<a href="#fnref:2" class="footnote-back-ref">&#8617;</a></p></li>
            //     <li id="fn:3">
            //     <p>Footnote 3 text<a href="#fnref:3" class="footnote-back-ref">&#8617;</a></p></li>
            //     <li id="fn:4">
            //     <p>Footnote 4 text<a href="#fnref:4" class="footnote-back-ref">&#8617;</a></p></li>
            //     </ol>
            //     </div>

            TestParser.TestSpec("Here is a footnote[^1]. And another one[^2]. And a third one[^3]. And a fourth[^4].\n\n[^1]: Footnote 1 text\n\n[^2]: Footnote 2 text\n\na\n\n[^3]: Footnote 3 text\n\n[^4]: Footnote 4 text", "<p>Here is a footnote<a id=\"fnref:1\" href=\"#fn:1\" class=\"footnote-ref\"><sup>1</sup></a>. And another one<a id=\"fnref:2\" href=\"#fn:2\" class=\"footnote-ref\"><sup>2</sup></a>. And a third one<a id=\"fnref:3\" href=\"#fn:3\" class=\"footnote-ref\"><sup>3</sup></a>. And a fourth<a id=\"fnref:4\" href=\"#fn:4\" class=\"footnote-ref\"><sup>4</sup></a>.</p>\n<p>a</p>\n<div class=\"footnotes\">\n<hr />\n<ol>\n<li id=\"fn:1\">\n<p>Footnote 1 text<a href=\"#fnref:1\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n<li id=\"fn:2\">\n<p>Footnote 2 text<a href=\"#fnref:2\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n<li id=\"fn:3\">\n<p>Footnote 3 text<a href=\"#fnref:3\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n<li id=\"fn:4\">\n<p>Footnote 4 text<a href=\"#fnref:4\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n</ol>\n</div>", "footnotes|advanced", context: "Example 2\nSection Extensions / Footnotes\n");
        }

        // Another test with consecutive footnotes without a blank line separator:
        [Test]
        public void ExtensionsFootnotes_Example003()
        {
            // Example 3
            // Section: Extensions / Footnotes
            //
            // The following Markdown:
            //     Here is a footnote[^1]. And another one[^2]. And a third one[^3]. And a fourth[^4].
            //     
            //     [^1]: Footnote 1 text
            //     [^2]: Footnote 2 text
            //     [^3]: Footnote 3 text
            //     [^4]: Footnote 4 text
            //
            // Should be rendered as:
            //     <p>Here is a footnote<a id="fnref:1" href="#fn:1" class="footnote-ref"><sup>1</sup></a>. And another one<a id="fnref:2" href="#fn:2" class="footnote-ref"><sup>2</sup></a>. And a third one<a id="fnref:3" href="#fn:3" class="footnote-ref"><sup>3</sup></a>. And a fourth<a id="fnref:4" href="#fn:4" class="footnote-ref"><sup>4</sup></a>.</p>
            //     <div class="footnotes">
            //     <hr />
            //     <ol>
            //     <li id="fn:1">
            //     <p>Footnote 1 text<a href="#fnref:1" class="footnote-back-ref">&#8617;</a></p></li>
            //     <li id="fn:2">
            //     <p>Footnote 2 text<a href="#fnref:2" class="footnote-back-ref">&#8617;</a></p></li>
            //     <li id="fn:3">
            //     <p>Footnote 3 text<a href="#fnref:3" class="footnote-back-ref">&#8617;</a></p></li>
            //     <li id="fn:4">
            //     <p>Footnote 4 text<a href="#fnref:4" class="footnote-back-ref">&#8617;</a></p></li>
            //     </ol>
            //     </div>

            TestParser.TestSpec("Here is a footnote[^1]. And another one[^2]. And a third one[^3]. And a fourth[^4].\n\n[^1]: Footnote 1 text\n[^2]: Footnote 2 text\n[^3]: Footnote 3 text\n[^4]: Footnote 4 text", "<p>Here is a footnote<a id=\"fnref:1\" href=\"#fn:1\" class=\"footnote-ref\"><sup>1</sup></a>. And another one<a id=\"fnref:2\" href=\"#fn:2\" class=\"footnote-ref\"><sup>2</sup></a>. And a third one<a id=\"fnref:3\" href=\"#fn:3\" class=\"footnote-ref\"><sup>3</sup></a>. And a fourth<a id=\"fnref:4\" href=\"#fn:4\" class=\"footnote-ref\"><sup>4</sup></a>.</p>\n<div class=\"footnotes\">\n<hr />\n<ol>\n<li id=\"fn:1\">\n<p>Footnote 1 text<a href=\"#fnref:1\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n<li id=\"fn:2\">\n<p>Footnote 2 text<a href=\"#fnref:2\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n<li id=\"fn:3\">\n<p>Footnote 3 text<a href=\"#fnref:3\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n<li id=\"fn:4\">\n<p>Footnote 4 text<a href=\"#fnref:4\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n</ol>\n</div>", "footnotes|advanced", context: "Example 3\nSection Extensions / Footnotes\n");
        }

        // A footnote link inside a list should work as well:
        [Test]
        public void ExtensionsFootnotes_Example004()
        {
            // Example 4
            // Section: Extensions / Footnotes
            //
            // The following Markdown:
            //     - abc
            //     - def[^1]
            //     
            //     [^1]: Here is the footnote.
            //
            // Should be rendered as:
            //     <ul>
            //     <li>abc</li>
            //     <li>def<a id="fnref:1" href="#fn:1" class="footnote-ref"><sup>1</sup></a></li>
            //     </ul>
            //     <div class="footnotes">
            //     <hr />
            //     <ol>
            //     <li id="fn:1">
            //     <p>Here is the footnote.<a href="#fnref:1" class="footnote-back-ref">&#8617;</a></p></li>
            //     </ol>
            //     </div>

            TestParser.TestSpec("- abc\n- def[^1]\n\n[^1]: Here is the footnote.", "<ul>\n<li>abc</li>\n<li>def<a id=\"fnref:1\" href=\"#fn:1\" class=\"footnote-ref\"><sup>1</sup></a></li>\n</ul>\n<div class=\"footnotes\">\n<hr />\n<ol>\n<li id=\"fn:1\">\n<p>Here is the footnote.<a href=\"#fnref:1\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n</ol>\n</div>", "footnotes|advanced", context: "Example 4\nSection Extensions / Footnotes\n");
        }
    }
}
