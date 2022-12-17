
// --------------------------------
//   Footnotes Labels No BackLink
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.FootnotesLabelsNoBackLink
{
    [TestFixture]
    public class TestExtensionsFootnotesPreserveLabelsNoBackLink
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## Footnotes Preserve Labels No BackLink
        // 
        // Allows footnotes that preserve the original labels in markdown:
        [Test]
        public void ExtensionsFootnotesPreserveLabelsNoBackLink_Example001()
        {
            // Example 1
            // Section: Extensions / Footnotes Preserve Labels No BackLink
            //
            // The following Markdown:
            //     Here is a footnote reference,[^a] and another.[^somenote]
            //     
            //     This is another reference to [^a]
            //     
            //     [^a]: Here is the footnote.
            //     
            //     And another reference to [^somenote]
            //     
            //     [^somenote]: Here's one with multiple blocks.
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
            //     <p>Here is a footnote reference,<a id="fnref:1" href="#fn:a" class="footnote-ref"><sup>a</sup></a> and another.<a id="fnref:3" href="#fn:somenote" class="footnote-ref"><sup>somenote</sup></a></p>
            //     <p>This is another reference to <a id="fnref:2" href="#fn:a" class="footnote-ref"><sup>a</sup></a></p>
            //     <p>And another reference to <a id="fnref:4" href="#fn:somenote" class="footnote-ref"><sup>somenote</sup></a></p>
            //     <p>This paragraph won't be part of the note, because it
            //     isn't indented.</p>
            //     <div class="footnotes">
            //     <hr />
            //     <ol>
            //     <li id="fn:a">
            //     <p>Here is the footnote.</p>
            //     </li>
            //     <li id="fn:somenote">
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
            //     multi-paragraph list items.</p>
            //     </li>
            //     </ol>
            //     </div>

            TestParser.TestSpec("Here is a footnote reference,[^a] and another.[^somenote]\n\nThis is another reference to [^a]\n\n[^a]: Here is the footnote.\n\nAnd another reference to [^somenote]\n\n[^somenote]: Here's one with multiple blocks.\n\n    Subsequent paragraphs are indented to show that they\nbelong to the previous footnote.\n\n    > This is a block quote\n    > Inside a footnote\n\n        { some.code }\n\n    The whole paragraph can be indented, or just the first\n    line.  In this way, multi-paragraph footnotes work like\n    multi-paragraph list items.\n\nThis paragraph won't be part of the note, because it\nisn't indented.", "<p>Here is a footnote reference,<a id=\"fnref:1\" href=\"#fn:a\" class=\"footnote-ref\"><sup>a</sup></a> and another.<a id=\"fnref:3\" href=\"#fn:somenote\" class=\"footnote-ref\"><sup>somenote</sup></a></p>\n<p>This is another reference to <a id=\"fnref:2\" href=\"#fn:a\" class=\"footnote-ref\"><sup>a</sup></a></p>\n<p>And another reference to <a id=\"fnref:4\" href=\"#fn:somenote\" class=\"footnote-ref\"><sup>somenote</sup></a></p>\n<p>This paragraph won't be part of the note, because it\nisn't indented.</p>\n<div class=\"footnotes\">\n<hr />\n<ol>\n<li id=\"fn:a\">\n<p>Here is the footnote.</p>\n</li>\n<li id=\"fn:somenote\">\n<p>Here's one with multiple blocks.</p>\n<p>Subsequent paragraphs are indented to show that they\nbelong to the previous footnote.</p>\n<blockquote>\n<p>This is a block quote\nInside a footnote</p>\n</blockquote>\n<pre><code>{ some.code }\n</code></pre>\n<p>The whole paragraph can be indented, or just the first\nline.  In this way, multi-paragraph footnotes work like\nmulti-paragraph list items.</p>\n</li>\n</ol>\n</div>", "footnotes-labels-nobacklink", context: "Example 1\nSection Extensions / Footnotes Preserve Labels No BackLink\n");
        }

        // Check with multiple consecutive footnotes:
        [Test]
        public void ExtensionsFootnotesPreserveLabelsNoBackLink_Example002()
        {
            // Example 2
            // Section: Extensions / Footnotes Preserve Labels No BackLink
            //
            // The following Markdown:
            //     Here is a footnote[^h]. And another one[^j]. And a third one[^m]. And a fourth[^4].
            //     
            //     [^h]: Footnote 1 text
            //     
            //     [^j]: Footnote 2 text
            //     
            //     a
            //     
            //     [^m]: Footnote 3 text
            //     
            //     [^4]: Footnote 4 text
            //
            // Should be rendered as:
            //     <p>Here is a footnote<a id="fnref:1" href="#fn:h" class="footnote-ref"><sup>h</sup></a>. And another one<a id="fnref:2" href="#fn:j" class="footnote-ref"><sup>j</sup></a>. And a third one<a id="fnref:3" href="#fn:m" class="footnote-ref"><sup>m</sup></a>. And a fourth<a id="fnref:4" href="#fn:4" class="footnote-ref"><sup>4</sup></a>.</p>
            //     <p>a</p>
            //     <div class="footnotes">
            //     <hr />
            //     <ol>
            //     <li id="fn:h">
            //     <p>Footnote 1 text</p></li>
            //     <li id="fn:j">
            //     <p>Footnote 2 text</p></li>
            //     <li id="fn:m">
            //     <p>Footnote 3 text</p></li>
            //     <li id="fn:4">
            //     <p>Footnote 4 text</p></li>
            //     </ol>
            //     </div>

            TestParser.TestSpec("Here is a footnote[^h]. And another one[^j]. And a third one[^m]. And a fourth[^4].\n\n[^h]: Footnote 1 text\n\n[^j]: Footnote 2 text\n\na\n\n[^m]: Footnote 3 text\n\n[^4]: Footnote 4 text", "<p>Here is a footnote<a id=\"fnref:1\" href=\"#fn:h\" class=\"footnote-ref\"><sup>h</sup></a>. And another one<a id=\"fnref:2\" href=\"#fn:j\" class=\"footnote-ref\"><sup>j</sup></a>. And a third one<a id=\"fnref:3\" href=\"#fn:m\" class=\"footnote-ref\"><sup>m</sup></a>. And a fourth<a id=\"fnref:4\" href=\"#fn:4\" class=\"footnote-ref\"><sup>4</sup></a>.</p>\n<p>a</p>\n<div class=\"footnotes\">\n<hr />\n<ol>\n<li id=\"fn:h\">\n<p>Footnote 1 text</p></li>\n<li id=\"fn:j\">\n<p>Footnote 2 text</p></li>\n<li id=\"fn:m\">\n<p>Footnote 3 text</p></li>\n<li id=\"fn:4\">\n<p>Footnote 4 text</p></li>\n</ol>\n</div>", "footnotes-labels-nobacklink", context: "Example 2\nSection Extensions / Footnotes Preserve Labels No BackLink\n");
        }
    }
}
