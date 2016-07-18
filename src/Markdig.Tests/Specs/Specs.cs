using System;
using NUnit.Framework;

namespace Markdig.Tests
{
        // ---
        // title: CommonMark Spec
        // author: John MacFarlane
        // version: 0.25
        // date: '2016-03-24'
        // license: '[CC-BY-SA 4.0](http://creativecommons.org/licenses/by-sa/4.0/)'
        // ...
        //
        // # Introduction
        //
        // ## What is Markdown?
        //
        // Markdown is a plain text format for writing structured documents,
        // based on conventions used for indicating formatting in email and
        // usenet posts.  It was developed in 2004 by John Gruber, who wrote
        // the first Markdown-to-HTML converter in Perl, and it soon became
        // ubiquitous.  In the next decade, dozens of implementations were
        // developed in many languages.  Some extended the original
        // Markdown syntax with conventions for footnotes, tables, and
        // other document elements.  Some allowed Markdown documents to be
        // rendered in formats other than HTML.  Websites like Reddit,
        // StackOverflow, and GitHub had millions of people using Markdown.
        // And Markdown started to be used beyond the web, to author books,
        // articles, slide shows, letters, and lecture notes.
        //
        // What distinguishes Markdown from many other lightweight markup
        // syntaxes, which are often easier to write, is its readability.
        // As Gruber writes:
        //
        // > The overriding design goal for Markdown's formatting syntax is
        // > to make it as readable as possible. The idea is that a
        // > Markdown-formatted document should be publishable as-is, as
        // > plain text, without looking like it's been marked up with tags
        // > or formatting instructions.
        // > (<http://daringfireball.net/projects/markdown/>)
        //
        // The point can be illustrated by comparing a sample of
        // [AsciiDoc](http://www.methods.co.nz/asciidoc/) with
        // an equivalent sample of Markdown.  Here is a sample of
        // AsciiDoc from the AsciiDoc manual:
        //
        // ```
        // 1. List item one.
        // +
        // List item one continued with a second paragraph followed by an
        // Indented block.
        // +
        // .................
        // $ ls *.sh
        // $ mv *.sh ~/tmp
        // .................
        // +
        // List item continued with a third paragraph.
        //
        // 2. List item two continued with an open block.
        // +
        // --
        // This paragraph is part of the preceding list item.
        //
        // a. This list is nested and does not require explicit item
        // continuation.
        // +
        // This paragraph is part of the preceding list item.
        //
        // b. List item b.
        //
        // This paragraph belongs to item two of the outer list.
        // --
        // ```
        //
        // And here is the equivalent in Markdown:
        // ```
        // 1.  List item one.
        //
        // List item one continued with a second paragraph followed by an
        // Indented block.
        //
        // $ ls *.sh
        // $ mv *.sh ~/tmp
        //
        // List item continued with a third paragraph.
        //
        // 2.  List item two continued with an open block.
        //
        // This paragraph is part of the preceding list item.
        //
        // 1. This list is nested and does not require explicit item continuation.
        //
        // This paragraph is part of the preceding list item.
        //
        // 2. List item b.
        //
        // This paragraph belongs to item two of the outer list.
        // ```
        //
        // The AsciiDoc version is, arguably, easier to write. You don't need
        // to worry about indentation.  But the Markdown version is much easier
        // to read.  The nesting of list items is apparent to the eye in the
        // source, not just in the processed document.
        //
        // ## Why is a spec needed?
        //
        // John Gruber's [canonical description of Markdown's
        // syntax](http://daringfireball.net/projects/markdown/syntax)
        // does not specify the syntax unambiguously.  Here are some examples of
        // questions it does not answer:
        //
        // 1.  How much indentation is needed for a sublist?  The spec says that
        // continuation paragraphs need to be indented four spaces, but is
        // not fully explicit about sublists.  It is natural to think that
        // they, too, must be indented four spaces, but `Markdown.pl` does
        // not require that.  This is hardly a "corner case," and divergences
        // between implementations on this issue often lead to surprises for
        // users in real documents. (See [this comment by John
        // Gruber](http://article.gmane.org/gmane.text.markdown.general/1997).)
        //
        // 2.  Is a blank line needed before a block quote or heading?
        // Most implementations do not require the blank line.  However,
        // this can lead to unexpected results in hard-wrapped text, and
        // also to ambiguities in parsing (note that some implementations
        // put the heading inside the blockquote, while others do not).
        // (John Gruber has also spoken [in favor of requiring the blank
        // lines](http://article.gmane.org/gmane.text.markdown.general/2146).)
        //
        // 3.  Is a blank line needed before an indented code block?
        // (`Markdown.pl` requires it, but this is not mentioned in the
        // documentation, and some implementations do not require it.)
        //
        // ``` markdown
        // paragraph
        // code?
        // ```
        //
        // 4.  What is the exact rule for determining when list items get
        // wrapped in `<p>` tags?  Can a list be partially "loose" and partially
        // "tight"?  What should we do with a list like this?
        //
        // ``` markdown
        // 1. one
        //
        // 2. two
        // 3. three
        // ```
        //
        // Or this?
        //
        // ``` markdown
        // 1.  one
        // - a
        //
        // - b
        // 2.  two
        // ```
        //
        // (There are some relevant comments by John Gruber
        // [here](http://article.gmane.org/gmane.text.markdown.general/2554).)
        //
        // 5.  Can list markers be indented?  Can ordered list markers be right-aligned?
        //
        // ``` markdown
        // 8. item 1
        // 9. item 2
        // 10. item 2a
        // ```
        //
        // 6.  Is this one list with a thematic break in its second item,
        // or two lists separated by a thematic break?
        //
        // ``` markdown
        // * a
        // * * * * *
        // * b
        // ```
        //
        // 7.  When list markers change from numbers to bullets, do we have
        // two lists or one?  (The Markdown syntax description suggests two,
        // but the perl scripts and many other implementations produce one.)
        //
        // ``` markdown
        // 1. fee
        // 2. fie
        // -  foe
        // -  fum
        // ```
        //
        // 8.  What are the precedence rules for the markers of inline structure?
        // For example, is the following a valid link, or does the code span
        // take precedence ?
        //
        // ``` markdown
        // [a backtick (`)](/url) and [another backtick (`)](/url).
        // ```
        //
        // 9.  What are the precedence rules for markers of emphasis and strong
        // emphasis?  For example, how should the following be parsed?
        //
        // ``` markdown
        // *foo *bar* baz*
        // ```
        //
        // 10. What are the precedence rules between block-level and inline-level
        // structure?  For example, how should the following be parsed?
        //
        // ``` markdown
        // - `a long code span can contain a hyphen like this
        // - and it can screw things up`
        // ```
        //
        // 11. Can list items include section headings?  (`Markdown.pl` does not
        // allow this, but does allow blockquotes to include headings.)
        //
        // ``` markdown
        // - # Heading
        // ```
        //
        // 12. Can list items be empty?
        //
        // ``` markdown
        // * a
        // *
        // * b
        // ```
        //
        // 13. Can link references be defined inside block quotes or list items?
        //
        // ``` markdown
        // > Blockquote [foo].
        // >
        // > [foo]: /url
        // ```
        //
        // 14. If there are multiple definitions for the same reference, which takes
        // precedence?
        //
        // ``` markdown
        // [foo]: /url1
        // [foo]: /url2
        //
        // [foo][]
        // ```
        //
        // In the absence of a spec, early implementers consulted `Markdown.pl`
        // to resolve these ambiguities.  But `Markdown.pl` was quite buggy, and
        // gave manifestly bad results in many cases, so it was not a
        // satisfactory replacement for a spec.
        //
        // Because there is no unambiguous spec, implementations have diverged
        // considerably.  As a result, users are often surprised to find that
        // a document that renders one way on one system (say, a github wiki)
        // renders differently on another (say, converting to docbook using
        // pandoc).  To make matters worse, because nothing in Markdown counts
        // as a "syntax error," the divergence often isn't discovered right away.
        //
        // ## About this document
        //
        // This document attempts to specify Markdown syntax unambiguously.
        // It contains many examples with side-by-side Markdown and
        // HTML.  These are intended to double as conformance tests.  An
        // accompanying script `spec_tests.py` can be used to run the tests
        // against any Markdown program:
        //
        // python test/spec_tests.py --spec spec.txt --program PROGRAM
        //
        // Since this document describes how Markdown is to be parsed into
        // an abstract syntax tree, it would have made sense to use an abstract
        // representation of the syntax tree instead of HTML.  But HTML is capable
        // of representing the structural distinctions we need to make, and the
        // choice of HTML for the tests makes it possible to run the tests against
        // an implementation without writing an abstract syntax tree renderer.
        //
        // This document is generated from a text file, `spec.txt`, written
        // in Markdown with a small extension for the side-by-side tests.
        // The script `tools/makespec.py` can be used to convert `spec.txt` into
        // HTML or CommonMark (which can then be converted into other formats).
        //
        // In the examples, the `→` character is used to represent tabs.
        //
        // # Preliminaries
        //
        // ## Characters and lines
        //
        // Any sequence of [characters] is a valid CommonMark
        // document.
        //
        // A [character](@) is a Unicode code point.  Although some
        // code points (for example, combining accents) do not correspond to
        // characters in an intuitive sense, all code points count as characters
        // for purposes of this spec.
        //
        // This spec does not specify an encoding; it thinks of lines as composed
        // of [characters] rather than bytes.  A conforming parser may be limited
        // to a certain encoding.
        //
        // A [line](@) is a sequence of zero or more [characters]
        // other than newline (`U+000A`) or carriage return (`U+000D`),
        // followed by a [line ending] or by the end of file.
        //
        // A [line ending](@) is a newline (`U+000A`), a carriage return
        // (`U+000D`) not followed by a newline, or a carriage return and a
        // following newline.
        //
        // A line containing no characters, or a line containing only spaces
        // (`U+0020`) or tabs (`U+0009`), is called a [blank line](@).
        //
        // The following definitions of character classes will be used in this spec:
        //
        // A [whitespace character](@) is a space
        // (`U+0020`), tab (`U+0009`), newline (`U+000A`), line tabulation (`U+000B`),
        // form feed (`U+000C`), or carriage return (`U+000D`).
        //
        // [Whitespace](@) is a sequence of one or more [whitespace
        // characters].
        //
        // A [Unicode whitespace character](@) is
        // any code point in the Unicode `Zs` class, or a tab (`U+0009`),
        // carriage return (`U+000D`), newline (`U+000A`), or form feed
        // (`U+000C`).
        //
        // [Unicode whitespace](@) is a sequence of one
        // or more [Unicode whitespace characters].
        //
        // A [space](@) is `U+0020`.
        //
        // A [non-whitespace character](@) is any character
        // that is not a [whitespace character].
        //
        // An [ASCII punctuation character](@)
        // is `!`, `"`, `#`, `$`, `%`, `&`, `'`, `(`, `)`,
        // `*`, `+`, `,`, `-`, `.`, `/`, `:`, `;`, `<`, `=`, `>`, `?`, `@`,
        // `[`, `\`, `]`, `^`, `_`, `` ` ``, `{`, `|`, `}`, or `~`.
        //
        // A [punctuation character](@) is an [ASCII
        // punctuation character] or anything in
        // the Unicode classes `Pc`, `Pd`, `Pe`, `Pf`, `Pi`, `Po`, or `Ps`.
        //
        // ## Tabs
        //
        // Tabs in lines are not expanded to [spaces].  However,
        // in contexts where whitespace helps to define block structure,
        // tabs behave as if they were replaced by spaces with a tab stop
        // of 4 characters.
        //
        // Thus, for exmaple, a tab can be used instead of four spaces
        // in an indented code block.  (Note, however, that internal
        // tabs are passed through as literal tabs, not expanded to
        // spaces.)
    [TestFixture]
    public partial class TestPreliminariesTabs
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Preliminaries Tabs
            //
            // The following CommonMark:
            //     →foo→baz→→bim
            //
            // Should be rendered as:
            //     <pre><code>foo→baz→→bim
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Preliminaries Tabs");
			TestParser.TestSpec("\tfoo\tbaz\t\tbim", "<pre><code>foo\tbaz\t\tbim\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestPreliminariesTabs
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Preliminaries Tabs
            //
            // The following CommonMark:
            //       →foo→baz→→bim
            //
            // Should be rendered as:
            //     <pre><code>foo→baz→→bim
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Preliminaries Tabs");
			TestParser.TestSpec("  \tfoo\tbaz\t\tbim", "<pre><code>foo\tbaz\t\tbim\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestPreliminariesTabs
    {
        [Test]
        public void Example003()
        {
            // Example 3
            // Section: Preliminaries Tabs
            //
            // The following CommonMark:
            //         a→a
            //         ὐ→a
            //
            // Should be rendered as:
            //     <pre><code>a→a
            //     ὐ→a
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Preliminaries Tabs");
			TestParser.TestSpec("    a\ta\n    ὐ\ta", "<pre><code>a\ta\nὐ\ta\n</code></pre>", "");
        }
    }
        // In the following example, a continuation paragraph of a list
        // item is indented with a tab; this has exactly the same effect
        // as indentation with four spaces would:
    [TestFixture]
    public partial class TestPreliminariesTabs
    {
        [Test]
        public void Example004()
        {
            // Example 4
            // Section: Preliminaries Tabs
            //
            // The following CommonMark:
            //       - foo
            //     
            //     →bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>foo</p>
            //     <p>bar</p>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 4, "Preliminaries Tabs");
			TestParser.TestSpec("  - foo\n\n\tbar", "<ul>\n<li>\n<p>foo</p>\n<p>bar</p>\n</li>\n</ul>", "");
        }
    }
    [TestFixture]
    public partial class TestPreliminariesTabs
    {
        [Test]
        public void Example005()
        {
            // Example 5
            // Section: Preliminaries Tabs
            //
            // The following CommonMark:
            //     - foo
            //     
            //     →→bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>foo</p>
            //     <pre><code>  bar
            //     </code></pre>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 5, "Preliminaries Tabs");
			TestParser.TestSpec("- foo\n\n\t\tbar", "<ul>\n<li>\n<p>foo</p>\n<pre><code>  bar\n</code></pre>\n</li>\n</ul>", "");
        }
    }
        // Normally the `>` that begins a block quote may be followed
        // optionally by a space, which is not considered part of the
        // content.  In the following case `>` is followed by a tab,
        // which is treated as if it were expanded into spaces.
        // Since one of theses spaces is considered part of the
        // delimiter, `foo` is considered to be indented six spaces
        // inside the block quote context, so we get an indented
        // code block starting with two spaces.
    [TestFixture]
    public partial class TestPreliminariesTabs
    {
        [Test]
        public void Example006()
        {
            // Example 6
            // Section: Preliminaries Tabs
            //
            // The following CommonMark:
            //     >→→foo
            //
            // Should be rendered as:
            //     <blockquote>
            //     <pre><code>  foo
            //     </code></pre>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 6, "Preliminaries Tabs");
			TestParser.TestSpec(">\t\tfoo", "<blockquote>\n<pre><code>  foo\n</code></pre>\n</blockquote>", "");
        }
    }
    [TestFixture]
    public partial class TestPreliminariesTabs
    {
        [Test]
        public void Example007()
        {
            // Example 7
            // Section: Preliminaries Tabs
            //
            // The following CommonMark:
            //     -→→foo
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <pre><code>  foo
            //     </code></pre>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 7, "Preliminaries Tabs");
			TestParser.TestSpec("-\t\tfoo", "<ul>\n<li>\n<pre><code>  foo\n</code></pre>\n</li>\n</ul>", "");
        }
    }
    [TestFixture]
    public partial class TestPreliminariesTabs
    {
        [Test]
        public void Example008()
        {
            // Example 8
            // Section: Preliminaries Tabs
            //
            // The following CommonMark:
            //         foo
            //     →bar
            //
            // Should be rendered as:
            //     <pre><code>foo
            //     bar
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 8, "Preliminaries Tabs");
			TestParser.TestSpec("    foo\n\tbar", "<pre><code>foo\nbar\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestPreliminariesTabs
    {
        [Test]
        public void Example009()
        {
            // Example 9
            // Section: Preliminaries Tabs
            //
            // The following CommonMark:
            //      - foo
            //        - bar
            //     → - baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo
            //     <ul>
            //     <li>bar
            //     <ul>
            //     <li>baz</li>
            //     </ul>
            //     </li>
            //     </ul>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 9, "Preliminaries Tabs");
			TestParser.TestSpec(" - foo\n   - bar\n\t - baz", "<ul>\n<li>foo\n<ul>\n<li>bar\n<ul>\n<li>baz</li>\n</ul>\n</li>\n</ul>\n</li>\n</ul>", "");
        }
    }
    [TestFixture]
    public partial class TestPreliminariesTabs
    {
        [Test]
        public void Example010()
        {
            // Example 10
            // Section: Preliminaries Tabs
            //
            // The following CommonMark:
            //     #→Foo
            //
            // Should be rendered as:
            //     <h1>Foo</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 10, "Preliminaries Tabs");
			TestParser.TestSpec("#\tFoo", "<h1>Foo</h1>", "");
        }
    }
    [TestFixture]
    public partial class TestPreliminariesTabs
    {
        [Test]
        public void Example011()
        {
            // Example 11
            // Section: Preliminaries Tabs
            //
            // The following CommonMark:
            //     *→*→*→
            //
            // Should be rendered as:
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 11, "Preliminaries Tabs");
			TestParser.TestSpec("*\t*\t*\t", "<hr />", "");
        }
    }
        // ## Insecure characters
        //
        // For security reasons, the Unicode character `U+0000` must be replaced
        // with the REPLACEMENT CHARACTER (`U+FFFD`).
        //
        // # Blocks and inlines
        //
        // We can think of a document as a sequence of
        // [blocks](@)---structural elements like paragraphs, block
        // quotations, lists, headings, rules, and code blocks.  Some blocks (like
        // block quotes and list items) contain other blocks; others (like
        // headings and paragraphs) contain [inline](@) content---text,
        // links, emphasized text, images, code, and so on.
        //
        // ## Precedence
        //
        // Indicators of block structure always take precedence over indicators
        // of inline structure.  So, for example, the following is a list with
        // two items, not a list with one item containing a code span:
    [TestFixture]
    public partial class TestBlocksandinlinesPrecedence
    {
        [Test]
        public void Example012()
        {
            // Example 12
            // Section: Blocks and inlines Precedence
            //
            // The following CommonMark:
            //     - `one
            //     - two`
            //
            // Should be rendered as:
            //     <ul>
            //     <li>`one</li>
            //     <li>two`</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 12, "Blocks and inlines Precedence");
			TestParser.TestSpec("- `one\n- two`", "<ul>\n<li>`one</li>\n<li>two`</li>\n</ul>", "");
        }
    }
        // This means that parsing can proceed in two steps:  first, the block
        // structure of the document can be discerned; second, text lines inside
        // paragraphs, headings, and other block constructs can be parsed for inline
        // structure.  The second step requires information about link reference
        // definitions that will be available only at the end of the first
        // step.  Note that the first step requires processing lines in sequence,
        // but the second can be parallelized, since the inline parsing of
        // one block element does not affect the inline parsing of any other.
        //
        // ## Container blocks and leaf blocks
        //
        // We can divide blocks into two types:
        // [container block](@)s,
        // which can contain other blocks, and [leaf block](@)s,
        // which cannot.
        //
        // # Leaf blocks
        //
        // This section describes the different kinds of leaf block that make up a
        // Markdown document.
        //
        // ## Thematic breaks
        //
        // A line consisting of 0-3 spaces of indentation, followed by a sequence
        // of three or more matching `-`, `_`, or `*` characters, each followed
        // optionally by any number of spaces, forms a
        // [thematic break](@).
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example013()
        {
            // Example 13
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //     ***
            //     ---
            //     ___
            //
            // Should be rendered as:
            //     <hr />
            //     <hr />
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 13, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("***\n---\n___", "<hr />\n<hr />\n<hr />", "");
        }
    }
        // Wrong characters:
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example014()
        {
            // Example 14
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //     +++
            //
            // Should be rendered as:
            //     <p>+++</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 14, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("+++", "<p>+++</p>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example015()
        {
            // Example 15
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //     ===
            //
            // Should be rendered as:
            //     <p>===</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 15, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("===", "<p>===</p>", "");
        }
    }
        // Not enough characters:
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example016()
        {
            // Example 16
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //     --
            //     **
            //     __
            //
            // Should be rendered as:
            //     <p>--
            //     **
            //     __</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 16, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("--\n**\n__", "<p>--\n**\n__</p>", "");
        }
    }
        // One to three spaces indent are allowed:
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example017()
        {
            // Example 17
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //      ***
            //       ***
            //        ***
            //
            // Should be rendered as:
            //     <hr />
            //     <hr />
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 17, "Leaf blocks Thematic breaks");
			TestParser.TestSpec(" ***\n  ***\n   ***", "<hr />\n<hr />\n<hr />", "");
        }
    }
        // Four spaces is too many:
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example018()
        {
            // Example 18
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //         ***
            //
            // Should be rendered as:
            //     <pre><code>***
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 18, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("    ***", "<pre><code>***\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example019()
        {
            // Example 19
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //     Foo
            //         ***
            //
            // Should be rendered as:
            //     <p>Foo
            //     ***</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 19, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("Foo\n    ***", "<p>Foo\n***</p>", "");
        }
    }
        // More than three characters may be used:
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example020()
        {
            // Example 20
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //     _____________________________________
            //
            // Should be rendered as:
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 20, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("_____________________________________", "<hr />", "");
        }
    }
        // Spaces are allowed between the characters:
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example021()
        {
            // Example 21
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //      - - -
            //
            // Should be rendered as:
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 21, "Leaf blocks Thematic breaks");
			TestParser.TestSpec(" - - -", "<hr />", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example022()
        {
            // Example 22
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //      **  * ** * ** * **
            //
            // Should be rendered as:
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 22, "Leaf blocks Thematic breaks");
			TestParser.TestSpec(" **  * ** * ** * **", "<hr />", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example023()
        {
            // Example 23
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //     -     -      -      -
            //
            // Should be rendered as:
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 23, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("-     -      -      -", "<hr />", "");
        }
    }
        // Spaces are allowed at the end:
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example024()
        {
            // Example 24
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //     - - - -    
            //
            // Should be rendered as:
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 24, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("- - - -    ", "<hr />", "");
        }
    }
        // However, no other characters may occur in the line:
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example025()
        {
            // Example 25
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //     _ _ _ _ a
            //     
            //     a------
            //     
            //     ---a---
            //
            // Should be rendered as:
            //     <p>_ _ _ _ a</p>
            //     <p>a------</p>
            //     <p>---a---</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 25, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("_ _ _ _ a\n\na------\n\n---a---", "<p>_ _ _ _ a</p>\n<p>a------</p>\n<p>---a---</p>", "");
        }
    }
        // It is required that all of the [non-whitespace characters] be the same.
        // So, this is not a thematic break:
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example026()
        {
            // Example 26
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //      *-*
            //
            // Should be rendered as:
            //     <p><em>-</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 26, "Leaf blocks Thematic breaks");
			TestParser.TestSpec(" *-*", "<p><em>-</em></p>", "");
        }
    }
        // Thematic breaks do not need blank lines before or after:
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example027()
        {
            // Example 27
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //     - foo
            //     ***
            //     - bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     </ul>
            //     <hr />
            //     <ul>
            //     <li>bar</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 27, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("- foo\n***\n- bar", "<ul>\n<li>foo</li>\n</ul>\n<hr />\n<ul>\n<li>bar</li>\n</ul>", "");
        }
    }
        // Thematic breaks can interrupt a paragraph:
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example028()
        {
            // Example 28
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //     Foo
            //     ***
            //     bar
            //
            // Should be rendered as:
            //     <p>Foo</p>
            //     <hr />
            //     <p>bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 28, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("Foo\n***\nbar", "<p>Foo</p>\n<hr />\n<p>bar</p>", "");
        }
    }
        // If a line of dashes that meets the above conditions for being a
        // thematic break could also be interpreted as the underline of a [setext
        // heading], the interpretation as a
        // [setext heading] takes precedence. Thus, for example,
        // this is a setext heading, not a paragraph followed by a thematic break:
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example029()
        {
            // Example 29
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //     Foo
            //     ---
            //     bar
            //
            // Should be rendered as:
            //     <h2>Foo</h2>
            //     <p>bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 29, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("Foo\n---\nbar", "<h2>Foo</h2>\n<p>bar</p>", "");
        }
    }
        // When both a thematic break and a list item are possible
        // interpretations of a line, the thematic break takes precedence:
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example030()
        {
            // Example 30
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //     * Foo
            //     * * *
            //     * Bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>Foo</li>
            //     </ul>
            //     <hr />
            //     <ul>
            //     <li>Bar</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 30, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("* Foo\n* * *\n* Bar", "<ul>\n<li>Foo</li>\n</ul>\n<hr />\n<ul>\n<li>Bar</li>\n</ul>", "");
        }
    }
        // If you want a thematic break in a list item, use a different bullet:
    [TestFixture]
    public partial class TestLeafblocksThematicbreaks
    {
        [Test]
        public void Example031()
        {
            // Example 31
            // Section: Leaf blocks Thematic breaks
            //
            // The following CommonMark:
            //     - Foo
            //     - * * *
            //
            // Should be rendered as:
            //     <ul>
            //     <li>Foo</li>
            //     <li>
            //     <hr />
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 31, "Leaf blocks Thematic breaks");
			TestParser.TestSpec("- Foo\n- * * *", "<ul>\n<li>Foo</li>\n<li>\n<hr />\n</li>\n</ul>", "");
        }
    }
        // ## ATX headings
        //
        // An [ATX heading](@)
        // consists of a string of characters, parsed as inline content, between an
        // opening sequence of 1--6 unescaped `#` characters and an optional
        // closing sequence of any number of unescaped `#` characters.
        // The opening sequence of `#` characters must be followed by a
        // [space] or by the end of line. The optional closing sequence of `#`s must be
        // preceded by a [space] and may be followed by spaces only.  The opening
        // `#` character may be indented 0-3 spaces.  The raw contents of the
        // heading are stripped of leading and trailing spaces before being parsed
        // as inline content.  The heading level is equal to the number of `#`
        // characters in the opening sequence.
        //
        // Simple headings:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example032()
        {
            // Example 32
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     # foo
            //     ## foo
            //     ### foo
            //     #### foo
            //     ##### foo
            //     ###### foo
            //
            // Should be rendered as:
            //     <h1>foo</h1>
            //     <h2>foo</h2>
            //     <h3>foo</h3>
            //     <h4>foo</h4>
            //     <h5>foo</h5>
            //     <h6>foo</h6>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 32, "Leaf blocks ATX headings");
			TestParser.TestSpec("# foo\n## foo\n### foo\n#### foo\n##### foo\n###### foo", "<h1>foo</h1>\n<h2>foo</h2>\n<h3>foo</h3>\n<h4>foo</h4>\n<h5>foo</h5>\n<h6>foo</h6>", "");
        }
    }
        // More than six `#` characters is not a heading:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example033()
        {
            // Example 33
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     ####### foo
            //
            // Should be rendered as:
            //     <p>####### foo</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 33, "Leaf blocks ATX headings");
			TestParser.TestSpec("####### foo", "<p>####### foo</p>", "");
        }
    }
        // At least one space is required between the `#` characters and the
        // heading's contents, unless the heading is empty.  Note that many
        // implementations currently do not require the space.  However, the
        // space was required by the
        // [original ATX implementation](http://www.aaronsw.com/2002/atx/atx.py),
        // and it helps prevent things like the following from being parsed as
        // headings:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example034()
        {
            // Example 34
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     #5 bolt
            //     
            //     #hashtag
            //
            // Should be rendered as:
            //     <p>#5 bolt</p>
            //     <p>#hashtag</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 34, "Leaf blocks ATX headings");
			TestParser.TestSpec("#5 bolt\n\n#hashtag", "<p>#5 bolt</p>\n<p>#hashtag</p>", "");
        }
    }
        // This is not a heading, because the first `#` is escaped:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example035()
        {
            // Example 35
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     \## foo
            //
            // Should be rendered as:
            //     <p>## foo</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 35, "Leaf blocks ATX headings");
			TestParser.TestSpec("\\## foo", "<p>## foo</p>", "");
        }
    }
        // Contents are parsed as inlines:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example036()
        {
            // Example 36
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     # foo *bar* \*baz\*
            //
            // Should be rendered as:
            //     <h1>foo <em>bar</em> *baz*</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 36, "Leaf blocks ATX headings");
			TestParser.TestSpec("# foo *bar* \\*baz\\*", "<h1>foo <em>bar</em> *baz*</h1>", "");
        }
    }
        // Leading and trailing blanks are ignored in parsing inline content:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example037()
        {
            // Example 37
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     #                  foo                     
            //
            // Should be rendered as:
            //     <h1>foo</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 37, "Leaf blocks ATX headings");
			TestParser.TestSpec("#                  foo                     ", "<h1>foo</h1>", "");
        }
    }
        // One to three spaces indentation are allowed:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example038()
        {
            // Example 38
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //      ### foo
            //       ## foo
            //        # foo
            //
            // Should be rendered as:
            //     <h3>foo</h3>
            //     <h2>foo</h2>
            //     <h1>foo</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 38, "Leaf blocks ATX headings");
			TestParser.TestSpec(" ### foo\n  ## foo\n   # foo", "<h3>foo</h3>\n<h2>foo</h2>\n<h1>foo</h1>", "");
        }
    }
        // Four spaces are too much:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example039()
        {
            // Example 39
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //         # foo
            //
            // Should be rendered as:
            //     <pre><code># foo
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 39, "Leaf blocks ATX headings");
			TestParser.TestSpec("    # foo", "<pre><code># foo\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example040()
        {
            // Example 40
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     foo
            //         # bar
            //
            // Should be rendered as:
            //     <p>foo
            //     # bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 40, "Leaf blocks ATX headings");
			TestParser.TestSpec("foo\n    # bar", "<p>foo\n# bar</p>", "");
        }
    }
        // A closing sequence of `#` characters is optional:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example041()
        {
            // Example 41
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     ## foo ##
            //       ###   bar    ###
            //
            // Should be rendered as:
            //     <h2>foo</h2>
            //     <h3>bar</h3>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 41, "Leaf blocks ATX headings");
			TestParser.TestSpec("## foo ##\n  ###   bar    ###", "<h2>foo</h2>\n<h3>bar</h3>", "");
        }
    }
        // It need not be the same length as the opening sequence:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example042()
        {
            // Example 42
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     # foo ##################################
            //     ##### foo ##
            //
            // Should be rendered as:
            //     <h1>foo</h1>
            //     <h5>foo</h5>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 42, "Leaf blocks ATX headings");
			TestParser.TestSpec("# foo ##################################\n##### foo ##", "<h1>foo</h1>\n<h5>foo</h5>", "");
        }
    }
        // Spaces are allowed after the closing sequence:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example043()
        {
            // Example 43
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     ### foo ###     
            //
            // Should be rendered as:
            //     <h3>foo</h3>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 43, "Leaf blocks ATX headings");
			TestParser.TestSpec("### foo ###     ", "<h3>foo</h3>", "");
        }
    }
        // A sequence of `#` characters with anything but [spaces] following it
        // is not a closing sequence, but counts as part of the contents of the
        // heading:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example044()
        {
            // Example 44
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     ### foo ### b
            //
            // Should be rendered as:
            //     <h3>foo ### b</h3>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 44, "Leaf blocks ATX headings");
			TestParser.TestSpec("### foo ### b", "<h3>foo ### b</h3>", "");
        }
    }
        // The closing sequence must be preceded by a space:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example045()
        {
            // Example 45
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     # foo#
            //
            // Should be rendered as:
            //     <h1>foo#</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 45, "Leaf blocks ATX headings");
			TestParser.TestSpec("# foo#", "<h1>foo#</h1>", "");
        }
    }
        // Backslash-escaped `#` characters do not count as part
        // of the closing sequence:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example046()
        {
            // Example 46
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     ### foo \###
            //     ## foo #\##
            //     # foo \#
            //
            // Should be rendered as:
            //     <h3>foo ###</h3>
            //     <h2>foo ###</h2>
            //     <h1>foo #</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 46, "Leaf blocks ATX headings");
			TestParser.TestSpec("### foo \\###\n## foo #\\##\n# foo \\#", "<h3>foo ###</h3>\n<h2>foo ###</h2>\n<h1>foo #</h1>", "");
        }
    }
        // ATX headings need not be separated from surrounding content by blank
        // lines, and they can interrupt paragraphs:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example047()
        {
            // Example 47
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     ****
            //     ## foo
            //     ****
            //
            // Should be rendered as:
            //     <hr />
            //     <h2>foo</h2>
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 47, "Leaf blocks ATX headings");
			TestParser.TestSpec("****\n## foo\n****", "<hr />\n<h2>foo</h2>\n<hr />", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example048()
        {
            // Example 48
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     Foo bar
            //     # baz
            //     Bar foo
            //
            // Should be rendered as:
            //     <p>Foo bar</p>
            //     <h1>baz</h1>
            //     <p>Bar foo</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 48, "Leaf blocks ATX headings");
			TestParser.TestSpec("Foo bar\n# baz\nBar foo", "<p>Foo bar</p>\n<h1>baz</h1>\n<p>Bar foo</p>", "");
        }
    }
        // ATX headings can be empty:
    [TestFixture]
    public partial class TestLeafblocksATXheadings
    {
        [Test]
        public void Example049()
        {
            // Example 49
            // Section: Leaf blocks ATX headings
            //
            // The following CommonMark:
            //     ## 
            //     #
            //     ### ###
            //
            // Should be rendered as:
            //     <h2></h2>
            //     <h1></h1>
            //     <h3></h3>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 49, "Leaf blocks ATX headings");
			TestParser.TestSpec("## \n#\n### ###", "<h2></h2>\n<h1></h1>\n<h3></h3>", "");
        }
    }
        // ## Setext headings
        //
        // A [setext heading](@) consists of one or more
        // lines of text, each containing at least one [non-whitespace
        // character], with no more than 3 spaces indentation, followed by
        // a [setext heading underline].  The lines of text must be such
        // that, were they not followed by the setext heading underline,
        // they would be interpreted as a paragraph:  they cannot be
        // interpretable as a [code fence], [ATX heading][ATX headings],
        // [block quote][block quotes], [thematic break][thematic breaks],
        // [list item][list items], or [HTML block][HTML blocks].
        //
        // A [setext heading underline](@) is a sequence of
        // `=` characters or a sequence of `-` characters, with no more than 3
        // spaces indentation and any number of trailing spaces.  If a line
        // containing a single `-` can be interpreted as an
        // empty [list items], it should be interpreted this way
        // and not as a [setext heading underline].
        //
        // The heading is a level 1 heading if `=` characters are used in
        // the [setext heading underline], and a level 2 heading if `-`
        // characters are used.  The contents of the heading are the result
        // of parsing the preceding lines of text as CommonMark inline
        // content.
        //
        // In general, a setext heading need not be preceded or followed by a
        // blank line.  However, it cannot interrupt a paragraph, so when a
        // setext heading comes after a paragraph, a blank line is needed between
        // them.
        //
        // Simple examples:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example050()
        {
            // Example 50
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     Foo *bar*
            //     =========
            //     
            //     Foo *bar*
            //     ---------
            //
            // Should be rendered as:
            //     <h1>Foo <em>bar</em></h1>
            //     <h2>Foo <em>bar</em></h2>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 50, "Leaf blocks Setext headings");
			TestParser.TestSpec("Foo *bar*\n=========\n\nFoo *bar*\n---------", "<h1>Foo <em>bar</em></h1>\n<h2>Foo <em>bar</em></h2>", "");
        }
    }
        // The content of the header may span more than one line:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example051()
        {
            // Example 51
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     Foo *bar
            //     baz*
            //     ====
            //
            // Should be rendered as:
            //     <h1>Foo <em>bar
            //     baz</em></h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 51, "Leaf blocks Setext headings");
			TestParser.TestSpec("Foo *bar\nbaz*\n====", "<h1>Foo <em>bar\nbaz</em></h1>", "");
        }
    }
        // The underlining can be any length:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example052()
        {
            // Example 52
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     Foo
            //     -------------------------
            //     
            //     Foo
            //     =
            //
            // Should be rendered as:
            //     <h2>Foo</h2>
            //     <h1>Foo</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 52, "Leaf blocks Setext headings");
			TestParser.TestSpec("Foo\n-------------------------\n\nFoo\n=", "<h2>Foo</h2>\n<h1>Foo</h1>", "");
        }
    }
        // The heading content can be indented up to three spaces, and need
        // not line up with the underlining:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example053()
        {
            // Example 53
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //        Foo
            //     ---
            //     
            //       Foo
            //     -----
            //     
            //       Foo
            //       ===
            //
            // Should be rendered as:
            //     <h2>Foo</h2>
            //     <h2>Foo</h2>
            //     <h1>Foo</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 53, "Leaf blocks Setext headings");
			TestParser.TestSpec("   Foo\n---\n\n  Foo\n-----\n\n  Foo\n  ===", "<h2>Foo</h2>\n<h2>Foo</h2>\n<h1>Foo</h1>", "");
        }
    }
        // Four spaces indent is too much:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example054()
        {
            // Example 54
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //         Foo
            //         ---
            //     
            //         Foo
            //     ---
            //
            // Should be rendered as:
            //     <pre><code>Foo
            //     ---
            //     
            //     Foo
            //     </code></pre>
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 54, "Leaf blocks Setext headings");
			TestParser.TestSpec("    Foo\n    ---\n\n    Foo\n---", "<pre><code>Foo\n---\n\nFoo\n</code></pre>\n<hr />", "");
        }
    }
        // The setext heading underline can be indented up to three spaces, and
        // may have trailing spaces:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example055()
        {
            // Example 55
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     Foo
            //        ----      
            //
            // Should be rendered as:
            //     <h2>Foo</h2>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 55, "Leaf blocks Setext headings");
			TestParser.TestSpec("Foo\n   ----      ", "<h2>Foo</h2>", "");
        }
    }
        // Four spaces is too much:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example056()
        {
            // Example 56
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     Foo
            //         ---
            //
            // Should be rendered as:
            //     <p>Foo
            //     ---</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 56, "Leaf blocks Setext headings");
			TestParser.TestSpec("Foo\n    ---", "<p>Foo\n---</p>", "");
        }
    }
        // The setext heading underline cannot contain internal spaces:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example057()
        {
            // Example 57
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     Foo
            //     = =
            //     
            //     Foo
            //     --- -
            //
            // Should be rendered as:
            //     <p>Foo
            //     = =</p>
            //     <p>Foo</p>
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 57, "Leaf blocks Setext headings");
			TestParser.TestSpec("Foo\n= =\n\nFoo\n--- -", "<p>Foo\n= =</p>\n<p>Foo</p>\n<hr />", "");
        }
    }
        // Trailing spaces in the content line do not cause a line break:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example058()
        {
            // Example 58
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     Foo  
            //     -----
            //
            // Should be rendered as:
            //     <h2>Foo</h2>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 58, "Leaf blocks Setext headings");
			TestParser.TestSpec("Foo  \n-----", "<h2>Foo</h2>", "");
        }
    }
        // Nor does a backslash at the end:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example059()
        {
            // Example 59
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     Foo\
            //     ----
            //
            // Should be rendered as:
            //     <h2>Foo\</h2>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 59, "Leaf blocks Setext headings");
			TestParser.TestSpec("Foo\\\n----", "<h2>Foo\\</h2>", "");
        }
    }
        // Since indicators of block structure take precedence over
        // indicators of inline structure, the following are setext headings:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example060()
        {
            // Example 60
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     `Foo
            //     ----
            //     `
            //     
            //     <a title="a lot
            //     ---
            //     of dashes"/>
            //
            // Should be rendered as:
            //     <h2>`Foo</h2>
            //     <p>`</p>
            //     <h2>&lt;a title=&quot;a lot</h2>
            //     <p>of dashes&quot;/&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 60, "Leaf blocks Setext headings");
			TestParser.TestSpec("`Foo\n----\n`\n\n<a title=\"a lot\n---\nof dashes\"/>", "<h2>`Foo</h2>\n<p>`</p>\n<h2>&lt;a title=&quot;a lot</h2>\n<p>of dashes&quot;/&gt;</p>", "");
        }
    }
        // The setext heading underline cannot be a [lazy continuation
        // line] in a list item or block quote:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example061()
        {
            // Example 61
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     > Foo
            //     ---
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>Foo</p>
            //     </blockquote>
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 61, "Leaf blocks Setext headings");
			TestParser.TestSpec("> Foo\n---", "<blockquote>\n<p>Foo</p>\n</blockquote>\n<hr />", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example062()
        {
            // Example 62
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     > foo
            //     bar
            //     ===
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>foo
            //     bar
            //     ===</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 62, "Leaf blocks Setext headings");
			TestParser.TestSpec("> foo\nbar\n===", "<blockquote>\n<p>foo\nbar\n===</p>\n</blockquote>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example063()
        {
            // Example 63
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     - Foo
            //     ---
            //
            // Should be rendered as:
            //     <ul>
            //     <li>Foo</li>
            //     </ul>
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 63, "Leaf blocks Setext headings");
			TestParser.TestSpec("- Foo\n---", "<ul>\n<li>Foo</li>\n</ul>\n<hr />", "");
        }
    }
        // A blank line is needed between a paragraph and a following
        // setext heading, since otherwise the paragraph becomes part
        // of the heading's content:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example064()
        {
            // Example 64
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     Foo
            //     Bar
            //     ---
            //
            // Should be rendered as:
            //     <h2>Foo
            //     Bar</h2>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 64, "Leaf blocks Setext headings");
			TestParser.TestSpec("Foo\nBar\n---", "<h2>Foo\nBar</h2>", "");
        }
    }
        // But in general a blank line is not required before or after
        // setext headings:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example065()
        {
            // Example 65
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     ---
            //     Foo
            //     ---
            //     Bar
            //     ---
            //     Baz
            //
            // Should be rendered as:
            //     <hr />
            //     <h2>Foo</h2>
            //     <h2>Bar</h2>
            //     <p>Baz</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 65, "Leaf blocks Setext headings");
			TestParser.TestSpec("---\nFoo\n---\nBar\n---\nBaz", "<hr />\n<h2>Foo</h2>\n<h2>Bar</h2>\n<p>Baz</p>", "");
        }
    }
        // Setext headings cannot be empty:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example066()
        {
            // Example 66
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     ====
            //
            // Should be rendered as:
            //     <p>====</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 66, "Leaf blocks Setext headings");
			TestParser.TestSpec("====", "<p>====</p>", "");
        }
    }
        // Setext heading text lines must not be interpretable as block
        // constructs other than paragraphs.  So, the line of dashes
        // in these examples gets interpreted as a thematic break:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example067()
        {
            // Example 67
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     ---
            //     ---
            //
            // Should be rendered as:
            //     <hr />
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 67, "Leaf blocks Setext headings");
			TestParser.TestSpec("---\n---", "<hr />\n<hr />", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example068()
        {
            // Example 68
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     - foo
            //     -----
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     </ul>
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 68, "Leaf blocks Setext headings");
			TestParser.TestSpec("- foo\n-----", "<ul>\n<li>foo</li>\n</ul>\n<hr />", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example069()
        {
            // Example 69
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //         foo
            //     ---
            //
            // Should be rendered as:
            //     <pre><code>foo
            //     </code></pre>
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 69, "Leaf blocks Setext headings");
			TestParser.TestSpec("    foo\n---", "<pre><code>foo\n</code></pre>\n<hr />", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example070()
        {
            // Example 70
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     > foo
            //     -----
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>foo</p>
            //     </blockquote>
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 70, "Leaf blocks Setext headings");
			TestParser.TestSpec("> foo\n-----", "<blockquote>\n<p>foo</p>\n</blockquote>\n<hr />", "");
        }
    }
        // If you want a heading with `> foo` as its literal text, you can
        // use backslash escapes:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example071()
        {
            // Example 71
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     \> foo
            //     ------
            //
            // Should be rendered as:
            //     <h2>&gt; foo</h2>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 71, "Leaf blocks Setext headings");
			TestParser.TestSpec("\\> foo\n------", "<h2>&gt; foo</h2>", "");
        }
    }
        // **Compatibility note:**  Most existing Markdown implementations
        // do not allow the text of setext headings to span multiple lines.
        // But there is no consensus about how to interpret
        //
        // ``` markdown
        // Foo
        // bar
        // ---
        // baz
        // ```
        //
        // One can find four different interpretations:
        //
        // 1. paragraph "Foo", heading "bar", paragraph "baz"
        // 2. paragraph "Foo bar", thematic break, paragraph "baz"
        // 3. paragraph "Foo bar --- baz"
        // 4. heading "Foo bar", paragraph "baz"
        //
        // We find interpretation 4 most natural, and interpretation 4
        // increases the expressive power of CommonMark, by allowing
        // multiline headings.  Authors who want interpretation 1 can
        // put a blank line after the first paragraph:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example072()
        {
            // Example 72
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     Foo
            //     
            //     bar
            //     ---
            //     baz
            //
            // Should be rendered as:
            //     <p>Foo</p>
            //     <h2>bar</h2>
            //     <p>baz</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 72, "Leaf blocks Setext headings");
			TestParser.TestSpec("Foo\n\nbar\n---\nbaz", "<p>Foo</p>\n<h2>bar</h2>\n<p>baz</p>", "");
        }
    }
        // Authors who want interpretation 2 can put blank lines around
        // the thematic break,
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example073()
        {
            // Example 73
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     Foo
            //     bar
            //     
            //     ---
            //     
            //     baz
            //
            // Should be rendered as:
            //     <p>Foo
            //     bar</p>
            //     <hr />
            //     <p>baz</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 73, "Leaf blocks Setext headings");
			TestParser.TestSpec("Foo\nbar\n\n---\n\nbaz", "<p>Foo\nbar</p>\n<hr />\n<p>baz</p>", "");
        }
    }
        // or use a thematic break that cannot count as a [setext heading
        // underline], such as
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example074()
        {
            // Example 74
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     Foo
            //     bar
            //     * * *
            //     baz
            //
            // Should be rendered as:
            //     <p>Foo
            //     bar</p>
            //     <hr />
            //     <p>baz</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 74, "Leaf blocks Setext headings");
			TestParser.TestSpec("Foo\nbar\n* * *\nbaz", "<p>Foo\nbar</p>\n<hr />\n<p>baz</p>", "");
        }
    }
        // Authors who want interpretation 3 can use backslash escapes:
    [TestFixture]
    public partial class TestLeafblocksSetextheadings
    {
        [Test]
        public void Example075()
        {
            // Example 75
            // Section: Leaf blocks Setext headings
            //
            // The following CommonMark:
            //     Foo
            //     bar
            //     \---
            //     baz
            //
            // Should be rendered as:
            //     <p>Foo
            //     bar
            //     ---
            //     baz</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 75, "Leaf blocks Setext headings");
			TestParser.TestSpec("Foo\nbar\n\\---\nbaz", "<p>Foo\nbar\n---\nbaz</p>", "");
        }
    }
        // ## Indented code blocks
        //
        // An [indented code block](@) is composed of one or more
        // [indented chunks] separated by blank lines.
        // An [indented chunk](@) is a sequence of non-blank lines,
        // each indented four or more spaces. The contents of the code block are
        // the literal contents of the lines, including trailing
        // [line endings], minus four spaces of indentation.
        // An indented code block has no [info string].
        //
        // An indented code block cannot interrupt a paragraph, so there must be
        // a blank line between a paragraph and a following indented code block.
        // (A blank line is not needed, however, between a code block and a following
        // paragraph.)
    [TestFixture]
    public partial class TestLeafblocksIndentedcodeblocks
    {
        [Test]
        public void Example076()
        {
            // Example 76
            // Section: Leaf blocks Indented code blocks
            //
            // The following CommonMark:
            //         a simple
            //           indented code block
            //
            // Should be rendered as:
            //     <pre><code>a simple
            //       indented code block
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 76, "Leaf blocks Indented code blocks");
			TestParser.TestSpec("    a simple\n      indented code block", "<pre><code>a simple\n  indented code block\n</code></pre>", "");
        }
    }
        // If there is any ambiguity between an interpretation of indentation
        // as a code block and as indicating that material belongs to a [list
        // item][list items], the list item interpretation takes precedence:
    [TestFixture]
    public partial class TestLeafblocksIndentedcodeblocks
    {
        [Test]
        public void Example077()
        {
            // Example 77
            // Section: Leaf blocks Indented code blocks
            //
            // The following CommonMark:
            //       - foo
            //     
            //         bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>foo</p>
            //     <p>bar</p>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 77, "Leaf blocks Indented code blocks");
			TestParser.TestSpec("  - foo\n\n    bar", "<ul>\n<li>\n<p>foo</p>\n<p>bar</p>\n</li>\n</ul>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksIndentedcodeblocks
    {
        [Test]
        public void Example078()
        {
            // Example 78
            // Section: Leaf blocks Indented code blocks
            //
            // The following CommonMark:
            //     1.  foo
            //     
            //         - bar
            //
            // Should be rendered as:
            //     <ol>
            //     <li>
            //     <p>foo</p>
            //     <ul>
            //     <li>bar</li>
            //     </ul>
            //     </li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 78, "Leaf blocks Indented code blocks");
			TestParser.TestSpec("1.  foo\n\n    - bar", "<ol>\n<li>\n<p>foo</p>\n<ul>\n<li>bar</li>\n</ul>\n</li>\n</ol>", "");
        }
    }
        // The contents of a code block are literal text, and do not get parsed
        // as Markdown:
    [TestFixture]
    public partial class TestLeafblocksIndentedcodeblocks
    {
        [Test]
        public void Example079()
        {
            // Example 79
            // Section: Leaf blocks Indented code blocks
            //
            // The following CommonMark:
            //         <a/>
            //         *hi*
            //     
            //         - one
            //
            // Should be rendered as:
            //     <pre><code>&lt;a/&gt;
            //     *hi*
            //     
            //     - one
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 79, "Leaf blocks Indented code blocks");
			TestParser.TestSpec("    <a/>\n    *hi*\n\n    - one", "<pre><code>&lt;a/&gt;\n*hi*\n\n- one\n</code></pre>", "");
        }
    }
        // Here we have three chunks separated by blank lines:
    [TestFixture]
    public partial class TestLeafblocksIndentedcodeblocks
    {
        [Test]
        public void Example080()
        {
            // Example 80
            // Section: Leaf blocks Indented code blocks
            //
            // The following CommonMark:
            //         chunk1
            //     
            //         chunk2
            //       
            //      
            //      
            //         chunk3
            //
            // Should be rendered as:
            //     <pre><code>chunk1
            //     
            //     chunk2
            //     
            //     
            //     
            //     chunk3
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 80, "Leaf blocks Indented code blocks");
			TestParser.TestSpec("    chunk1\n\n    chunk2\n  \n \n \n    chunk3", "<pre><code>chunk1\n\nchunk2\n\n\n\nchunk3\n</code></pre>", "");
        }
    }
        // Any initial spaces beyond four will be included in the content, even
        // in interior blank lines:
    [TestFixture]
    public partial class TestLeafblocksIndentedcodeblocks
    {
        [Test]
        public void Example081()
        {
            // Example 81
            // Section: Leaf blocks Indented code blocks
            //
            // The following CommonMark:
            //         chunk1
            //           
            //           chunk2
            //
            // Should be rendered as:
            //     <pre><code>chunk1
            //       
            //       chunk2
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 81, "Leaf blocks Indented code blocks");
			TestParser.TestSpec("    chunk1\n      \n      chunk2", "<pre><code>chunk1\n  \n  chunk2\n</code></pre>", "");
        }
    }
        // An indented code block cannot interrupt a paragraph.  (This
        // allows hanging indents and the like.)
    [TestFixture]
    public partial class TestLeafblocksIndentedcodeblocks
    {
        [Test]
        public void Example082()
        {
            // Example 82
            // Section: Leaf blocks Indented code blocks
            //
            // The following CommonMark:
            //     Foo
            //         bar
            //     
            //
            // Should be rendered as:
            //     <p>Foo
            //     bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 82, "Leaf blocks Indented code blocks");
			TestParser.TestSpec("Foo\n    bar\n", "<p>Foo\nbar</p>", "");
        }
    }
        // However, any non-blank line with fewer than four leading spaces ends
        // the code block immediately.  So a paragraph may occur immediately
        // after indented code:
    [TestFixture]
    public partial class TestLeafblocksIndentedcodeblocks
    {
        [Test]
        public void Example083()
        {
            // Example 83
            // Section: Leaf blocks Indented code blocks
            //
            // The following CommonMark:
            //         foo
            //     bar
            //
            // Should be rendered as:
            //     <pre><code>foo
            //     </code></pre>
            //     <p>bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 83, "Leaf blocks Indented code blocks");
			TestParser.TestSpec("    foo\nbar", "<pre><code>foo\n</code></pre>\n<p>bar</p>", "");
        }
    }
        // And indented code can occur immediately before and after other kinds of
        // blocks:
    [TestFixture]
    public partial class TestLeafblocksIndentedcodeblocks
    {
        [Test]
        public void Example084()
        {
            // Example 84
            // Section: Leaf blocks Indented code blocks
            //
            // The following CommonMark:
            //     # Heading
            //         foo
            //     Heading
            //     ------
            //         foo
            //     ----
            //
            // Should be rendered as:
            //     <h1>Heading</h1>
            //     <pre><code>foo
            //     </code></pre>
            //     <h2>Heading</h2>
            //     <pre><code>foo
            //     </code></pre>
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 84, "Leaf blocks Indented code blocks");
			TestParser.TestSpec("# Heading\n    foo\nHeading\n------\n    foo\n----", "<h1>Heading</h1>\n<pre><code>foo\n</code></pre>\n<h2>Heading</h2>\n<pre><code>foo\n</code></pre>\n<hr />", "");
        }
    }
        // The first line can be indented more than four spaces:
    [TestFixture]
    public partial class TestLeafblocksIndentedcodeblocks
    {
        [Test]
        public void Example085()
        {
            // Example 85
            // Section: Leaf blocks Indented code blocks
            //
            // The following CommonMark:
            //             foo
            //         bar
            //
            // Should be rendered as:
            //     <pre><code>    foo
            //     bar
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 85, "Leaf blocks Indented code blocks");
			TestParser.TestSpec("        foo\n    bar", "<pre><code>    foo\nbar\n</code></pre>", "");
        }
    }
        // Blank lines preceding or following an indented code block
        // are not included in it:
    [TestFixture]
    public partial class TestLeafblocksIndentedcodeblocks
    {
        [Test]
        public void Example086()
        {
            // Example 86
            // Section: Leaf blocks Indented code blocks
            //
            // The following CommonMark:
            //         
            //         foo
            //         
            //     
            //
            // Should be rendered as:
            //     <pre><code>foo
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 86, "Leaf blocks Indented code blocks");
			TestParser.TestSpec("    \n    foo\n    \n", "<pre><code>foo\n</code></pre>", "");
        }
    }
        // Trailing spaces are included in the code block's content:
    [TestFixture]
    public partial class TestLeafblocksIndentedcodeblocks
    {
        [Test]
        public void Example087()
        {
            // Example 87
            // Section: Leaf blocks Indented code blocks
            //
            // The following CommonMark:
            //         foo  
            //
            // Should be rendered as:
            //     <pre><code>foo  
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 87, "Leaf blocks Indented code blocks");
			TestParser.TestSpec("    foo  ", "<pre><code>foo  \n</code></pre>", "");
        }
    }
        // ## Fenced code blocks
        //
        // A [code fence](@) is a sequence
        // of at least three consecutive backtick characters (`` ` ``) or
        // tildes (`~`).  (Tildes and backticks cannot be mixed.)
        // A [fenced code block](@)
        // begins with a code fence, indented no more than three spaces.
        //
        // The line with the opening code fence may optionally contain some text
        // following the code fence; this is trimmed of leading and trailing
        // spaces and called the [info string](@).
        // The [info string] may not contain any backtick
        // characters.  (The reason for this restriction is that otherwise
        // some inline code would be incorrectly interpreted as the
        // beginning of a fenced code block.)
        //
        // The content of the code block consists of all subsequent lines, until
        // a closing [code fence] of the same type as the code block
        // began with (backticks or tildes), and with at least as many backticks
        // or tildes as the opening code fence.  If the leading code fence is
        // indented N spaces, then up to N spaces of indentation are removed from
        // each line of the content (if present).  (If a content line is not
        // indented, it is preserved unchanged.  If it is indented less than N
        // spaces, all of the indentation is removed.)
        //
        // The closing code fence may be indented up to three spaces, and may be
        // followed only by spaces, which are ignored.  If the end of the
        // containing block (or document) is reached and no closing code fence
        // has been found, the code block contains all of the lines after the
        // opening code fence until the end of the containing block (or
        // document).  (An alternative spec would require backtracking in the
        // event that a closing code fence is not found.  But this makes parsing
        // much less efficient, and there seems to be no real down side to the
        // behavior described here.)
        //
        // A fenced code block may interrupt a paragraph, and does not require
        // a blank line either before or after.
        //
        // The content of a code fence is treated as literal text, not parsed
        // as inlines.  The first word of the [info string] is typically used to
        // specify the language of the code sample, and rendered in the `class`
        // attribute of the `code` tag.  However, this spec does not mandate any
        // particular treatment of the [info string].
        //
        // Here is a simple example with backticks:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example088()
        {
            // Example 88
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     <
            //      >
            //     ```
            //
            // Should be rendered as:
            //     <pre><code>&lt;
            //      &gt;
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 88, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("```\n<\n >\n```", "<pre><code>&lt;\n &gt;\n</code></pre>", "");
        }
    }
        // With tildes:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example089()
        {
            // Example 89
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ~~~
            //     <
            //      >
            //     ~~~
            //
            // Should be rendered as:
            //     <pre><code>&lt;
            //      &gt;
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 89, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("~~~\n<\n >\n~~~", "<pre><code>&lt;\n &gt;\n</code></pre>", "");
        }
    }
        // The closing code fence must use the same character as the opening
        // fence:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example090()
        {
            // Example 90
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     aaa
            //     ~~~
            //     ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     ~~~
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 90, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("```\naaa\n~~~\n```", "<pre><code>aaa\n~~~\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example091()
        {
            // Example 91
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ~~~
            //     aaa
            //     ```
            //     ~~~
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     ```
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 91, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("~~~\naaa\n```\n~~~", "<pre><code>aaa\n```\n</code></pre>", "");
        }
    }
        // The closing code fence must be at least as long as the opening fence:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example092()
        {
            // Example 92
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ````
            //     aaa
            //     ```
            //     ``````
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     ```
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 92, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("````\naaa\n```\n``````", "<pre><code>aaa\n```\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example093()
        {
            // Example 93
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ~~~~
            //     aaa
            //     ~~~
            //     ~~~~
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     ~~~
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 93, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("~~~~\naaa\n~~~\n~~~~", "<pre><code>aaa\n~~~\n</code></pre>", "");
        }
    }
        // Unclosed code blocks are closed by the end of the document
        // (or the enclosing [block quote][block quotes] or [list item][list items]):
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example094()
        {
            // Example 94
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //
            // Should be rendered as:
            //     <pre><code></code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 94, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("```", "<pre><code></code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example095()
        {
            // Example 95
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     `````
            //     
            //     ```
            //     aaa
            //
            // Should be rendered as:
            //     <pre><code>
            //     ```
            //     aaa
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 95, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("`````\n\n```\naaa", "<pre><code>\n```\naaa\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example096()
        {
            // Example 96
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     > ```
            //     > aaa
            //     
            //     bbb
            //
            // Should be rendered as:
            //     <blockquote>
            //     <pre><code>aaa
            //     </code></pre>
            //     </blockquote>
            //     <p>bbb</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 96, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("> ```\n> aaa\n\nbbb", "<blockquote>\n<pre><code>aaa\n</code></pre>\n</blockquote>\n<p>bbb</p>", "");
        }
    }
        // A code block can have all empty lines as its content:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example097()
        {
            // Example 97
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     
            //       
            //     ```
            //
            // Should be rendered as:
            //     <pre><code>
            //       
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 97, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("```\n\n  \n```", "<pre><code>\n  \n</code></pre>", "");
        }
    }
        // A code block can be empty:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example098()
        {
            // Example 98
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     ```
            //
            // Should be rendered as:
            //     <pre><code></code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 98, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("```\n```", "<pre><code></code></pre>", "");
        }
    }
        // Fences can be indented.  If the opening fence is indented,
        // content lines will have equivalent opening indentation removed,
        // if present:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example099()
        {
            // Example 99
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //      ```
            //      aaa
            //     aaa
            //     ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     aaa
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 99, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec(" ```\n aaa\naaa\n```", "<pre><code>aaa\naaa\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example100()
        {
            // Example 100
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //       ```
            //     aaa
            //       aaa
            //     aaa
            //       ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     aaa
            //     aaa
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 100, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("  ```\naaa\n  aaa\naaa\n  ```", "<pre><code>aaa\naaa\naaa\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example101()
        {
            // Example 101
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //        ```
            //        aaa
            //         aaa
            //       aaa
            //        ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //      aaa
            //     aaa
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 101, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("   ```\n   aaa\n    aaa\n  aaa\n   ```", "<pre><code>aaa\n aaa\naaa\n</code></pre>", "");
        }
    }
        // Four spaces indentation produces an indented code block:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example102()
        {
            // Example 102
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //         ```
            //         aaa
            //         ```
            //
            // Should be rendered as:
            //     <pre><code>```
            //     aaa
            //     ```
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 102, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("    ```\n    aaa\n    ```", "<pre><code>```\naaa\n```\n</code></pre>", "");
        }
    }
        // Closing fences may be indented by 0-3 spaces, and their indentation
        // need not match that of the opening fence:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example103()
        {
            // Example 103
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     aaa
            //       ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 103, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("```\naaa\n  ```", "<pre><code>aaa\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example104()
        {
            // Example 104
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //        ```
            //     aaa
            //       ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 104, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("   ```\naaa\n  ```", "<pre><code>aaa\n</code></pre>", "");
        }
    }
        // This is not a closing fence, because it is indented 4 spaces:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example105()
        {
            // Example 105
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     aaa
            //         ```
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //         ```
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 105, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("```\naaa\n    ```", "<pre><code>aaa\n    ```\n</code></pre>", "");
        }
    }
        // Code fences (opening and closing) cannot contain internal spaces:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example106()
        {
            // Example 106
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ``` ```
            //     aaa
            //
            // Should be rendered as:
            //     <p><code></code>
            //     aaa</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 106, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("``` ```\naaa", "<p><code></code>\naaa</p>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example107()
        {
            // Example 107
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ~~~~~~
            //     aaa
            //     ~~~ ~~
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     ~~~ ~~
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 107, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("~~~~~~\naaa\n~~~ ~~", "<pre><code>aaa\n~~~ ~~\n</code></pre>", "");
        }
    }
        // Fenced code blocks can interrupt paragraphs, and can be followed
        // directly by paragraphs, without a blank line between:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example108()
        {
            // Example 108
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     foo
            //     ```
            //     bar
            //     ```
            //     baz
            //
            // Should be rendered as:
            //     <p>foo</p>
            //     <pre><code>bar
            //     </code></pre>
            //     <p>baz</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 108, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("foo\n```\nbar\n```\nbaz", "<p>foo</p>\n<pre><code>bar\n</code></pre>\n<p>baz</p>", "");
        }
    }
        // Other blocks can also occur before and after fenced code blocks
        // without an intervening blank line:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example109()
        {
            // Example 109
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     foo
            //     ---
            //     ~~~
            //     bar
            //     ~~~
            //     # baz
            //
            // Should be rendered as:
            //     <h2>foo</h2>
            //     <pre><code>bar
            //     </code></pre>
            //     <h1>baz</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 109, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("foo\n---\n~~~\nbar\n~~~\n# baz", "<h2>foo</h2>\n<pre><code>bar\n</code></pre>\n<h1>baz</h1>", "");
        }
    }
        // An [info string] can be provided after the opening code fence.
        // Opening and closing spaces will be stripped, and the first word, prefixed
        // with `language-`, is used as the value for the `class` attribute of the
        // `code` element within the enclosing `pre` element.
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example110()
        {
            // Example 110
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ```ruby
            //     def foo(x)
            //       return 3
            //     end
            //     ```
            //
            // Should be rendered as:
            //     <pre><code class="language-ruby">def foo(x)
            //       return 3
            //     end
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 110, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("```ruby\ndef foo(x)\n  return 3\nend\n```", "<pre><code class=\"language-ruby\">def foo(x)\n  return 3\nend\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example111()
        {
            // Example 111
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ~~~~    ruby startline=3 $%@#$
            //     def foo(x)
            //       return 3
            //     end
            //     ~~~~~~~
            //
            // Should be rendered as:
            //     <pre><code class="language-ruby">def foo(x)
            //       return 3
            //     end
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 111, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("~~~~    ruby startline=3 $%@#$\ndef foo(x)\n  return 3\nend\n~~~~~~~", "<pre><code class=\"language-ruby\">def foo(x)\n  return 3\nend\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example112()
        {
            // Example 112
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ````;
            //     ````
            //
            // Should be rendered as:
            //     <pre><code class="language-;"></code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 112, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("````;\n````", "<pre><code class=\"language-;\"></code></pre>", "");
        }
    }
        // [Info strings] for backtick code blocks cannot contain backticks:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example113()
        {
            // Example 113
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ``` aa ```
            //     foo
            //
            // Should be rendered as:
            //     <p><code>aa</code>
            //     foo</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 113, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("``` aa ```\nfoo", "<p><code>aa</code>\nfoo</p>", "");
        }
    }
        // Closing code fences cannot have [info strings]:
    [TestFixture]
    public partial class TestLeafblocksFencedcodeblocks
    {
        [Test]
        public void Example114()
        {
            // Example 114
            // Section: Leaf blocks Fenced code blocks
            //
            // The following CommonMark:
            //     ```
            //     ``` aaa
            //     ```
            //
            // Should be rendered as:
            //     <pre><code>``` aaa
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 114, "Leaf blocks Fenced code blocks");
			TestParser.TestSpec("```\n``` aaa\n```", "<pre><code>``` aaa\n</code></pre>", "");
        }
    }
        // ## HTML blocks
        //
        // An [HTML block](@) is a group of lines that is treated
        // as raw HTML (and will not be escaped in HTML output).
        //
        // There are seven kinds of [HTML block], which can be defined
        // by their start and end conditions.  The block begins with a line that
        // meets a [start condition](@) (after up to three spaces
        // optional indentation).  It ends with the first subsequent line that
        // meets a matching [end condition](@), or the last line of
        // the document or other [container block](@), if no line is encountered that meets the
        // [end condition].  If the first line meets both the [start condition]
        // and the [end condition], the block will contain just that line.
        //
        // 1.  **Start condition:**  line begins with the string `<script`,
        // `<pre`, or `<style` (case-insensitive), followed by whitespace,
        // the string `>`, or the end of the line.\
        // **End condition:**  line contains an end tag
        // `</script>`, `</pre>`, or `</style>` (case-insensitive; it
        // need not match the start tag).
        //
        // 2.  **Start condition:** line begins with the string `<!--`.\
        // **End condition:**  line contains the string `-->`.
        //
        // 3.  **Start condition:** line begins with the string `<?`.\
        // **End condition:** line contains the string `?>`.
        //
        // 4.  **Start condition:** line begins with the string `<!`
        // followed by an uppercase ASCII letter.\
        // **End condition:** line contains the character `>`.
        //
        // 5.  **Start condition:**  line begins with the string
        // `<![CDATA[`.\
        // **End condition:** line contains the string `]]>`.
        //
        // 6.  **Start condition:** line begins the string `<` or `</`
        // followed by one of the strings (case-insensitive) `address`,
        // `article`, `aside`, `base`, `basefont`, `blockquote`, `body`,
        // `caption`, `center`, `col`, `colgroup`, `dd`, `details`, `dialog`,
        // `dir`, `div`, `dl`, `dt`, `fieldset`, `figcaption`, `figure`,
        // `footer`, `form`, `frame`, `frameset`, `h1`, `head`, `header`, `hr`,
        // `html`, `iframe`, `legend`, `li`, `link`, `main`, `menu`, `menuitem`,
        // `meta`, `nav`, `noframes`, `ol`, `optgroup`, `option`, `p`, `param`,
        // `section`, `source`, `summary`, `table`, `tbody`, `td`,
        // `tfoot`, `th`, `thead`, `title`, `tr`, `track`, `ul`, followed
        // by [whitespace], the end of the line, the string `>`, or
        // the string `/>`.\
        // **End condition:** line is followed by a [blank line].
        //
        // 7.  **Start condition:**  line begins with a complete [open tag]
        // or [closing tag] (with any [tag name] other than `script`,
        // `style`, or `pre`) followed only by [whitespace]
        // or the end of the line.\
        // **End condition:** line is followed by a [blank line].
        //
        // All types of [HTML blocks] except type 7 may interrupt
        // a paragraph.  Blocks of type 7 may not interrupt a paragraph.
        // (This restriction is intended to prevent unwanted interpretation
        // of long tags inside a wrapped paragraph as starting HTML blocks.)
        //
        // Some simple examples follow.  Here are some basic HTML blocks
        // of type 6:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example115()
        {
            // Example 115
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <table>
            //       <tr>
            //         <td>
            //                hi
            //         </td>
            //       </tr>
            //     </table>
            //     
            //     okay.
            //
            // Should be rendered as:
            //     <table>
            //       <tr>
            //         <td>
            //                hi
            //         </td>
            //       </tr>
            //     </table>
            //     <p>okay.</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 115, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<table>\n  <tr>\n    <td>\n           hi\n    </td>\n  </tr>\n</table>\n\nokay.", "<table>\n  <tr>\n    <td>\n           hi\n    </td>\n  </tr>\n</table>\n<p>okay.</p>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example116()
        {
            // Example 116
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //      <div>
            //       *hello*
            //              <foo><a>
            //
            // Should be rendered as:
            //      <div>
            //       *hello*
            //              <foo><a>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 116, "Leaf blocks HTML blocks");
			TestParser.TestSpec(" <div>\n  *hello*\n         <foo><a>", " <div>\n  *hello*\n         <foo><a>", "");
        }
    }
        // A block can also start with a closing tag:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example117()
        {
            // Example 117
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     </div>
            //     *foo*
            //
            // Should be rendered as:
            //     </div>
            //     *foo*

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 117, "Leaf blocks HTML blocks");
			TestParser.TestSpec("</div>\n*foo*", "</div>\n*foo*", "");
        }
    }
        // Here we have two HTML blocks with a Markdown paragraph between them:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example118()
        {
            // Example 118
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <DIV CLASS="foo">
            //     
            //     *Markdown*
            //     
            //     </DIV>
            //
            // Should be rendered as:
            //     <DIV CLASS="foo">
            //     <p><em>Markdown</em></p>
            //     </DIV>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 118, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<DIV CLASS=\"foo\">\n\n*Markdown*\n\n</DIV>", "<DIV CLASS=\"foo\">\n<p><em>Markdown</em></p>\n</DIV>", "");
        }
    }
        // The tag on the first line can be partial, as long
        // as it is split where there would be whitespace:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example119()
        {
            // Example 119
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <div id="foo"
            //       class="bar">
            //     </div>
            //
            // Should be rendered as:
            //     <div id="foo"
            //       class="bar">
            //     </div>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 119, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<div id=\"foo\"\n  class=\"bar\">\n</div>", "<div id=\"foo\"\n  class=\"bar\">\n</div>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example120()
        {
            // Example 120
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <div id="foo" class="bar
            //       baz">
            //     </div>
            //
            // Should be rendered as:
            //     <div id="foo" class="bar
            //       baz">
            //     </div>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 120, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<div id=\"foo\" class=\"bar\n  baz\">\n</div>", "<div id=\"foo\" class=\"bar\n  baz\">\n</div>", "");
        }
    }
        // An open tag need not be closed:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example121()
        {
            // Example 121
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <div>
            //     *foo*
            //     
            //     *bar*
            //
            // Should be rendered as:
            //     <div>
            //     *foo*
            //     <p><em>bar</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 121, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<div>\n*foo*\n\n*bar*", "<div>\n*foo*\n<p><em>bar</em></p>", "");
        }
    }
        // A partial tag need not even be completed (garbage
        // in, garbage out):
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example122()
        {
            // Example 122
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <div id="foo"
            //     *hi*
            //
            // Should be rendered as:
            //     <div id="foo"
            //     *hi*

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 122, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<div id=\"foo\"\n*hi*", "<div id=\"foo\"\n*hi*", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example123()
        {
            // Example 123
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <div class
            //     foo
            //
            // Should be rendered as:
            //     <div class
            //     foo

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 123, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<div class\nfoo", "<div class\nfoo", "");
        }
    }
        // The initial tag doesn't even need to be a valid
        // tag, as long as it starts like one:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example124()
        {
            // Example 124
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <div *???-&&&-<---
            //     *foo*
            //
            // Should be rendered as:
            //     <div *???-&&&-<---
            //     *foo*

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 124, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<div *???-&&&-<---\n*foo*", "<div *???-&&&-<---\n*foo*", "");
        }
    }
        // In type 6 blocks, the initial tag need not be on a line by
        // itself:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example125()
        {
            // Example 125
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <div><a href="bar">*foo*</a></div>
            //
            // Should be rendered as:
            //     <div><a href="bar">*foo*</a></div>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 125, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<div><a href=\"bar\">*foo*</a></div>", "<div><a href=\"bar\">*foo*</a></div>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example126()
        {
            // Example 126
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <table><tr><td>
            //     foo
            //     </td></tr></table>
            //
            // Should be rendered as:
            //     <table><tr><td>
            //     foo
            //     </td></tr></table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 126, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<table><tr><td>\nfoo\n</td></tr></table>", "<table><tr><td>\nfoo\n</td></tr></table>", "");
        }
    }
        // Everything until the next blank line or end of document
        // gets included in the HTML block.  So, in the following
        // example, what looks like a Markdown code block
        // is actually part of the HTML block, which continues until a blank
        // line or the end of the document is reached:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example127()
        {
            // Example 127
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <div></div>
            //     ``` c
            //     int x = 33;
            //     ```
            //
            // Should be rendered as:
            //     <div></div>
            //     ``` c
            //     int x = 33;
            //     ```

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 127, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<div></div>\n``` c\nint x = 33;\n```", "<div></div>\n``` c\nint x = 33;\n```", "");
        }
    }
        // To start an [HTML block] with a tag that is *not* in the
        // list of block-level tags in (6), you must put the tag by
        // itself on the first line (and it must be complete):
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example128()
        {
            // Example 128
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <a href="foo">
            //     *bar*
            //     </a>
            //
            // Should be rendered as:
            //     <a href="foo">
            //     *bar*
            //     </a>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 128, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<a href=\"foo\">\n*bar*\n</a>", "<a href=\"foo\">\n*bar*\n</a>", "");
        }
    }
        // In type 7 blocks, the [tag name] can be anything:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example129()
        {
            // Example 129
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <Warning>
            //     *bar*
            //     </Warning>
            //
            // Should be rendered as:
            //     <Warning>
            //     *bar*
            //     </Warning>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 129, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<Warning>\n*bar*\n</Warning>", "<Warning>\n*bar*\n</Warning>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example130()
        {
            // Example 130
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <i class="foo">
            //     *bar*
            //     </i>
            //
            // Should be rendered as:
            //     <i class="foo">
            //     *bar*
            //     </i>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 130, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<i class=\"foo\">\n*bar*\n</i>", "<i class=\"foo\">\n*bar*\n</i>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example131()
        {
            // Example 131
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     </ins>
            //     *bar*
            //
            // Should be rendered as:
            //     </ins>
            //     *bar*

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 131, "Leaf blocks HTML blocks");
			TestParser.TestSpec("</ins>\n*bar*", "</ins>\n*bar*", "");
        }
    }
        // These rules are designed to allow us to work with tags that
        // can function as either block-level or inline-level tags.
        // The `<del>` tag is a nice example.  We can surround content with
        // `<del>` tags in three different ways.  In this case, we get a raw
        // HTML block, because the `<del>` tag is on a line by itself:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example132()
        {
            // Example 132
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <del>
            //     *foo*
            //     </del>
            //
            // Should be rendered as:
            //     <del>
            //     *foo*
            //     </del>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 132, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<del>\n*foo*\n</del>", "<del>\n*foo*\n</del>", "");
        }
    }
        // In this case, we get a raw HTML block that just includes
        // the `<del>` tag (because it ends with the following blank
        // line).  So the contents get interpreted as CommonMark:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example133()
        {
            // Example 133
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <del>
            //     
            //     *foo*
            //     
            //     </del>
            //
            // Should be rendered as:
            //     <del>
            //     <p><em>foo</em></p>
            //     </del>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 133, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<del>\n\n*foo*\n\n</del>", "<del>\n<p><em>foo</em></p>\n</del>", "");
        }
    }
        // Finally, in this case, the `<del>` tags are interpreted
        // as [raw HTML] *inside* the CommonMark paragraph.  (Because
        // the tag is not on a line by itself, we get inline HTML
        // rather than an [HTML block].)
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example134()
        {
            // Example 134
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <del>*foo*</del>
            //
            // Should be rendered as:
            //     <p><del><em>foo</em></del></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 134, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<del>*foo*</del>", "<p><del><em>foo</em></del></p>", "");
        }
    }
        // HTML tags designed to contain literal content
        // (`script`, `style`, `pre`), comments, processing instructions,
        // and declarations are treated somewhat differently.
        // Instead of ending at the first blank line, these blocks
        // end at the first line containing a corresponding end tag.
        // As a result, these blocks can contain blank lines:
        //
        // A pre tag (type 1):
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example135()
        {
            // Example 135
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <pre language="haskell"><code>
            //     import Text.HTML.TagSoup
            //     
            //     main :: IO ()
            //     main = print $ parseTags tags
            //     </code></pre>
            //     okay
            //
            // Should be rendered as:
            //     <pre language="haskell"><code>
            //     import Text.HTML.TagSoup
            //     
            //     main :: IO ()
            //     main = print $ parseTags tags
            //     </code></pre>
            //     <p>okay</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 135, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<pre language=\"haskell\"><code>\nimport Text.HTML.TagSoup\n\nmain :: IO ()\nmain = print $ parseTags tags\n</code></pre>\nokay", "<pre language=\"haskell\"><code>\nimport Text.HTML.TagSoup\n\nmain :: IO ()\nmain = print $ parseTags tags\n</code></pre>\n<p>okay</p>", "");
        }
    }
        // A script tag (type 1):
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example136()
        {
            // Example 136
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <script type="text/javascript">
            //     // JavaScript example
            //     
            //     document.getElementById("demo").innerHTML = "Hello JavaScript!";
            //     </script>
            //     okay
            //
            // Should be rendered as:
            //     <script type="text/javascript">
            //     // JavaScript example
            //     
            //     document.getElementById("demo").innerHTML = "Hello JavaScript!";
            //     </script>
            //     <p>okay</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 136, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<script type=\"text/javascript\">\n// JavaScript example\n\ndocument.getElementById(\"demo\").innerHTML = \"Hello JavaScript!\";\n</script>\nokay", "<script type=\"text/javascript\">\n// JavaScript example\n\ndocument.getElementById(\"demo\").innerHTML = \"Hello JavaScript!\";\n</script>\n<p>okay</p>", "");
        }
    }
        // A style tag (type 1):
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example137()
        {
            // Example 137
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <style
            //       type="text/css">
            //     h1 {color:red;}
            //     
            //     p {color:blue;}
            //     </style>
            //     okay
            //
            // Should be rendered as:
            //     <style
            //       type="text/css">
            //     h1 {color:red;}
            //     
            //     p {color:blue;}
            //     </style>
            //     <p>okay</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 137, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<style\n  type=\"text/css\">\nh1 {color:red;}\n\np {color:blue;}\n</style>\nokay", "<style\n  type=\"text/css\">\nh1 {color:red;}\n\np {color:blue;}\n</style>\n<p>okay</p>", "");
        }
    }
        // If there is no matching end tag, the block will end at the
        // end of the document (or the enclosing [block quote][block quotes]
        // or [list item][list items]):
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example138()
        {
            // Example 138
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <style
            //       type="text/css">
            //     
            //     foo
            //
            // Should be rendered as:
            //     <style
            //       type="text/css">
            //     
            //     foo

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 138, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<style\n  type=\"text/css\">\n\nfoo", "<style\n  type=\"text/css\">\n\nfoo", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example139()
        {
            // Example 139
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     > <div>
            //     > foo
            //     
            //     bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <div>
            //     foo
            //     </blockquote>
            //     <p>bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 139, "Leaf blocks HTML blocks");
			TestParser.TestSpec("> <div>\n> foo\n\nbar", "<blockquote>\n<div>\nfoo\n</blockquote>\n<p>bar</p>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example140()
        {
            // Example 140
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     - <div>
            //     - foo
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <div>
            //     </li>
            //     <li>foo</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 140, "Leaf blocks HTML blocks");
			TestParser.TestSpec("- <div>\n- foo", "<ul>\n<li>\n<div>\n</li>\n<li>foo</li>\n</ul>", "");
        }
    }
        // The end tag can occur on the same line as the start tag:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example141()
        {
            // Example 141
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <style>p{color:red;}</style>
            //     *foo*
            //
            // Should be rendered as:
            //     <style>p{color:red;}</style>
            //     <p><em>foo</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 141, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<style>p{color:red;}</style>\n*foo*", "<style>p{color:red;}</style>\n<p><em>foo</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example142()
        {
            // Example 142
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <!-- foo -->*bar*
            //     *baz*
            //
            // Should be rendered as:
            //     <!-- foo -->*bar*
            //     <p><em>baz</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 142, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<!-- foo -->*bar*\n*baz*", "<!-- foo -->*bar*\n<p><em>baz</em></p>", "");
        }
    }
        // Note that anything on the last line after the
        // end tag will be included in the [HTML block]:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example143()
        {
            // Example 143
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <script>
            //     foo
            //     </script>1. *bar*
            //
            // Should be rendered as:
            //     <script>
            //     foo
            //     </script>1. *bar*

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 143, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<script>\nfoo\n</script>1. *bar*", "<script>\nfoo\n</script>1. *bar*", "");
        }
    }
        // A comment (type 2):
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example144()
        {
            // Example 144
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <!-- Foo
            //     
            //     bar
            //        baz -->
            //     okay
            //
            // Should be rendered as:
            //     <!-- Foo
            //     
            //     bar
            //        baz -->
            //     <p>okay</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 144, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<!-- Foo\n\nbar\n   baz -->\nokay", "<!-- Foo\n\nbar\n   baz -->\n<p>okay</p>", "");
        }
    }
        // A processing instruction (type 3):
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example145()
        {
            // Example 145
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <?php
            //     
            //       echo '>';
            //     
            //     ?>
            //     okay
            //
            // Should be rendered as:
            //     <?php
            //     
            //       echo '>';
            //     
            //     ?>
            //     <p>okay</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 145, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<?php\n\n  echo '>';\n\n?>\nokay", "<?php\n\n  echo '>';\n\n?>\n<p>okay</p>", "");
        }
    }
        // A declaration (type 4):
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example146()
        {
            // Example 146
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <!DOCTYPE html>
            //
            // Should be rendered as:
            //     <!DOCTYPE html>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 146, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<!DOCTYPE html>", "<!DOCTYPE html>", "");
        }
    }
        // CDATA (type 5):
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example147()
        {
            // Example 147
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <![CDATA[
            //     function matchwo(a,b)
            //     {
            //       if (a < b && a < 0) then {
            //         return 1;
            //     
            //       } else {
            //     
            //         return 0;
            //       }
            //     }
            //     ]]>
            //     okay
            //
            // Should be rendered as:
            //     <![CDATA[
            //     function matchwo(a,b)
            //     {
            //       if (a < b && a < 0) then {
            //         return 1;
            //     
            //       } else {
            //     
            //         return 0;
            //       }
            //     }
            //     ]]>
            //     <p>okay</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 147, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<![CDATA[\nfunction matchwo(a,b)\n{\n  if (a < b && a < 0) then {\n    return 1;\n\n  } else {\n\n    return 0;\n  }\n}\n]]>\nokay", "<![CDATA[\nfunction matchwo(a,b)\n{\n  if (a < b && a < 0) then {\n    return 1;\n\n  } else {\n\n    return 0;\n  }\n}\n]]>\n<p>okay</p>", "");
        }
    }
        // The opening tag can be indented 1-3 spaces, but not 4:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example148()
        {
            // Example 148
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //       <!-- foo -->
            //     
            //         <!-- foo -->
            //
            // Should be rendered as:
            //       <!-- foo -->
            //     <pre><code>&lt;!-- foo --&gt;
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 148, "Leaf blocks HTML blocks");
			TestParser.TestSpec("  <!-- foo -->\n\n    <!-- foo -->", "  <!-- foo -->\n<pre><code>&lt;!-- foo --&gt;\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example149()
        {
            // Example 149
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //       <div>
            //     
            //         <div>
            //
            // Should be rendered as:
            //       <div>
            //     <pre><code>&lt;div&gt;
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 149, "Leaf blocks HTML blocks");
			TestParser.TestSpec("  <div>\n\n    <div>", "  <div>\n<pre><code>&lt;div&gt;\n</code></pre>", "");
        }
    }
        // An HTML block of types 1--6 can interrupt a paragraph, and need not be
        // preceded by a blank line.
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example150()
        {
            // Example 150
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     Foo
            //     <div>
            //     bar
            //     </div>
            //
            // Should be rendered as:
            //     <p>Foo</p>
            //     <div>
            //     bar
            //     </div>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 150, "Leaf blocks HTML blocks");
			TestParser.TestSpec("Foo\n<div>\nbar\n</div>", "<p>Foo</p>\n<div>\nbar\n</div>", "");
        }
    }
        // However, a following blank line is needed, except at the end of
        // a document, and except for blocks of types 1--5, above:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example151()
        {
            // Example 151
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <div>
            //     bar
            //     </div>
            //     *foo*
            //
            // Should be rendered as:
            //     <div>
            //     bar
            //     </div>
            //     *foo*

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 151, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<div>\nbar\n</div>\n*foo*", "<div>\nbar\n</div>\n*foo*", "");
        }
    }
        // HTML blocks of type 7 cannot interrupt a paragraph:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example152()
        {
            // Example 152
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     Foo
            //     <a href="bar">
            //     baz
            //
            // Should be rendered as:
            //     <p>Foo
            //     <a href="bar">
            //     baz</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 152, "Leaf blocks HTML blocks");
			TestParser.TestSpec("Foo\n<a href=\"bar\">\nbaz", "<p>Foo\n<a href=\"bar\">\nbaz</p>", "");
        }
    }
        // This rule differs from John Gruber's original Markdown syntax
        // specification, which says:
        //
        // > The only restrictions are that block-level HTML elements —
        // > e.g. `<div>`, `<table>`, `<pre>`, `<p>`, etc. — must be separated from
        // > surrounding content by blank lines, and the start and end tags of the
        // > block should not be indented with tabs or spaces.
        //
        // In some ways Gruber's rule is more restrictive than the one given
        // here:
        //
        // - It requires that an HTML block be preceded by a blank line.
        // - It does not allow the start tag to be indented.
        // - It requires a matching end tag, which it also does not allow to
        // be indented.
        //
        // Most Markdown implementations (including some of Gruber's own) do not
        // respect all of these restrictions.
        //
        // There is one respect, however, in which Gruber's rule is more liberal
        // than the one given here, since it allows blank lines to occur inside
        // an HTML block.  There are two reasons for disallowing them here.
        // First, it removes the need to parse balanced tags, which is
        // expensive and can require backtracking from the end of the document
        // if no matching end tag is found. Second, it provides a very simple
        // and flexible way of including Markdown content inside HTML tags:
        // simply separate the Markdown from the HTML using blank lines:
        //
        // Compare:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example153()
        {
            // Example 153
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <div>
            //     
            //     *Emphasized* text.
            //     
            //     </div>
            //
            // Should be rendered as:
            //     <div>
            //     <p><em>Emphasized</em> text.</p>
            //     </div>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 153, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<div>\n\n*Emphasized* text.\n\n</div>", "<div>\n<p><em>Emphasized</em> text.</p>\n</div>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example154()
        {
            // Example 154
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <div>
            //     *Emphasized* text.
            //     </div>
            //
            // Should be rendered as:
            //     <div>
            //     *Emphasized* text.
            //     </div>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 154, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<div>\n*Emphasized* text.\n</div>", "<div>\n*Emphasized* text.\n</div>", "");
        }
    }
        // Some Markdown implementations have adopted a convention of
        // interpreting content inside tags as text if the open tag has
        // the attribute `markdown=1`.  The rule given above seems a simpler and
        // more elegant way of achieving the same expressive power, which is also
        // much simpler to parse.
        //
        // The main potential drawback is that one can no longer paste HTML
        // blocks into Markdown documents with 100% reliability.  However,
        // *in most cases* this will work fine, because the blank lines in
        // HTML are usually followed by HTML block tags.  For example:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example155()
        {
            // Example 155
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <table>
            //     
            //     <tr>
            //     
            //     <td>
            //     Hi
            //     </td>
            //     
            //     </tr>
            //     
            //     </table>
            //
            // Should be rendered as:
            //     <table>
            //     <tr>
            //     <td>
            //     Hi
            //     </td>
            //     </tr>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 155, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<table>\n\n<tr>\n\n<td>\nHi\n</td>\n\n</tr>\n\n</table>", "<table>\n<tr>\n<td>\nHi\n</td>\n</tr>\n</table>", "");
        }
    }
        // There are problems, however, if the inner tags are indented
        // *and* separated by spaces, as then they will be interpreted as
        // an indented code block:
    [TestFixture]
    public partial class TestLeafblocksHTMLblocks
    {
        [Test]
        public void Example156()
        {
            // Example 156
            // Section: Leaf blocks HTML blocks
            //
            // The following CommonMark:
            //     <table>
            //     
            //       <tr>
            //     
            //         <td>
            //           Hi
            //         </td>
            //     
            //       </tr>
            //     
            //     </table>
            //
            // Should be rendered as:
            //     <table>
            //       <tr>
            //     <pre><code>&lt;td&gt;
            //       Hi
            //     &lt;/td&gt;
            //     </code></pre>
            //       </tr>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 156, "Leaf blocks HTML blocks");
			TestParser.TestSpec("<table>\n\n  <tr>\n\n    <td>\n      Hi\n    </td>\n\n  </tr>\n\n</table>", "<table>\n  <tr>\n<pre><code>&lt;td&gt;\n  Hi\n&lt;/td&gt;\n</code></pre>\n  </tr>\n</table>", "");
        }
    }
        // Fortunately, blank lines are usually not necessary and can be
        // deleted.  The exception is inside `<pre>` tags, but as described
        // above, raw HTML blocks starting with `<pre>` *can* contain blank
        // lines.
        //
        // ## Link reference definitions
        //
        // A [link reference definition](@)
        // consists of a [link label], indented up to three spaces, followed
        // by a colon (`:`), optional [whitespace] (including up to one
        // [line ending]), a [link destination],
        // optional [whitespace] (including up to one
        // [line ending]), and an optional [link
        // title], which if it is present must be separated
        // from the [link destination] by [whitespace].
        // No further [non-whitespace characters] may occur on the line.
        //
        // A [link reference definition]
        // does not correspond to a structural element of a document.  Instead, it
        // defines a label which can be used in [reference links]
        // and reference-style [images] elsewhere in the document.  [Link
        // reference definitions] can come either before or after the links that use
        // them.
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example157()
        {
            // Example 157
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url "title"
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 157, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[foo]: /url \"title\"\n\n[foo]", "<p><a href=\"/url\" title=\"title\">foo</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example158()
        {
            // Example 158
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //        [foo]: 
            //           /url  
            //                'the title'  
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <p><a href="/url" title="the title">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 158, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("   [foo]: \n      /url  \n           'the title'  \n\n[foo]", "<p><a href=\"/url\" title=\"the title\">foo</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example159()
        {
            // Example 159
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [Foo*bar\]]:my_(url) 'title (with parens)'
            //     
            //     [Foo*bar\]]
            //
            // Should be rendered as:
            //     <p><a href="my_(url)" title="title (with parens)">Foo*bar]</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 159, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[Foo*bar\\]]:my_(url) 'title (with parens)'\n\n[Foo*bar\\]]", "<p><a href=\"my_(url)\" title=\"title (with parens)\">Foo*bar]</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example160()
        {
            // Example 160
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [Foo bar]:
            //     <my%20url>
            //     'title'
            //     
            //     [Foo bar]
            //
            // Should be rendered as:
            //     <p><a href="my%20url" title="title">Foo bar</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 160, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[Foo bar]:\n<my%20url>\n'title'\n\n[Foo bar]", "<p><a href=\"my%20url\" title=\"title\">Foo bar</a></p>", "");
        }
    }
        // The title may extend over multiple lines:
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example161()
        {
            // Example 161
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url '
            //     title
            //     line1
            //     line2
            //     '
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <p><a href="/url" title="
            //     title
            //     line1
            //     line2
            //     ">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 161, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[foo]: /url '\ntitle\nline1\nline2\n'\n\n[foo]", "<p><a href=\"/url\" title=\"\ntitle\nline1\nline2\n\">foo</a></p>", "");
        }
    }
        // However, it may not contain a [blank line]:
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example162()
        {
            // Example 162
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url 'title
            //     
            //     with blank line'
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <p>[foo]: /url 'title</p>
            //     <p>with blank line'</p>
            //     <p>[foo]</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 162, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[foo]: /url 'title\n\nwith blank line'\n\n[foo]", "<p>[foo]: /url 'title</p>\n<p>with blank line'</p>\n<p>[foo]</p>", "");
        }
    }
        // The title may be omitted:
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example163()
        {
            // Example 163
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [foo]:
            //     /url
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <p><a href="/url">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 163, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[foo]:\n/url\n\n[foo]", "<p><a href=\"/url\">foo</a></p>", "");
        }
    }
        // The link destination may not be omitted:
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example164()
        {
            // Example 164
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [foo]:
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <p>[foo]:</p>
            //     <p>[foo]</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 164, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[foo]:\n\n[foo]", "<p>[foo]:</p>\n<p>[foo]</p>", "");
        }
    }
        // Both title and destination can contain backslash escapes
        // and literal backslashes:
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example165()
        {
            // Example 165
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url\bar\*baz "foo\"bar\baz"
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <p><a href="/url%5Cbar*baz" title="foo&quot;bar\baz">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 165, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[foo]: /url\\bar\\*baz \"foo\\\"bar\\baz\"\n\n[foo]", "<p><a href=\"/url%5Cbar*baz\" title=\"foo&quot;bar\\baz\">foo</a></p>", "");
        }
    }
        // A link can come before its corresponding definition:
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example166()
        {
            // Example 166
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     [foo]: url
            //
            // Should be rendered as:
            //     <p><a href="url">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 166, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[foo]\n\n[foo]: url", "<p><a href=\"url\">foo</a></p>", "");
        }
    }
        // If there are several matching definitions, the first one takes
        // precedence:
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example167()
        {
            // Example 167
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     [foo]: first
            //     [foo]: second
            //
            // Should be rendered as:
            //     <p><a href="first">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 167, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[foo]\n\n[foo]: first\n[foo]: second", "<p><a href=\"first\">foo</a></p>", "");
        }
    }
        // As noted in the section on [Links], matching of labels is
        // case-insensitive (see [matches]).
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example168()
        {
            // Example 168
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [FOO]: /url
            //     
            //     [Foo]
            //
            // Should be rendered as:
            //     <p><a href="/url">Foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 168, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[FOO]: /url\n\n[Foo]", "<p><a href=\"/url\">Foo</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example169()
        {
            // Example 169
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [ΑΓΩ]: /φου
            //     
            //     [αγω]
            //
            // Should be rendered as:
            //     <p><a href="/%CF%86%CE%BF%CF%85">αγω</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 169, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[ΑΓΩ]: /φου\n\n[αγω]", "<p><a href=\"/%CF%86%CE%BF%CF%85\">αγω</a></p>", "");
        }
    }
        // Here is a link reference definition with no corresponding link.
        // It contributes nothing to the document.
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example170()
        {
            // Example 170
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url
            //
            // Should be rendered as:
            //     

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 170, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[foo]: /url", "", "");
        }
    }
        // Here is another one:
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example171()
        {
            // Example 171
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [
            //     foo
            //     ]: /url
            //     bar
            //
            // Should be rendered as:
            //     <p>bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 171, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[\nfoo\n]: /url\nbar", "<p>bar</p>", "");
        }
    }
        // This is not a link reference definition, because there are
        // [non-whitespace characters] after the title:
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example172()
        {
            // Example 172
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url "title" ok
            //
            // Should be rendered as:
            //     <p>[foo]: /url &quot;title&quot; ok</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 172, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[foo]: /url \"title\" ok", "<p>[foo]: /url &quot;title&quot; ok</p>", "");
        }
    }
        // This is a link reference definition, but it has no title:
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example173()
        {
            // Example 173
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /url
            //     "title" ok
            //
            // Should be rendered as:
            //     <p>&quot;title&quot; ok</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 173, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[foo]: /url\n\"title\" ok", "<p>&quot;title&quot; ok</p>", "");
        }
    }
        // This is not a link reference definition, because it is indented
        // four spaces:
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example174()
        {
            // Example 174
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //         [foo]: /url "title"
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <pre><code>[foo]: /url &quot;title&quot;
            //     </code></pre>
            //     <p>[foo]</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 174, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("    [foo]: /url \"title\"\n\n[foo]", "<pre><code>[foo]: /url &quot;title&quot;\n</code></pre>\n<p>[foo]</p>", "");
        }
    }
        // This is not a link reference definition, because it occurs inside
        // a code block:
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example175()
        {
            // Example 175
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     ```
            //     [foo]: /url
            //     ```
            //     
            //     [foo]
            //
            // Should be rendered as:
            //     <pre><code>[foo]: /url
            //     </code></pre>
            //     <p>[foo]</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 175, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("```\n[foo]: /url\n```\n\n[foo]", "<pre><code>[foo]: /url\n</code></pre>\n<p>[foo]</p>", "");
        }
    }
        // A [link reference definition] cannot interrupt a paragraph.
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example176()
        {
            // Example 176
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     Foo
            //     [bar]: /baz
            //     
            //     [bar]
            //
            // Should be rendered as:
            //     <p>Foo
            //     [bar]: /baz</p>
            //     <p>[bar]</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 176, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("Foo\n[bar]: /baz\n\n[bar]", "<p>Foo\n[bar]: /baz</p>\n<p>[bar]</p>", "");
        }
    }
        // However, it can directly follow other block elements, such as headings
        // and thematic breaks, and it need not be followed by a blank line.
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example177()
        {
            // Example 177
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     # [Foo]
            //     [foo]: /url
            //     > bar
            //
            // Should be rendered as:
            //     <h1><a href="/url">Foo</a></h1>
            //     <blockquote>
            //     <p>bar</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 177, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("# [Foo]\n[foo]: /url\n> bar", "<h1><a href=\"/url\">Foo</a></h1>\n<blockquote>\n<p>bar</p>\n</blockquote>", "");
        }
    }
        // Several [link reference definitions]
        // can occur one after another, without intervening blank lines.
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example178()
        {
            // Example 178
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [foo]: /foo-url "foo"
            //     [bar]: /bar-url
            //       "bar"
            //     [baz]: /baz-url
            //     
            //     [foo],
            //     [bar],
            //     [baz]
            //
            // Should be rendered as:
            //     <p><a href="/foo-url" title="foo">foo</a>,
            //     <a href="/bar-url" title="bar">bar</a>,
            //     <a href="/baz-url">baz</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 178, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[foo]: /foo-url \"foo\"\n[bar]: /bar-url\n  \"bar\"\n[baz]: /baz-url\n\n[foo],\n[bar],\n[baz]", "<p><a href=\"/foo-url\" title=\"foo\">foo</a>,\n<a href=\"/bar-url\" title=\"bar\">bar</a>,\n<a href=\"/baz-url\">baz</a></p>", "");
        }
    }
        // [Link reference definitions] can occur
        // inside block containers, like lists and block quotations.  They
        // affect the entire document, not just the container in which they
        // are defined:
    [TestFixture]
    public partial class TestLeafblocksLinkreferencedefinitions
    {
        [Test]
        public void Example179()
        {
            // Example 179
            // Section: Leaf blocks Link reference definitions
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     > [foo]: /url
            //
            // Should be rendered as:
            //     <p><a href="/url">foo</a></p>
            //     <blockquote>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 179, "Leaf blocks Link reference definitions");
			TestParser.TestSpec("[foo]\n\n> [foo]: /url", "<p><a href=\"/url\">foo</a></p>\n<blockquote>\n</blockquote>", "");
        }
    }
        // ## Paragraphs
        //
        // A sequence of non-blank lines that cannot be interpreted as other
        // kinds of blocks forms a [paragraph](@).
        // The contents of the paragraph are the result of parsing the
        // paragraph's raw content as inlines.  The paragraph's raw content
        // is formed by concatenating the lines and removing initial and final
        // [whitespace].
        //
        // A simple example with two paragraphs:
    [TestFixture]
    public partial class TestLeafblocksParagraphs
    {
        [Test]
        public void Example180()
        {
            // Example 180
            // Section: Leaf blocks Paragraphs
            //
            // The following CommonMark:
            //     aaa
            //     
            //     bbb
            //
            // Should be rendered as:
            //     <p>aaa</p>
            //     <p>bbb</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 180, "Leaf blocks Paragraphs");
			TestParser.TestSpec("aaa\n\nbbb", "<p>aaa</p>\n<p>bbb</p>", "");
        }
    }
        // Paragraphs can contain multiple lines, but no blank lines:
    [TestFixture]
    public partial class TestLeafblocksParagraphs
    {
        [Test]
        public void Example181()
        {
            // Example 181
            // Section: Leaf blocks Paragraphs
            //
            // The following CommonMark:
            //     aaa
            //     bbb
            //     
            //     ccc
            //     ddd
            //
            // Should be rendered as:
            //     <p>aaa
            //     bbb</p>
            //     <p>ccc
            //     ddd</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 181, "Leaf blocks Paragraphs");
			TestParser.TestSpec("aaa\nbbb\n\nccc\nddd", "<p>aaa\nbbb</p>\n<p>ccc\nddd</p>", "");
        }
    }
        // Multiple blank lines between paragraph have no effect:
    [TestFixture]
    public partial class TestLeafblocksParagraphs
    {
        [Test]
        public void Example182()
        {
            // Example 182
            // Section: Leaf blocks Paragraphs
            //
            // The following CommonMark:
            //     aaa
            //     
            //     
            //     bbb
            //
            // Should be rendered as:
            //     <p>aaa</p>
            //     <p>bbb</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 182, "Leaf blocks Paragraphs");
			TestParser.TestSpec("aaa\n\n\nbbb", "<p>aaa</p>\n<p>bbb</p>", "");
        }
    }
        // Leading spaces are skipped:
    [TestFixture]
    public partial class TestLeafblocksParagraphs
    {
        [Test]
        public void Example183()
        {
            // Example 183
            // Section: Leaf blocks Paragraphs
            //
            // The following CommonMark:
            //       aaa
            //      bbb
            //
            // Should be rendered as:
            //     <p>aaa
            //     bbb</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 183, "Leaf blocks Paragraphs");
			TestParser.TestSpec("  aaa\n bbb", "<p>aaa\nbbb</p>", "");
        }
    }
        // Lines after the first may be indented any amount, since indented
        // code blocks cannot interrupt paragraphs.
    [TestFixture]
    public partial class TestLeafblocksParagraphs
    {
        [Test]
        public void Example184()
        {
            // Example 184
            // Section: Leaf blocks Paragraphs
            //
            // The following CommonMark:
            //     aaa
            //                  bbb
            //                                            ccc
            //
            // Should be rendered as:
            //     <p>aaa
            //     bbb
            //     ccc</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 184, "Leaf blocks Paragraphs");
			TestParser.TestSpec("aaa\n             bbb\n                                       ccc", "<p>aaa\nbbb\nccc</p>", "");
        }
    }
        // However, the first line may be indented at most three spaces,
        // or an indented code block will be triggered:
    [TestFixture]
    public partial class TestLeafblocksParagraphs
    {
        [Test]
        public void Example185()
        {
            // Example 185
            // Section: Leaf blocks Paragraphs
            //
            // The following CommonMark:
            //        aaa
            //     bbb
            //
            // Should be rendered as:
            //     <p>aaa
            //     bbb</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 185, "Leaf blocks Paragraphs");
			TestParser.TestSpec("   aaa\nbbb", "<p>aaa\nbbb</p>", "");
        }
    }
    [TestFixture]
    public partial class TestLeafblocksParagraphs
    {
        [Test]
        public void Example186()
        {
            // Example 186
            // Section: Leaf blocks Paragraphs
            //
            // The following CommonMark:
            //         aaa
            //     bbb
            //
            // Should be rendered as:
            //     <pre><code>aaa
            //     </code></pre>
            //     <p>bbb</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 186, "Leaf blocks Paragraphs");
			TestParser.TestSpec("    aaa\nbbb", "<pre><code>aaa\n</code></pre>\n<p>bbb</p>", "");
        }
    }
        // Final spaces are stripped before inline parsing, so a paragraph
        // that ends with two or more spaces will not end with a [hard line
        // break]:
    [TestFixture]
    public partial class TestLeafblocksParagraphs
    {
        [Test]
        public void Example187()
        {
            // Example 187
            // Section: Leaf blocks Paragraphs
            //
            // The following CommonMark:
            //     aaa     
            //     bbb     
            //
            // Should be rendered as:
            //     <p>aaa<br />
            //     bbb</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 187, "Leaf blocks Paragraphs");
			TestParser.TestSpec("aaa     \nbbb     ", "<p>aaa<br />\nbbb</p>", "");
        }
    }
        // ## Blank lines
        //
        // [Blank lines] between block-level elements are ignored,
        // except for the role they play in determining whether a [list]
        // is [tight] or [loose].
        //
        // Blank lines at the beginning and end of the document are also ignored.
    [TestFixture]
    public partial class TestLeafblocksBlanklines
    {
        [Test]
        public void Example188()
        {
            // Example 188
            // Section: Leaf blocks Blank lines
            //
            // The following CommonMark:
            //       
            //     
            //     aaa
            //       
            //     
            //     # aaa
            //     
            //       
            //
            // Should be rendered as:
            //     <p>aaa</p>
            //     <h1>aaa</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 188, "Leaf blocks Blank lines");
			TestParser.TestSpec("  \n\naaa\n  \n\n# aaa\n\n  ", "<p>aaa</p>\n<h1>aaa</h1>", "");
        }
    }
        // # Container blocks
        //
        // A [container block] is a block that has other
        // blocks as its contents.  There are two basic kinds of container blocks:
        // [block quotes] and [list items].
        // [Lists] are meta-containers for [list items].
        //
        // We define the syntax for container blocks recursively.  The general
        // form of the definition is:
        //
        // > If X is a sequence of blocks, then the result of
        // > transforming X in such-and-such a way is a container of type Y
        // > with these blocks as its content.
        //
        // So, we explain what counts as a block quote or list item by explaining
        // how these can be *generated* from their contents. This should suffice
        // to define the syntax, although it does not give a recipe for *parsing*
        // these constructions.  (A recipe is provided below in the section entitled
        // [A parsing strategy](#appendix-a-parsing-strategy).)
        //
        // ## Block quotes
        //
        // A [block quote marker](@)
        // consists of 0-3 spaces of initial indent, plus (a) the character `>` together
        // with a following space, or (b) a single character `>` not followed by a space.
        //
        // The following rules define [block quotes]:
        //
        // 1.  **Basic case.**  If a string of lines *Ls* constitute a sequence
        // of blocks *Bs*, then the result of prepending a [block quote
        // marker] to the beginning of each line in *Ls*
        // is a [block quote](#block-quotes) containing *Bs*.
        //
        // 2.  **Laziness.**  If a string of lines *Ls* constitute a [block
        // quote](#block-quotes) with contents *Bs*, then the result of deleting
        // the initial [block quote marker] from one or
        // more lines in which the next [non-whitespace character] after the [block
        // quote marker] is [paragraph continuation
        // text] is a block quote with *Bs* as its content.
        // [Paragraph continuation text](@) is text
        // that will be parsed as part of the content of a paragraph, but does
        // not occur at the beginning of the paragraph.
        //
        // 3.  **Consecutiveness.**  A document cannot contain two [block
        // quotes] in a row unless there is a [blank line] between them.
        //
        // Nothing else counts as a [block quote](#block-quotes).
        //
        // Here is a simple example:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example189()
        {
            // Example 189
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > # Foo
            //     > bar
            //     > baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <h1>Foo</h1>
            //     <p>bar
            //     baz</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 189, "Container blocks Block quotes");
			TestParser.TestSpec("> # Foo\n> bar\n> baz", "<blockquote>\n<h1>Foo</h1>\n<p>bar\nbaz</p>\n</blockquote>", "");
        }
    }
        // The spaces after the `>` characters can be omitted:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example190()
        {
            // Example 190
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     ># Foo
            //     >bar
            //     > baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <h1>Foo</h1>
            //     <p>bar
            //     baz</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 190, "Container blocks Block quotes");
			TestParser.TestSpec("># Foo\n>bar\n> baz", "<blockquote>\n<h1>Foo</h1>\n<p>bar\nbaz</p>\n</blockquote>", "");
        }
    }
        // The `>` characters can be indented 1-3 spaces:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example191()
        {
            // Example 191
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //        > # Foo
            //        > bar
            //      > baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <h1>Foo</h1>
            //     <p>bar
            //     baz</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 191, "Container blocks Block quotes");
			TestParser.TestSpec("   > # Foo\n   > bar\n > baz", "<blockquote>\n<h1>Foo</h1>\n<p>bar\nbaz</p>\n</blockquote>", "");
        }
    }
        // Four spaces gives us a code block:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example192()
        {
            // Example 192
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //         > # Foo
            //         > bar
            //         > baz
            //
            // Should be rendered as:
            //     <pre><code>&gt; # Foo
            //     &gt; bar
            //     &gt; baz
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 192, "Container blocks Block quotes");
			TestParser.TestSpec("    > # Foo\n    > bar\n    > baz", "<pre><code>&gt; # Foo\n&gt; bar\n&gt; baz\n</code></pre>", "");
        }
    }
        // The Laziness clause allows us to omit the `>` before
        // [paragraph continuation text]:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example193()
        {
            // Example 193
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > # Foo
            //     > bar
            //     baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <h1>Foo</h1>
            //     <p>bar
            //     baz</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 193, "Container blocks Block quotes");
			TestParser.TestSpec("> # Foo\n> bar\nbaz", "<blockquote>\n<h1>Foo</h1>\n<p>bar\nbaz</p>\n</blockquote>", "");
        }
    }
        // A block quote can contain some lazy and some non-lazy
        // continuation lines:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example194()
        {
            // Example 194
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > bar
            //     baz
            //     > foo
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>bar
            //     baz
            //     foo</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 194, "Container blocks Block quotes");
			TestParser.TestSpec("> bar\nbaz\n> foo", "<blockquote>\n<p>bar\nbaz\nfoo</p>\n</blockquote>", "");
        }
    }
        // Laziness only applies to lines that would have been continuations of
        // paragraphs had they been prepended with [block quote markers].
        // For example, the `> ` cannot be omitted in the second line of
        //
        // ``` markdown
        // > foo
        // > ---
        // ```
        //
        // without changing the meaning:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example195()
        {
            // Example 195
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > foo
            //     ---
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>foo</p>
            //     </blockquote>
            //     <hr />

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 195, "Container blocks Block quotes");
			TestParser.TestSpec("> foo\n---", "<blockquote>\n<p>foo</p>\n</blockquote>\n<hr />", "");
        }
    }
        // Similarly, if we omit the `> ` in the second line of
        //
        // ``` markdown
        // > - foo
        // > - bar
        // ```
        //
        // then the block quote ends after the first line:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example196()
        {
            // Example 196
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > - foo
            //     - bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <ul>
            //     <li>foo</li>
            //     </ul>
            //     </blockquote>
            //     <ul>
            //     <li>bar</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 196, "Container blocks Block quotes");
			TestParser.TestSpec("> - foo\n- bar", "<blockquote>\n<ul>\n<li>foo</li>\n</ul>\n</blockquote>\n<ul>\n<li>bar</li>\n</ul>", "");
        }
    }
        // For the same reason, we can't omit the `> ` in front of
        // subsequent lines of an indented or fenced code block:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example197()
        {
            // Example 197
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     >     foo
            //         bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <pre><code>foo
            //     </code></pre>
            //     </blockquote>
            //     <pre><code>bar
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 197, "Container blocks Block quotes");
			TestParser.TestSpec(">     foo\n    bar", "<blockquote>\n<pre><code>foo\n</code></pre>\n</blockquote>\n<pre><code>bar\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example198()
        {
            // Example 198
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > ```
            //     foo
            //     ```
            //
            // Should be rendered as:
            //     <blockquote>
            //     <pre><code></code></pre>
            //     </blockquote>
            //     <p>foo</p>
            //     <pre><code></code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 198, "Container blocks Block quotes");
			TestParser.TestSpec("> ```\nfoo\n```", "<blockquote>\n<pre><code></code></pre>\n</blockquote>\n<p>foo</p>\n<pre><code></code></pre>", "");
        }
    }
        // Note that in the following case, we have a [lazy
        // continuation line]:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example199()
        {
            // Example 199
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > foo
            //         - bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>foo
            //     - bar</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 199, "Container blocks Block quotes");
			TestParser.TestSpec("> foo\n    - bar", "<blockquote>\n<p>foo\n- bar</p>\n</blockquote>", "");
        }
    }
        // To see why, note that in
        //
        // ```markdown
        // > foo
        // >     - bar
        // ```
        //
        // the `- bar` is indented too far to start a list, and can't
        // be an indented code block because indented code blocks cannot
        // interrupt paragraphs, so it is [paragraph continuation text].
        //
        // A block quote can be empty:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example200()
        {
            // Example 200
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     >
            //
            // Should be rendered as:
            //     <blockquote>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 200, "Container blocks Block quotes");
			TestParser.TestSpec(">", "<blockquote>\n</blockquote>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example201()
        {
            // Example 201
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     >
            //     >  
            //     > 
            //
            // Should be rendered as:
            //     <blockquote>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 201, "Container blocks Block quotes");
			TestParser.TestSpec(">\n>  \n> ", "<blockquote>\n</blockquote>", "");
        }
    }
        // A block quote can have initial or final blank lines:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example202()
        {
            // Example 202
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     >
            //     > foo
            //     >  
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>foo</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 202, "Container blocks Block quotes");
			TestParser.TestSpec(">\n> foo\n>  ", "<blockquote>\n<p>foo</p>\n</blockquote>", "");
        }
    }
        // A blank line always separates block quotes:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example203()
        {
            // Example 203
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > foo
            //     
            //     > bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>foo</p>
            //     </blockquote>
            //     <blockquote>
            //     <p>bar</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 203, "Container blocks Block quotes");
			TestParser.TestSpec("> foo\n\n> bar", "<blockquote>\n<p>foo</p>\n</blockquote>\n<blockquote>\n<p>bar</p>\n</blockquote>", "");
        }
    }
        // (Most current Markdown implementations, including John Gruber's
        // original `Markdown.pl`, will parse this example as a single block quote
        // with two paragraphs.  But it seems better to allow the author to decide
        // whether two block quotes or one are wanted.)
        //
        // Consecutiveness means that if we put these block quotes together,
        // we get a single block quote:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example204()
        {
            // Example 204
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > foo
            //     > bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>foo
            //     bar</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 204, "Container blocks Block quotes");
			TestParser.TestSpec("> foo\n> bar", "<blockquote>\n<p>foo\nbar</p>\n</blockquote>", "");
        }
    }
        // To get a block quote with two paragraphs, use:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example205()
        {
            // Example 205
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > foo
            //     >
            //     > bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>foo</p>
            //     <p>bar</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 205, "Container blocks Block quotes");
			TestParser.TestSpec("> foo\n>\n> bar", "<blockquote>\n<p>foo</p>\n<p>bar</p>\n</blockquote>", "");
        }
    }
        // Block quotes can interrupt paragraphs:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example206()
        {
            // Example 206
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     foo
            //     > bar
            //
            // Should be rendered as:
            //     <p>foo</p>
            //     <blockquote>
            //     <p>bar</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 206, "Container blocks Block quotes");
			TestParser.TestSpec("foo\n> bar", "<p>foo</p>\n<blockquote>\n<p>bar</p>\n</blockquote>", "");
        }
    }
        // In general, blank lines are not needed before or after block
        // quotes:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example207()
        {
            // Example 207
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > aaa
            //     ***
            //     > bbb
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>aaa</p>
            //     </blockquote>
            //     <hr />
            //     <blockquote>
            //     <p>bbb</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 207, "Container blocks Block quotes");
			TestParser.TestSpec("> aaa\n***\n> bbb", "<blockquote>\n<p>aaa</p>\n</blockquote>\n<hr />\n<blockquote>\n<p>bbb</p>\n</blockquote>", "");
        }
    }
        // However, because of laziness, a blank line is needed between
        // a block quote and a following paragraph:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example208()
        {
            // Example 208
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > bar
            //     baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>bar
            //     baz</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 208, "Container blocks Block quotes");
			TestParser.TestSpec("> bar\nbaz", "<blockquote>\n<p>bar\nbaz</p>\n</blockquote>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example209()
        {
            // Example 209
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > bar
            //     
            //     baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>bar</p>
            //     </blockquote>
            //     <p>baz</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 209, "Container blocks Block quotes");
			TestParser.TestSpec("> bar\n\nbaz", "<blockquote>\n<p>bar</p>\n</blockquote>\n<p>baz</p>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example210()
        {
            // Example 210
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > bar
            //     >
            //     baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <p>bar</p>
            //     </blockquote>
            //     <p>baz</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 210, "Container blocks Block quotes");
			TestParser.TestSpec("> bar\n>\nbaz", "<blockquote>\n<p>bar</p>\n</blockquote>\n<p>baz</p>", "");
        }
    }
        // It is a consequence of the Laziness rule that any number
        // of initial `>`s may be omitted on a continuation line of a
        // nested block quote:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example211()
        {
            // Example 211
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     > > > foo
            //     bar
            //
            // Should be rendered as:
            //     <blockquote>
            //     <blockquote>
            //     <blockquote>
            //     <p>foo
            //     bar</p>
            //     </blockquote>
            //     </blockquote>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 211, "Container blocks Block quotes");
			TestParser.TestSpec("> > > foo\nbar", "<blockquote>\n<blockquote>\n<blockquote>\n<p>foo\nbar</p>\n</blockquote>\n</blockquote>\n</blockquote>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example212()
        {
            // Example 212
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     >>> foo
            //     > bar
            //     >>baz
            //
            // Should be rendered as:
            //     <blockquote>
            //     <blockquote>
            //     <blockquote>
            //     <p>foo
            //     bar
            //     baz</p>
            //     </blockquote>
            //     </blockquote>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 212, "Container blocks Block quotes");
			TestParser.TestSpec(">>> foo\n> bar\n>>baz", "<blockquote>\n<blockquote>\n<blockquote>\n<p>foo\nbar\nbaz</p>\n</blockquote>\n</blockquote>\n</blockquote>", "");
        }
    }
        // When including an indented code block in a block quote,
        // remember that the [block quote marker] includes
        // both the `>` and a following space.  So *five spaces* are needed after
        // the `>`:
    [TestFixture]
    public partial class TestContainerblocksBlockquotes
    {
        [Test]
        public void Example213()
        {
            // Example 213
            // Section: Container blocks Block quotes
            //
            // The following CommonMark:
            //     >     code
            //     
            //     >    not code
            //
            // Should be rendered as:
            //     <blockquote>
            //     <pre><code>code
            //     </code></pre>
            //     </blockquote>
            //     <blockquote>
            //     <p>not code</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 213, "Container blocks Block quotes");
			TestParser.TestSpec(">     code\n\n>    not code", "<blockquote>\n<pre><code>code\n</code></pre>\n</blockquote>\n<blockquote>\n<p>not code</p>\n</blockquote>", "");
        }
    }
        // ## List items
        //
        // A [list marker](@) is a
        // [bullet list marker] or an [ordered list marker].
        //
        // A [bullet list marker](@)
        // is a `-`, `+`, or `*` character.
        //
        // An [ordered list marker](@)
        // is a sequence of 1--9 arabic digits (`0-9`), followed by either a
        // `.` character or a `)` character.  (The reason for the length
        // limit is that with 10 digits we start seeing integer overflows
        // in some browsers.)
        //
        // The following rules define [list items]:
        //
        // 1.  **Basic case.**  If a sequence of lines *Ls* constitute a sequence of
        // blocks *Bs* starting with a [non-whitespace character] and not separated
        // from each other by more than one blank line, and *M* is a list
        // marker of width *W* followed by 0 < *N* < 5 spaces, then the result
        // of prepending *M* and the following spaces to the first line of
        // *Ls*, and indenting subsequent lines of *Ls* by *W + N* spaces, is a
        // list item with *Bs* as its contents.  The type of the list item
        // (bullet or ordered) is determined by the type of its list marker.
        // If the list item is ordered, then it is also assigned a start
        // number, based on the ordered list marker.
        //
        // For example, let *Ls* be the lines
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example214()
        {
            // Example 214
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     A paragraph
            //     with two lines.
            //     
            //         indented code
            //     
            //     > A block quote.
            //
            // Should be rendered as:
            //     <p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 214, "Container blocks List items");
			TestParser.TestSpec("A paragraph\nwith two lines.\n\n    indented code\n\n> A block quote.", "<p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote>", "");
        }
    }
        // And let *M* be the marker `1.`, and *N* = 2.  Then rule #1 says
        // that the following is an ordered list item with start number 1,
        // and the same contents as *Ls*:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example215()
        {
            // Example 215
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     1.  A paragraph
            //         with two lines.
            //     
            //             indented code
            //     
            //         > A block quote.
            //
            // Should be rendered as:
            //     <ol>
            //     <li>
            //     <p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote>
            //     </li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 215, "Container blocks List items");
			TestParser.TestSpec("1.  A paragraph\n    with two lines.\n\n        indented code\n\n    > A block quote.", "<ol>\n<li>\n<p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote>\n</li>\n</ol>", "");
        }
    }
        // The most important thing to notice is that the position of
        // the text after the list marker determines how much indentation
        // is needed in subsequent blocks in the list item.  If the list
        // marker takes up two spaces, and there are three spaces between
        // the list marker and the next [non-whitespace character], then blocks
        // must be indented five spaces in order to fall under the list
        // item.
        //
        // Here are some examples showing how far content must be indented to be
        // put under the list item:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example216()
        {
            // Example 216
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     - one
            //     
            //      two
            //
            // Should be rendered as:
            //     <ul>
            //     <li>one</li>
            //     </ul>
            //     <p>two</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 216, "Container blocks List items");
			TestParser.TestSpec("- one\n\n two", "<ul>\n<li>one</li>\n</ul>\n<p>two</p>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example217()
        {
            // Example 217
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     - one
            //     
            //       two
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>one</p>
            //     <p>two</p>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 217, "Container blocks List items");
			TestParser.TestSpec("- one\n\n  two", "<ul>\n<li>\n<p>one</p>\n<p>two</p>\n</li>\n</ul>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example218()
        {
            // Example 218
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //      -    one
            //     
            //          two
            //
            // Should be rendered as:
            //     <ul>
            //     <li>one</li>
            //     </ul>
            //     <pre><code> two
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 218, "Container blocks List items");
			TestParser.TestSpec(" -    one\n\n     two", "<ul>\n<li>one</li>\n</ul>\n<pre><code> two\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example219()
        {
            // Example 219
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //      -    one
            //     
            //           two
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>one</p>
            //     <p>two</p>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 219, "Container blocks List items");
			TestParser.TestSpec(" -    one\n\n      two", "<ul>\n<li>\n<p>one</p>\n<p>two</p>\n</li>\n</ul>", "");
        }
    }
        // It is tempting to think of this in terms of columns:  the continuation
        // blocks must be indented at least to the column of the first
        // [non-whitespace character] after the list marker. However, that is not quite right.
        // The spaces after the list marker determine how much relative indentation
        // is needed.  Which column this indentation reaches will depend on
        // how the list item is embedded in other constructions, as shown by
        // this example:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example220()
        {
            // Example 220
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //        > > 1.  one
            //     >>
            //     >>     two
            //
            // Should be rendered as:
            //     <blockquote>
            //     <blockquote>
            //     <ol>
            //     <li>
            //     <p>one</p>
            //     <p>two</p>
            //     </li>
            //     </ol>
            //     </blockquote>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 220, "Container blocks List items");
			TestParser.TestSpec("   > > 1.  one\n>>\n>>     two", "<blockquote>\n<blockquote>\n<ol>\n<li>\n<p>one</p>\n<p>two</p>\n</li>\n</ol>\n</blockquote>\n</blockquote>", "");
        }
    }
        // Here `two` occurs in the same column as the list marker `1.`,
        // but is actually contained in the list item, because there is
        // sufficient indentation after the last containing blockquote marker.
        //
        // The converse is also possible.  In the following example, the word `two`
        // occurs far to the right of the initial text of the list item, `one`, but
        // it is not considered part of the list item, because it is not indented
        // far enough past the blockquote marker:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example221()
        {
            // Example 221
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     >>- one
            //     >>
            //       >  > two
            //
            // Should be rendered as:
            //     <blockquote>
            //     <blockquote>
            //     <ul>
            //     <li>one</li>
            //     </ul>
            //     <p>two</p>
            //     </blockquote>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 221, "Container blocks List items");
			TestParser.TestSpec(">>- one\n>>\n  >  > two", "<blockquote>\n<blockquote>\n<ul>\n<li>one</li>\n</ul>\n<p>two</p>\n</blockquote>\n</blockquote>", "");
        }
    }
        // Note that at least one space is needed between the list marker and
        // any following content, so these are not list items:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example222()
        {
            // Example 222
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     -one
            //     
            //     2.two
            //
            // Should be rendered as:
            //     <p>-one</p>
            //     <p>2.two</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 222, "Container blocks List items");
			TestParser.TestSpec("-one\n\n2.two", "<p>-one</p>\n<p>2.two</p>", "");
        }
    }
        // A list item may not contain blocks that are separated by more than
        // one blank line.  Thus, two blank lines will end a list, unless the
        // two blanks are contained in a [fenced code block].
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example223()
        {
            // Example 223
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     - foo
            //     
            //       bar
            //     
            //     - foo
            //     
            //     
            //       bar
            //     
            //     - ```
            //       foo
            //     
            //     
            //       bar
            //       ```
            //     
            //     - baz
            //     
            //       + ```
            //         foo
            //     
            //     
            //         bar
            //         ```
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>foo</p>
            //     <p>bar</p>
            //     </li>
            //     <li>
            //     <p>foo</p>
            //     </li>
            //     </ul>
            //     <p>bar</p>
            //     <ul>
            //     <li>
            //     <pre><code>foo
            //     
            //     
            //     bar
            //     </code></pre>
            //     </li>
            //     <li>
            //     <p>baz</p>
            //     <ul>
            //     <li>
            //     <pre><code>foo
            //     
            //     
            //     bar
            //     </code></pre>
            //     </li>
            //     </ul>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 223, "Container blocks List items");
			TestParser.TestSpec("- foo\n\n  bar\n\n- foo\n\n\n  bar\n\n- ```\n  foo\n\n\n  bar\n  ```\n\n- baz\n\n  + ```\n    foo\n\n\n    bar\n    ```", "<ul>\n<li>\n<p>foo</p>\n<p>bar</p>\n</li>\n<li>\n<p>foo</p>\n</li>\n</ul>\n<p>bar</p>\n<ul>\n<li>\n<pre><code>foo\n\n\nbar\n</code></pre>\n</li>\n<li>\n<p>baz</p>\n<ul>\n<li>\n<pre><code>foo\n\n\nbar\n</code></pre>\n</li>\n</ul>\n</li>\n</ul>", "");
        }
    }
        // A list item may contain any kind of block:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example224()
        {
            // Example 224
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     1.  foo
            //     
            //         ```
            //         bar
            //         ```
            //     
            //         baz
            //     
            //         > bam
            //
            // Should be rendered as:
            //     <ol>
            //     <li>
            //     <p>foo</p>
            //     <pre><code>bar
            //     </code></pre>
            //     <p>baz</p>
            //     <blockquote>
            //     <p>bam</p>
            //     </blockquote>
            //     </li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 224, "Container blocks List items");
			TestParser.TestSpec("1.  foo\n\n    ```\n    bar\n    ```\n\n    baz\n\n    > bam", "<ol>\n<li>\n<p>foo</p>\n<pre><code>bar\n</code></pre>\n<p>baz</p>\n<blockquote>\n<p>bam</p>\n</blockquote>\n</li>\n</ol>", "");
        }
    }
        // A list item that contains an indented code block will preserve
        // empty lines within the code block verbatim, unless there are two
        // or more empty lines in a row (since as described above, two
        // blank lines end the list):
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example225()
        {
            // Example 225
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     - Foo
            //     
            //           bar
            //     
            //           baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>Foo</p>
            //     <pre><code>bar
            //     
            //     baz
            //     </code></pre>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 225, "Container blocks List items");
			TestParser.TestSpec("- Foo\n\n      bar\n\n      baz", "<ul>\n<li>\n<p>Foo</p>\n<pre><code>bar\n\nbaz\n</code></pre>\n</li>\n</ul>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example226()
        {
            // Example 226
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     - Foo
            //     
            //           bar
            //     
            //     
            //           baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>Foo</p>
            //     <pre><code>bar
            //     </code></pre>
            //     </li>
            //     </ul>
            //     <pre><code>  baz
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 226, "Container blocks List items");
			TestParser.TestSpec("- Foo\n\n      bar\n\n\n      baz", "<ul>\n<li>\n<p>Foo</p>\n<pre><code>bar\n</code></pre>\n</li>\n</ul>\n<pre><code>  baz\n</code></pre>", "");
        }
    }
        // Note that ordered list start numbers must be nine digits or less:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example227()
        {
            // Example 227
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     123456789. ok
            //
            // Should be rendered as:
            //     <ol start="123456789">
            //     <li>ok</li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 227, "Container blocks List items");
			TestParser.TestSpec("123456789. ok", "<ol start=\"123456789\">\n<li>ok</li>\n</ol>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example228()
        {
            // Example 228
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     1234567890. not ok
            //
            // Should be rendered as:
            //     <p>1234567890. not ok</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 228, "Container blocks List items");
			TestParser.TestSpec("1234567890. not ok", "<p>1234567890. not ok</p>", "");
        }
    }
        // A start number may begin with 0s:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example229()
        {
            // Example 229
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     0. ok
            //
            // Should be rendered as:
            //     <ol start="0">
            //     <li>ok</li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 229, "Container blocks List items");
			TestParser.TestSpec("0. ok", "<ol start=\"0\">\n<li>ok</li>\n</ol>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example230()
        {
            // Example 230
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     003. ok
            //
            // Should be rendered as:
            //     <ol start="3">
            //     <li>ok</li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 230, "Container blocks List items");
			TestParser.TestSpec("003. ok", "<ol start=\"3\">\n<li>ok</li>\n</ol>", "");
        }
    }
        // A start number may not be negative:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example231()
        {
            // Example 231
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     -1. not ok
            //
            // Should be rendered as:
            //     <p>-1. not ok</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 231, "Container blocks List items");
			TestParser.TestSpec("-1. not ok", "<p>-1. not ok</p>", "");
        }
    }
        // 2.  **Item starting with indented code.**  If a sequence of lines *Ls*
        // constitute a sequence of blocks *Bs* starting with an indented code
        // block and not separated from each other by more than one blank line,
        // and *M* is a list marker of width *W* followed by
        // one space, then the result of prepending *M* and the following
        // space to the first line of *Ls*, and indenting subsequent lines of
        // *Ls* by *W + 1* spaces, is a list item with *Bs* as its contents.
        // If a line is empty, then it need not be indented.  The type of the
        // list item (bullet or ordered) is determined by the type of its list
        // marker.  If the list item is ordered, then it is also assigned a
        // start number, based on the ordered list marker.
        //
        // An indented code block will have to be indented four spaces beyond
        // the edge of the region where text will be included in the list item.
        // In the following case that is 6 spaces:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example232()
        {
            // Example 232
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     - foo
            //     
            //           bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>foo</p>
            //     <pre><code>bar
            //     </code></pre>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 232, "Container blocks List items");
			TestParser.TestSpec("- foo\n\n      bar", "<ul>\n<li>\n<p>foo</p>\n<pre><code>bar\n</code></pre>\n</li>\n</ul>", "");
        }
    }
        // And in this case it is 11 spaces:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example233()
        {
            // Example 233
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //       10.  foo
            //     
            //                bar
            //
            // Should be rendered as:
            //     <ol start="10">
            //     <li>
            //     <p>foo</p>
            //     <pre><code>bar
            //     </code></pre>
            //     </li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 233, "Container blocks List items");
			TestParser.TestSpec("  10.  foo\n\n           bar", "<ol start=\"10\">\n<li>\n<p>foo</p>\n<pre><code>bar\n</code></pre>\n</li>\n</ol>", "");
        }
    }
        // If the *first* block in the list item is an indented code block,
        // then by rule #2, the contents must be indented *one* space after the
        // list marker:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example234()
        {
            // Example 234
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //         indented code
            //     
            //     paragraph
            //     
            //         more code
            //
            // Should be rendered as:
            //     <pre><code>indented code
            //     </code></pre>
            //     <p>paragraph</p>
            //     <pre><code>more code
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 234, "Container blocks List items");
			TestParser.TestSpec("    indented code\n\nparagraph\n\n    more code", "<pre><code>indented code\n</code></pre>\n<p>paragraph</p>\n<pre><code>more code\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example235()
        {
            // Example 235
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     1.     indented code
            //     
            //        paragraph
            //     
            //            more code
            //
            // Should be rendered as:
            //     <ol>
            //     <li>
            //     <pre><code>indented code
            //     </code></pre>
            //     <p>paragraph</p>
            //     <pre><code>more code
            //     </code></pre>
            //     </li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 235, "Container blocks List items");
			TestParser.TestSpec("1.     indented code\n\n   paragraph\n\n       more code", "<ol>\n<li>\n<pre><code>indented code\n</code></pre>\n<p>paragraph</p>\n<pre><code>more code\n</code></pre>\n</li>\n</ol>", "");
        }
    }
        // Note that an additional space indent is interpreted as space
        // inside the code block:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example236()
        {
            // Example 236
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     1.      indented code
            //     
            //        paragraph
            //     
            //            more code
            //
            // Should be rendered as:
            //     <ol>
            //     <li>
            //     <pre><code> indented code
            //     </code></pre>
            //     <p>paragraph</p>
            //     <pre><code>more code
            //     </code></pre>
            //     </li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 236, "Container blocks List items");
			TestParser.TestSpec("1.      indented code\n\n   paragraph\n\n       more code", "<ol>\n<li>\n<pre><code> indented code\n</code></pre>\n<p>paragraph</p>\n<pre><code>more code\n</code></pre>\n</li>\n</ol>", "");
        }
    }
        // Note that rules #1 and #2 only apply to two cases:  (a) cases
        // in which the lines to be included in a list item begin with a
        // [non-whitespace character], and (b) cases in which
        // they begin with an indented code
        // block.  In a case like the following, where the first block begins with
        // a three-space indent, the rules do not allow us to form a list item by
        // indenting the whole thing and prepending a list marker:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example237()
        {
            // Example 237
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //        foo
            //     
            //     bar
            //
            // Should be rendered as:
            //     <p>foo</p>
            //     <p>bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 237, "Container blocks List items");
			TestParser.TestSpec("   foo\n\nbar", "<p>foo</p>\n<p>bar</p>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example238()
        {
            // Example 238
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     -    foo
            //     
            //       bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     </ul>
            //     <p>bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 238, "Container blocks List items");
			TestParser.TestSpec("-    foo\n\n  bar", "<ul>\n<li>foo</li>\n</ul>\n<p>bar</p>", "");
        }
    }
        // This is not a significant restriction, because when a block begins
        // with 1-3 spaces indent, the indentation can always be removed without
        // a change in interpretation, allowing rule #1 to be applied.  So, in
        // the above case:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example239()
        {
            // Example 239
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     -  foo
            //     
            //        bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>foo</p>
            //     <p>bar</p>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 239, "Container blocks List items");
			TestParser.TestSpec("-  foo\n\n   bar", "<ul>\n<li>\n<p>foo</p>\n<p>bar</p>\n</li>\n</ul>", "");
        }
    }
        // 3.  **Item starting with a blank line.**  If a sequence of lines *Ls*
        // starting with a single [blank line] constitute a (possibly empty)
        // sequence of blocks *Bs*, not separated from each other by more than
        // one blank line, and *M* is a list marker of width *W*,
        // then the result of prepending *M* to the first line of *Ls*, and
        // indenting subsequent lines of *Ls* by *W + 1* spaces, is a list
        // item with *Bs* as its contents.
        // If a line is empty, then it need not be indented.  The type of the
        // list item (bullet or ordered) is determined by the type of its list
        // marker.  If the list item is ordered, then it is also assigned a
        // start number, based on the ordered list marker.
        //
        // Here are some list items that start with a blank line but are not empty:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example240()
        {
            // Example 240
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     -
            //       foo
            //     -
            //       ```
            //       bar
            //       ```
            //     -
            //           baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     <li>
            //     <pre><code>bar
            //     </code></pre>
            //     </li>
            //     <li>
            //     <pre><code>baz
            //     </code></pre>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 240, "Container blocks List items");
			TestParser.TestSpec("-\n  foo\n-\n  ```\n  bar\n  ```\n-\n      baz", "<ul>\n<li>foo</li>\n<li>\n<pre><code>bar\n</code></pre>\n</li>\n<li>\n<pre><code>baz\n</code></pre>\n</li>\n</ul>", "");
        }
    }
        // When the list item starts with a blank line, the number of spaces
        // following the list marker doesn't change the required indentation:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example241()
        {
            // Example 241
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     -   
            //       foo
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 241, "Container blocks List items");
			TestParser.TestSpec("-   \n  foo", "<ul>\n<li>foo</li>\n</ul>", "");
        }
    }
        // A list item can begin with at most one blank line.
        // In the following example, `foo` is not part of the list
        // item:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example242()
        {
            // Example 242
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     -
            //     
            //       foo
            //
            // Should be rendered as:
            //     <ul>
            //     <li></li>
            //     </ul>
            //     <p>foo</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 242, "Container blocks List items");
			TestParser.TestSpec("-\n\n  foo", "<ul>\n<li></li>\n</ul>\n<p>foo</p>", "");
        }
    }
        // Here is an empty bullet list item:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example243()
        {
            // Example 243
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     - foo
            //     -
            //     - bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     <li></li>
            //     <li>bar</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 243, "Container blocks List items");
			TestParser.TestSpec("- foo\n-\n- bar", "<ul>\n<li>foo</li>\n<li></li>\n<li>bar</li>\n</ul>", "");
        }
    }
        // It does not matter whether there are spaces following the [list marker]:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example244()
        {
            // Example 244
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     - foo
            //     -   
            //     - bar
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     <li></li>
            //     <li>bar</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 244, "Container blocks List items");
			TestParser.TestSpec("- foo\n-   \n- bar", "<ul>\n<li>foo</li>\n<li></li>\n<li>bar</li>\n</ul>", "");
        }
    }
        // Here is an empty ordered list item:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example245()
        {
            // Example 245
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     1. foo
            //     2.
            //     3. bar
            //
            // Should be rendered as:
            //     <ol>
            //     <li>foo</li>
            //     <li></li>
            //     <li>bar</li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 245, "Container blocks List items");
			TestParser.TestSpec("1. foo\n2.\n3. bar", "<ol>\n<li>foo</li>\n<li></li>\n<li>bar</li>\n</ol>", "");
        }
    }
        // A list may start or end with an empty list item:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example246()
        {
            // Example 246
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     *
            //
            // Should be rendered as:
            //     <ul>
            //     <li></li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 246, "Container blocks List items");
			TestParser.TestSpec("*", "<ul>\n<li></li>\n</ul>", "");
        }
    }
        // 4.  **Indentation.**  If a sequence of lines *Ls* constitutes a list item
        // according to rule #1, #2, or #3, then the result of indenting each line
        // of *Ls* by 1-3 spaces (the same for each line) also constitutes a
        // list item with the same contents and attributes.  If a line is
        // empty, then it need not be indented.
        //
        // Indented one space:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example247()
        {
            // Example 247
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //      1.  A paragraph
            //          with two lines.
            //     
            //              indented code
            //     
            //          > A block quote.
            //
            // Should be rendered as:
            //     <ol>
            //     <li>
            //     <p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote>
            //     </li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 247, "Container blocks List items");
			TestParser.TestSpec(" 1.  A paragraph\n     with two lines.\n\n         indented code\n\n     > A block quote.", "<ol>\n<li>\n<p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote>\n</li>\n</ol>", "");
        }
    }
        // Indented two spaces:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example248()
        {
            // Example 248
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //       1.  A paragraph
            //           with two lines.
            //     
            //               indented code
            //     
            //           > A block quote.
            //
            // Should be rendered as:
            //     <ol>
            //     <li>
            //     <p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote>
            //     </li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 248, "Container blocks List items");
			TestParser.TestSpec("  1.  A paragraph\n      with two lines.\n\n          indented code\n\n      > A block quote.", "<ol>\n<li>\n<p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote>\n</li>\n</ol>", "");
        }
    }
        // Indented three spaces:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example249()
        {
            // Example 249
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //        1.  A paragraph
            //            with two lines.
            //     
            //                indented code
            //     
            //            > A block quote.
            //
            // Should be rendered as:
            //     <ol>
            //     <li>
            //     <p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote>
            //     </li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 249, "Container blocks List items");
			TestParser.TestSpec("   1.  A paragraph\n       with two lines.\n\n           indented code\n\n       > A block quote.", "<ol>\n<li>\n<p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote>\n</li>\n</ol>", "");
        }
    }
        // Four spaces indent gives a code block:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example250()
        {
            // Example 250
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //         1.  A paragraph
            //             with two lines.
            //     
            //                 indented code
            //     
            //             > A block quote.
            //
            // Should be rendered as:
            //     <pre><code>1.  A paragraph
            //         with two lines.
            //     
            //             indented code
            //     
            //         &gt; A block quote.
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 250, "Container blocks List items");
			TestParser.TestSpec("    1.  A paragraph\n        with two lines.\n\n            indented code\n\n        > A block quote.", "<pre><code>1.  A paragraph\n    with two lines.\n\n        indented code\n\n    &gt; A block quote.\n</code></pre>", "");
        }
    }
        // 5.  **Laziness.**  If a string of lines *Ls* constitute a [list
        // item](#list-items) with contents *Bs*, then the result of deleting
        // some or all of the indentation from one or more lines in which the
        // next [non-whitespace character] after the indentation is
        // [paragraph continuation text] is a
        // list item with the same contents and attributes.  The unindented
        // lines are called
        // [lazy continuation line](@)s.
        //
        // Here is an example with [lazy continuation lines]:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example251()
        {
            // Example 251
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //       1.  A paragraph
            //     with two lines.
            //     
            //               indented code
            //     
            //           > A block quote.
            //
            // Should be rendered as:
            //     <ol>
            //     <li>
            //     <p>A paragraph
            //     with two lines.</p>
            //     <pre><code>indented code
            //     </code></pre>
            //     <blockquote>
            //     <p>A block quote.</p>
            //     </blockquote>
            //     </li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 251, "Container blocks List items");
			TestParser.TestSpec("  1.  A paragraph\nwith two lines.\n\n          indented code\n\n      > A block quote.", "<ol>\n<li>\n<p>A paragraph\nwith two lines.</p>\n<pre><code>indented code\n</code></pre>\n<blockquote>\n<p>A block quote.</p>\n</blockquote>\n</li>\n</ol>", "");
        }
    }
        // Indentation can be partially deleted:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example252()
        {
            // Example 252
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //       1.  A paragraph
            //         with two lines.
            //
            // Should be rendered as:
            //     <ol>
            //     <li>A paragraph
            //     with two lines.</li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 252, "Container blocks List items");
			TestParser.TestSpec("  1.  A paragraph\n    with two lines.", "<ol>\n<li>A paragraph\nwith two lines.</li>\n</ol>", "");
        }
    }
        // These examples show how laziness can work in nested structures:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example253()
        {
            // Example 253
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     > 1. > Blockquote
            //     continued here.
            //
            // Should be rendered as:
            //     <blockquote>
            //     <ol>
            //     <li>
            //     <blockquote>
            //     <p>Blockquote
            //     continued here.</p>
            //     </blockquote>
            //     </li>
            //     </ol>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 253, "Container blocks List items");
			TestParser.TestSpec("> 1. > Blockquote\ncontinued here.", "<blockquote>\n<ol>\n<li>\n<blockquote>\n<p>Blockquote\ncontinued here.</p>\n</blockquote>\n</li>\n</ol>\n</blockquote>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example254()
        {
            // Example 254
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     > 1. > Blockquote
            //     > continued here.
            //
            // Should be rendered as:
            //     <blockquote>
            //     <ol>
            //     <li>
            //     <blockquote>
            //     <p>Blockquote
            //     continued here.</p>
            //     </blockquote>
            //     </li>
            //     </ol>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 254, "Container blocks List items");
			TestParser.TestSpec("> 1. > Blockquote\n> continued here.", "<blockquote>\n<ol>\n<li>\n<blockquote>\n<p>Blockquote\ncontinued here.</p>\n</blockquote>\n</li>\n</ol>\n</blockquote>", "");
        }
    }
        // 6.  **That's all.** Nothing that is not counted as a list item by rules
        // #1--5 counts as a [list item](#list-items).
        //
        // The rules for sublists follow from the general rules above.  A sublist
        // must be indented the same number of spaces a paragraph would need to be
        // in order to be included in the list item.
        //
        // So, in this case we need two spaces indent:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example255()
        {
            // Example 255
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     - foo
            //       - bar
            //         - baz
            //           - boo
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo
            //     <ul>
            //     <li>bar
            //     <ul>
            //     <li>baz
            //     <ul>
            //     <li>boo</li>
            //     </ul>
            //     </li>
            //     </ul>
            //     </li>
            //     </ul>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 255, "Container blocks List items");
			TestParser.TestSpec("- foo\n  - bar\n    - baz\n      - boo", "<ul>\n<li>foo\n<ul>\n<li>bar\n<ul>\n<li>baz\n<ul>\n<li>boo</li>\n</ul>\n</li>\n</ul>\n</li>\n</ul>\n</li>\n</ul>", "");
        }
    }
        // One is not enough:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example256()
        {
            // Example 256
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     - foo
            //      - bar
            //       - baz
            //        - boo
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     <li>bar</li>
            //     <li>baz</li>
            //     <li>boo</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 256, "Container blocks List items");
			TestParser.TestSpec("- foo\n - bar\n  - baz\n   - boo", "<ul>\n<li>foo</li>\n<li>bar</li>\n<li>baz</li>\n<li>boo</li>\n</ul>", "");
        }
    }
        // Here we need four, because the list marker is wider:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example257()
        {
            // Example 257
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     10) foo
            //         - bar
            //
            // Should be rendered as:
            //     <ol start="10">
            //     <li>foo
            //     <ul>
            //     <li>bar</li>
            //     </ul>
            //     </li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 257, "Container blocks List items");
			TestParser.TestSpec("10) foo\n    - bar", "<ol start=\"10\">\n<li>foo\n<ul>\n<li>bar</li>\n</ul>\n</li>\n</ol>", "");
        }
    }
        // Three is not enough:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example258()
        {
            // Example 258
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     10) foo
            //        - bar
            //
            // Should be rendered as:
            //     <ol start="10">
            //     <li>foo</li>
            //     </ol>
            //     <ul>
            //     <li>bar</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 258, "Container blocks List items");
			TestParser.TestSpec("10) foo\n   - bar", "<ol start=\"10\">\n<li>foo</li>\n</ol>\n<ul>\n<li>bar</li>\n</ul>", "");
        }
    }
        // A list may be the first block in a list item:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example259()
        {
            // Example 259
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     - - foo
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <ul>
            //     <li>foo</li>
            //     </ul>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 259, "Container blocks List items");
			TestParser.TestSpec("- - foo", "<ul>\n<li>\n<ul>\n<li>foo</li>\n</ul>\n</li>\n</ul>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example260()
        {
            // Example 260
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     1. - 2. foo
            //
            // Should be rendered as:
            //     <ol>
            //     <li>
            //     <ul>
            //     <li>
            //     <ol start="2">
            //     <li>foo</li>
            //     </ol>
            //     </li>
            //     </ul>
            //     </li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 260, "Container blocks List items");
			TestParser.TestSpec("1. - 2. foo", "<ol>\n<li>\n<ul>\n<li>\n<ol start=\"2\">\n<li>foo</li>\n</ol>\n</li>\n</ul>\n</li>\n</ol>", "");
        }
    }
        // A list item can contain a heading:
    [TestFixture]
    public partial class TestContainerblocksListitems
    {
        [Test]
        public void Example261()
        {
            // Example 261
            // Section: Container blocks List items
            //
            // The following CommonMark:
            //     - # Foo
            //     - Bar
            //       ---
            //       baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <h1>Foo</h1>
            //     </li>
            //     <li>
            //     <h2>Bar</h2>
            //     baz</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 261, "Container blocks List items");
			TestParser.TestSpec("- # Foo\n- Bar\n  ---\n  baz", "<ul>\n<li>\n<h1>Foo</h1>\n</li>\n<li>\n<h2>Bar</h2>\nbaz</li>\n</ul>", "");
        }
    }
        // ### Motivation
        //
        // John Gruber's Markdown spec says the following about list items:
        //
        // 1. "List markers typically start at the left margin, but may be indented
        // by up to three spaces. List markers must be followed by one or more
        // spaces or a tab."
        //
        // 2. "To make lists look nice, you can wrap items with hanging indents....
        // But if you don't want to, you don't have to."
        //
        // 3. "List items may consist of multiple paragraphs. Each subsequent
        // paragraph in a list item must be indented by either 4 spaces or one
        // tab."
        //
        // 4. "It looks nice if you indent every line of the subsequent paragraphs,
        // but here again, Markdown will allow you to be lazy."
        //
        // 5. "To put a blockquote within a list item, the blockquote's `>`
        // delimiters need to be indented."
        //
        // 6. "To put a code block within a list item, the code block needs to be
        // indented twice — 8 spaces or two tabs."
        //
        // These rules specify that a paragraph under a list item must be indented
        // four spaces (presumably, from the left margin, rather than the start of
        // the list marker, but this is not said), and that code under a list item
        // must be indented eight spaces instead of the usual four.  They also say
        // that a block quote must be indented, but not by how much; however, the
        // example given has four spaces indentation.  Although nothing is said
        // about other kinds of block-level content, it is certainly reasonable to
        // infer that *all* block elements under a list item, including other
        // lists, must be indented four spaces.  This principle has been called the
        // *four-space rule*.
        //
        // The four-space rule is clear and principled, and if the reference
        // implementation `Markdown.pl` had followed it, it probably would have
        // become the standard.  However, `Markdown.pl` allowed paragraphs and
        // sublists to start with only two spaces indentation, at least on the
        // outer level.  Worse, its behavior was inconsistent: a sublist of an
        // outer-level list needed two spaces indentation, but a sublist of this
        // sublist needed three spaces.  It is not surprising, then, that different
        // implementations of Markdown have developed very different rules for
        // determining what comes under a list item.  (Pandoc and python-Markdown,
        // for example, stuck with Gruber's syntax description and the four-space
        // rule, while discount, redcarpet, marked, PHP Markdown, and others
        // followed `Markdown.pl`'s behavior more closely.)
        //
        // Unfortunately, given the divergences between implementations, there
        // is no way to give a spec for list items that will be guaranteed not
        // to break any existing documents.  However, the spec given here should
        // correctly handle lists formatted with either the four-space rule or
        // the more forgiving `Markdown.pl` behavior, provided they are laid out
        // in a way that is natural for a human to read.
        //
        // The strategy here is to let the width and indentation of the list marker
        // determine the indentation necessary for blocks to fall under the list
        // item, rather than having a fixed and arbitrary number.  The writer can
        // think of the body of the list item as a unit which gets indented to the
        // right enough to fit the list marker (and any indentation on the list
        // marker).  (The laziness rule, #5, then allows continuation lines to be
        // unindented if needed.)
        //
        // This rule is superior, we claim, to any rule requiring a fixed level of
        // indentation from the margin.  The four-space rule is clear but
        // unnatural. It is quite unintuitive that
        //
        // ``` markdown
        // - foo
        //
        // bar
        //
        // - baz
        // ```
        //
        // should be parsed as two lists with an intervening paragraph,
        //
        // ``` html
        // <ul>
        // <li>foo</li>
        // </ul>
        // <p>bar</p>
        // <ul>
        // <li>baz</li>
        // </ul>
        // ```
        //
        // as the four-space rule demands, rather than a single list,
        //
        // ``` html
        // <ul>
        // <li>
        // <p>foo</p>
        // <p>bar</p>
        // <ul>
        // <li>baz</li>
        // </ul>
        // </li>
        // </ul>
        // ```
        //
        // The choice of four spaces is arbitrary.  It can be learned, but it is
        // not likely to be guessed, and it trips up beginners regularly.
        //
        // Would it help to adopt a two-space rule?  The problem is that such
        // a rule, together with the rule allowing 1--3 spaces indentation of the
        // initial list marker, allows text that is indented *less than* the
        // original list marker to be included in the list item. For example,
        // `Markdown.pl` parses
        //
        // ``` markdown
        // - one
        //
        // two
        // ```
        //
        // as a single list item, with `two` a continuation paragraph:
        //
        // ``` html
        // <ul>
        // <li>
        // <p>one</p>
        // <p>two</p>
        // </li>
        // </ul>
        // ```
        //
        // and similarly
        //
        // ``` markdown
        // >   - one
        // >
        // >  two
        // ```
        //
        // as
        //
        // ``` html
        // <blockquote>
        // <ul>
        // <li>
        // <p>one</p>
        // <p>two</p>
        // </li>
        // </ul>
        // </blockquote>
        // ```
        //
        // This is extremely unintuitive.
        //
        // Rather than requiring a fixed indent from the margin, we could require
        // a fixed indent (say, two spaces, or even one space) from the list marker (which
        // may itself be indented).  This proposal would remove the last anomaly
        // discussed.  Unlike the spec presented above, it would count the following
        // as a list item with a subparagraph, even though the paragraph `bar`
        // is not indented as far as the first paragraph `foo`:
        //
        // ``` markdown
        // 10. foo
        //
        // bar
        // ```
        //
        // Arguably this text does read like a list item with `bar` as a subparagraph,
        // which may count in favor of the proposal.  However, on this proposal indented
        // code would have to be indented six spaces after the list marker.  And this
        // would break a lot of existing Markdown, which has the pattern:
        //
        // ``` markdown
        // 1.  foo
        //
        // indented code
        // ```
        //
        // where the code is indented eight spaces.  The spec above, by contrast, will
        // parse this text as expected, since the code block's indentation is measured
        // from the beginning of `foo`.
        //
        // The one case that needs special treatment is a list item that *starts*
        // with indented code.  How much indentation is required in that case, since
        // we don't have a "first paragraph" to measure from?  Rule #2 simply stipulates
        // that in such cases, we require one space indentation from the list marker
        // (and then the normal four spaces for the indented code).  This will match the
        // four-space rule in cases where the list marker plus its initial indentation
        // takes four spaces (a common case), but diverge in other cases.
        //
        // ## Lists
        //
        // A [list](@) is a sequence of one or more
        // list items [of the same type].  The list items
        // may be separated by single [blank lines], but two
        // blank lines end all containing lists.
        //
        // Two list items are [of the same type](@)
        // if they begin with a [list marker] of the same type.
        // Two list markers are of the
        // same type if (a) they are bullet list markers using the same character
        // (`-`, `+`, or `*`) or (b) they are ordered list numbers with the same
        // delimiter (either `.` or `)`).
        //
        // A list is an [ordered list](@)
        // if its constituent list items begin with
        // [ordered list markers], and a
        // [bullet list](@) if its constituent list
        // items begin with [bullet list markers].
        //
        // The [start number](@)
        // of an [ordered list] is determined by the list number of
        // its initial list item.  The numbers of subsequent list items are
        // disregarded.
        //
        // A list is [loose](@) if any of its constituent
        // list items are separated by blank lines, or if any of its constituent
        // list items directly contain two block-level elements with a blank line
        // between them.  Otherwise a list is [tight](@).
        // (The difference in HTML output is that paragraphs in a loose list are
        // wrapped in `<p>` tags, while paragraphs in a tight list are not.)
        //
        // Changing the bullet or ordered list delimiter starts a new list:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example262()
        {
            // Example 262
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - foo
            //     - bar
            //     + baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     <li>bar</li>
            //     </ul>
            //     <ul>
            //     <li>baz</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 262, "Container blocks Lists");
			TestParser.TestSpec("- foo\n- bar\n+ baz", "<ul>\n<li>foo</li>\n<li>bar</li>\n</ul>\n<ul>\n<li>baz</li>\n</ul>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example263()
        {
            // Example 263
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     1. foo
            //     2. bar
            //     3) baz
            //
            // Should be rendered as:
            //     <ol>
            //     <li>foo</li>
            //     <li>bar</li>
            //     </ol>
            //     <ol start="3">
            //     <li>baz</li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 263, "Container blocks Lists");
			TestParser.TestSpec("1. foo\n2. bar\n3) baz", "<ol>\n<li>foo</li>\n<li>bar</li>\n</ol>\n<ol start=\"3\">\n<li>baz</li>\n</ol>", "");
        }
    }
        // In CommonMark, a list can interrupt a paragraph. That is,
        // no blank line is needed to separate a paragraph from a following
        // list:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example264()
        {
            // Example 264
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     Foo
            //     - bar
            //     - baz
            //
            // Should be rendered as:
            //     <p>Foo</p>
            //     <ul>
            //     <li>bar</li>
            //     <li>baz</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 264, "Container blocks Lists");
			TestParser.TestSpec("Foo\n- bar\n- baz", "<p>Foo</p>\n<ul>\n<li>bar</li>\n<li>baz</li>\n</ul>", "");
        }
    }
        // `Markdown.pl` does not allow this, through fear of triggering a list
        // via a numeral in a hard-wrapped line:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example265()
        {
            // Example 265
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     The number of windows in my house is
            //     14.  The number of doors is 6.
            //
            // Should be rendered as:
            //     <p>The number of windows in my house is</p>
            //     <ol start="14">
            //     <li>The number of doors is 6.</li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 265, "Container blocks Lists");
			TestParser.TestSpec("The number of windows in my house is\n14.  The number of doors is 6.", "<p>The number of windows in my house is</p>\n<ol start=\"14\">\n<li>The number of doors is 6.</li>\n</ol>", "");
        }
    }
        // Oddly, `Markdown.pl` *does* allow a blockquote to interrupt a paragraph,
        // even though the same considerations might apply.  We think that the two
        // cases should be treated the same.  Here are two reasons for allowing
        // lists to interrupt paragraphs:
        //
        // First, it is natural and not uncommon for people to start lists without
        // blank lines:
        //
        // I need to buy
        // - new shoes
        // - a coat
        // - a plane ticket
        //
        // Second, we are attracted to a
        //
        // > [principle of uniformity](@):
        // > if a chunk of text has a certain
        // > meaning, it will continue to have the same meaning when put into a
        // > container block (such as a list item or blockquote).
        //
        // (Indeed, the spec for [list items] and [block quotes] presupposes
        // this principle.) This principle implies that if
        //
        // * I need to buy
        // - new shoes
        // - a coat
        // - a plane ticket
        //
        // is a list item containing a paragraph followed by a nested sublist,
        // as all Markdown implementations agree it is (though the paragraph
        // may be rendered without `<p>` tags, since the list is "tight"),
        // then
        //
        // I need to buy
        // - new shoes
        // - a coat
        // - a plane ticket
        //
        // by itself should be a paragraph followed by a nested sublist.
        //
        // Our adherence to the [principle of uniformity]
        // thus inclines us to think that there are two coherent packages:
        //
        // 1.  Require blank lines before *all* lists and blockquotes,
        // including lists that occur as sublists inside other list items.
        //
        // 2.  Require blank lines in none of these places.
        //
        // [reStructuredText](http://docutils.sourceforge.net/rst.html) takes
        // the first approach, for which there is much to be said.  But the second
        // seems more consistent with established practice with Markdown.
        //
        // There can be blank lines between items, but two blank lines end
        // a list:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example266()
        {
            // Example 266
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - foo
            //     
            //     - bar
            //     
            //     
            //     - baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>foo</p>
            //     </li>
            //     <li>
            //     <p>bar</p>
            //     </li>
            //     </ul>
            //     <ul>
            //     <li>baz</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 266, "Container blocks Lists");
			TestParser.TestSpec("- foo\n\n- bar\n\n\n- baz", "<ul>\n<li>\n<p>foo</p>\n</li>\n<li>\n<p>bar</p>\n</li>\n</ul>\n<ul>\n<li>baz</li>\n</ul>", "");
        }
    }
        // As illustrated above in the section on [list items],
        // two blank lines between blocks *within* a list item will also end a
        // list:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example267()
        {
            // Example 267
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - foo
            //     
            //     
            //       bar
            //     - baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     </ul>
            //     <p>bar</p>
            //     <ul>
            //     <li>baz</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 267, "Container blocks Lists");
			TestParser.TestSpec("- foo\n\n\n  bar\n- baz", "<ul>\n<li>foo</li>\n</ul>\n<p>bar</p>\n<ul>\n<li>baz</li>\n</ul>", "");
        }
    }
        // Indeed, two blank lines will end *all* containing lists:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example268()
        {
            // Example 268
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - foo
            //       - bar
            //         - baz
            //     
            //     
            //           bim
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo
            //     <ul>
            //     <li>bar
            //     <ul>
            //     <li>baz</li>
            //     </ul>
            //     </li>
            //     </ul>
            //     </li>
            //     </ul>
            //     <pre><code>  bim
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 268, "Container blocks Lists");
			TestParser.TestSpec("- foo\n  - bar\n    - baz\n\n\n      bim", "<ul>\n<li>foo\n<ul>\n<li>bar\n<ul>\n<li>baz</li>\n</ul>\n</li>\n</ul>\n</li>\n</ul>\n<pre><code>  bim\n</code></pre>", "");
        }
    }
        // Thus, two blank lines can be used to separate consecutive lists of
        // the same type, or to separate a list from an indented code block
        // that would otherwise be parsed as a subparagraph of the final list
        // item:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example269()
        {
            // Example 269
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - foo
            //     - bar
            //     
            //     
            //     - baz
            //     - bim
            //
            // Should be rendered as:
            //     <ul>
            //     <li>foo</li>
            //     <li>bar</li>
            //     </ul>
            //     <ul>
            //     <li>baz</li>
            //     <li>bim</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 269, "Container blocks Lists");
			TestParser.TestSpec("- foo\n- bar\n\n\n- baz\n- bim", "<ul>\n<li>foo</li>\n<li>bar</li>\n</ul>\n<ul>\n<li>baz</li>\n<li>bim</li>\n</ul>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example270()
        {
            // Example 270
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     -   foo
            //     
            //         notcode
            //     
            //     -   foo
            //     
            //     
            //         code
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>foo</p>
            //     <p>notcode</p>
            //     </li>
            //     <li>
            //     <p>foo</p>
            //     </li>
            //     </ul>
            //     <pre><code>code
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 270, "Container blocks Lists");
			TestParser.TestSpec("-   foo\n\n    notcode\n\n-   foo\n\n\n    code", "<ul>\n<li>\n<p>foo</p>\n<p>notcode</p>\n</li>\n<li>\n<p>foo</p>\n</li>\n</ul>\n<pre><code>code\n</code></pre>", "");
        }
    }
        // List items need not be indented to the same level.  The following
        // list items will be treated as items at the same list level,
        // since none is indented enough to belong to the previous list
        // item:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example271()
        {
            // Example 271
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - a
            //      - b
            //       - c
            //        - d
            //         - e
            //        - f
            //       - g
            //      - h
            //     - i
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a</li>
            //     <li>b</li>
            //     <li>c</li>
            //     <li>d</li>
            //     <li>e</li>
            //     <li>f</li>
            //     <li>g</li>
            //     <li>h</li>
            //     <li>i</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 271, "Container blocks Lists");
			TestParser.TestSpec("- a\n - b\n  - c\n   - d\n    - e\n   - f\n  - g\n - h\n- i", "<ul>\n<li>a</li>\n<li>b</li>\n<li>c</li>\n<li>d</li>\n<li>e</li>\n<li>f</li>\n<li>g</li>\n<li>h</li>\n<li>i</li>\n</ul>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example272()
        {
            // Example 272
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     1. a
            //     
            //       2. b
            //     
            //         3. c
            //
            // Should be rendered as:
            //     <ol>
            //     <li>
            //     <p>a</p>
            //     </li>
            //     <li>
            //     <p>b</p>
            //     </li>
            //     <li>
            //     <p>c</p>
            //     </li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 272, "Container blocks Lists");
			TestParser.TestSpec("1. a\n\n  2. b\n\n    3. c", "<ol>\n<li>\n<p>a</p>\n</li>\n<li>\n<p>b</p>\n</li>\n<li>\n<p>c</p>\n</li>\n</ol>", "");
        }
    }
        // This is a loose list, because there is a blank line between
        // two of the list items:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example273()
        {
            // Example 273
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - a
            //     - b
            //     
            //     - c
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>a</p>
            //     </li>
            //     <li>
            //     <p>b</p>
            //     </li>
            //     <li>
            //     <p>c</p>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 273, "Container blocks Lists");
			TestParser.TestSpec("- a\n- b\n\n- c", "<ul>\n<li>\n<p>a</p>\n</li>\n<li>\n<p>b</p>\n</li>\n<li>\n<p>c</p>\n</li>\n</ul>", "");
        }
    }
        // So is this, with a empty second item:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example274()
        {
            // Example 274
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     * a
            //     *
            //     
            //     * c
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>a</p>
            //     </li>
            //     <li></li>
            //     <li>
            //     <p>c</p>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 274, "Container blocks Lists");
			TestParser.TestSpec("* a\n*\n\n* c", "<ul>\n<li>\n<p>a</p>\n</li>\n<li></li>\n<li>\n<p>c</p>\n</li>\n</ul>", "");
        }
    }
        // These are loose lists, even though there is no space between the items,
        // because one of the items directly contains two block-level elements
        // with a blank line between them:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example275()
        {
            // Example 275
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - a
            //     - b
            //     
            //       c
            //     - d
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>a</p>
            //     </li>
            //     <li>
            //     <p>b</p>
            //     <p>c</p>
            //     </li>
            //     <li>
            //     <p>d</p>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 275, "Container blocks Lists");
			TestParser.TestSpec("- a\n- b\n\n  c\n- d", "<ul>\n<li>\n<p>a</p>\n</li>\n<li>\n<p>b</p>\n<p>c</p>\n</li>\n<li>\n<p>d</p>\n</li>\n</ul>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example276()
        {
            // Example 276
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - a
            //     - b
            //     
            //       [ref]: /url
            //     - d
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>a</p>
            //     </li>
            //     <li>
            //     <p>b</p>
            //     </li>
            //     <li>
            //     <p>d</p>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 276, "Container blocks Lists");
			TestParser.TestSpec("- a\n- b\n\n  [ref]: /url\n- d", "<ul>\n<li>\n<p>a</p>\n</li>\n<li>\n<p>b</p>\n</li>\n<li>\n<p>d</p>\n</li>\n</ul>", "");
        }
    }
        // This is a tight list, because the blank lines are in a code block:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example277()
        {
            // Example 277
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - a
            //     - ```
            //       b
            //     
            //     
            //       ```
            //     - c
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a</li>
            //     <li>
            //     <pre><code>b
            //     
            //     
            //     </code></pre>
            //     </li>
            //     <li>c</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 277, "Container blocks Lists");
			TestParser.TestSpec("- a\n- ```\n  b\n\n\n  ```\n- c", "<ul>\n<li>a</li>\n<li>\n<pre><code>b\n\n\n</code></pre>\n</li>\n<li>c</li>\n</ul>", "");
        }
    }
        // This is a tight list, because the blank line is between two
        // paragraphs of a sublist.  So the sublist is loose while
        // the outer list is tight:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example278()
        {
            // Example 278
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - a
            //       - b
            //     
            //         c
            //     - d
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a
            //     <ul>
            //     <li>
            //     <p>b</p>
            //     <p>c</p>
            //     </li>
            //     </ul>
            //     </li>
            //     <li>d</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 278, "Container blocks Lists");
			TestParser.TestSpec("- a\n  - b\n\n    c\n- d", "<ul>\n<li>a\n<ul>\n<li>\n<p>b</p>\n<p>c</p>\n</li>\n</ul>\n</li>\n<li>d</li>\n</ul>", "");
        }
    }
        // This is a tight list, because the blank line is inside the
        // block quote:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example279()
        {
            // Example 279
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     * a
            //       > b
            //       >
            //     * c
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a
            //     <blockquote>
            //     <p>b</p>
            //     </blockquote>
            //     </li>
            //     <li>c</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 279, "Container blocks Lists");
			TestParser.TestSpec("* a\n  > b\n  >\n* c", "<ul>\n<li>a\n<blockquote>\n<p>b</p>\n</blockquote>\n</li>\n<li>c</li>\n</ul>", "");
        }
    }
        // This list is tight, because the consecutive block elements
        // are not separated by blank lines:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example280()
        {
            // Example 280
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - a
            //       > b
            //       ```
            //       c
            //       ```
            //     - d
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a
            //     <blockquote>
            //     <p>b</p>
            //     </blockquote>
            //     <pre><code>c
            //     </code></pre>
            //     </li>
            //     <li>d</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 280, "Container blocks Lists");
			TestParser.TestSpec("- a\n  > b\n  ```\n  c\n  ```\n- d", "<ul>\n<li>a\n<blockquote>\n<p>b</p>\n</blockquote>\n<pre><code>c\n</code></pre>\n</li>\n<li>d</li>\n</ul>", "");
        }
    }
        // A single-paragraph list is tight:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example281()
        {
            // Example 281
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - a
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 281, "Container blocks Lists");
			TestParser.TestSpec("- a", "<ul>\n<li>a</li>\n</ul>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example282()
        {
            // Example 282
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - a
            //       - b
            //
            // Should be rendered as:
            //     <ul>
            //     <li>a
            //     <ul>
            //     <li>b</li>
            //     </ul>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 282, "Container blocks Lists");
			TestParser.TestSpec("- a\n  - b", "<ul>\n<li>a\n<ul>\n<li>b</li>\n</ul>\n</li>\n</ul>", "");
        }
    }
        // This list is loose, because of the blank line between the
        // two block elements in the list item:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example283()
        {
            // Example 283
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     1. ```
            //        foo
            //        ```
            //     
            //        bar
            //
            // Should be rendered as:
            //     <ol>
            //     <li>
            //     <pre><code>foo
            //     </code></pre>
            //     <p>bar</p>
            //     </li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 283, "Container blocks Lists");
			TestParser.TestSpec("1. ```\n   foo\n   ```\n\n   bar", "<ol>\n<li>\n<pre><code>foo\n</code></pre>\n<p>bar</p>\n</li>\n</ol>", "");
        }
    }
        // Here the outer list is loose, the inner list tight:
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example284()
        {
            // Example 284
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     * foo
            //       * bar
            //     
            //       baz
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>foo</p>
            //     <ul>
            //     <li>bar</li>
            //     </ul>
            //     <p>baz</p>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 284, "Container blocks Lists");
			TestParser.TestSpec("* foo\n  * bar\n\n  baz", "<ul>\n<li>\n<p>foo</p>\n<ul>\n<li>bar</li>\n</ul>\n<p>baz</p>\n</li>\n</ul>", "");
        }
    }
    [TestFixture]
    public partial class TestContainerblocksLists
    {
        [Test]
        public void Example285()
        {
            // Example 285
            // Section: Container blocks Lists
            //
            // The following CommonMark:
            //     - a
            //       - b
            //       - c
            //     
            //     - d
            //       - e
            //       - f
            //
            // Should be rendered as:
            //     <ul>
            //     <li>
            //     <p>a</p>
            //     <ul>
            //     <li>b</li>
            //     <li>c</li>
            //     </ul>
            //     </li>
            //     <li>
            //     <p>d</p>
            //     <ul>
            //     <li>e</li>
            //     <li>f</li>
            //     </ul>
            //     </li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 285, "Container blocks Lists");
			TestParser.TestSpec("- a\n  - b\n  - c\n\n- d\n  - e\n  - f", "<ul>\n<li>\n<p>a</p>\n<ul>\n<li>b</li>\n<li>c</li>\n</ul>\n</li>\n<li>\n<p>d</p>\n<ul>\n<li>e</li>\n<li>f</li>\n</ul>\n</li>\n</ul>", "");
        }
    }
        // # Inlines
        //
        // Inlines are parsed sequentially from the beginning of the character
        // stream to the end (left to right, in left-to-right languages).
        // Thus, for example, in
    [TestFixture]
    public partial class TestInlines
    {
        [Test]
        public void Example286()
        {
            // Example 286
            // Section: Inlines
            //
            // The following CommonMark:
            //     `hi`lo`
            //
            // Should be rendered as:
            //     <p><code>hi</code>lo`</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 286, "Inlines");
			TestParser.TestSpec("`hi`lo`", "<p><code>hi</code>lo`</p>", "");
        }
    }
        // `hi` is parsed as code, leaving the backtick at the end as a literal
        // backtick.
        //
        // ## Backslash escapes
        //
        // Any ASCII punctuation character may be backslash-escaped:
    [TestFixture]
    public partial class TestInlinesBackslashescapes
    {
        [Test]
        public void Example287()
        {
            // Example 287
            // Section: Inlines Backslash escapes
            //
            // The following CommonMark:
            //     \!\"\#\$\%\&\'\(\)\*\+\,\-\.\/\:\;\<\=\>\?\@\[\\\]\^\_\`\{\|\}\~
            //
            // Should be rendered as:
            //     <p>!&quot;#$%&amp;'()*+,-./:;&lt;=&gt;?@[\]^_`{|}~</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 287, "Inlines Backslash escapes");
			TestParser.TestSpec("\\!\\\"\\#\\$\\%\\&\\'\\(\\)\\*\\+\\,\\-\\.\\/\\:\\;\\<\\=\\>\\?\\@\\[\\\\\\]\\^\\_\\`\\{\\|\\}\\~", "<p>!&quot;#$%&amp;'()*+,-./:;&lt;=&gt;?@[\\]^_`{|}~</p>", "");
        }
    }
        // Backslashes before other characters are treated as literal
        // backslashes:
    [TestFixture]
    public partial class TestInlinesBackslashescapes
    {
        [Test]
        public void Example288()
        {
            // Example 288
            // Section: Inlines Backslash escapes
            //
            // The following CommonMark:
            //     \→\A\a\ \3\φ\«
            //
            // Should be rendered as:
            //     <p>\→\A\a\ \3\φ\«</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 288, "Inlines Backslash escapes");
			TestParser.TestSpec("\\\t\\A\\a\\ \\3\\φ\\«", "<p>\\\t\\A\\a\\ \\3\\φ\\«</p>", "");
        }
    }
        // Escaped characters are treated as regular characters and do
        // not have their usual Markdown meanings:
    [TestFixture]
    public partial class TestInlinesBackslashescapes
    {
        [Test]
        public void Example289()
        {
            // Example 289
            // Section: Inlines Backslash escapes
            //
            // The following CommonMark:
            //     \*not emphasized*
            //     \<br/> not a tag
            //     \[not a link](/foo)
            //     \`not code`
            //     1\. not a list
            //     \* not a list
            //     \# not a heading
            //     \[foo]: /url "not a reference"
            //
            // Should be rendered as:
            //     <p>*not emphasized*
            //     &lt;br/&gt; not a tag
            //     [not a link](/foo)
            //     `not code`
            //     1. not a list
            //     * not a list
            //     # not a heading
            //     [foo]: /url &quot;not a reference&quot;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 289, "Inlines Backslash escapes");
			TestParser.TestSpec("\\*not emphasized*\n\\<br/> not a tag\n\\[not a link](/foo)\n\\`not code`\n1\\. not a list\n\\* not a list\n\\# not a heading\n\\[foo]: /url \"not a reference\"", "<p>*not emphasized*\n&lt;br/&gt; not a tag\n[not a link](/foo)\n`not code`\n1. not a list\n* not a list\n# not a heading\n[foo]: /url &quot;not a reference&quot;</p>", "");
        }
    }
        // If a backslash is itself escaped, the following character is not:
    [TestFixture]
    public partial class TestInlinesBackslashescapes
    {
        [Test]
        public void Example290()
        {
            // Example 290
            // Section: Inlines Backslash escapes
            //
            // The following CommonMark:
            //     \\*emphasis*
            //
            // Should be rendered as:
            //     <p>\<em>emphasis</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 290, "Inlines Backslash escapes");
			TestParser.TestSpec("\\\\*emphasis*", "<p>\\<em>emphasis</em></p>", "");
        }
    }
        // A backslash at the end of the line is a [hard line break]:
    [TestFixture]
    public partial class TestInlinesBackslashescapes
    {
        [Test]
        public void Example291()
        {
            // Example 291
            // Section: Inlines Backslash escapes
            //
            // The following CommonMark:
            //     foo\
            //     bar
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 291, "Inlines Backslash escapes");
			TestParser.TestSpec("foo\\\nbar", "<p>foo<br />\nbar</p>", "");
        }
    }
        // Backslash escapes do not work in code blocks, code spans, autolinks, or
        // raw HTML:
    [TestFixture]
    public partial class TestInlinesBackslashescapes
    {
        [Test]
        public void Example292()
        {
            // Example 292
            // Section: Inlines Backslash escapes
            //
            // The following CommonMark:
            //     `` \[\` ``
            //
            // Should be rendered as:
            //     <p><code>\[\`</code></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 292, "Inlines Backslash escapes");
			TestParser.TestSpec("`` \\[\\` ``", "<p><code>\\[\\`</code></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesBackslashescapes
    {
        [Test]
        public void Example293()
        {
            // Example 293
            // Section: Inlines Backslash escapes
            //
            // The following CommonMark:
            //         \[\]
            //
            // Should be rendered as:
            //     <pre><code>\[\]
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 293, "Inlines Backslash escapes");
			TestParser.TestSpec("    \\[\\]", "<pre><code>\\[\\]\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesBackslashescapes
    {
        [Test]
        public void Example294()
        {
            // Example 294
            // Section: Inlines Backslash escapes
            //
            // The following CommonMark:
            //     ~~~
            //     \[\]
            //     ~~~
            //
            // Should be rendered as:
            //     <pre><code>\[\]
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 294, "Inlines Backslash escapes");
			TestParser.TestSpec("~~~\n\\[\\]\n~~~", "<pre><code>\\[\\]\n</code></pre>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesBackslashescapes
    {
        [Test]
        public void Example295()
        {
            // Example 295
            // Section: Inlines Backslash escapes
            //
            // The following CommonMark:
            //     <http://example.com?find=\*>
            //
            // Should be rendered as:
            //     <p><a href="http://example.com?find=%5C*">http://example.com?find=\*</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 295, "Inlines Backslash escapes");
			TestParser.TestSpec("<http://example.com?find=\\*>", "<p><a href=\"http://example.com?find=%5C*\">http://example.com?find=\\*</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesBackslashescapes
    {
        [Test]
        public void Example296()
        {
            // Example 296
            // Section: Inlines Backslash escapes
            //
            // The following CommonMark:
            //     <a href="/bar\/)">
            //
            // Should be rendered as:
            //     <a href="/bar\/)">

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 296, "Inlines Backslash escapes");
			TestParser.TestSpec("<a href=\"/bar\\/)\">", "<a href=\"/bar\\/)\">", "");
        }
    }
        // But they work in all other contexts, including URLs and link titles,
        // link references, and [info strings] in [fenced code blocks]:
    [TestFixture]
    public partial class TestInlinesBackslashescapes
    {
        [Test]
        public void Example297()
        {
            // Example 297
            // Section: Inlines Backslash escapes
            //
            // The following CommonMark:
            //     [foo](/bar\* "ti\*tle")
            //
            // Should be rendered as:
            //     <p><a href="/bar*" title="ti*tle">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 297, "Inlines Backslash escapes");
			TestParser.TestSpec("[foo](/bar\\* \"ti\\*tle\")", "<p><a href=\"/bar*\" title=\"ti*tle\">foo</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesBackslashescapes
    {
        [Test]
        public void Example298()
        {
            // Example 298
            // Section: Inlines Backslash escapes
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     [foo]: /bar\* "ti\*tle"
            //
            // Should be rendered as:
            //     <p><a href="/bar*" title="ti*tle">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 298, "Inlines Backslash escapes");
			TestParser.TestSpec("[foo]\n\n[foo]: /bar\\* \"ti\\*tle\"", "<p><a href=\"/bar*\" title=\"ti*tle\">foo</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesBackslashescapes
    {
        [Test]
        public void Example299()
        {
            // Example 299
            // Section: Inlines Backslash escapes
            //
            // The following CommonMark:
            //     ``` foo\+bar
            //     foo
            //     ```
            //
            // Should be rendered as:
            //     <pre><code class="language-foo+bar">foo
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 299, "Inlines Backslash escapes");
			TestParser.TestSpec("``` foo\\+bar\nfoo\n```", "<pre><code class=\"language-foo+bar\">foo\n</code></pre>", "");
        }
    }
        // ## Entity and numeric character references
        //
        // All valid HTML entity references and numeric character
        // references, except those occuring in code blocks and code spans,
        // are recognized as such and treated as equivalent to the
        // corresponding Unicode characters.  Conforming CommonMark parsers
        // need not store information about whether a particular character
        // was represented in the source using a Unicode character or
        // an entity reference.
        //
        // [Entity references](@) consist of `&` + any of the valid
        // HTML5 entity names + `;`. The
        // document <https://html.spec.whatwg.org/multipage/entities.json>
        // is used as an authoritative source for the valid entity
        // references and their corresponding code points.
    [TestFixture]
    public partial class TestInlinesEntityandnumericcharacterreferences
    {
        [Test]
        public void Example300()
        {
            // Example 300
            // Section: Inlines Entity and numeric character references
            //
            // The following CommonMark:
            //     &nbsp; &amp; &copy; &AElig; &Dcaron;
            //     &frac34; &HilbertSpace; &DifferentialD;
            //     &ClockwiseContourIntegral; &ngE;
            //
            // Should be rendered as:
            //     <p>  &amp; © Æ Ď
            //     ¾ ℋ ⅆ
            //     ∲ ≧̸</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 300, "Inlines Entity and numeric character references");
			TestParser.TestSpec("&nbsp; &amp; &copy; &AElig; &Dcaron;\n&frac34; &HilbertSpace; &DifferentialD;\n&ClockwiseContourIntegral; &ngE;", "<p>  &amp; © Æ Ď\n¾ ℋ ⅆ\n∲ ≧̸</p>", "");
        }
    }
        // [Decimal numeric character
        // references](@)
        // consist of `&#` + a string of 1--8 arabic digits + `;`. A
        // numeric character reference is parsed as the corresponding
        // Unicode character. Invalid Unicode code points will be replaced by
        // the REPLACEMENT CHARACTER (`U+FFFD`).  For security reasons,
        // the code point `U+0000` will also be replaced by `U+FFFD`.
    [TestFixture]
    public partial class TestInlinesEntityandnumericcharacterreferences
    {
        [Test]
        public void Example301()
        {
            // Example 301
            // Section: Inlines Entity and numeric character references
            //
            // The following CommonMark:
            //     &#35; &#1234; &#992; &#98765432; &#0;
            //
            // Should be rendered as:
            //     <p># Ӓ Ϡ � �</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 301, "Inlines Entity and numeric character references");
			TestParser.TestSpec("&#35; &#1234; &#992; &#98765432; &#0;", "<p># Ӓ Ϡ � �</p>", "");
        }
    }
        // [Hexadecimal numeric character
        // references](@) consist of `&#` +
        // either `X` or `x` + a string of 1-8 hexadecimal digits + `;`.
        // They too are parsed as the corresponding Unicode character (this
        // time specified with a hexadecimal numeral instead of decimal).
    [TestFixture]
    public partial class TestInlinesEntityandnumericcharacterreferences
    {
        [Test]
        public void Example302()
        {
            // Example 302
            // Section: Inlines Entity and numeric character references
            //
            // The following CommonMark:
            //     &#X22; &#XD06; &#xcab;
            //
            // Should be rendered as:
            //     <p>&quot; ആ ಫ</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 302, "Inlines Entity and numeric character references");
			TestParser.TestSpec("&#X22; &#XD06; &#xcab;", "<p>&quot; ആ ಫ</p>", "");
        }
    }
        // Here are some nonentities:
    [TestFixture]
    public partial class TestInlinesEntityandnumericcharacterreferences
    {
        [Test]
        public void Example303()
        {
            // Example 303
            // Section: Inlines Entity and numeric character references
            //
            // The following CommonMark:
            //     &nbsp &x; &#; &#x;
            //     &ThisIsNotDefined; &hi?;
            //
            // Should be rendered as:
            //     <p>&amp;nbsp &amp;x; &amp;#; &amp;#x;
            //     &amp;ThisIsNotDefined; &amp;hi?;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 303, "Inlines Entity and numeric character references");
			TestParser.TestSpec("&nbsp &x; &#; &#x;\n&ThisIsNotDefined; &hi?;", "<p>&amp;nbsp &amp;x; &amp;#; &amp;#x;\n&amp;ThisIsNotDefined; &amp;hi?;</p>", "");
        }
    }
        // Although HTML5 does accept some entity references
        // without a trailing semicolon (such as `&copy`), these are not
        // recognized here, because it makes the grammar too ambiguous:
    [TestFixture]
    public partial class TestInlinesEntityandnumericcharacterreferences
    {
        [Test]
        public void Example304()
        {
            // Example 304
            // Section: Inlines Entity and numeric character references
            //
            // The following CommonMark:
            //     &copy
            //
            // Should be rendered as:
            //     <p>&amp;copy</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 304, "Inlines Entity and numeric character references");
			TestParser.TestSpec("&copy", "<p>&amp;copy</p>", "");
        }
    }
        // Strings that are not on the list of HTML5 named entities are not
        // recognized as entity references either:
    [TestFixture]
    public partial class TestInlinesEntityandnumericcharacterreferences
    {
        [Test]
        public void Example305()
        {
            // Example 305
            // Section: Inlines Entity and numeric character references
            //
            // The following CommonMark:
            //     &MadeUpEntity;
            //
            // Should be rendered as:
            //     <p>&amp;MadeUpEntity;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 305, "Inlines Entity and numeric character references");
			TestParser.TestSpec("&MadeUpEntity;", "<p>&amp;MadeUpEntity;</p>", "");
        }
    }
        // Entity and numeric character references are recognized in any
        // context besides code spans or code blocks, including
        // URLs, [link titles], and [fenced code block][] [info strings]:
    [TestFixture]
    public partial class TestInlinesEntityandnumericcharacterreferences
    {
        [Test]
        public void Example306()
        {
            // Example 306
            // Section: Inlines Entity and numeric character references
            //
            // The following CommonMark:
            //     <a href="&ouml;&ouml;.html">
            //
            // Should be rendered as:
            //     <a href="&ouml;&ouml;.html">

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 306, "Inlines Entity and numeric character references");
			TestParser.TestSpec("<a href=\"&ouml;&ouml;.html\">", "<a href=\"&ouml;&ouml;.html\">", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEntityandnumericcharacterreferences
    {
        [Test]
        public void Example307()
        {
            // Example 307
            // Section: Inlines Entity and numeric character references
            //
            // The following CommonMark:
            //     [foo](/f&ouml;&ouml; "f&ouml;&ouml;")
            //
            // Should be rendered as:
            //     <p><a href="/f%C3%B6%C3%B6" title="föö">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 307, "Inlines Entity and numeric character references");
			TestParser.TestSpec("[foo](/f&ouml;&ouml; \"f&ouml;&ouml;\")", "<p><a href=\"/f%C3%B6%C3%B6\" title=\"föö\">foo</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEntityandnumericcharacterreferences
    {
        [Test]
        public void Example308()
        {
            // Example 308
            // Section: Inlines Entity and numeric character references
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     [foo]: /f&ouml;&ouml; "f&ouml;&ouml;"
            //
            // Should be rendered as:
            //     <p><a href="/f%C3%B6%C3%B6" title="föö">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 308, "Inlines Entity and numeric character references");
			TestParser.TestSpec("[foo]\n\n[foo]: /f&ouml;&ouml; \"f&ouml;&ouml;\"", "<p><a href=\"/f%C3%B6%C3%B6\" title=\"föö\">foo</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEntityandnumericcharacterreferences
    {
        [Test]
        public void Example309()
        {
            // Example 309
            // Section: Inlines Entity and numeric character references
            //
            // The following CommonMark:
            //     ``` f&ouml;&ouml;
            //     foo
            //     ```
            //
            // Should be rendered as:
            //     <pre><code class="language-föö">foo
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 309, "Inlines Entity and numeric character references");
			TestParser.TestSpec("``` f&ouml;&ouml;\nfoo\n```", "<pre><code class=\"language-föö\">foo\n</code></pre>", "");
        }
    }
        // Entity and numeric character references are treated as literal
        // text in code spans and code blocks:
    [TestFixture]
    public partial class TestInlinesEntityandnumericcharacterreferences
    {
        [Test]
        public void Example310()
        {
            // Example 310
            // Section: Inlines Entity and numeric character references
            //
            // The following CommonMark:
            //     `f&ouml;&ouml;`
            //
            // Should be rendered as:
            //     <p><code>f&amp;ouml;&amp;ouml;</code></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 310, "Inlines Entity and numeric character references");
			TestParser.TestSpec("`f&ouml;&ouml;`", "<p><code>f&amp;ouml;&amp;ouml;</code></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEntityandnumericcharacterreferences
    {
        [Test]
        public void Example311()
        {
            // Example 311
            // Section: Inlines Entity and numeric character references
            //
            // The following CommonMark:
            //         f&ouml;f&ouml;
            //
            // Should be rendered as:
            //     <pre><code>f&amp;ouml;f&amp;ouml;
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 311, "Inlines Entity and numeric character references");
			TestParser.TestSpec("    f&ouml;f&ouml;", "<pre><code>f&amp;ouml;f&amp;ouml;\n</code></pre>", "");
        }
    }
        // ## Code spans
        //
        // A [backtick string](@)
        // is a string of one or more backtick characters (`` ` ``) that is neither
        // preceded nor followed by a backtick.
        //
        // A [code span](@) begins with a backtick string and ends with
        // a backtick string of equal length.  The contents of the code span are
        // the characters between the two backtick strings, with leading and
        // trailing spaces and [line endings] removed, and
        // [whitespace] collapsed to single spaces.
        //
        // This is a simple code span:
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example312()
        {
            // Example 312
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     `foo`
            //
            // Should be rendered as:
            //     <p><code>foo</code></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 312, "Inlines Code spans");
			TestParser.TestSpec("`foo`", "<p><code>foo</code></p>", "");
        }
    }
        // Here two backticks are used, because the code contains a backtick.
        // This example also illustrates stripping of leading and trailing spaces:
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example313()
        {
            // Example 313
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     `` foo ` bar  ``
            //
            // Should be rendered as:
            //     <p><code>foo ` bar</code></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 313, "Inlines Code spans");
			TestParser.TestSpec("`` foo ` bar  ``", "<p><code>foo ` bar</code></p>", "");
        }
    }
        // This example shows the motivation for stripping leading and trailing
        // spaces:
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example314()
        {
            // Example 314
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     ` `` `
            //
            // Should be rendered as:
            //     <p><code>``</code></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 314, "Inlines Code spans");
			TestParser.TestSpec("` `` `", "<p><code>``</code></p>", "");
        }
    }
        // [Line endings] are treated like spaces:
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example315()
        {
            // Example 315
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     ``
            //     foo
            //     ``
            //
            // Should be rendered as:
            //     <p><code>foo</code></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 315, "Inlines Code spans");
			TestParser.TestSpec("``\nfoo\n``", "<p><code>foo</code></p>", "");
        }
    }
        // Interior spaces and [line endings] are collapsed into
        // single spaces, just as they would be by a browser:
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example316()
        {
            // Example 316
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     `foo   bar
            //       baz`
            //
            // Should be rendered as:
            //     <p><code>foo bar baz</code></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 316, "Inlines Code spans");
			TestParser.TestSpec("`foo   bar\n  baz`", "<p><code>foo bar baz</code></p>", "");
        }
    }
        // Q: Why not just leave the spaces, since browsers will collapse them
        // anyway?  A:  Because we might be targeting a non-HTML format, and we
        // shouldn't rely on HTML-specific rendering assumptions.
        //
        // (Existing implementations differ in their treatment of internal
        // spaces and [line endings].  Some, including `Markdown.pl` and
        // `showdown`, convert an internal [line ending] into a
        // `<br />` tag.  But this makes things difficult for those who like to
        // hard-wrap their paragraphs, since a line break in the midst of a code
        // span will cause an unintended line break in the output.  Others just
        // leave internal spaces as they are, which is fine if only HTML is being
        // targeted.)
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example317()
        {
            // Example 317
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     `foo `` bar`
            //
            // Should be rendered as:
            //     <p><code>foo `` bar</code></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 317, "Inlines Code spans");
			TestParser.TestSpec("`foo `` bar`", "<p><code>foo `` bar</code></p>", "");
        }
    }
        // Note that backslash escapes do not work in code spans. All backslashes
        // are treated literally:
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example318()
        {
            // Example 318
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     `foo\`bar`
            //
            // Should be rendered as:
            //     <p><code>foo\</code>bar`</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 318, "Inlines Code spans");
			TestParser.TestSpec("`foo\\`bar`", "<p><code>foo\\</code>bar`</p>", "");
        }
    }
        // Backslash escapes are never needed, because one can always choose a
        // string of *n* backtick characters as delimiters, where the code does
        // not contain any strings of exactly *n* backtick characters.
        //
        // Code span backticks have higher precedence than any other inline
        // constructs except HTML tags and autolinks.  Thus, for example, this is
        // not parsed as emphasized text, since the second `*` is part of a code
        // span:
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example319()
        {
            // Example 319
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     *foo`*`
            //
            // Should be rendered as:
            //     <p>*foo<code>*</code></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 319, "Inlines Code spans");
			TestParser.TestSpec("*foo`*`", "<p>*foo<code>*</code></p>", "");
        }
    }
        // And this is not parsed as a link:
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example320()
        {
            // Example 320
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     [not a `link](/foo`)
            //
            // Should be rendered as:
            //     <p>[not a <code>link](/foo</code>)</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 320, "Inlines Code spans");
			TestParser.TestSpec("[not a `link](/foo`)", "<p>[not a <code>link](/foo</code>)</p>", "");
        }
    }
        // Code spans, HTML tags, and autolinks have the same precedence.
        // Thus, this is code:
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example321()
        {
            // Example 321
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     `<a href="`">`
            //
            // Should be rendered as:
            //     <p><code>&lt;a href=&quot;</code>&quot;&gt;`</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 321, "Inlines Code spans");
			TestParser.TestSpec("`<a href=\"`\">`", "<p><code>&lt;a href=&quot;</code>&quot;&gt;`</p>", "");
        }
    }
        // But this is an HTML tag:
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example322()
        {
            // Example 322
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     <a href="`">`
            //
            // Should be rendered as:
            //     <p><a href="`">`</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 322, "Inlines Code spans");
			TestParser.TestSpec("<a href=\"`\">`", "<p><a href=\"`\">`</p>", "");
        }
    }
        // And this is code:
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example323()
        {
            // Example 323
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     `<http://foo.bar.`baz>`
            //
            // Should be rendered as:
            //     <p><code>&lt;http://foo.bar.</code>baz&gt;`</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 323, "Inlines Code spans");
			TestParser.TestSpec("`<http://foo.bar.`baz>`", "<p><code>&lt;http://foo.bar.</code>baz&gt;`</p>", "");
        }
    }
        // But this is an autolink:
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example324()
        {
            // Example 324
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     <http://foo.bar.`baz>`
            //
            // Should be rendered as:
            //     <p><a href="http://foo.bar.%60baz">http://foo.bar.`baz</a>`</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 324, "Inlines Code spans");
			TestParser.TestSpec("<http://foo.bar.`baz>`", "<p><a href=\"http://foo.bar.%60baz\">http://foo.bar.`baz</a>`</p>", "");
        }
    }
        // When a backtick string is not closed by a matching backtick string,
        // we just have literal backticks:
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example325()
        {
            // Example 325
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     ```foo``
            //
            // Should be rendered as:
            //     <p>```foo``</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 325, "Inlines Code spans");
			TestParser.TestSpec("```foo``", "<p>```foo``</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesCodespans
    {
        [Test]
        public void Example326()
        {
            // Example 326
            // Section: Inlines Code spans
            //
            // The following CommonMark:
            //     `foo
            //
            // Should be rendered as:
            //     <p>`foo</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 326, "Inlines Code spans");
			TestParser.TestSpec("`foo", "<p>`foo</p>", "");
        }
    }
        // ## Emphasis and strong emphasis
        //
        // John Gruber's original [Markdown syntax
        // description](http://daringfireball.net/projects/markdown/syntax#em) says:
        //
        // > Markdown treats asterisks (`*`) and underscores (`_`) as indicators of
        // > emphasis. Text wrapped with one `*` or `_` will be wrapped with an HTML
        // > `<em>` tag; double `*`'s or `_`'s will be wrapped with an HTML `<strong>`
        // > tag.
        //
        // This is enough for most users, but these rules leave much undecided,
        // especially when it comes to nested emphasis.  The original
        // `Markdown.pl` test suite makes it clear that triple `***` and
        // `___` delimiters can be used for strong emphasis, and most
        // implementations have also allowed the following patterns:
        //
        // ``` markdown
        // ***strong emph***
        // ***strong** in emph*
        // ***emph* in strong**
        // **in strong *emph***
        // *in emph **strong***
        // ```
        //
        // The following patterns are less widely supported, but the intent
        // is clear and they are useful (especially in contexts like bibliography
        // entries):
        //
        // ``` markdown
        // *emph *with emph* in it*
        // **strong **with strong** in it**
        // ```
        //
        // Many implementations have also restricted intraword emphasis to
        // the `*` forms, to avoid unwanted emphasis in words containing
        // internal underscores.  (It is best practice to put these in code
        // spans, but users often do not.)
        //
        // ``` markdown
        // internal emphasis: foo*bar*baz
        // no emphasis: foo_bar_baz
        // ```
        //
        // The rules given below capture all of these patterns, while allowing
        // for efficient parsing strategies that do not backtrack.
        //
        // First, some definitions.  A [delimiter run](@) is either
        // a sequence of one or more `*` characters that is not preceded or
        // followed by a `*` character, or a sequence of one or more `_`
        // characters that is not preceded or followed by a `_` character.
        //
        // A [left-flanking delimiter run](@) is
        // a [delimiter run] that is (a) not followed by [Unicode whitespace],
        // and (b) either not followed by a [punctuation character], or
        // preceded by [Unicode whitespace] or a [punctuation character].
        // For purposes of this definition, the beginning and the end of
        // the line count as Unicode whitespace.
        //
        // A [right-flanking delimiter run](@) is
        // a [delimiter run] that is (a) not preceded by [Unicode whitespace],
        // and (b) either not preceded by a [punctuation character], or
        // followed by [Unicode whitespace] or a [punctuation character].
        // For purposes of this definition, the beginning and the end of
        // the line count as Unicode whitespace.
        //
        // Here are some examples of delimiter runs.
        //
        // - left-flanking but not right-flanking:
        //
        // ```
        // ***abc
        // _abc
        // **"abc"
        // _"abc"
        // ```
        //
        // - right-flanking but not left-flanking:
        //
        // ```
        // abc***
        // abc_
        // "abc"**
        // "abc"_
        // ```
        //
        // - Both left and right-flanking:
        //
        // ```
        // abc***def
        // "abc"_"def"
        // ```
        //
        // - Neither left nor right-flanking:
        //
        // ```
        // abc *** def
        // a _ b
        // ```
        //
        // (The idea of distinguishing left-flanking and right-flanking
        // delimiter runs based on the character before and the character
        // after comes from Roopesh Chander's
        // [vfmd](http://www.vfmd.org/vfmd-spec/specification/#procedure-for-identifying-emphasis-tags).
        // vfmd uses the terminology "emphasis indicator string" instead of "delimiter
        // run," and its rules for distinguishing left- and right-flanking runs
        // are a bit more complex than the ones given here.)
        //
        // The following rules define emphasis and strong emphasis:
        //
        // 1.  A single `*` character [can open emphasis](@)
        // iff (if and only if) it is part of a [left-flanking delimiter run].
        //
        // 2.  A single `_` character [can open emphasis] iff
        // it is part of a [left-flanking delimiter run]
        // and either (a) not part of a [right-flanking delimiter run]
        // or (b) part of a [right-flanking delimiter run]
        // preceded by punctuation.
        //
        // 3.  A single `*` character [can close emphasis](@)
        // iff it is part of a [right-flanking delimiter run].
        //
        // 4.  A single `_` character [can close emphasis] iff
        // it is part of a [right-flanking delimiter run]
        // and either (a) not part of a [left-flanking delimiter run]
        // or (b) part of a [left-flanking delimiter run]
        // followed by punctuation.
        //
        // 5.  A double `**` [can open strong emphasis](@)
        // iff it is part of a [left-flanking delimiter run].
        //
        // 6.  A double `__` [can open strong emphasis] iff
        // it is part of a [left-flanking delimiter run]
        // and either (a) not part of a [right-flanking delimiter run]
        // or (b) part of a [right-flanking delimiter run]
        // preceded by punctuation.
        //
        // 7.  A double `**` [can close strong emphasis](@)
        // iff it is part of a [right-flanking delimiter run].
        //
        // 8.  A double `__` [can close strong emphasis]
        // it is part of a [right-flanking delimiter run]
        // and either (a) not part of a [left-flanking delimiter run]
        // or (b) part of a [left-flanking delimiter run]
        // followed by punctuation.
        //
        // 9.  Emphasis begins with a delimiter that [can open emphasis] and ends
        // with a delimiter that [can close emphasis], and that uses the same
        // character (`_` or `*`) as the opening delimiter.  The
        // opening and closing delimiters must belong to separate
        // [delimiter runs].  If one of the delimiters can both
        // open and close emphasis, then the sum of the lengths of the
        // delimiter runs containing the opening and closing delimiters
        // must not be a multiple of 3.
        //
        // 10. Strong emphasis begins with a delimiter that
        // [can open strong emphasis] and ends with a delimiter that
        // [can close strong emphasis], and that uses the same character
        // (`_` or `*`) as the opening delimiter.  The
        // opening and closing delimiters must belong to separate
        // [delimiter runs].  If one of the delimiters can both open
        // and close strong emphasis, then the sum of the lengths of
        // the delimiter runs containing the opening and closing
        // delimiters must not be a multiple of 3.
        //
        // 11. A literal `*` character cannot occur at the beginning or end of
        // `*`-delimited emphasis or `**`-delimited strong emphasis, unless it
        // is backslash-escaped.
        //
        // 12. A literal `_` character cannot occur at the beginning or end of
        // `_`-delimited emphasis or `__`-delimited strong emphasis, unless it
        // is backslash-escaped.
        //
        // Where rules 1--12 above are compatible with multiple parsings,
        // the following principles resolve ambiguity:
        //
        // 13. The number of nestings should be minimized. Thus, for example,
        // an interpretation `<strong>...</strong>` is always preferred to
        // `<em><em>...</em></em>`.
        //
        // 14. An interpretation `<strong><em>...</em></strong>` is always
        // preferred to `<em><strong>..</strong></em>`.
        //
        // 15. When two potential emphasis or strong emphasis spans overlap,
        // so that the second begins before the first ends and ends after
        // the first ends, the first takes precedence. Thus, for example,
        // `*foo _bar* baz_` is parsed as `<em>foo _bar</em> baz_` rather
        // than `*foo <em>bar* baz</em>`.
        //
        // 16. When there are two potential emphasis or strong emphasis spans
        // with the same closing delimiter, the shorter one (the one that
        // opens later) takes precedence. Thus, for example,
        // `**foo **bar baz**` is parsed as `**foo <strong>bar baz</strong>`
        // rather than `<strong>foo **bar baz</strong>`.
        //
        // 17. Inline code spans, links, images, and HTML tags group more tightly
        // than emphasis.  So, when there is a choice between an interpretation
        // that contains one of these elements and one that does not, the
        // former always wins.  Thus, for example, `*[foo*](bar)` is
        // parsed as `*<a href="bar">foo*</a>` rather than as
        // `<em>[foo</em>](bar)`.
        //
        // These rules can be illustrated through a series of examples.
        //
        // Rule 1:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example327()
        {
            // Example 327
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo bar*
            //
            // Should be rendered as:
            //     <p><em>foo bar</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 327, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo bar*", "<p><em>foo bar</em></p>", "");
        }
    }
        // This is not emphasis, because the opening `*` is followed by
        // whitespace, and hence not part of a [left-flanking delimiter run]:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example328()
        {
            // Example 328
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     a * foo bar*
            //
            // Should be rendered as:
            //     <p>a * foo bar*</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 328, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("a * foo bar*", "<p>a * foo bar*</p>", "");
        }
    }
        // This is not emphasis, because the opening `*` is preceded
        // by an alphanumeric and followed by punctuation, and hence
        // not part of a [left-flanking delimiter run]:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example329()
        {
            // Example 329
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     a*"foo"*
            //
            // Should be rendered as:
            //     <p>a*&quot;foo&quot;*</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 329, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("a*\"foo\"*", "<p>a*&quot;foo&quot;*</p>", "");
        }
    }
        // Unicode nonbreaking spaces count as whitespace, too:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example330()
        {
            // Example 330
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     * a *
            //
            // Should be rendered as:
            //     <p>* a *</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 330, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("* a *", "<p>* a *</p>", "");
        }
    }
        // Intraword emphasis with `*` is permitted:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example331()
        {
            // Example 331
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo*bar*
            //
            // Should be rendered as:
            //     <p>foo<em>bar</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 331, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo*bar*", "<p>foo<em>bar</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example332()
        {
            // Example 332
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     5*6*78
            //
            // Should be rendered as:
            //     <p>5<em>6</em>78</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 332, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("5*6*78", "<p>5<em>6</em>78</p>", "");
        }
    }
        // Rule 2:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example333()
        {
            // Example 333
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo bar_
            //
            // Should be rendered as:
            //     <p><em>foo bar</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 333, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_foo bar_", "<p><em>foo bar</em></p>", "");
        }
    }
        // This is not emphasis, because the opening `_` is followed by
        // whitespace:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example334()
        {
            // Example 334
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _ foo bar_
            //
            // Should be rendered as:
            //     <p>_ foo bar_</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 334, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_ foo bar_", "<p>_ foo bar_</p>", "");
        }
    }
        // This is not emphasis, because the opening `_` is preceded
        // by an alphanumeric and followed by punctuation:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example335()
        {
            // Example 335
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     a_"foo"_
            //
            // Should be rendered as:
            //     <p>a_&quot;foo&quot;_</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 335, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("a_\"foo\"_", "<p>a_&quot;foo&quot;_</p>", "");
        }
    }
        // Emphasis with `_` is not allowed inside words:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example336()
        {
            // Example 336
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo_bar_
            //
            // Should be rendered as:
            //     <p>foo_bar_</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 336, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo_bar_", "<p>foo_bar_</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example337()
        {
            // Example 337
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     5_6_78
            //
            // Should be rendered as:
            //     <p>5_6_78</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 337, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("5_6_78", "<p>5_6_78</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example338()
        {
            // Example 338
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     пристаням_стремятся_
            //
            // Should be rendered as:
            //     <p>пристаням_стремятся_</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 338, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("пристаням_стремятся_", "<p>пристаням_стремятся_</p>", "");
        }
    }
        // Here `_` does not generate emphasis, because the first delimiter run
        // is right-flanking and the second left-flanking:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example339()
        {
            // Example 339
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     aa_"bb"_cc
            //
            // Should be rendered as:
            //     <p>aa_&quot;bb&quot;_cc</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 339, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("aa_\"bb\"_cc", "<p>aa_&quot;bb&quot;_cc</p>", "");
        }
    }
        // This is emphasis, even though the opening delimiter is
        // both left- and right-flanking, because it is preceded by
        // punctuation:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example340()
        {
            // Example 340
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo-_(bar)_
            //
            // Should be rendered as:
            //     <p>foo-<em>(bar)</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 340, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo-_(bar)_", "<p>foo-<em>(bar)</em></p>", "");
        }
    }
        // Rule 3:
        //
        // This is not emphasis, because the closing delimiter does
        // not match the opening delimiter:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example341()
        {
            // Example 341
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo*
            //
            // Should be rendered as:
            //     <p>_foo*</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 341, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_foo*", "<p>_foo*</p>", "");
        }
    }
        // This is not emphasis, because the closing `*` is preceded by
        // whitespace:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example342()
        {
            // Example 342
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo bar *
            //
            // Should be rendered as:
            //     <p>*foo bar *</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 342, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo bar *", "<p>*foo bar *</p>", "");
        }
    }
        // A newline also counts as whitespace:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example343()
        {
            // Example 343
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo bar
            //     *
            //
            // Should be rendered as:
            //     <p>*foo bar</p>
            //     <ul>
            //     <li></li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 343, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo bar\n*", "<p>*foo bar</p>\n<ul>\n<li></li>\n</ul>", "");
        }
    }
        // This is not emphasis, because the second `*` is
        // preceded by punctuation and followed by an alphanumeric
        // (hence it is not part of a [right-flanking delimiter run]:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example344()
        {
            // Example 344
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *(*foo)
            //
            // Should be rendered as:
            //     <p>*(*foo)</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 344, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*(*foo)", "<p>*(*foo)</p>", "");
        }
    }
        // The point of this restriction is more easily appreciated
        // with this example:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example345()
        {
            // Example 345
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *(*foo*)*
            //
            // Should be rendered as:
            //     <p><em>(<em>foo</em>)</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 345, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*(*foo*)*", "<p><em>(<em>foo</em>)</em></p>", "");
        }
    }
        // Intraword emphasis with `*` is allowed:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example346()
        {
            // Example 346
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo*bar
            //
            // Should be rendered as:
            //     <p><em>foo</em>bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 346, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo*bar", "<p><em>foo</em>bar</p>", "");
        }
    }
        // Rule 4:
        //
        // This is not emphasis, because the closing `_` is preceded by
        // whitespace:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example347()
        {
            // Example 347
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo bar _
            //
            // Should be rendered as:
            //     <p>_foo bar _</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 347, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_foo bar _", "<p>_foo bar _</p>", "");
        }
    }
        // This is not emphasis, because the second `_` is
        // preceded by punctuation and followed by an alphanumeric:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example348()
        {
            // Example 348
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _(_foo)
            //
            // Should be rendered as:
            //     <p>_(_foo)</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 348, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_(_foo)", "<p>_(_foo)</p>", "");
        }
    }
        // This is emphasis within emphasis:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example349()
        {
            // Example 349
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _(_foo_)_
            //
            // Should be rendered as:
            //     <p><em>(<em>foo</em>)</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 349, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_(_foo_)_", "<p><em>(<em>foo</em>)</em></p>", "");
        }
    }
        // Intraword emphasis is disallowed for `_`:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example350()
        {
            // Example 350
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo_bar
            //
            // Should be rendered as:
            //     <p>_foo_bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 350, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_foo_bar", "<p>_foo_bar</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example351()
        {
            // Example 351
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _пристаням_стремятся
            //
            // Should be rendered as:
            //     <p>_пристаням_стремятся</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 351, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_пристаням_стремятся", "<p>_пристаням_стремятся</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example352()
        {
            // Example 352
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo_bar_baz_
            //
            // Should be rendered as:
            //     <p><em>foo_bar_baz</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 352, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_foo_bar_baz_", "<p><em>foo_bar_baz</em></p>", "");
        }
    }
        // This is emphasis, even though the closing delimiter is
        // both left- and right-flanking, because it is followed by
        // punctuation:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example353()
        {
            // Example 353
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _(bar)_.
            //
            // Should be rendered as:
            //     <p><em>(bar)</em>.</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 353, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_(bar)_.", "<p><em>(bar)</em>.</p>", "");
        }
    }
        // Rule 5:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example354()
        {
            // Example 354
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo bar**
            //
            // Should be rendered as:
            //     <p><strong>foo bar</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 354, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo bar**", "<p><strong>foo bar</strong></p>", "");
        }
    }
        // This is not strong emphasis, because the opening delimiter is
        // followed by whitespace:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example355()
        {
            // Example 355
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ** foo bar**
            //
            // Should be rendered as:
            //     <p>** foo bar**</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 355, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("** foo bar**", "<p>** foo bar**</p>", "");
        }
    }
        // This is not strong emphasis, because the opening `**` is preceded
        // by an alphanumeric and followed by punctuation, and hence
        // not part of a [left-flanking delimiter run]:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example356()
        {
            // Example 356
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     a**"foo"**
            //
            // Should be rendered as:
            //     <p>a**&quot;foo&quot;**</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 356, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("a**\"foo\"**", "<p>a**&quot;foo&quot;**</p>", "");
        }
    }
        // Intraword strong emphasis with `**` is permitted:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example357()
        {
            // Example 357
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo**bar**
            //
            // Should be rendered as:
            //     <p>foo<strong>bar</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 357, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo**bar**", "<p>foo<strong>bar</strong></p>", "");
        }
    }
        // Rule 6:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example358()
        {
            // Example 358
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo bar__
            //
            // Should be rendered as:
            //     <p><strong>foo bar</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 358, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__foo bar__", "<p><strong>foo bar</strong></p>", "");
        }
    }
        // This is not strong emphasis, because the opening delimiter is
        // followed by whitespace:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example359()
        {
            // Example 359
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __ foo bar__
            //
            // Should be rendered as:
            //     <p>__ foo bar__</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 359, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__ foo bar__", "<p>__ foo bar__</p>", "");
        }
    }
        // A newline counts as whitespace:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example360()
        {
            // Example 360
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __
            //     foo bar__
            //
            // Should be rendered as:
            //     <p>__
            //     foo bar__</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 360, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__\nfoo bar__", "<p>__\nfoo bar__</p>", "");
        }
    }
        // This is not strong emphasis, because the opening `__` is preceded
        // by an alphanumeric and followed by punctuation:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example361()
        {
            // Example 361
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     a__"foo"__
            //
            // Should be rendered as:
            //     <p>a__&quot;foo&quot;__</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 361, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("a__\"foo\"__", "<p>a__&quot;foo&quot;__</p>", "");
        }
    }
        // Intraword strong emphasis is forbidden with `__`:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example362()
        {
            // Example 362
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo__bar__
            //
            // Should be rendered as:
            //     <p>foo__bar__</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 362, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo__bar__", "<p>foo__bar__</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example363()
        {
            // Example 363
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     5__6__78
            //
            // Should be rendered as:
            //     <p>5__6__78</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 363, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("5__6__78", "<p>5__6__78</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example364()
        {
            // Example 364
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     пристаням__стремятся__
            //
            // Should be rendered as:
            //     <p>пристаням__стремятся__</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 364, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("пристаням__стремятся__", "<p>пристаням__стремятся__</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example365()
        {
            // Example 365
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo, __bar__, baz__
            //
            // Should be rendered as:
            //     <p><strong>foo, <strong>bar</strong>, baz</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 365, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__foo, __bar__, baz__", "<p><strong>foo, <strong>bar</strong>, baz</strong></p>", "");
        }
    }
        // This is strong emphasis, even though the opening delimiter is
        // both left- and right-flanking, because it is preceded by
        // punctuation:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example366()
        {
            // Example 366
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo-__(bar)__
            //
            // Should be rendered as:
            //     <p>foo-<strong>(bar)</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 366, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo-__(bar)__", "<p>foo-<strong>(bar)</strong></p>", "");
        }
    }
        // Rule 7:
        //
        // This is not strong emphasis, because the closing delimiter is preceded
        // by whitespace:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example367()
        {
            // Example 367
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo bar **
            //
            // Should be rendered as:
            //     <p>**foo bar **</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 367, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo bar **", "<p>**foo bar **</p>", "");
        }
    }
        // (Nor can it be interpreted as an emphasized `*foo bar *`, because of
        // Rule 11.)
        //
        // This is not strong emphasis, because the second `**` is
        // preceded by punctuation and followed by an alphanumeric:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example368()
        {
            // Example 368
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **(**foo)
            //
            // Should be rendered as:
            //     <p>**(**foo)</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 368, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**(**foo)", "<p>**(**foo)</p>", "");
        }
    }
        // The point of this restriction is more easily appreciated
        // with these examples:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example369()
        {
            // Example 369
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *(**foo**)*
            //
            // Should be rendered as:
            //     <p><em>(<strong>foo</strong>)</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 369, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*(**foo**)*", "<p><em>(<strong>foo</strong>)</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example370()
        {
            // Example 370
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **Gomphocarpus (*Gomphocarpus physocarpus*, syn.
            //     *Asclepias physocarpa*)**
            //
            // Should be rendered as:
            //     <p><strong>Gomphocarpus (<em>Gomphocarpus physocarpus</em>, syn.
            //     <em>Asclepias physocarpa</em>)</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 370, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**Gomphocarpus (*Gomphocarpus physocarpus*, syn.\n*Asclepias physocarpa*)**", "<p><strong>Gomphocarpus (<em>Gomphocarpus physocarpus</em>, syn.\n<em>Asclepias physocarpa</em>)</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example371()
        {
            // Example 371
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo "*bar*" foo**
            //
            // Should be rendered as:
            //     <p><strong>foo &quot;<em>bar</em>&quot; foo</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 371, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo \"*bar*\" foo**", "<p><strong>foo &quot;<em>bar</em>&quot; foo</strong></p>", "");
        }
    }
        // Intraword emphasis:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example372()
        {
            // Example 372
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo**bar
            //
            // Should be rendered as:
            //     <p><strong>foo</strong>bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 372, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo**bar", "<p><strong>foo</strong>bar</p>", "");
        }
    }
        // Rule 8:
        //
        // This is not strong emphasis, because the closing delimiter is
        // preceded by whitespace:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example373()
        {
            // Example 373
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo bar __
            //
            // Should be rendered as:
            //     <p>__foo bar __</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 373, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__foo bar __", "<p>__foo bar __</p>", "");
        }
    }
        // This is not strong emphasis, because the second `__` is
        // preceded by punctuation and followed by an alphanumeric:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example374()
        {
            // Example 374
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __(__foo)
            //
            // Should be rendered as:
            //     <p>__(__foo)</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 374, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__(__foo)", "<p>__(__foo)</p>", "");
        }
    }
        // The point of this restriction is more easily appreciated
        // with this example:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example375()
        {
            // Example 375
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _(__foo__)_
            //
            // Should be rendered as:
            //     <p><em>(<strong>foo</strong>)</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 375, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_(__foo__)_", "<p><em>(<strong>foo</strong>)</em></p>", "");
        }
    }
        // Intraword strong emphasis is forbidden with `__`:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example376()
        {
            // Example 376
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo__bar
            //
            // Should be rendered as:
            //     <p>__foo__bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 376, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__foo__bar", "<p>__foo__bar</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example377()
        {
            // Example 377
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __пристаням__стремятся
            //
            // Should be rendered as:
            //     <p>__пристаням__стремятся</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 377, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__пристаням__стремятся", "<p>__пристаням__стремятся</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example378()
        {
            // Example 378
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo__bar__baz__
            //
            // Should be rendered as:
            //     <p><strong>foo__bar__baz</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 378, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__foo__bar__baz__", "<p><strong>foo__bar__baz</strong></p>", "");
        }
    }
        // This is strong emphasis, even though the closing delimiter is
        // both left- and right-flanking, because it is followed by
        // punctuation:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example379()
        {
            // Example 379
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __(bar)__.
            //
            // Should be rendered as:
            //     <p><strong>(bar)</strong>.</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 379, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__(bar)__.", "<p><strong>(bar)</strong>.</p>", "");
        }
    }
        // Rule 9:
        //
        // Any nonempty sequence of inline elements can be the contents of an
        // emphasized span.
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example380()
        {
            // Example 380
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo [bar](/url)*
            //
            // Should be rendered as:
            //     <p><em>foo <a href="/url">bar</a></em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 380, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo [bar](/url)*", "<p><em>foo <a href=\"/url\">bar</a></em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example381()
        {
            // Example 381
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo
            //     bar*
            //
            // Should be rendered as:
            //     <p><em>foo
            //     bar</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 381, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo\nbar*", "<p><em>foo\nbar</em></p>", "");
        }
    }
        // In particular, emphasis and strong emphasis can be nested
        // inside emphasis:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example382()
        {
            // Example 382
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo __bar__ baz_
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar</strong> baz</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 382, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_foo __bar__ baz_", "<p><em>foo <strong>bar</strong> baz</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example383()
        {
            // Example 383
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo _bar_ baz_
            //
            // Should be rendered as:
            //     <p><em>foo <em>bar</em> baz</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 383, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_foo _bar_ baz_", "<p><em>foo <em>bar</em> baz</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example384()
        {
            // Example 384
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo_ bar_
            //
            // Should be rendered as:
            //     <p><em><em>foo</em> bar</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 384, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__foo_ bar_", "<p><em><em>foo</em> bar</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example385()
        {
            // Example 385
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo *bar**
            //
            // Should be rendered as:
            //     <p><em>foo <em>bar</em></em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 385, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo *bar**", "<p><em>foo <em>bar</em></em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example386()
        {
            // Example 386
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo **bar** baz*
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar</strong> baz</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 386, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo **bar** baz*", "<p><em>foo <strong>bar</strong> baz</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example387()
        {
            // Example 387
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo**bar**baz*
            //
            // Should be rendered as:
            //     <p><em>foo<strong>bar</strong>baz</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 387, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo**bar**baz*", "<p><em>foo<strong>bar</strong>baz</em></p>", "");
        }
    }
        // Note that in the preceding case, the interpretation
        //
        // ``` markdown
        // <p><em>foo</em><em>bar<em></em>baz</em></p>
        // ```
        //
        // is precluded by the condition that a delimiter that
        // can both open and close (like the `*` after `foo`
        // cannot form emphasis if the sum of the lengths of
        // the delimiter runs containing the opening and
        // closing delimiters is a multiple of 3.
        //
        // The same condition ensures that the following
        // cases are all strong emphasis nested inside
        // emphasis, even when the interior spaces are
        // omitted:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example388()
        {
            // Example 388
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo** bar*
            //
            // Should be rendered as:
            //     <p><em><strong>foo</strong> bar</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 388, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("***foo** bar*", "<p><em><strong>foo</strong> bar</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example389()
        {
            // Example 389
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo **bar***
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar</strong></em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 389, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo **bar***", "<p><em>foo <strong>bar</strong></em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example390()
        {
            // Example 390
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo**bar***
            //
            // Should be rendered as:
            //     <p><em>foo<strong>bar</strong></em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 390, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo**bar***", "<p><em>foo<strong>bar</strong></em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example391()
        {
            // Example 391
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo**bar***
            //
            // Should be rendered as:
            //     <p><em>foo<strong>bar</strong></em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 391, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo**bar***", "<p><em>foo<strong>bar</strong></em></p>", "");
        }
    }
        // Indefinite levels of nesting are possible:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example392()
        {
            // Example 392
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo **bar *baz* bim** bop*
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar <em>baz</em> bim</strong> bop</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 392, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo **bar *baz* bim** bop*", "<p><em>foo <strong>bar <em>baz</em> bim</strong> bop</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example393()
        {
            // Example 393
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo [*bar*](/url)*
            //
            // Should be rendered as:
            //     <p><em>foo <a href="/url"><em>bar</em></a></em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 393, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo [*bar*](/url)*", "<p><em>foo <a href=\"/url\"><em>bar</em></a></em></p>", "");
        }
    }
        // There can be no empty emphasis or strong emphasis:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example394()
        {
            // Example 394
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ** is not an empty emphasis
            //
            // Should be rendered as:
            //     <p>** is not an empty emphasis</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 394, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("** is not an empty emphasis", "<p>** is not an empty emphasis</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example395()
        {
            // Example 395
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **** is not an empty strong emphasis
            //
            // Should be rendered as:
            //     <p>**** is not an empty strong emphasis</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 395, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**** is not an empty strong emphasis", "<p>**** is not an empty strong emphasis</p>", "");
        }
    }
        // Rule 10:
        //
        // Any nonempty sequence of inline elements can be the contents of an
        // strongly emphasized span.
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example396()
        {
            // Example 396
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo [bar](/url)**
            //
            // Should be rendered as:
            //     <p><strong>foo <a href="/url">bar</a></strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 396, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo [bar](/url)**", "<p><strong>foo <a href=\"/url\">bar</a></strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example397()
        {
            // Example 397
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo
            //     bar**
            //
            // Should be rendered as:
            //     <p><strong>foo
            //     bar</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 397, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo\nbar**", "<p><strong>foo\nbar</strong></p>", "");
        }
    }
        // In particular, emphasis and strong emphasis can be nested
        // inside strong emphasis:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example398()
        {
            // Example 398
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo _bar_ baz__
            //
            // Should be rendered as:
            //     <p><strong>foo <em>bar</em> baz</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 398, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__foo _bar_ baz__", "<p><strong>foo <em>bar</em> baz</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example399()
        {
            // Example 399
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo __bar__ baz__
            //
            // Should be rendered as:
            //     <p><strong>foo <strong>bar</strong> baz</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 399, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__foo __bar__ baz__", "<p><strong>foo <strong>bar</strong> baz</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example400()
        {
            // Example 400
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ____foo__ bar__
            //
            // Should be rendered as:
            //     <p><strong><strong>foo</strong> bar</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 400, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("____foo__ bar__", "<p><strong><strong>foo</strong> bar</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example401()
        {
            // Example 401
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo **bar****
            //
            // Should be rendered as:
            //     <p><strong>foo <strong>bar</strong></strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 401, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo **bar****", "<p><strong>foo <strong>bar</strong></strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example402()
        {
            // Example 402
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo *bar* baz**
            //
            // Should be rendered as:
            //     <p><strong>foo <em>bar</em> baz</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 402, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo *bar* baz**", "<p><strong>foo <em>bar</em> baz</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example403()
        {
            // Example 403
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo*bar*baz**
            //
            // Should be rendered as:
            //     <p><strong>foo<em>bar</em>baz</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 403, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo*bar*baz**", "<p><strong>foo<em>bar</em>baz</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example404()
        {
            // Example 404
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo* bar**
            //
            // Should be rendered as:
            //     <p><strong><em>foo</em> bar</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 404, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("***foo* bar**", "<p><strong><em>foo</em> bar</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example405()
        {
            // Example 405
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo *bar***
            //
            // Should be rendered as:
            //     <p><strong>foo <em>bar</em></strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 405, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo *bar***", "<p><strong>foo <em>bar</em></strong></p>", "");
        }
    }
        // Indefinite levels of nesting are possible:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example406()
        {
            // Example 406
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo *bar **baz**
            //     bim* bop**
            //
            // Should be rendered as:
            //     <p><strong>foo <em>bar <strong>baz</strong>
            //     bim</em> bop</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 406, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo *bar **baz**\nbim* bop**", "<p><strong>foo <em>bar <strong>baz</strong>\nbim</em> bop</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example407()
        {
            // Example 407
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo [*bar*](/url)**
            //
            // Should be rendered as:
            //     <p><strong>foo <a href="/url"><em>bar</em></a></strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 407, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo [*bar*](/url)**", "<p><strong>foo <a href=\"/url\"><em>bar</em></a></strong></p>", "");
        }
    }
        // There can be no empty emphasis or strong emphasis:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example408()
        {
            // Example 408
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __ is not an empty emphasis
            //
            // Should be rendered as:
            //     <p>__ is not an empty emphasis</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 408, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__ is not an empty emphasis", "<p>__ is not an empty emphasis</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example409()
        {
            // Example 409
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ____ is not an empty strong emphasis
            //
            // Should be rendered as:
            //     <p>____ is not an empty strong emphasis</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 409, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("____ is not an empty strong emphasis", "<p>____ is not an empty strong emphasis</p>", "");
        }
    }
        // Rule 11:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example410()
        {
            // Example 410
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo ***
            //
            // Should be rendered as:
            //     <p>foo ***</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 410, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo ***", "<p>foo ***</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example411()
        {
            // Example 411
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo *\**
            //
            // Should be rendered as:
            //     <p>foo <em>*</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 411, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo *\\**", "<p>foo <em>*</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example412()
        {
            // Example 412
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo *_*
            //
            // Should be rendered as:
            //     <p>foo <em>_</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 412, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo *_*", "<p>foo <em>_</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example413()
        {
            // Example 413
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo *****
            //
            // Should be rendered as:
            //     <p>foo *****</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 413, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo *****", "<p>foo *****</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example414()
        {
            // Example 414
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo **\***
            //
            // Should be rendered as:
            //     <p>foo <strong>*</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 414, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo **\\***", "<p>foo <strong>*</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example415()
        {
            // Example 415
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo **_**
            //
            // Should be rendered as:
            //     <p>foo <strong>_</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 415, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo **_**", "<p>foo <strong>_</strong></p>", "");
        }
    }
        // Note that when delimiters do not match evenly, Rule 11 determines
        // that the excess literal `*` characters will appear outside of the
        // emphasis, rather than inside it:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example416()
        {
            // Example 416
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo*
            //
            // Should be rendered as:
            //     <p>*<em>foo</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 416, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo*", "<p>*<em>foo</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example417()
        {
            // Example 417
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo**
            //
            // Should be rendered as:
            //     <p><em>foo</em>*</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 417, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo**", "<p><em>foo</em>*</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example418()
        {
            // Example 418
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo**
            //
            // Should be rendered as:
            //     <p>*<strong>foo</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 418, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("***foo**", "<p>*<strong>foo</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example419()
        {
            // Example 419
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ****foo*
            //
            // Should be rendered as:
            //     <p>***<em>foo</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 419, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("****foo*", "<p>***<em>foo</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example420()
        {
            // Example 420
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo***
            //
            // Should be rendered as:
            //     <p><strong>foo</strong>*</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 420, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo***", "<p><strong>foo</strong>*</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example421()
        {
            // Example 421
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo****
            //
            // Should be rendered as:
            //     <p><em>foo</em>***</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 421, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo****", "<p><em>foo</em>***</p>", "");
        }
    }
        // Rule 12:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example422()
        {
            // Example 422
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo ___
            //
            // Should be rendered as:
            //     <p>foo ___</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 422, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo ___", "<p>foo ___</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example423()
        {
            // Example 423
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo _\__
            //
            // Should be rendered as:
            //     <p>foo <em>_</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 423, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo _\\__", "<p>foo <em>_</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example424()
        {
            // Example 424
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo _*_
            //
            // Should be rendered as:
            //     <p>foo <em>*</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 424, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo _*_", "<p>foo <em>*</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example425()
        {
            // Example 425
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo _____
            //
            // Should be rendered as:
            //     <p>foo _____</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 425, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo _____", "<p>foo _____</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example426()
        {
            // Example 426
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo __\___
            //
            // Should be rendered as:
            //     <p>foo <strong>_</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 426, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo __\\___", "<p>foo <strong>_</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example427()
        {
            // Example 427
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     foo __*__
            //
            // Should be rendered as:
            //     <p>foo <strong>*</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 427, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("foo __*__", "<p>foo <strong>*</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example428()
        {
            // Example 428
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo_
            //
            // Should be rendered as:
            //     <p>_<em>foo</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 428, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__foo_", "<p>_<em>foo</em></p>", "");
        }
    }
        // Note that when delimiters do not match evenly, Rule 12 determines
        // that the excess literal `_` characters will appear outside of the
        // emphasis, rather than inside it:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example429()
        {
            // Example 429
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo__
            //
            // Should be rendered as:
            //     <p><em>foo</em>_</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 429, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_foo__", "<p><em>foo</em>_</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example430()
        {
            // Example 430
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ___foo__
            //
            // Should be rendered as:
            //     <p>_<strong>foo</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 430, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("___foo__", "<p>_<strong>foo</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example431()
        {
            // Example 431
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ____foo_
            //
            // Should be rendered as:
            //     <p>___<em>foo</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 431, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("____foo_", "<p>___<em>foo</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example432()
        {
            // Example 432
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo___
            //
            // Should be rendered as:
            //     <p><strong>foo</strong>_</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 432, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__foo___", "<p><strong>foo</strong>_</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example433()
        {
            // Example 433
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo____
            //
            // Should be rendered as:
            //     <p><em>foo</em>___</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 433, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_foo____", "<p><em>foo</em>___</p>", "");
        }
    }
        // Rule 13 implies that if you want emphasis nested directly inside
        // emphasis, you must use different delimiters:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example434()
        {
            // Example 434
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo**
            //
            // Should be rendered as:
            //     <p><strong>foo</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 434, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo**", "<p><strong>foo</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example435()
        {
            // Example 435
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *_foo_*
            //
            // Should be rendered as:
            //     <p><em><em>foo</em></em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 435, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*_foo_*", "<p><em><em>foo</em></em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example436()
        {
            // Example 436
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __foo__
            //
            // Should be rendered as:
            //     <p><strong>foo</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 436, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__foo__", "<p><strong>foo</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example437()
        {
            // Example 437
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _*foo*_
            //
            // Should be rendered as:
            //     <p><em><em>foo</em></em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 437, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_*foo*_", "<p><em><em>foo</em></em></p>", "");
        }
    }
        // However, strong emphasis within strong emphasis is possible without
        // switching delimiters:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example438()
        {
            // Example 438
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ****foo****
            //
            // Should be rendered as:
            //     <p><strong><strong>foo</strong></strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 438, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("****foo****", "<p><strong><strong>foo</strong></strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example439()
        {
            // Example 439
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ____foo____
            //
            // Should be rendered as:
            //     <p><strong><strong>foo</strong></strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 439, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("____foo____", "<p><strong><strong>foo</strong></strong></p>", "");
        }
    }
        // Rule 13 can be applied to arbitrarily long sequences of
        // delimiters:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example440()
        {
            // Example 440
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ******foo******
            //
            // Should be rendered as:
            //     <p><strong><strong><strong>foo</strong></strong></strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 440, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("******foo******", "<p><strong><strong><strong>foo</strong></strong></strong></p>", "");
        }
    }
        // Rule 14:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example441()
        {
            // Example 441
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     ***foo***
            //
            // Should be rendered as:
            //     <p><strong><em>foo</em></strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 441, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("***foo***", "<p><strong><em>foo</em></strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example442()
        {
            // Example 442
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _____foo_____
            //
            // Should be rendered as:
            //     <p><strong><strong><em>foo</em></strong></strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 442, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_____foo_____", "<p><strong><strong><em>foo</em></strong></strong></p>", "");
        }
    }
        // Rule 15:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example443()
        {
            // Example 443
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo _bar* baz_
            //
            // Should be rendered as:
            //     <p><em>foo _bar</em> baz_</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 443, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo _bar* baz_", "<p><em>foo _bar</em> baz_</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example444()
        {
            // Example 444
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo __bar *baz bim__ bam*
            //
            // Should be rendered as:
            //     <p><em>foo <strong>bar *baz bim</strong> bam</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 444, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo __bar *baz bim__ bam*", "<p><em>foo <strong>bar *baz bim</strong> bam</em></p>", "");
        }
    }
        // Rule 16:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example445()
        {
            // Example 445
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **foo **bar baz**
            //
            // Should be rendered as:
            //     <p>**foo <strong>bar baz</strong></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 445, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**foo **bar baz**", "<p>**foo <strong>bar baz</strong></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example446()
        {
            // Example 446
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *foo *bar baz*
            //
            // Should be rendered as:
            //     <p>*foo <em>bar baz</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 446, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*foo *bar baz*", "<p>*foo <em>bar baz</em></p>", "");
        }
    }
        // Rule 17:
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example447()
        {
            // Example 447
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *[bar*](/url)
            //
            // Should be rendered as:
            //     <p>*<a href="/url">bar*</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 447, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*[bar*](/url)", "<p>*<a href=\"/url\">bar*</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example448()
        {
            // Example 448
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _foo [bar_](/url)
            //
            // Should be rendered as:
            //     <p>_foo <a href="/url">bar_</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 448, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_foo [bar_](/url)", "<p>_foo <a href=\"/url\">bar_</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example449()
        {
            // Example 449
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *<img src="foo" title="*"/>
            //
            // Should be rendered as:
            //     <p>*<img src="foo" title="*"/></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 449, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*<img src=\"foo\" title=\"*\"/>", "<p>*<img src=\"foo\" title=\"*\"/></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example450()
        {
            // Example 450
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **<a href="**">
            //
            // Should be rendered as:
            //     <p>**<a href="**"></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 450, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**<a href=\"**\">", "<p>**<a href=\"**\"></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example451()
        {
            // Example 451
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __<a href="__">
            //
            // Should be rendered as:
            //     <p>__<a href="__"></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 451, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__<a href=\"__\">", "<p>__<a href=\"__\"></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example452()
        {
            // Example 452
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     *a `*`*
            //
            // Should be rendered as:
            //     <p><em>a <code>*</code></em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 452, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("*a `*`*", "<p><em>a <code>*</code></em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example453()
        {
            // Example 453
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     _a `_`_
            //
            // Should be rendered as:
            //     <p><em>a <code>_</code></em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 453, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("_a `_`_", "<p><em>a <code>_</code></em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example454()
        {
            // Example 454
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     **a<http://foo.bar/?q=**>
            //
            // Should be rendered as:
            //     <p>**a<a href="http://foo.bar/?q=**">http://foo.bar/?q=**</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 454, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("**a<http://foo.bar/?q=**>", "<p>**a<a href=\"http://foo.bar/?q=**\">http://foo.bar/?q=**</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesEmphasisandstrongemphasis
    {
        [Test]
        public void Example455()
        {
            // Example 455
            // Section: Inlines Emphasis and strong emphasis
            //
            // The following CommonMark:
            //     __a<http://foo.bar/?q=__>
            //
            // Should be rendered as:
            //     <p>__a<a href="http://foo.bar/?q=__">http://foo.bar/?q=__</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 455, "Inlines Emphasis and strong emphasis");
			TestParser.TestSpec("__a<http://foo.bar/?q=__>", "<p>__a<a href=\"http://foo.bar/?q=__\">http://foo.bar/?q=__</a></p>", "");
        }
    }
        // ## Links
        //
        // A link contains [link text] (the visible text), a [link destination]
        // (the URI that is the link destination), and optionally a [link title].
        // There are two basic kinds of links in Markdown.  In [inline links] the
        // destination and title are given immediately after the link text.  In
        // [reference links] the destination and title are defined elsewhere in
        // the document.
        //
        // A [link text](@) consists of a sequence of zero or more
        // inline elements enclosed by square brackets (`[` and `]`).  The
        // following rules apply:
        //
        // - Links may not contain other links, at any level of nesting. If
        // multiple otherwise valid link definitions appear nested inside each
        // other, the inner-most definition is used.
        //
        // - Brackets are allowed in the [link text] only if (a) they
        // are backslash-escaped or (b) they appear as a matched pair of brackets,
        // with an open bracket `[`, a sequence of zero or more inlines, and
        // a close bracket `]`.
        //
        // - Backtick [code spans], [autolinks], and raw [HTML tags] bind more tightly
        // than the brackets in link text.  Thus, for example,
        // `` [foo`]` `` could not be a link text, since the second `]`
        // is part of a code span.
        //
        // - The brackets in link text bind more tightly than markers for
        // [emphasis and strong emphasis]. Thus, for example, `*[foo*](url)` is a link.
        //
        // A [link destination](@) consists of either
        //
        // - a sequence of zero or more characters between an opening `<` and a
        // closing `>` that contains no spaces, line breaks, or unescaped
        // `<` or `>` characters, or
        //
        // - a nonempty sequence of characters that does not include
        // ASCII space or control characters, and includes parentheses
        // only if (a) they are backslash-escaped or (b) they are part of
        // a balanced pair of unescaped parentheses that is not itself
        // inside a balanced pair of unescaped parentheses.
        //
        // A [link title](@)  consists of either
        //
        // - a sequence of zero or more characters between straight double-quote
        // characters (`"`), including a `"` character only if it is
        // backslash-escaped, or
        //
        // - a sequence of zero or more characters between straight single-quote
        // characters (`'`), including a `'` character only if it is
        // backslash-escaped, or
        //
        // - a sequence of zero or more characters between matching parentheses
        // (`(...)`), including a `)` character only if it is backslash-escaped.
        //
        // Although [link titles] may span multiple lines, they may not contain
        // a [blank line].
        //
        // An [inline link](@) consists of a [link text] followed immediately
        // by a left parenthesis `(`, optional [whitespace], an optional
        // [link destination], an optional [link title] separated from the link
        // destination by [whitespace], optional [whitespace], and a right
        // parenthesis `)`. The link's text consists of the inlines contained
        // in the [link text] (excluding the enclosing square brackets).
        // The link's URI consists of the link destination, excluding enclosing
        // `<...>` if present, with backslash-escapes in effect as described
        // above.  The link's title consists of the link title, excluding its
        // enclosing delimiters, with backslash-escapes in effect as described
        // above.
        //
        // Here is a simple inline link:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example456()
        {
            // Example 456
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](/uri "title")
            //
            // Should be rendered as:
            //     <p><a href="/uri" title="title">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 456, "Inlines Links");
			TestParser.TestSpec("[link](/uri \"title\")", "<p><a href=\"/uri\" title=\"title\">link</a></p>", "");
        }
    }
        // The title may be omitted:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example457()
        {
            // Example 457
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](/uri)
            //
            // Should be rendered as:
            //     <p><a href="/uri">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 457, "Inlines Links");
			TestParser.TestSpec("[link](/uri)", "<p><a href=\"/uri\">link</a></p>", "");
        }
    }
        // Both the title and the destination may be omitted:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example458()
        {
            // Example 458
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link]()
            //
            // Should be rendered as:
            //     <p><a href="">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 458, "Inlines Links");
			TestParser.TestSpec("[link]()", "<p><a href=\"\">link</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example459()
        {
            // Example 459
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](<>)
            //
            // Should be rendered as:
            //     <p><a href="">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 459, "Inlines Links");
			TestParser.TestSpec("[link](<>)", "<p><a href=\"\">link</a></p>", "");
        }
    }
        // The destination cannot contain spaces or line breaks,
        // even if enclosed in pointy brackets:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example460()
        {
            // Example 460
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](/my uri)
            //
            // Should be rendered as:
            //     <p>[link](/my uri)</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 460, "Inlines Links");
			TestParser.TestSpec("[link](/my uri)", "<p>[link](/my uri)</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example461()
        {
            // Example 461
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](</my uri>)
            //
            // Should be rendered as:
            //     <p>[link](&lt;/my uri&gt;)</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 461, "Inlines Links");
			TestParser.TestSpec("[link](</my uri>)", "<p>[link](&lt;/my uri&gt;)</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example462()
        {
            // Example 462
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](foo
            //     bar)
            //
            // Should be rendered as:
            //     <p>[link](foo
            //     bar)</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 462, "Inlines Links");
			TestParser.TestSpec("[link](foo\nbar)", "<p>[link](foo\nbar)</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example463()
        {
            // Example 463
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](<foo
            //     bar>)
            //
            // Should be rendered as:
            //     <p>[link](<foo
            //     bar>)</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 463, "Inlines Links");
			TestParser.TestSpec("[link](<foo\nbar>)", "<p>[link](<foo\nbar>)</p>", "");
        }
    }
        // Parentheses inside the link destination may be escaped:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example464()
        {
            // Example 464
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](\(foo\))
            //
            // Should be rendered as:
            //     <p><a href="(foo)">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 464, "Inlines Links");
			TestParser.TestSpec("[link](\\(foo\\))", "<p><a href=\"(foo)\">link</a></p>", "");
        }
    }
        // One level of balanced parentheses is allowed without escaping:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example465()
        {
            // Example 465
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link]((foo)and(bar))
            //
            // Should be rendered as:
            //     <p><a href="(foo)and(bar)">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 465, "Inlines Links");
			TestParser.TestSpec("[link]((foo)and(bar))", "<p><a href=\"(foo)and(bar)\">link</a></p>", "");
        }
    }
        // However, if you have parentheses within parentheses, you need to escape
        // or use the `<...>` form:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example466()
        {
            // Example 466
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](foo(and(bar)))
            //
            // Should be rendered as:
            //     <p>[link](foo(and(bar)))</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 466, "Inlines Links");
			TestParser.TestSpec("[link](foo(and(bar)))", "<p>[link](foo(and(bar)))</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example467()
        {
            // Example 467
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](foo(and\(bar\)))
            //
            // Should be rendered as:
            //     <p><a href="foo(and(bar))">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 467, "Inlines Links");
			TestParser.TestSpec("[link](foo(and\\(bar\\)))", "<p><a href=\"foo(and(bar))\">link</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example468()
        {
            // Example 468
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](<foo(and(bar))>)
            //
            // Should be rendered as:
            //     <p><a href="foo(and(bar))">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 468, "Inlines Links");
			TestParser.TestSpec("[link](<foo(and(bar))>)", "<p><a href=\"foo(and(bar))\">link</a></p>", "");
        }
    }
        // Parentheses and other symbols can also be escaped, as usual
        // in Markdown:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example469()
        {
            // Example 469
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](foo\)\:)
            //
            // Should be rendered as:
            //     <p><a href="foo):">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 469, "Inlines Links");
			TestParser.TestSpec("[link](foo\\)\\:)", "<p><a href=\"foo):\">link</a></p>", "");
        }
    }
        // A link can contain fragment identifiers and queries:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example470()
        {
            // Example 470
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](#fragment)
            //     
            //     [link](http://example.com#fragment)
            //     
            //     [link](http://example.com?foo=3#frag)
            //
            // Should be rendered as:
            //     <p><a href="#fragment">link</a></p>
            //     <p><a href="http://example.com#fragment">link</a></p>
            //     <p><a href="http://example.com?foo=3#frag">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 470, "Inlines Links");
			TestParser.TestSpec("[link](#fragment)\n\n[link](http://example.com#fragment)\n\n[link](http://example.com?foo=3#frag)", "<p><a href=\"#fragment\">link</a></p>\n<p><a href=\"http://example.com#fragment\">link</a></p>\n<p><a href=\"http://example.com?foo=3#frag\">link</a></p>", "");
        }
    }
        // Note that a backslash before a non-escapable character is
        // just a backslash:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example471()
        {
            // Example 471
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](foo\bar)
            //
            // Should be rendered as:
            //     <p><a href="foo%5Cbar">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 471, "Inlines Links");
			TestParser.TestSpec("[link](foo\\bar)", "<p><a href=\"foo%5Cbar\">link</a></p>", "");
        }
    }
        // URL-escaping should be left alone inside the destination, as all
        // URL-escaped characters are also valid URL characters. Entity and
        // numerical character references in the destination will be parsed
        // into the corresponding Unicode code points, as usual.  These may
        // be optionally URL-escaped when written as HTML, but this spec
        // does not enforce any particular policy for rendering URLs in
        // HTML or other formats.  Renderers may make different decisions
        // about how to escape or normalize URLs in the output.
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example472()
        {
            // Example 472
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](foo%20b&auml;)
            //
            // Should be rendered as:
            //     <p><a href="foo%20b%C3%A4">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 472, "Inlines Links");
			TestParser.TestSpec("[link](foo%20b&auml;)", "<p><a href=\"foo%20b%C3%A4\">link</a></p>", "");
        }
    }
        // Note that, because titles can often be parsed as destinations,
        // if you try to omit the destination and keep the title, you'll
        // get unexpected results:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example473()
        {
            // Example 473
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link]("title")
            //
            // Should be rendered as:
            //     <p><a href="%22title%22">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 473, "Inlines Links");
			TestParser.TestSpec("[link](\"title\")", "<p><a href=\"%22title%22\">link</a></p>", "");
        }
    }
        // Titles may be in single quotes, double quotes, or parentheses:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example474()
        {
            // Example 474
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](/url "title")
            //     [link](/url 'title')
            //     [link](/url (title))
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">link</a>
            //     <a href="/url" title="title">link</a>
            //     <a href="/url" title="title">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 474, "Inlines Links");
			TestParser.TestSpec("[link](/url \"title\")\n[link](/url 'title')\n[link](/url (title))", "<p><a href=\"/url\" title=\"title\">link</a>\n<a href=\"/url\" title=\"title\">link</a>\n<a href=\"/url\" title=\"title\">link</a></p>", "");
        }
    }
        // Backslash escapes and entity and numeric character references
        // may be used in titles:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example475()
        {
            // Example 475
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](/url "title \"&quot;")
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title &quot;&quot;">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 475, "Inlines Links");
			TestParser.TestSpec("[link](/url \"title \\\"&quot;\")", "<p><a href=\"/url\" title=\"title &quot;&quot;\">link</a></p>", "");
        }
    }
        // Nested balanced quotes are not allowed without escaping:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example476()
        {
            // Example 476
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](/url "title "and" title")
            //
            // Should be rendered as:
            //     <p>[link](/url &quot;title &quot;and&quot; title&quot;)</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 476, "Inlines Links");
			TestParser.TestSpec("[link](/url \"title \"and\" title\")", "<p>[link](/url &quot;title &quot;and&quot; title&quot;)</p>", "");
        }
    }
        // But it is easy to work around this by using a different quote type:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example477()
        {
            // Example 477
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](/url 'title "and" title')
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title &quot;and&quot; title">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 477, "Inlines Links");
			TestParser.TestSpec("[link](/url 'title \"and\" title')", "<p><a href=\"/url\" title=\"title &quot;and&quot; title\">link</a></p>", "");
        }
    }
        // (Note:  `Markdown.pl` did allow double quotes inside a double-quoted
        // title, and its test suite included a test demonstrating this.
        // But it is hard to see a good rationale for the extra complexity this
        // brings, since there are already many ways---backslash escaping,
        // entity and numeric character references, or using a different
        // quote type for the enclosing title---to write titles containing
        // double quotes.  `Markdown.pl`'s handling of titles has a number
        // of other strange features.  For example, it allows single-quoted
        // titles in inline links, but not reference links.  And, in
        // reference links but not inline links, it allows a title to begin
        // with `"` and end with `)`.  `Markdown.pl` 1.0.1 even allows
        // titles with no closing quotation mark, though 1.0.2b8 does not.
        // It seems preferable to adopt a simple, rational rule that works
        // the same way in inline links and link reference definitions.)
        //
        // [Whitespace] is allowed around the destination and title:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example478()
        {
            // Example 478
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link](   /uri
            //       "title"  )
            //
            // Should be rendered as:
            //     <p><a href="/uri" title="title">link</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 478, "Inlines Links");
			TestParser.TestSpec("[link](   /uri\n  \"title\"  )", "<p><a href=\"/uri\" title=\"title\">link</a></p>", "");
        }
    }
        // But it is not allowed between the link text and the
        // following parenthesis:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example479()
        {
            // Example 479
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link] (/uri)
            //
            // Should be rendered as:
            //     <p>[link] (/uri)</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 479, "Inlines Links");
			TestParser.TestSpec("[link] (/uri)", "<p>[link] (/uri)</p>", "");
        }
    }
        // The link text may contain balanced brackets, but not unbalanced ones,
        // unless they are escaped:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example480()
        {
            // Example 480
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link [foo [bar]]](/uri)
            //
            // Should be rendered as:
            //     <p><a href="/uri">link [foo [bar]]</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 480, "Inlines Links");
			TestParser.TestSpec("[link [foo [bar]]](/uri)", "<p><a href=\"/uri\">link [foo [bar]]</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example481()
        {
            // Example 481
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link] bar](/uri)
            //
            // Should be rendered as:
            //     <p>[link] bar](/uri)</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 481, "Inlines Links");
			TestParser.TestSpec("[link] bar](/uri)", "<p>[link] bar](/uri)</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example482()
        {
            // Example 482
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link [bar](/uri)
            //
            // Should be rendered as:
            //     <p>[link <a href="/uri">bar</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 482, "Inlines Links");
			TestParser.TestSpec("[link [bar](/uri)", "<p>[link <a href=\"/uri\">bar</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example483()
        {
            // Example 483
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link \[bar](/uri)
            //
            // Should be rendered as:
            //     <p><a href="/uri">link [bar</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 483, "Inlines Links");
			TestParser.TestSpec("[link \\[bar](/uri)", "<p><a href=\"/uri\">link [bar</a></p>", "");
        }
    }
        // The link text may contain inline content:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example484()
        {
            // Example 484
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link *foo **bar** `#`*](/uri)
            //
            // Should be rendered as:
            //     <p><a href="/uri">link <em>foo <strong>bar</strong> <code>#</code></em></a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 484, "Inlines Links");
			TestParser.TestSpec("[link *foo **bar** `#`*](/uri)", "<p><a href=\"/uri\">link <em>foo <strong>bar</strong> <code>#</code></em></a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example485()
        {
            // Example 485
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [![moon](moon.jpg)](/uri)
            //
            // Should be rendered as:
            //     <p><a href="/uri"><img src="moon.jpg" alt="moon" /></a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 485, "Inlines Links");
			TestParser.TestSpec("[![moon](moon.jpg)](/uri)", "<p><a href=\"/uri\"><img src=\"moon.jpg\" alt=\"moon\" /></a></p>", "");
        }
    }
        // However, links may not contain other links, at any level of nesting.
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example486()
        {
            // Example 486
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo [bar](/uri)](/uri)
            //
            // Should be rendered as:
            //     <p>[foo <a href="/uri">bar</a>](/uri)</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 486, "Inlines Links");
			TestParser.TestSpec("[foo [bar](/uri)](/uri)", "<p>[foo <a href=\"/uri\">bar</a>](/uri)</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example487()
        {
            // Example 487
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo *[bar [baz](/uri)](/uri)*](/uri)
            //
            // Should be rendered as:
            //     <p>[foo <em>[bar <a href="/uri">baz</a>](/uri)</em>](/uri)</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 487, "Inlines Links");
			TestParser.TestSpec("[foo *[bar [baz](/uri)](/uri)*](/uri)", "<p>[foo <em>[bar <a href=\"/uri\">baz</a>](/uri)</em>](/uri)</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example488()
        {
            // Example 488
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     ![[[foo](uri1)](uri2)](uri3)
            //
            // Should be rendered as:
            //     <p><img src="uri3" alt="[foo](uri2)" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 488, "Inlines Links");
			TestParser.TestSpec("![[[foo](uri1)](uri2)](uri3)", "<p><img src=\"uri3\" alt=\"[foo](uri2)\" /></p>", "");
        }
    }
        // These cases illustrate the precedence of link text grouping over
        // emphasis grouping:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example489()
        {
            // Example 489
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     *[foo*](/uri)
            //
            // Should be rendered as:
            //     <p>*<a href="/uri">foo*</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 489, "Inlines Links");
			TestParser.TestSpec("*[foo*](/uri)", "<p>*<a href=\"/uri\">foo*</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example490()
        {
            // Example 490
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo *bar](baz*)
            //
            // Should be rendered as:
            //     <p><a href="baz*">foo *bar</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 490, "Inlines Links");
			TestParser.TestSpec("[foo *bar](baz*)", "<p><a href=\"baz*\">foo *bar</a></p>", "");
        }
    }
        // Note that brackets that *aren't* part of links do not take
        // precedence:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example491()
        {
            // Example 491
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     *foo [bar* baz]
            //
            // Should be rendered as:
            //     <p><em>foo [bar</em> baz]</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 491, "Inlines Links");
			TestParser.TestSpec("*foo [bar* baz]", "<p><em>foo [bar</em> baz]</p>", "");
        }
    }
        // These cases illustrate the precedence of HTML tags, code spans,
        // and autolinks over link grouping:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example492()
        {
            // Example 492
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo <bar attr="](baz)">
            //
            // Should be rendered as:
            //     <p>[foo <bar attr="](baz)"></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 492, "Inlines Links");
			TestParser.TestSpec("[foo <bar attr=\"](baz)\">", "<p>[foo <bar attr=\"](baz)\"></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example493()
        {
            // Example 493
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo`](/uri)`
            //
            // Should be rendered as:
            //     <p>[foo<code>](/uri)</code></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 493, "Inlines Links");
			TestParser.TestSpec("[foo`](/uri)`", "<p>[foo<code>](/uri)</code></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example494()
        {
            // Example 494
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo<http://example.com/?search=](uri)>
            //
            // Should be rendered as:
            //     <p>[foo<a href="http://example.com/?search=%5D(uri)">http://example.com/?search=](uri)</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 494, "Inlines Links");
			TestParser.TestSpec("[foo<http://example.com/?search=](uri)>", "<p>[foo<a href=\"http://example.com/?search=%5D(uri)\">http://example.com/?search=](uri)</a></p>", "");
        }
    }
        // There are three kinds of [reference link](@)s:
        // [full](#full-reference-link), [collapsed](#collapsed-reference-link),
        // and [shortcut](#shortcut-reference-link).
        //
        // A [full reference link](@)
        // consists of a [link text] immediately followed by a [link label]
        // that [matches] a [link reference definition] elsewhere in the document.
        //
        // A [link label](@)  begins with a left bracket (`[`) and ends
        // with the first right bracket (`]`) that is not backslash-escaped.
        // Between these brackets there must be at least one [non-whitespace character].
        // Unescaped square bracket characters are not allowed in
        // [link labels].  A link label can have at most 999
        // characters inside the square brackets.
        //
        // One label [matches](@)
        // another just in case their normalized forms are equal.  To normalize a
        // label, perform the *Unicode case fold* and collapse consecutive internal
        // [whitespace] to a single space.  If there are multiple
        // matching reference link definitions, the one that comes first in the
        // document is used.  (It is desirable in such cases to emit a warning.)
        //
        // The contents of the first link label are parsed as inlines, which are
        // used as the link's text.  The link's URI and title are provided by the
        // matching [link reference definition].
        //
        // Here is a simple example:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example495()
        {
            // Example 495
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo][bar]
            //     
            //     [bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 495, "Inlines Links");
			TestParser.TestSpec("[foo][bar]\n\n[bar]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">foo</a></p>", "");
        }
    }
        // The rules for the [link text] are the same as with
        // [inline links].  Thus:
        //
        // The link text may contain balanced brackets, but not unbalanced ones,
        // unless they are escaped:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example496()
        {
            // Example 496
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link [foo [bar]]][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p><a href="/uri">link [foo [bar]]</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 496, "Inlines Links");
			TestParser.TestSpec("[link [foo [bar]]][ref]\n\n[ref]: /uri", "<p><a href=\"/uri\">link [foo [bar]]</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example497()
        {
            // Example 497
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link \[bar][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p><a href="/uri">link [bar</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 497, "Inlines Links");
			TestParser.TestSpec("[link \\[bar][ref]\n\n[ref]: /uri", "<p><a href=\"/uri\">link [bar</a></p>", "");
        }
    }
        // The link text may contain inline content:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example498()
        {
            // Example 498
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [link *foo **bar** `#`*][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p><a href="/uri">link <em>foo <strong>bar</strong> <code>#</code></em></a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 498, "Inlines Links");
			TestParser.TestSpec("[link *foo **bar** `#`*][ref]\n\n[ref]: /uri", "<p><a href=\"/uri\">link <em>foo <strong>bar</strong> <code>#</code></em></a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example499()
        {
            // Example 499
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [![moon](moon.jpg)][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p><a href="/uri"><img src="moon.jpg" alt="moon" /></a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 499, "Inlines Links");
			TestParser.TestSpec("[![moon](moon.jpg)][ref]\n\n[ref]: /uri", "<p><a href=\"/uri\"><img src=\"moon.jpg\" alt=\"moon\" /></a></p>", "");
        }
    }
        // However, links may not contain other links, at any level of nesting.
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example500()
        {
            // Example 500
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo [bar](/uri)][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p>[foo <a href="/uri">bar</a>]<a href="/uri">ref</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 500, "Inlines Links");
			TestParser.TestSpec("[foo [bar](/uri)][ref]\n\n[ref]: /uri", "<p>[foo <a href=\"/uri\">bar</a>]<a href=\"/uri\">ref</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example501()
        {
            // Example 501
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo *bar [baz][ref]*][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p>[foo <em>bar <a href="/uri">baz</a></em>]<a href="/uri">ref</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 501, "Inlines Links");
			TestParser.TestSpec("[foo *bar [baz][ref]*][ref]\n\n[ref]: /uri", "<p>[foo <em>bar <a href=\"/uri\">baz</a></em>]<a href=\"/uri\">ref</a></p>", "");
        }
    }
        // (In the examples above, we have two [shortcut reference links]
        // instead of one [full reference link].)
        //
        // The following cases illustrate the precedence of link text grouping over
        // emphasis grouping:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example502()
        {
            // Example 502
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     *[foo*][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p>*<a href="/uri">foo*</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 502, "Inlines Links");
			TestParser.TestSpec("*[foo*][ref]\n\n[ref]: /uri", "<p>*<a href=\"/uri\">foo*</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example503()
        {
            // Example 503
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo *bar][ref]
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p><a href="/uri">foo *bar</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 503, "Inlines Links");
			TestParser.TestSpec("[foo *bar][ref]\n\n[ref]: /uri", "<p><a href=\"/uri\">foo *bar</a></p>", "");
        }
    }
        // These cases illustrate the precedence of HTML tags, code spans,
        // and autolinks over link grouping:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example504()
        {
            // Example 504
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo <bar attr="][ref]">
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p>[foo <bar attr="][ref]"></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 504, "Inlines Links");
			TestParser.TestSpec("[foo <bar attr=\"][ref]\">\n\n[ref]: /uri", "<p>[foo <bar attr=\"][ref]\"></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example505()
        {
            // Example 505
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo`][ref]`
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p>[foo<code>][ref]</code></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 505, "Inlines Links");
			TestParser.TestSpec("[foo`][ref]`\n\n[ref]: /uri", "<p>[foo<code>][ref]</code></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example506()
        {
            // Example 506
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo<http://example.com/?search=][ref]>
            //     
            //     [ref]: /uri
            //
            // Should be rendered as:
            //     <p>[foo<a href="http://example.com/?search=%5D%5Bref%5D">http://example.com/?search=][ref]</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 506, "Inlines Links");
			TestParser.TestSpec("[foo<http://example.com/?search=][ref]>\n\n[ref]: /uri", "<p>[foo<a href=\"http://example.com/?search=%5D%5Bref%5D\">http://example.com/?search=][ref]</a></p>", "");
        }
    }
        // Matching is case-insensitive:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example507()
        {
            // Example 507
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo][BaR]
            //     
            //     [bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 507, "Inlines Links");
			TestParser.TestSpec("[foo][BaR]\n\n[bar]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">foo</a></p>", "");
        }
    }
        // Unicode case fold is used:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example508()
        {
            // Example 508
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [Толпой][Толпой] is a Russian word.
            //     
            //     [ТОЛПОЙ]: /url
            //
            // Should be rendered as:
            //     <p><a href="/url">Толпой</a> is a Russian word.</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 508, "Inlines Links");
			TestParser.TestSpec("[Толпой][Толпой] is a Russian word.\n\n[ТОЛПОЙ]: /url", "<p><a href=\"/url\">Толпой</a> is a Russian word.</p>", "");
        }
    }
        // Consecutive internal [whitespace] is treated as one space for
        // purposes of determining matching:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example509()
        {
            // Example 509
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [Foo
            //       bar]: /url
            //     
            //     [Baz][Foo bar]
            //
            // Should be rendered as:
            //     <p><a href="/url">Baz</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 509, "Inlines Links");
			TestParser.TestSpec("[Foo\n  bar]: /url\n\n[Baz][Foo bar]", "<p><a href=\"/url\">Baz</a></p>", "");
        }
    }
        // No [whitespace] is allowed between the [link text] and the
        // [link label]:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example510()
        {
            // Example 510
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo] [bar]
            //     
            //     [bar]: /url "title"
            //
            // Should be rendered as:
            //     <p>[foo] <a href="/url" title="title">bar</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 510, "Inlines Links");
			TestParser.TestSpec("[foo] [bar]\n\n[bar]: /url \"title\"", "<p>[foo] <a href=\"/url\" title=\"title\">bar</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example511()
        {
            // Example 511
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo]
            //     [bar]
            //     
            //     [bar]: /url "title"
            //
            // Should be rendered as:
            //     <p>[foo]
            //     <a href="/url" title="title">bar</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 511, "Inlines Links");
			TestParser.TestSpec("[foo]\n[bar]\n\n[bar]: /url \"title\"", "<p>[foo]\n<a href=\"/url\" title=\"title\">bar</a></p>", "");
        }
    }
        // This is a departure from John Gruber's original Markdown syntax
        // description, which explicitly allows whitespace between the link
        // text and the link label.  It brings reference links in line with
        // [inline links], which (according to both original Markdown and
        // this spec) cannot have whitespace after the link text.  More
        // importantly, it prevents inadvertent capture of consecutive
        // [shortcut reference links]. If whitespace is allowed between the
        // link text and the link label, then in the following we will have
        // a single reference link, not two shortcut reference links, as
        // intended:
        //
        // ``` markdown
        // [foo]
        // [bar]
        //
        // [foo]: /url1
        // [bar]: /url2
        // ```
        //
        // (Note that [shortcut reference links] were introduced by Gruber
        // himself in a beta version of `Markdown.pl`, but never included
        // in the official syntax description.  Without shortcut reference
        // links, it is harmless to allow space between the link text and
        // link label; but once shortcut references are introduced, it is
        // too dangerous to allow this, as it frequently leads to
        // unintended results.)
        //
        // When there are multiple matching [link reference definitions],
        // the first is used:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example512()
        {
            // Example 512
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo]: /url1
            //     
            //     [foo]: /url2
            //     
            //     [bar][foo]
            //
            // Should be rendered as:
            //     <p><a href="/url1">bar</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 512, "Inlines Links");
			TestParser.TestSpec("[foo]: /url1\n\n[foo]: /url2\n\n[bar][foo]", "<p><a href=\"/url1\">bar</a></p>", "");
        }
    }
        // Note that matching is performed on normalized strings, not parsed
        // inline content.  So the following does not match, even though the
        // labels define equivalent inline content:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example513()
        {
            // Example 513
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [bar][foo\!]
            //     
            //     [foo!]: /url
            //
            // Should be rendered as:
            //     <p>[bar][foo!]</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 513, "Inlines Links");
			TestParser.TestSpec("[bar][foo\\!]\n\n[foo!]: /url", "<p>[bar][foo!]</p>", "");
        }
    }
        // [Link labels] cannot contain brackets, unless they are
        // backslash-escaped:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example514()
        {
            // Example 514
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo][ref[]
            //     
            //     [ref[]: /uri
            //
            // Should be rendered as:
            //     <p>[foo][ref[]</p>
            //     <p>[ref[]: /uri</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 514, "Inlines Links");
			TestParser.TestSpec("[foo][ref[]\n\n[ref[]: /uri", "<p>[foo][ref[]</p>\n<p>[ref[]: /uri</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example515()
        {
            // Example 515
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo][ref[bar]]
            //     
            //     [ref[bar]]: /uri
            //
            // Should be rendered as:
            //     <p>[foo][ref[bar]]</p>
            //     <p>[ref[bar]]: /uri</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 515, "Inlines Links");
			TestParser.TestSpec("[foo][ref[bar]]\n\n[ref[bar]]: /uri", "<p>[foo][ref[bar]]</p>\n<p>[ref[bar]]: /uri</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example516()
        {
            // Example 516
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [[[foo]]]
            //     
            //     [[[foo]]]: /url
            //
            // Should be rendered as:
            //     <p>[[[foo]]]</p>
            //     <p>[[[foo]]]: /url</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 516, "Inlines Links");
			TestParser.TestSpec("[[[foo]]]\n\n[[[foo]]]: /url", "<p>[[[foo]]]</p>\n<p>[[[foo]]]: /url</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example517()
        {
            // Example 517
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo][ref\[]
            //     
            //     [ref\[]: /uri
            //
            // Should be rendered as:
            //     <p><a href="/uri">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 517, "Inlines Links");
			TestParser.TestSpec("[foo][ref\\[]\n\n[ref\\[]: /uri", "<p><a href=\"/uri\">foo</a></p>", "");
        }
    }
        // Note that in this example `]` is not backslash-escaped:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example518()
        {
            // Example 518
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [bar\\]: /uri
            //     
            //     [bar\\]
            //
            // Should be rendered as:
            //     <p><a href="/uri">bar\</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 518, "Inlines Links");
			TestParser.TestSpec("[bar\\\\]: /uri\n\n[bar\\\\]", "<p><a href=\"/uri\">bar\\</a></p>", "");
        }
    }
        // A [link label] must contain at least one [non-whitespace character]:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example519()
        {
            // Example 519
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     []
            //     
            //     []: /uri
            //
            // Should be rendered as:
            //     <p>[]</p>
            //     <p>[]: /uri</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 519, "Inlines Links");
			TestParser.TestSpec("[]\n\n[]: /uri", "<p>[]</p>\n<p>[]: /uri</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example520()
        {
            // Example 520
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [
            //      ]
            //     
            //     [
            //      ]: /uri
            //
            // Should be rendered as:
            //     <p>[
            //     ]</p>
            //     <p>[
            //     ]: /uri</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 520, "Inlines Links");
			TestParser.TestSpec("[\n ]\n\n[\n ]: /uri", "<p>[\n]</p>\n<p>[\n]: /uri</p>", "");
        }
    }
        // A [collapsed reference link](@)
        // consists of a [link label] that [matches] a
        // [link reference definition] elsewhere in the
        // document, followed by the string `[]`.
        // The contents of the first link label are parsed as inlines,
        // which are used as the link's text.  The link's URI and title are
        // provided by the matching reference link definition.  Thus,
        // `[foo][]` is equivalent to `[foo][foo]`.
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example521()
        {
            // Example 521
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo][]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 521, "Inlines Links");
			TestParser.TestSpec("[foo][]\n\n[foo]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">foo</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example522()
        {
            // Example 522
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [*foo* bar][]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title"><em>foo</em> bar</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 522, "Inlines Links");
			TestParser.TestSpec("[*foo* bar][]\n\n[*foo* bar]: /url \"title\"", "<p><a href=\"/url\" title=\"title\"><em>foo</em> bar</a></p>", "");
        }
    }
        // The link labels are case-insensitive:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example523()
        {
            // Example 523
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [Foo][]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">Foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 523, "Inlines Links");
			TestParser.TestSpec("[Foo][]\n\n[foo]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">Foo</a></p>", "");
        }
    }
        // As with full reference links, [whitespace] is not
        // allowed between the two sets of brackets:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example524()
        {
            // Example 524
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo] 
            //     []
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a>
            //     []</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 524, "Inlines Links");
			TestParser.TestSpec("[foo] \n[]\n\n[foo]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">foo</a>\n[]</p>", "");
        }
    }
        // A [shortcut reference link](@)
        // consists of a [link label] that [matches] a
        // [link reference definition] elsewhere in the
        // document and is not followed by `[]` or a link label.
        // The contents of the first link label are parsed as inlines,
        // which are used as the link's text.  The link's URI and title
        // are provided by the matching link reference definition.
        // Thus, `[foo]` is equivalent to `[foo][]`.
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example525()
        {
            // Example 525
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 525, "Inlines Links");
			TestParser.TestSpec("[foo]\n\n[foo]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">foo</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example526()
        {
            // Example 526
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [*foo* bar]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title"><em>foo</em> bar</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 526, "Inlines Links");
			TestParser.TestSpec("[*foo* bar]\n\n[*foo* bar]: /url \"title\"", "<p><a href=\"/url\" title=\"title\"><em>foo</em> bar</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example527()
        {
            // Example 527
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [[*foo* bar]]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p>[<a href="/url" title="title"><em>foo</em> bar</a>]</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 527, "Inlines Links");
			TestParser.TestSpec("[[*foo* bar]]\n\n[*foo* bar]: /url \"title\"", "<p>[<a href=\"/url\" title=\"title\"><em>foo</em> bar</a>]</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example528()
        {
            // Example 528
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [[bar [foo]
            //     
            //     [foo]: /url
            //
            // Should be rendered as:
            //     <p>[[bar <a href="/url">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 528, "Inlines Links");
			TestParser.TestSpec("[[bar [foo]\n\n[foo]: /url", "<p>[[bar <a href=\"/url\">foo</a></p>", "");
        }
    }
        // The link labels are case-insensitive:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example529()
        {
            // Example 529
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [Foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><a href="/url" title="title">Foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 529, "Inlines Links");
			TestParser.TestSpec("[Foo]\n\n[foo]: /url \"title\"", "<p><a href=\"/url\" title=\"title\">Foo</a></p>", "");
        }
    }
        // A space after the link text should be preserved:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example530()
        {
            // Example 530
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo] bar
            //     
            //     [foo]: /url
            //
            // Should be rendered as:
            //     <p><a href="/url">foo</a> bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 530, "Inlines Links");
			TestParser.TestSpec("[foo] bar\n\n[foo]: /url", "<p><a href=\"/url\">foo</a> bar</p>", "");
        }
    }
        // If you just want bracketed text, you can backslash-escape the
        // opening bracket to avoid links:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example531()
        {
            // Example 531
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     \[foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p>[foo]</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 531, "Inlines Links");
			TestParser.TestSpec("\\[foo]\n\n[foo]: /url \"title\"", "<p>[foo]</p>", "");
        }
    }
        // Note that this is a link, because a link label ends with the first
        // following closing bracket:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example532()
        {
            // Example 532
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo*]: /url
            //     
            //     *[foo*]
            //
            // Should be rendered as:
            //     <p>*<a href="/url">foo*</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 532, "Inlines Links");
			TestParser.TestSpec("[foo*]: /url\n\n*[foo*]", "<p>*<a href=\"/url\">foo*</a></p>", "");
        }
    }
        // Full references take precedence over shortcut references:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example533()
        {
            // Example 533
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo][bar]
            //     
            //     [foo]: /url1
            //     [bar]: /url2
            //
            // Should be rendered as:
            //     <p><a href="/url2">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 533, "Inlines Links");
			TestParser.TestSpec("[foo][bar]\n\n[foo]: /url1\n[bar]: /url2", "<p><a href=\"/url2\">foo</a></p>", "");
        }
    }
        // In the following case `[bar][baz]` is parsed as a reference,
        // `[foo]` as normal text:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example534()
        {
            // Example 534
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo][bar][baz]
            //     
            //     [baz]: /url
            //
            // Should be rendered as:
            //     <p>[foo]<a href="/url">bar</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 534, "Inlines Links");
			TestParser.TestSpec("[foo][bar][baz]\n\n[baz]: /url", "<p>[foo]<a href=\"/url\">bar</a></p>", "");
        }
    }
        // Here, though, `[foo][bar]` is parsed as a reference, since
        // `[bar]` is defined:
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example535()
        {
            // Example 535
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo][bar][baz]
            //     
            //     [baz]: /url1
            //     [bar]: /url2
            //
            // Should be rendered as:
            //     <p><a href="/url2">foo</a><a href="/url1">baz</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 535, "Inlines Links");
			TestParser.TestSpec("[foo][bar][baz]\n\n[baz]: /url1\n[bar]: /url2", "<p><a href=\"/url2\">foo</a><a href=\"/url1\">baz</a></p>", "");
        }
    }
        // Here `[foo]` is not parsed as a shortcut reference, because it
        // is followed by a link label (even though `[bar]` is not defined):
    [TestFixture]
    public partial class TestInlinesLinks
    {
        [Test]
        public void Example536()
        {
            // Example 536
            // Section: Inlines Links
            //
            // The following CommonMark:
            //     [foo][bar][baz]
            //     
            //     [baz]: /url1
            //     [foo]: /url2
            //
            // Should be rendered as:
            //     <p>[foo]<a href="/url1">bar</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 536, "Inlines Links");
			TestParser.TestSpec("[foo][bar][baz]\n\n[baz]: /url1\n[foo]: /url2", "<p>[foo]<a href=\"/url1\">bar</a></p>", "");
        }
    }
        // ## Images
        //
        // Syntax for images is like the syntax for links, with one
        // difference. Instead of [link text], we have an
        // [image description](@).  The rules for this are the
        // same as for [link text], except that (a) an
        // image description starts with `![` rather than `[`, and
        // (b) an image description may contain links.
        // An image description has inline elements
        // as its contents.  When an image is rendered to HTML,
        // this is standardly used as the image's `alt` attribute.
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example537()
        {
            // Example 537
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![foo](/url "title")
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" title="title" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 537, "Inlines Images");
			TestParser.TestSpec("![foo](/url \"title\")", "<p><img src=\"/url\" alt=\"foo\" title=\"title\" /></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example538()
        {
            // Example 538
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![foo *bar*]
            //     
            //     [foo *bar*]: train.jpg "train & tracks"
            //
            // Should be rendered as:
            //     <p><img src="train.jpg" alt="foo bar" title="train &amp; tracks" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 538, "Inlines Images");
			TestParser.TestSpec("![foo *bar*]\n\n[foo *bar*]: train.jpg \"train & tracks\"", "<p><img src=\"train.jpg\" alt=\"foo bar\" title=\"train &amp; tracks\" /></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example539()
        {
            // Example 539
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![foo ![bar](/url)](/url2)
            //
            // Should be rendered as:
            //     <p><img src="/url2" alt="foo bar" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 539, "Inlines Images");
			TestParser.TestSpec("![foo ![bar](/url)](/url2)", "<p><img src=\"/url2\" alt=\"foo bar\" /></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example540()
        {
            // Example 540
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![foo [bar](/url)](/url2)
            //
            // Should be rendered as:
            //     <p><img src="/url2" alt="foo bar" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 540, "Inlines Images");
			TestParser.TestSpec("![foo [bar](/url)](/url2)", "<p><img src=\"/url2\" alt=\"foo bar\" /></p>", "");
        }
    }
        // Though this spec is concerned with parsing, not rendering, it is
        // recommended that in rendering to HTML, only the plain string content
        // of the [image description] be used.  Note that in
        // the above example, the alt attribute's value is `foo bar`, not `foo
        // [bar](/url)` or `foo <a href="/url">bar</a>`.  Only the plain string
        // content is rendered, without formatting.
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example541()
        {
            // Example 541
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![foo *bar*][]
            //     
            //     [foo *bar*]: train.jpg "train & tracks"
            //
            // Should be rendered as:
            //     <p><img src="train.jpg" alt="foo bar" title="train &amp; tracks" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 541, "Inlines Images");
			TestParser.TestSpec("![foo *bar*][]\n\n[foo *bar*]: train.jpg \"train & tracks\"", "<p><img src=\"train.jpg\" alt=\"foo bar\" title=\"train &amp; tracks\" /></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example542()
        {
            // Example 542
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![foo *bar*][foobar]
            //     
            //     [FOOBAR]: train.jpg "train & tracks"
            //
            // Should be rendered as:
            //     <p><img src="train.jpg" alt="foo bar" title="train &amp; tracks" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 542, "Inlines Images");
			TestParser.TestSpec("![foo *bar*][foobar]\n\n[FOOBAR]: train.jpg \"train & tracks\"", "<p><img src=\"train.jpg\" alt=\"foo bar\" title=\"train &amp; tracks\" /></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example543()
        {
            // Example 543
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![foo](train.jpg)
            //
            // Should be rendered as:
            //     <p><img src="train.jpg" alt="foo" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 543, "Inlines Images");
			TestParser.TestSpec("![foo](train.jpg)", "<p><img src=\"train.jpg\" alt=\"foo\" /></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example544()
        {
            // Example 544
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     My ![foo bar](/path/to/train.jpg  "title"   )
            //
            // Should be rendered as:
            //     <p>My <img src="/path/to/train.jpg" alt="foo bar" title="title" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 544, "Inlines Images");
			TestParser.TestSpec("My ![foo bar](/path/to/train.jpg  \"title\"   )", "<p>My <img src=\"/path/to/train.jpg\" alt=\"foo bar\" title=\"title\" /></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example545()
        {
            // Example 545
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![foo](<url>)
            //
            // Should be rendered as:
            //     <p><img src="url" alt="foo" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 545, "Inlines Images");
			TestParser.TestSpec("![foo](<url>)", "<p><img src=\"url\" alt=\"foo\" /></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example546()
        {
            // Example 546
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![](/url)
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 546, "Inlines Images");
			TestParser.TestSpec("![](/url)", "<p><img src=\"/url\" alt=\"\" /></p>", "");
        }
    }
        // Reference-style:
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example547()
        {
            // Example 547
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![foo][bar]
            //     
            //     [bar]: /url
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 547, "Inlines Images");
			TestParser.TestSpec("![foo][bar]\n\n[bar]: /url", "<p><img src=\"/url\" alt=\"foo\" /></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example548()
        {
            // Example 548
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![foo][bar]
            //     
            //     [BAR]: /url
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 548, "Inlines Images");
			TestParser.TestSpec("![foo][bar]\n\n[BAR]: /url", "<p><img src=\"/url\" alt=\"foo\" /></p>", "");
        }
    }
        // Collapsed:
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example549()
        {
            // Example 549
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![foo][]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" title="title" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 549, "Inlines Images");
			TestParser.TestSpec("![foo][]\n\n[foo]: /url \"title\"", "<p><img src=\"/url\" alt=\"foo\" title=\"title\" /></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example550()
        {
            // Example 550
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![*foo* bar][]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo bar" title="title" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 550, "Inlines Images");
			TestParser.TestSpec("![*foo* bar][]\n\n[*foo* bar]: /url \"title\"", "<p><img src=\"/url\" alt=\"foo bar\" title=\"title\" /></p>", "");
        }
    }
        // The labels are case-insensitive:
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example551()
        {
            // Example 551
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![Foo][]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="Foo" title="title" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 551, "Inlines Images");
			TestParser.TestSpec("![Foo][]\n\n[foo]: /url \"title\"", "<p><img src=\"/url\" alt=\"Foo\" title=\"title\" /></p>", "");
        }
    }
        // As with reference links, [whitespace] is not allowed
        // between the two sets of brackets:
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example552()
        {
            // Example 552
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![foo] 
            //     []
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" title="title" />
            //     []</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 552, "Inlines Images");
			TestParser.TestSpec("![foo] \n[]\n\n[foo]: /url \"title\"", "<p><img src=\"/url\" alt=\"foo\" title=\"title\" />\n[]</p>", "");
        }
    }
        // Shortcut:
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example553()
        {
            // Example 553
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo" title="title" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 553, "Inlines Images");
			TestParser.TestSpec("![foo]\n\n[foo]: /url \"title\"", "<p><img src=\"/url\" alt=\"foo\" title=\"title\" /></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example554()
        {
            // Example 554
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![*foo* bar]
            //     
            //     [*foo* bar]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="foo bar" title="title" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 554, "Inlines Images");
			TestParser.TestSpec("![*foo* bar]\n\n[*foo* bar]: /url \"title\"", "<p><img src=\"/url\" alt=\"foo bar\" title=\"title\" /></p>", "");
        }
    }
        // Note that link labels cannot contain unescaped brackets:
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example555()
        {
            // Example 555
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![[foo]]
            //     
            //     [[foo]]: /url "title"
            //
            // Should be rendered as:
            //     <p>![[foo]]</p>
            //     <p>[[foo]]: /url &quot;title&quot;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 555, "Inlines Images");
			TestParser.TestSpec("![[foo]]\n\n[[foo]]: /url \"title\"", "<p>![[foo]]</p>\n<p>[[foo]]: /url &quot;title&quot;</p>", "");
        }
    }
        // The link labels are case-insensitive:
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example556()
        {
            // Example 556
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     ![Foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p><img src="/url" alt="Foo" title="title" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 556, "Inlines Images");
			TestParser.TestSpec("![Foo]\n\n[foo]: /url \"title\"", "<p><img src=\"/url\" alt=\"Foo\" title=\"title\" /></p>", "");
        }
    }
        // If you just want bracketed text, you can backslash-escape the
        // opening `!` and `[`:
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example557()
        {
            // Example 557
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     \!\[foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p>![foo]</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 557, "Inlines Images");
			TestParser.TestSpec("\\!\\[foo]\n\n[foo]: /url \"title\"", "<p>![foo]</p>", "");
        }
    }
        // If you want a link after a literal `!`, backslash-escape the
        // `!`:
    [TestFixture]
    public partial class TestInlinesImages
    {
        [Test]
        public void Example558()
        {
            // Example 558
            // Section: Inlines Images
            //
            // The following CommonMark:
            //     \![foo]
            //     
            //     [foo]: /url "title"
            //
            // Should be rendered as:
            //     <p>!<a href="/url" title="title">foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 558, "Inlines Images");
			TestParser.TestSpec("\\![foo]\n\n[foo]: /url \"title\"", "<p>!<a href=\"/url\" title=\"title\">foo</a></p>", "");
        }
    }
        // ## Autolinks
        //
        // [Autolink](@)s are absolute URIs and email addresses inside
        // `<` and `>`. They are parsed as links, with the URL or email address
        // as the link label.
        //
        // A [URI autolink](@) consists of `<`, followed by an
        // [absolute URI] not containing `<`, followed by `>`.  It is parsed as
        // a link to the URI, with the URI as the link's label.
        //
        // An [absolute URI](@),
        // for these purposes, consists of a [scheme] followed by a colon (`:`)
        // followed by zero or more characters other than ASCII
        // [whitespace] and control characters, `<`, and `>`.  If
        // the URI includes these characters, they must be percent-encoded
        // (e.g. `%20` for a space).
        //
        // For purposes of this spec, a [scheme](@) is any sequence
        // of 2--32 characters beginning with an ASCII letter and followed
        // by any combination of ASCII letters, digits, or the symbols plus
        // ("+"), period ("."), or hyphen ("-").
        //
        // Here are some valid autolinks:
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example559()
        {
            // Example 559
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <http://foo.bar.baz>
            //
            // Should be rendered as:
            //     <p><a href="http://foo.bar.baz">http://foo.bar.baz</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 559, "Inlines Autolinks");
			TestParser.TestSpec("<http://foo.bar.baz>", "<p><a href=\"http://foo.bar.baz\">http://foo.bar.baz</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example560()
        {
            // Example 560
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <http://foo.bar.baz/test?q=hello&id=22&boolean>
            //
            // Should be rendered as:
            //     <p><a href="http://foo.bar.baz/test?q=hello&amp;id=22&amp;boolean">http://foo.bar.baz/test?q=hello&amp;id=22&amp;boolean</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 560, "Inlines Autolinks");
			TestParser.TestSpec("<http://foo.bar.baz/test?q=hello&id=22&boolean>", "<p><a href=\"http://foo.bar.baz/test?q=hello&amp;id=22&amp;boolean\">http://foo.bar.baz/test?q=hello&amp;id=22&amp;boolean</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example561()
        {
            // Example 561
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <irc://foo.bar:2233/baz>
            //
            // Should be rendered as:
            //     <p><a href="irc://foo.bar:2233/baz">irc://foo.bar:2233/baz</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 561, "Inlines Autolinks");
			TestParser.TestSpec("<irc://foo.bar:2233/baz>", "<p><a href=\"irc://foo.bar:2233/baz\">irc://foo.bar:2233/baz</a></p>", "");
        }
    }
        // Uppercase is also fine:
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example562()
        {
            // Example 562
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <MAILTO:FOO@BAR.BAZ>
            //
            // Should be rendered as:
            //     <p><a href="MAILTO:FOO@BAR.BAZ">MAILTO:FOO@BAR.BAZ</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 562, "Inlines Autolinks");
			TestParser.TestSpec("<MAILTO:FOO@BAR.BAZ>", "<p><a href=\"MAILTO:FOO@BAR.BAZ\">MAILTO:FOO@BAR.BAZ</a></p>", "");
        }
    }
        // Note that many strings that count as [absolute URIs] for
        // purposes of this spec are not valid URIs, because their
        // schemes are not registered or because of other problems
        // with their syntax:
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example563()
        {
            // Example 563
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <a+b+c:d>
            //
            // Should be rendered as:
            //     <p><a href="a+b+c:d">a+b+c:d</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 563, "Inlines Autolinks");
			TestParser.TestSpec("<a+b+c:d>", "<p><a href=\"a+b+c:d\">a+b+c:d</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example564()
        {
            // Example 564
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <made-up-scheme://foo,bar>
            //
            // Should be rendered as:
            //     <p><a href="made-up-scheme://foo,bar">made-up-scheme://foo,bar</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 564, "Inlines Autolinks");
			TestParser.TestSpec("<made-up-scheme://foo,bar>", "<p><a href=\"made-up-scheme://foo,bar\">made-up-scheme://foo,bar</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example565()
        {
            // Example 565
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <http://../>
            //
            // Should be rendered as:
            //     <p><a href="http://../">http://../</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 565, "Inlines Autolinks");
			TestParser.TestSpec("<http://../>", "<p><a href=\"http://../\">http://../</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example566()
        {
            // Example 566
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <localhost:5001/foo>
            //
            // Should be rendered as:
            //     <p><a href="localhost:5001/foo">localhost:5001/foo</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 566, "Inlines Autolinks");
			TestParser.TestSpec("<localhost:5001/foo>", "<p><a href=\"localhost:5001/foo\">localhost:5001/foo</a></p>", "");
        }
    }
        // Spaces are not allowed in autolinks:
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example567()
        {
            // Example 567
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <http://foo.bar/baz bim>
            //
            // Should be rendered as:
            //     <p>&lt;http://foo.bar/baz bim&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 567, "Inlines Autolinks");
			TestParser.TestSpec("<http://foo.bar/baz bim>", "<p>&lt;http://foo.bar/baz bim&gt;</p>", "");
        }
    }
        // Backslash-escapes do not work inside autolinks:
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example568()
        {
            // Example 568
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <http://example.com/\[\>
            //
            // Should be rendered as:
            //     <p><a href="http://example.com/%5C%5B%5C">http://example.com/\[\</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 568, "Inlines Autolinks");
			TestParser.TestSpec("<http://example.com/\\[\\>", "<p><a href=\"http://example.com/%5C%5B%5C\">http://example.com/\\[\\</a></p>", "");
        }
    }
        // An [email autolink](@)
        // consists of `<`, followed by an [email address],
        // followed by `>`.  The link's label is the email address,
        // and the URL is `mailto:` followed by the email address.
        //
        // An [email address](@),
        // for these purposes, is anything that matches
        // the [non-normative regex from the HTML5
        // spec](https://html.spec.whatwg.org/multipage/forms.html#e-mail-state-(type=email)):
        //
        // /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?
        // (?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/
        //
        // Examples of email autolinks:
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example569()
        {
            // Example 569
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <foo@bar.example.com>
            //
            // Should be rendered as:
            //     <p><a href="mailto:foo@bar.example.com">foo@bar.example.com</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 569, "Inlines Autolinks");
			TestParser.TestSpec("<foo@bar.example.com>", "<p><a href=\"mailto:foo@bar.example.com\">foo@bar.example.com</a></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example570()
        {
            // Example 570
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <foo+special@Bar.baz-bar0.com>
            //
            // Should be rendered as:
            //     <p><a href="mailto:foo+special@Bar.baz-bar0.com">foo+special@Bar.baz-bar0.com</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 570, "Inlines Autolinks");
			TestParser.TestSpec("<foo+special@Bar.baz-bar0.com>", "<p><a href=\"mailto:foo+special@Bar.baz-bar0.com\">foo+special@Bar.baz-bar0.com</a></p>", "");
        }
    }
        // Backslash-escapes do not work inside email autolinks:
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example571()
        {
            // Example 571
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <foo\+@bar.example.com>
            //
            // Should be rendered as:
            //     <p>&lt;foo+@bar.example.com&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 571, "Inlines Autolinks");
			TestParser.TestSpec("<foo\\+@bar.example.com>", "<p>&lt;foo+@bar.example.com&gt;</p>", "");
        }
    }
        // These are not autolinks:
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example572()
        {
            // Example 572
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <>
            //
            // Should be rendered as:
            //     <p>&lt;&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 572, "Inlines Autolinks");
			TestParser.TestSpec("<>", "<p>&lt;&gt;</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example573()
        {
            // Example 573
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     < http://foo.bar >
            //
            // Should be rendered as:
            //     <p>&lt; http://foo.bar &gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 573, "Inlines Autolinks");
			TestParser.TestSpec("< http://foo.bar >", "<p>&lt; http://foo.bar &gt;</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example574()
        {
            // Example 574
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <m:abc>
            //
            // Should be rendered as:
            //     <p>&lt;m:abc&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 574, "Inlines Autolinks");
			TestParser.TestSpec("<m:abc>", "<p>&lt;m:abc&gt;</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example575()
        {
            // Example 575
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     <foo.bar.baz>
            //
            // Should be rendered as:
            //     <p>&lt;foo.bar.baz&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 575, "Inlines Autolinks");
			TestParser.TestSpec("<foo.bar.baz>", "<p>&lt;foo.bar.baz&gt;</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example576()
        {
            // Example 576
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     http://example.com
            //
            // Should be rendered as:
            //     <p>http://example.com</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 576, "Inlines Autolinks");
			TestParser.TestSpec("http://example.com", "<p>http://example.com</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesAutolinks
    {
        [Test]
        public void Example577()
        {
            // Example 577
            // Section: Inlines Autolinks
            //
            // The following CommonMark:
            //     foo@bar.example.com
            //
            // Should be rendered as:
            //     <p>foo@bar.example.com</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 577, "Inlines Autolinks");
			TestParser.TestSpec("foo@bar.example.com", "<p>foo@bar.example.com</p>", "");
        }
    }
        // ## Raw HTML
        //
        // Text between `<` and `>` that looks like an HTML tag is parsed as a
        // raw HTML tag and will be rendered in HTML without escaping.
        // Tag and attribute names are not limited to current HTML tags,
        // so custom tags (and even, say, DocBook tags) may be used.
        //
        // Here is the grammar for tags:
        //
        // A [tag name](@) consists of an ASCII letter
        // followed by zero or more ASCII letters, digits, or
        // hyphens (`-`).
        //
        // An [attribute](@) consists of [whitespace],
        // an [attribute name], and an optional
        // [attribute value specification].
        //
        // An [attribute name](@)
        // consists of an ASCII letter, `_`, or `:`, followed by zero or more ASCII
        // letters, digits, `_`, `.`, `:`, or `-`.  (Note:  This is the XML
        // specification restricted to ASCII.  HTML5 is laxer.)
        //
        // An [attribute value specification](@)
        // consists of optional [whitespace],
        // a `=` character, optional [whitespace], and an [attribute
        // value].
        //
        // An [attribute value](@)
        // consists of an [unquoted attribute value],
        // a [single-quoted attribute value], or a [double-quoted attribute value].
        //
        // An [unquoted attribute value](@)
        // is a nonempty string of characters not
        // including spaces, `"`, `'`, `=`, `<`, `>`, or `` ` ``.
        //
        // A [single-quoted attribute value](@)
        // consists of `'`, zero or more
        // characters not including `'`, and a final `'`.
        //
        // A [double-quoted attribute value](@)
        // consists of `"`, zero or more
        // characters not including `"`, and a final `"`.
        //
        // An [open tag](@) consists of a `<` character, a [tag name],
        // zero or more [attributes], optional [whitespace], an optional `/`
        // character, and a `>` character.
        //
        // A [closing tag](@) consists of the string `</`, a
        // [tag name], optional [whitespace], and the character `>`.
        //
        // An [HTML comment](@) consists of `<!--` + *text* + `-->`,
        // where *text* does not start with `>` or `->`, does not end with `-`,
        // and does not contain `--`.  (See the
        // [HTML5 spec](http://www.w3.org/TR/html5/syntax.html#comments).)
        //
        // A [processing instruction](@)
        // consists of the string `<?`, a string
        // of characters not including the string `?>`, and the string
        // `?>`.
        //
        // A [declaration](@) consists of the
        // string `<!`, a name consisting of one or more uppercase ASCII letters,
        // [whitespace], a string of characters not including the
        // character `>`, and the character `>`.
        //
        // A [CDATA section](@) consists of
        // the string `<![CDATA[`, a string of characters not including the string
        // `]]>`, and the string `]]>`.
        //
        // An [HTML tag](@) consists of an [open tag], a [closing tag],
        // an [HTML comment], a [processing instruction], a [declaration],
        // or a [CDATA section].
        //
        // Here are some simple open tags:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example578()
        {
            // Example 578
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     <a><bab><c2c>
            //
            // Should be rendered as:
            //     <p><a><bab><c2c></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 578, "Inlines Raw HTML");
			TestParser.TestSpec("<a><bab><c2c>", "<p><a><bab><c2c></p>", "");
        }
    }
        // Empty elements:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example579()
        {
            // Example 579
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     <a/><b2/>
            //
            // Should be rendered as:
            //     <p><a/><b2/></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 579, "Inlines Raw HTML");
			TestParser.TestSpec("<a/><b2/>", "<p><a/><b2/></p>", "");
        }
    }
        // [Whitespace] is allowed:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example580()
        {
            // Example 580
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     <a  /><b2
            //     data="foo" >
            //
            // Should be rendered as:
            //     <p><a  /><b2
            //     data="foo" ></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 580, "Inlines Raw HTML");
			TestParser.TestSpec("<a  /><b2\ndata=\"foo\" >", "<p><a  /><b2\ndata=\"foo\" ></p>", "");
        }
    }
        // With attributes:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example581()
        {
            // Example 581
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     <a foo="bar" bam = 'baz <em>"</em>'
            //     _boolean zoop:33=zoop:33 />
            //
            // Should be rendered as:
            //     <p><a foo="bar" bam = 'baz <em>"</em>'
            //     _boolean zoop:33=zoop:33 /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 581, "Inlines Raw HTML");
			TestParser.TestSpec("<a foo=\"bar\" bam = 'baz <em>\"</em>'\n_boolean zoop:33=zoop:33 />", "<p><a foo=\"bar\" bam = 'baz <em>\"</em>'\n_boolean zoop:33=zoop:33 /></p>", "");
        }
    }
        // Custom tag names can be used:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example582()
        {
            // Example 582
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     Foo <responsive-image src="foo.jpg" />
            //
            // Should be rendered as:
            //     <p>Foo <responsive-image src="foo.jpg" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 582, "Inlines Raw HTML");
			TestParser.TestSpec("Foo <responsive-image src=\"foo.jpg\" />", "<p>Foo <responsive-image src=\"foo.jpg\" /></p>", "");
        }
    }
        // Illegal tag names, not parsed as HTML:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example583()
        {
            // Example 583
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     <33> <__>
            //
            // Should be rendered as:
            //     <p>&lt;33&gt; &lt;__&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 583, "Inlines Raw HTML");
			TestParser.TestSpec("<33> <__>", "<p>&lt;33&gt; &lt;__&gt;</p>", "");
        }
    }
        // Illegal attribute names:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example584()
        {
            // Example 584
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     <a h*#ref="hi">
            //
            // Should be rendered as:
            //     <p>&lt;a h*#ref=&quot;hi&quot;&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 584, "Inlines Raw HTML");
			TestParser.TestSpec("<a h*#ref=\"hi\">", "<p>&lt;a h*#ref=&quot;hi&quot;&gt;</p>", "");
        }
    }
        // Illegal attribute values:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example585()
        {
            // Example 585
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     <a href="hi'> <a href=hi'>
            //
            // Should be rendered as:
            //     <p>&lt;a href=&quot;hi'&gt; &lt;a href=hi'&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 585, "Inlines Raw HTML");
			TestParser.TestSpec("<a href=\"hi'> <a href=hi'>", "<p>&lt;a href=&quot;hi'&gt; &lt;a href=hi'&gt;</p>", "");
        }
    }
        // Illegal [whitespace]:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example586()
        {
            // Example 586
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     < a><
            //     foo><bar/ >
            //
            // Should be rendered as:
            //     <p>&lt; a&gt;&lt;
            //     foo&gt;&lt;bar/ &gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 586, "Inlines Raw HTML");
			TestParser.TestSpec("< a><\nfoo><bar/ >", "<p>&lt; a&gt;&lt;\nfoo&gt;&lt;bar/ &gt;</p>", "");
        }
    }
        // Missing [whitespace]:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example587()
        {
            // Example 587
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     <a href='bar'title=title>
            //
            // Should be rendered as:
            //     <p>&lt;a href='bar'title=title&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 587, "Inlines Raw HTML");
			TestParser.TestSpec("<a href='bar'title=title>", "<p>&lt;a href='bar'title=title&gt;</p>", "");
        }
    }
        // Closing tags:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example588()
        {
            // Example 588
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     </a></foo >
            //
            // Should be rendered as:
            //     <p></a></foo ></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 588, "Inlines Raw HTML");
			TestParser.TestSpec("</a></foo >", "<p></a></foo ></p>", "");
        }
    }
        // Illegal attributes in closing tag:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example589()
        {
            // Example 589
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     </a href="foo">
            //
            // Should be rendered as:
            //     <p>&lt;/a href=&quot;foo&quot;&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 589, "Inlines Raw HTML");
			TestParser.TestSpec("</a href=\"foo\">", "<p>&lt;/a href=&quot;foo&quot;&gt;</p>", "");
        }
    }
        // Comments:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example590()
        {
            // Example 590
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     foo <!-- this is a
            //     comment - with hyphen -->
            //
            // Should be rendered as:
            //     <p>foo <!-- this is a
            //     comment - with hyphen --></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 590, "Inlines Raw HTML");
			TestParser.TestSpec("foo <!-- this is a\ncomment - with hyphen -->", "<p>foo <!-- this is a\ncomment - with hyphen --></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example591()
        {
            // Example 591
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     foo <!-- not a comment -- two hyphens -->
            //
            // Should be rendered as:
            //     <p>foo &lt;!-- not a comment -- two hyphens --&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 591, "Inlines Raw HTML");
			TestParser.TestSpec("foo <!-- not a comment -- two hyphens -->", "<p>foo &lt;!-- not a comment -- two hyphens --&gt;</p>", "");
        }
    }
        // Not comments:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example592()
        {
            // Example 592
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     foo <!--> foo -->
            //     
            //     foo <!-- foo--->
            //
            // Should be rendered as:
            //     <p>foo &lt;!--&gt; foo --&gt;</p>
            //     <p>foo &lt;!-- foo---&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 592, "Inlines Raw HTML");
			TestParser.TestSpec("foo <!--> foo -->\n\nfoo <!-- foo--->", "<p>foo &lt;!--&gt; foo --&gt;</p>\n<p>foo &lt;!-- foo---&gt;</p>", "");
        }
    }
        // Processing instructions:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example593()
        {
            // Example 593
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     foo <?php echo $a; ?>
            //
            // Should be rendered as:
            //     <p>foo <?php echo $a; ?></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 593, "Inlines Raw HTML");
			TestParser.TestSpec("foo <?php echo $a; ?>", "<p>foo <?php echo $a; ?></p>", "");
        }
    }
        // Declarations:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example594()
        {
            // Example 594
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     foo <!ELEMENT br EMPTY>
            //
            // Should be rendered as:
            //     <p>foo <!ELEMENT br EMPTY></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 594, "Inlines Raw HTML");
			TestParser.TestSpec("foo <!ELEMENT br EMPTY>", "<p>foo <!ELEMENT br EMPTY></p>", "");
        }
    }
        // CDATA sections:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example595()
        {
            // Example 595
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     foo <![CDATA[>&<]]>
            //
            // Should be rendered as:
            //     <p>foo <![CDATA[>&<]]></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 595, "Inlines Raw HTML");
			TestParser.TestSpec("foo <![CDATA[>&<]]>", "<p>foo <![CDATA[>&<]]></p>", "");
        }
    }
        // Entity and numeric character references are preserved in HTML
        // attributes:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example596()
        {
            // Example 596
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     foo <a href="&ouml;">
            //
            // Should be rendered as:
            //     <p>foo <a href="&ouml;"></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 596, "Inlines Raw HTML");
			TestParser.TestSpec("foo <a href=\"&ouml;\">", "<p>foo <a href=\"&ouml;\"></p>", "");
        }
    }
        // Backslash escapes do not work in HTML attributes:
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example597()
        {
            // Example 597
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     foo <a href="\*">
            //
            // Should be rendered as:
            //     <p>foo <a href="\*"></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 597, "Inlines Raw HTML");
			TestParser.TestSpec("foo <a href=\"\\*\">", "<p>foo <a href=\"\\*\"></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesRawHTML
    {
        [Test]
        public void Example598()
        {
            // Example 598
            // Section: Inlines Raw HTML
            //
            // The following CommonMark:
            //     <a href="\"">
            //
            // Should be rendered as:
            //     <p>&lt;a href=&quot;&quot;&quot;&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 598, "Inlines Raw HTML");
			TestParser.TestSpec("<a href=\"\\\"\">", "<p>&lt;a href=&quot;&quot;&quot;&gt;</p>", "");
        }
    }
        // ## Hard line breaks
        //
        // A line break (not in a code span or HTML tag) that is preceded
        // by two or more spaces and does not occur at the end of a block
        // is parsed as a [hard line break](@) (rendered
        // in HTML as a `<br />` tag):
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example599()
        {
            // Example 599
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     foo  
            //     baz
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     baz</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 599, "Inlines Hard line breaks");
			TestParser.TestSpec("foo  \nbaz", "<p>foo<br />\nbaz</p>", "");
        }
    }
        // For a more visible alternative, a backslash before the
        // [line ending] may be used instead of two spaces:
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example600()
        {
            // Example 600
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     foo\
            //     baz
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     baz</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 600, "Inlines Hard line breaks");
			TestParser.TestSpec("foo\\\nbaz", "<p>foo<br />\nbaz</p>", "");
        }
    }
        // More than two spaces can be used:
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example601()
        {
            // Example 601
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     foo       
            //     baz
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     baz</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 601, "Inlines Hard line breaks");
			TestParser.TestSpec("foo       \nbaz", "<p>foo<br />\nbaz</p>", "");
        }
    }
        // Leading spaces at the beginning of the next line are ignored:
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example602()
        {
            // Example 602
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     foo  
            //          bar
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 602, "Inlines Hard line breaks");
			TestParser.TestSpec("foo  \n     bar", "<p>foo<br />\nbar</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example603()
        {
            // Example 603
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     foo\
            //          bar
            //
            // Should be rendered as:
            //     <p>foo<br />
            //     bar</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 603, "Inlines Hard line breaks");
			TestParser.TestSpec("foo\\\n     bar", "<p>foo<br />\nbar</p>", "");
        }
    }
        // Line breaks can occur inside emphasis, links, and other constructs
        // that allow inline content:
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example604()
        {
            // Example 604
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     *foo  
            //     bar*
            //
            // Should be rendered as:
            //     <p><em>foo<br />
            //     bar</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 604, "Inlines Hard line breaks");
			TestParser.TestSpec("*foo  \nbar*", "<p><em>foo<br />\nbar</em></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example605()
        {
            // Example 605
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     *foo\
            //     bar*
            //
            // Should be rendered as:
            //     <p><em>foo<br />
            //     bar</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 605, "Inlines Hard line breaks");
			TestParser.TestSpec("*foo\\\nbar*", "<p><em>foo<br />\nbar</em></p>", "");
        }
    }
        // Line breaks do not occur inside code spans
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example606()
        {
            // Example 606
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     `code  
            //     span`
            //
            // Should be rendered as:
            //     <p><code>code span</code></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 606, "Inlines Hard line breaks");
			TestParser.TestSpec("`code  \nspan`", "<p><code>code span</code></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example607()
        {
            // Example 607
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     `code\
            //     span`
            //
            // Should be rendered as:
            //     <p><code>code\ span</code></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 607, "Inlines Hard line breaks");
			TestParser.TestSpec("`code\\\nspan`", "<p><code>code\\ span</code></p>", "");
        }
    }
        // or HTML tags:
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example608()
        {
            // Example 608
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     <a href="foo  
            //     bar">
            //
            // Should be rendered as:
            //     <p><a href="foo  
            //     bar"></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 608, "Inlines Hard line breaks");
			TestParser.TestSpec("<a href=\"foo  \nbar\">", "<p><a href=\"foo  \nbar\"></p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example609()
        {
            // Example 609
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     <a href="foo\
            //     bar">
            //
            // Should be rendered as:
            //     <p><a href="foo\
            //     bar"></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 609, "Inlines Hard line breaks");
			TestParser.TestSpec("<a href=\"foo\\\nbar\">", "<p><a href=\"foo\\\nbar\"></p>", "");
        }
    }
        // Hard line breaks are for separating inline content within a block.
        // Neither syntax for hard line breaks works at the end of a paragraph or
        // other block element:
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example610()
        {
            // Example 610
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     foo\
            //
            // Should be rendered as:
            //     <p>foo\</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 610, "Inlines Hard line breaks");
			TestParser.TestSpec("foo\\", "<p>foo\\</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example611()
        {
            // Example 611
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     foo  
            //
            // Should be rendered as:
            //     <p>foo</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 611, "Inlines Hard line breaks");
			TestParser.TestSpec("foo  ", "<p>foo</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example612()
        {
            // Example 612
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     ### foo\
            //
            // Should be rendered as:
            //     <h3>foo\</h3>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 612, "Inlines Hard line breaks");
			TestParser.TestSpec("### foo\\", "<h3>foo\\</h3>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesHardlinebreaks
    {
        [Test]
        public void Example613()
        {
            // Example 613
            // Section: Inlines Hard line breaks
            //
            // The following CommonMark:
            //     ### foo  
            //
            // Should be rendered as:
            //     <h3>foo</h3>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 613, "Inlines Hard line breaks");
			TestParser.TestSpec("### foo  ", "<h3>foo</h3>", "");
        }
    }
        // ## Soft line breaks
        //
        // A regular line break (not in a code span or HTML tag) that is not
        // preceded by two or more spaces or a backslash is parsed as a
        // [softbreak](@).  (A softbreak may be rendered in HTML either as a
        // [line ending] or as a space. The result will be the same in
        // browsers. In the examples here, a [line ending] will be used.)
    [TestFixture]
    public partial class TestInlinesSoftlinebreaks
    {
        [Test]
        public void Example614()
        {
            // Example 614
            // Section: Inlines Soft line breaks
            //
            // The following CommonMark:
            //     foo
            //     baz
            //
            // Should be rendered as:
            //     <p>foo
            //     baz</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 614, "Inlines Soft line breaks");
			TestParser.TestSpec("foo\nbaz", "<p>foo\nbaz</p>", "");
        }
    }
        // Spaces at the end of the line and beginning of the next line are
        // removed:
    [TestFixture]
    public partial class TestInlinesSoftlinebreaks
    {
        [Test]
        public void Example615()
        {
            // Example 615
            // Section: Inlines Soft line breaks
            //
            // The following CommonMark:
            //     foo 
            //      baz
            //
            // Should be rendered as:
            //     <p>foo
            //     baz</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 615, "Inlines Soft line breaks");
			TestParser.TestSpec("foo \n baz", "<p>foo\nbaz</p>", "");
        }
    }
        // A conforming parser may render a soft line break in HTML either as a
        // line break or as a space.
        //
        // A renderer may also provide an option to render soft line breaks
        // as hard line breaks.
        //
        // ## Textual content
        //
        // Any characters not given an interpretation by the above rules will
        // be parsed as plain textual content.
    [TestFixture]
    public partial class TestInlinesTextualcontent
    {
        [Test]
        public void Example616()
        {
            // Example 616
            // Section: Inlines Textual content
            //
            // The following CommonMark:
            //     hello $.;'there
            //
            // Should be rendered as:
            //     <p>hello $.;'there</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 616, "Inlines Textual content");
			TestParser.TestSpec("hello $.;'there", "<p>hello $.;'there</p>", "");
        }
    }
    [TestFixture]
    public partial class TestInlinesTextualcontent
    {
        [Test]
        public void Example617()
        {
            // Example 617
            // Section: Inlines Textual content
            //
            // The following CommonMark:
            //     Foo χρῆν
            //
            // Should be rendered as:
            //     <p>Foo χρῆν</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 617, "Inlines Textual content");
			TestParser.TestSpec("Foo χρῆν", "<p>Foo χρῆν</p>", "");
        }
    }
        // Internal spaces are preserved verbatim:
    [TestFixture]
    public partial class TestInlinesTextualcontent
    {
        [Test]
        public void Example618()
        {
            // Example 618
            // Section: Inlines Textual content
            //
            // The following CommonMark:
            //     Multiple     spaces
            //
            // Should be rendered as:
            //     <p>Multiple     spaces</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 618, "Inlines Textual content");
			TestParser.TestSpec("Multiple     spaces", "<p>Multiple     spaces</p>", "");
        }
    }
        // # Appendix: A parsing strategy
        //
        // In this appendix we describe some features of the parsing strategy
        // used in the CommonMark reference implementations.
        //
        // ## Overview
        //
        // Parsing has two phases:
        //
        // 1. In the first phase, lines of input are consumed and the block
        // structure of the document---its division into paragraphs, block quotes,
        // list items, and so on---is constructed.  Text is assigned to these
        // blocks but not parsed. Link reference definitions are parsed and a
        // map of links is constructed.
        //
        // 2. In the second phase, the raw text contents of paragraphs and headings
        // are parsed into sequences of Markdown inline elements (strings,
        // code spans, links, emphasis, and so on), using the map of link
        // references constructed in phase 1.
        //
        // At each point in processing, the document is represented as a tree of
        // **blocks**.  The root of the tree is a `document` block.  The `document`
        // may have any number of other blocks as **children**.  These children
        // may, in turn, have other blocks as children.  The last child of a block
        // is normally considered **open**, meaning that subsequent lines of input
        // can alter its contents.  (Blocks that are not open are **closed**.)
        // Here, for example, is a possible document tree, with the open blocks
        // marked by arrows:
        //
        // ``` tree
        // -> document
        // -> block_quote
        // paragraph
        // "Lorem ipsum dolor\nsit amet."
        // -> list (type=bullet tight=true bullet_char=-)
        // list_item
        // paragraph
        // "Qui *quodsi iracundia*"
        // -> list_item
        // -> paragraph
        // "aliquando id"
        // ```
        //
        // ## Phase 1: block structure
        //
        // Each line that is processed has an effect on this tree.  The line is
        // analyzed and, depending on its contents, the document may be altered
        // in one or more of the following ways:
        //
        // 1. One or more open blocks may be closed.
        // 2. One or more new blocks may be created as children of the
        // last open block.
        // 3. Text may be added to the last (deepest) open block remaining
        // on the tree.
        //
        // Once a line has been incorporated into the tree in this way,
        // it can be discarded, so input can be read in a stream.
        //
        // For each line, we follow this procedure:
        //
        // 1. First we iterate through the open blocks, starting with the
        // root document, and descending through last children down to the last
        // open block.  Each block imposes a condition that the line must satisfy
        // if the block is to remain open.  For example, a block quote requires a
        // `>` character.  A paragraph requires a non-blank line.
        // In this phase we may match all or just some of the open
        // blocks.  But we cannot close unmatched blocks yet, because we may have a
        // [lazy continuation line].
        //
        // 2.  Next, after consuming the continuation markers for existing
        // blocks, we look for new block starts (e.g. `>` for a block quote.
        // If we encounter a new block start, we close any blocks unmatched
        // in step 1 before creating the new block as a child of the last
        // matched block.
        //
        // 3.  Finally, we look at the remainder of the line (after block
        // markers like `>`, list markers, and indentation have been consumed).
        // This is text that can be incorporated into the last open
        // block (a paragraph, code block, heading, or raw HTML).
        //
        // Setext headings are formed when we see a line of a paragraph
        // that is a [setext heading underline].
        //
        // Reference link definitions are detected when a paragraph is closed;
        // the accumulated text lines are parsed to see if they begin with
        // one or more reference link definitions.  Any remainder becomes a
        // normal paragraph.
        //
        // We can see how this works by considering how the tree above is
        // generated by four lines of Markdown:
        //
        // ``` markdown
        // > Lorem ipsum dolor
        // sit amet.
        // > - Qui *quodsi iracundia*
        // > - aliquando id
        // ```
        //
        // At the outset, our document model is just
        //
        // ``` tree
        // -> document
        // ```
        //
        // The first line of our text,
        //
        // ``` markdown
        // > Lorem ipsum dolor
        // ```
        //
        // causes a `block_quote` block to be created as a child of our
        // open `document` block, and a `paragraph` block as a child of
        // the `block_quote`.  Then the text is added to the last open
        // block, the `paragraph`:
        //
        // ``` tree
        // -> document
        // -> block_quote
        // -> paragraph
        // "Lorem ipsum dolor"
        // ```
        //
        // The next line,
        //
        // ``` markdown
        // sit amet.
        // ```
        //
        // is a "lazy continuation" of the open `paragraph`, so it gets added
        // to the paragraph's text:
        //
        // ``` tree
        // -> document
        // -> block_quote
        // -> paragraph
        // "Lorem ipsum dolor\nsit amet."
        // ```
        //
        // The third line,
        //
        // ``` markdown
        // > - Qui *quodsi iracundia*
        // ```
        //
        // causes the `paragraph` block to be closed, and a new `list` block
        // opened as a child of the `block_quote`.  A `list_item` is also
        // added as a child of the `list`, and a `paragraph` as a child of
        // the `list_item`.  The text is then added to the new `paragraph`:
        //
        // ``` tree
        // -> document
        // -> block_quote
        // paragraph
        // "Lorem ipsum dolor\nsit amet."
        // -> list (type=bullet tight=true bullet_char=-)
        // -> list_item
        // -> paragraph
        // "Qui *quodsi iracundia*"
        // ```
        //
        // The fourth line,
        //
        // ``` markdown
        // > - aliquando id
        // ```
        //
        // causes the `list_item` (and its child the `paragraph`) to be closed,
        // and a new `list_item` opened up as child of the `list`.  A `paragraph`
        // is added as a child of the new `list_item`, to contain the text.
        // We thus obtain the final tree:
        //
        // ``` tree
        // -> document
        // -> block_quote
        // paragraph
        // "Lorem ipsum dolor\nsit amet."
        // -> list (type=bullet tight=true bullet_char=-)
        // list_item
        // paragraph
        // "Qui *quodsi iracundia*"
        // -> list_item
        // -> paragraph
        // "aliquando id"
        // ```
        //
        // ## Phase 2: inline structure
        //
        // Once all of the input has been parsed, all open blocks are closed.
        //
        // We then "walk the tree," visiting every node, and parse raw
        // string contents of paragraphs and headings as inlines.  At this
        // point we have seen all the link reference definitions, so we can
        // resolve reference links as we go.
        //
        // ``` tree
        // document
        // block_quote
        // paragraph
        // str "Lorem ipsum dolor"
        // softbreak
        // str "sit amet."
        // list (type=bullet tight=true bullet_char=-)
        // list_item
        // paragraph
        // str "Qui "
        // emph
        // str "quodsi iracundia"
        // list_item
        // paragraph
        // str "aliquando id"
        // ```
        //
        // Notice how the [line ending] in the first paragraph has
        // been parsed as a `softbreak`, and the asterisks in the first list item
        // have become an `emph`.
        //
        // ### An algorithm for parsing nested emphasis and links
        //
        // By far the trickiest part of inline parsing is handling emphasis,
        // strong emphasis, links, and images.  This is done using the following
        // algorithm.
        //
        // When we're parsing inlines and we hit either
        //
        // - a run of `*` or `_` characters, or
        // - a `[` or `![`
        //
        // we insert a text node with these symbols as its literal content, and we
        // add a pointer to this text node to the [delimiter stack](@).
        //
        // The [delimiter stack] is a doubly linked list.  Each
        // element contains a pointer to a text node, plus information about
        //
        // - the type of delimiter (`[`, `![`, `*`, `_`)
        // - the number of delimiters,
        // - whether the delimiter is "active" (all are active to start), and
        // - whether the delimiter is a potential opener, a potential closer,
        // or both (which depends on what sort of characters precede
        // and follow the delimiters).
        //
        // When we hit a `]` character, we call the *look for link or image*
        // procedure (see below).
        //
        // When we hit the end of the input, we call the *process emphasis*
        // procedure (see below), with `stack_bottom` = NULL.
        //
        // #### *look for link or image*
        //
        // Starting at the top of the delimiter stack, we look backwards
        // through the stack for an opening `[` or `![` delimiter.
        //
        // - If we don't find one, we return a literal text node `]`.
        //
        // - If we do find one, but it's not *active*, we remove the inactive
        // delimiter from the stack, and return a literal text node `]`.
        //
        // - If we find one and it's active, then we parse ahead to see if
        // we have an inline link/image, reference link/image, compact reference
        // link/image, or shortcut reference link/image.
        //
        // + If we don't, then we remove the opening delimiter from the
        // delimiter stack and return a literal text node `]`.
        //
        // + If we do, then
        //
        // * We return a link or image node whose children are the inlines
        // after the text node pointed to by the opening delimiter.
        //
        // * We run *process emphasis* on these inlines, with the `[` opener
        // as `stack_bottom`.
        //
        // * We remove the opening delimiter.
        //
        // * If we have a link (and not an image), we also set all
        // `[` delimiters before the opening delimiter to *inactive*.  (This
        // will prevent us from getting links within links.)
        //
        // #### *process emphasis*
        //
        // Parameter `stack_bottom` sets a lower bound to how far we
        // descend in the [delimiter stack].  If it is NULL, we can
        // go all the way to the bottom.  Otherwise, we stop before
        // visiting `stack_bottom`.
        //
        // Let `current_position` point to the element on the [delimiter stack]
        // just above `stack_bottom` (or the first element if `stack_bottom`
        // is NULL).
        //
        // We keep track of the `openers_bottom` for each delimiter
        // type (`*`, `_`).  Initialize this to `stack_bottom`.
        //
        // Then we repeat the following until we run out of potential
        // closers:
        //
        // - Move `current_position` forward in the delimiter stack (if needed)
        // until we find the first potential closer with delimiter `*` or `_`.
        // (This will be the potential closer closest
        // to the beginning of the input -- the first one in parse order.)
        //
        // - Now, look back in the stack (staying above `stack_bottom` and
        // the `openers_bottom` for this delimiter type) for the
        // first matching potential opener ("matching" means same delimiter).
        //
        // - If one is found:
        //
        // + Figure out whether we have emphasis or strong emphasis:
        // if both closer and opener spans have length >= 2, we have
        // strong, otherwise regular.
        //
        // + Insert an emph or strong emph node accordingly, after
        // the text node corresponding to the opener.
        //
        // + Remove any delimiters between the opener and closer from
        // the delimiter stack.
        //
        // + Remove 1 (for regular emph) or 2 (for strong emph) delimiters
        // from the opening and closing text nodes.  If they become empty
        // as a result, remove them and remove the corresponding element
        // of the delimiter stack.  If the closing node is removed, reset
        // `current_position` to the next element in the stack.
        //
        // - If none in found:
        //
        // + Set `openers_bottom` to the element before `current_position`.
        // (We know that there are no openers for this kind of closer up to and
        // including this point, so this puts a lower bound on future searches.)
        //
        // + If the closer at `current_position` is not a potential opener,
        // remove it from the delimiter stack (since we know it can't
        // be a closer either).
        //
        // + Advance `current_position` to the next element in the stack.
        //
        // After we're done, we remove all delimiters above `stack_bottom` from the
        // delimiter stack.
        //
        // # Extensions
        //
        // This section describes the different extensions supported:
        //
        // ## Pipe Table
        //
        // A pipe table is detected when:
        //
        // **Rule #1**
        // - Each line of a paragraph block have to contain at least a **column delimiter** `|` that is not embedded by either a code inline (backstick \`) or a HTML inline.
        // - The second row must separate the first header row from sub-sequent rows by containing a **header column separator** for each column separated by a column delimiter. A header column separator is:
        // - starting by optional spaces
        // - followed by an optional `:` to specify left align
        // - followed by a sequence of at least one `-` character
        // - followed by an optional `:` to specify right align (or center align if left align is also defined)
        // - ending by optional spaces
        //
        // Because a list has a higher precedence than a pipe table, a table header row separator requires at least 2 dashes `--` to start a header row:
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     a | b
            //     -- | -
            //     0 | 1
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Pipe Table");
			TestParser.TestSpec("a | b\n-- | -\n0 | 1", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // The following is also considered as a table, even if the second line starts like a list:
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     a | b
            //     - | -
            //     0 | 1
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions Pipe Table");
			TestParser.TestSpec("a | b\n- | -\n0 | 1", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // A pipe table with only one header row is allowed:
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example003()
        {
            // Example 3
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     a | b
            //     -- | --
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Extensions Pipe Table");
			TestParser.TestSpec("a | b\n-- | --", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n</table>", "pipetables|advanced");
        }
    }
        // After a row separator header, they will be interpreted as plain column:
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example004()
        {
            // Example 4
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     a | b
            //     -- | --
            //     -- | --
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>--</td>
            //     <td>--</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 4, "Extensions Pipe Table");
			TestParser.TestSpec("a | b\n-- | --\n-- | --", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>--</td>\n<td>--</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // But if a table doesn't start with a column delimiter, it is not interpreted as a table, even if following lines have a column delimiter
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example005()
        {
            // Example 5
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     a b
            //     c | d
            //     e | f
            //
            // Should be rendered as:
            //     <p>a b
            //     c | d
            //     e | f</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 5, "Extensions Pipe Table");
			TestParser.TestSpec("a b\nc | d\ne | f", "<p>a b\nc | d\ne | f</p>", "pipetables|advanced");
        }
    }
        // If a line doesn't have a column delimiter `|` the table is not detected
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example006()
        {
            // Example 6
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     a | b
            //     c no d
            //
            // Should be rendered as:
            //     <p>a | b
            //     c no d</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 6, "Extensions Pipe Table");
			TestParser.TestSpec("a | b\nc no d", "<p>a | b\nc no d</p>", "pipetables|advanced");
        }
    }
        // The number of columns in the first row determine the number of columns for the whole table. Any extra columns delimiter `|` for sub-sequent lines are converted to literal strings instead:
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example007()
        {
            // Example 7
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     a  | b 
            //     -- | --
            //     0  | 1 | 2
            //     3  | 4
            //     5  |
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1 | 2</td>
            //     </tr>
            //     <tr>
            //     <td>3</td>
            //     <td>4</td>
            //     </tr>
            //     <tr>
            //     <td>5</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 7, "Extensions Pipe Table");
			TestParser.TestSpec("a  | b \n-- | --\n0  | 1 | 2\n3  | 4\n5  |", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1 | 2</td>\n</tr>\n<tr>\n<td>3</td>\n<td>4</td>\n</tr>\n<tr>\n<td>5</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // **Rule #2**
        // A pipe table ends after a blank line or the end of the file.
        //
        // **Rule #3**
        // A cell content is trimmed (start and end) from white-spaces.
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example008()
        {
            // Example 8
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     a          | b              |
            //     -- | --
            //     0      | 1       |
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 8, "Extensions Pipe Table");
			TestParser.TestSpec("a          | b              |\n-- | --\n0      | 1       |", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // **Rule #4**
        // Column delimiters `|` at the very beginning of a line or just before a line ending with only spaces and/or terminated by a newline can be omitted
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example009()
        {
            // Example 9
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //       a     | b     |
            //     --      | --
            //     | 0     | 1
            //     | 2     | 3     |
            //       4     | 5 
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     <tr>
            //     <td>2</td>
            //     <td>3</td>
            //     </tr>
            //     <tr>
            //     <td>4</td>
            //     <td>5</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 9, "Extensions Pipe Table");
			TestParser.TestSpec("  a     | b     |\n--      | --\n| 0     | 1\n| 2     | 3     |\n  4     | 5 ", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n<tr>\n<td>2</td>\n<td>3</td>\n</tr>\n<tr>\n<td>4</td>\n<td>5</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // A pipe may be present at both the beginning/ending of each line:
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example010()
        {
            // Example 10
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     |a|b|
            //     |-|-|
            //     |0|1|
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 10, "Extensions Pipe Table");
			TestParser.TestSpec("|a|b|\n|-|-|\n|0|1|", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // Or may be ommitted on one side:
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example011()
        {
            // Example 11
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     a|b|
            //     -|-|
            //     0|1|
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 11, "Extensions Pipe Table");
			TestParser.TestSpec("a|b|\n-|-|\n0|1|", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example012()
        {
            // Example 12
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     |a|b
            //     |-|-
            //     |0|1
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 12, "Extensions Pipe Table");
			TestParser.TestSpec("|a|b\n|-|-\n|0|1", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // Single column table can be declared with lines starting only by a column delimiter:
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example013()
        {
            // Example 13
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     | a
            //     | --
            //     | b
            //     | c 
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>b</td>
            //     </tr>
            //     <tr>
            //     <td>c</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 13, "Extensions Pipe Table");
			TestParser.TestSpec("| a\n| --\n| b\n| c ", "<table>\n<thead>\n<tr>\n<th>a</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>b</td>\n</tr>\n<tr>\n<td>c</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // **Rule #5**
        //
        // The first row is considered as a **header row** if it is separated from the regular rows by a row containing a **header column separator** for each column. A header column separator is:
        //
        // - starting by optional spaces
        // - followed by an optional `:` to specify left align
        // - followed by a sequence of at least one `-` character
        // - followed by an optional `:` to specify right align (or center align if left align is also defined)
        // - ending by optional spaces
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example014()
        {
            // Example 14
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //      a     | b 
            //     -------|-------
            //      0     | 1 
            //      2     | 3 
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     <tr>
            //     <td>2</td>
            //     <td>3</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 14, "Extensions Pipe Table");
			TestParser.TestSpec(" a     | b \n-------|-------\n 0     | 1 \n 2     | 3 ", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n<tr>\n<td>2</td>\n<td>3</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // The text alignment is defined by default to be left.
        // The text alignment can be changed by using the character `:` with the header column separator:
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example015()
        {
            // Example 15
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //      a     | b       | c 
            //     :------|:-------:| ----:
            //      0     | 1       | 2 
            //      3     | 4       | 5 
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th style="text-align: center;">b</th>
            //     <th style="text-align: right;">c</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td style="text-align: center;">1</td>
            //     <td style="text-align: right;">2</td>
            //     </tr>
            //     <tr>
            //     <td>3</td>
            //     <td style="text-align: center;">4</td>
            //     <td style="text-align: right;">5</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 15, "Extensions Pipe Table");
			TestParser.TestSpec(" a     | b       | c \n:------|:-------:| ----:\n 0     | 1       | 2 \n 3     | 4       | 5 ", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th style=\"text-align: center;\">b</th>\n<th style=\"text-align: right;\">c</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td style=\"text-align: center;\">1</td>\n<td style=\"text-align: right;\">2</td>\n</tr>\n<tr>\n<td>3</td>\n<td style=\"text-align: center;\">4</td>\n<td style=\"text-align: right;\">5</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // Test alignment with starting and ending pipes:
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example016()
        {
            // Example 16
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     | abc | def | ghi |
            //     |:---:|-----|----:|
            //     |  1  | 2   | 3   |
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th style="text-align: center;">abc</th>
            //     <th>def</th>
            //     <th style="text-align: right;">ghi</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td style="text-align: center;">1</td>
            //     <td>2</td>
            //     <td style="text-align: right;">3</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 16, "Extensions Pipe Table");
			TestParser.TestSpec("| abc | def | ghi |\n|:---:|-----|----:|\n|  1  | 2   | 3   |", "<table>\n<thead>\n<tr>\n<th style=\"text-align: center;\">abc</th>\n<th>def</th>\n<th style=\"text-align: right;\">ghi</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td style=\"text-align: center;\">1</td>\n<td>2</td>\n<td style=\"text-align: right;\">3</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // The following example shows a non matching header column separator:
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example017()
        {
            // Example 17
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //      a     | b
            //     -------|---x---
            //      0     | 1
            //      2     | 3 
            //
            // Should be rendered as:
            //     <p>a     | b
            //     -------|---x---
            //     0     | 1
            //     2     | 3</p> 

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 17, "Extensions Pipe Table");
			TestParser.TestSpec(" a     | b\n-------|---x---\n 0     | 1\n 2     | 3 ", "<p>a     | b\n-------|---x---\n0     | 1\n2     | 3</p> ", "pipetables|advanced");
        }
    }
        // **Rule #6**
        //
        // A column delimiter has a higher priority than emphasis delimiter
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example018()
        {
            // Example 18
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //      *a*   | b
            //     -----  |-----
            //      0     | _1_
            //      _2    | 3* 
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th><em>a</em></th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td><em>1</em></td>
            //     </tr>
            //     <tr>
            //     <td>_2</td>
            //     <td>3*</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 18, "Extensions Pipe Table");
			TestParser.TestSpec(" *a*   | b\n-----  |-----\n 0     | _1_\n _2    | 3* ", "<table>\n<thead>\n<tr>\n<th><em>a</em></th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td><em>1</em></td>\n</tr>\n<tr>\n<td>_2</td>\n<td>3*</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // **Rule #7**
        //
        // A backstick/code delimiter has a higher precedence than a column delimiter `|`:
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example019()
        {
            // Example 19
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     a | b `
            //     0 | ` 
            //
            // Should be rendered as:
            //     <p>a | b <code>0 |</code></p> 

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 19, "Extensions Pipe Table");
			TestParser.TestSpec("a | b `\n0 | ` ", "<p>a | b <code>0 |</code></p> ", "pipetables|advanced");
        }
    }
        // **Rule #7**
        //
        // A HTML inline has a higher precedence than a column delimiter `|`:
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example020()
        {
            // Example 20
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     a <a href="" title="|"></a> | b
            //     -- | --
            //     0  | 1
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a <a href="" title="|"></a></th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 20, "Extensions Pipe Table");
			TestParser.TestSpec("a <a href=\"\" title=\"|\"></a> | b\n-- | --\n0  | 1", "<table>\n<thead>\n<tr>\n<th>a <a href=\"\" title=\"|\"></a></th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // **Rule #8**
        //
        // Links have a higher precedence than the column delimiter character `|`:
    [TestFixture]
    public partial class TestExtensionsPipeTable
    {
        [Test]
        public void Example021()
        {
            // Example 21
            // Section: Extensions Pipe Table
            //
            // The following CommonMark:
            //     a  | b
            //     -- | --
            //     [This is a link with a | inside the label](http://google.com) | 1
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td><a href="http://google.com">This is a link with a | inside the label</a></td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 21, "Extensions Pipe Table");
			TestParser.TestSpec("a  | b\n-- | --\n[This is a link with a | inside the label](http://google.com) | 1", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td><a href=\"http://google.com\">This is a link with a | inside the label</a></td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "pipetables|advanced");
        }
    }
        // # Extensions
        //
        // This section describes the different extensions supported:
        //
        // ## Footontes
        //
        // Allows footnotes using the following syntax (taken from pandoc example):
    [TestFixture]
    public partial class TestExtensionsFootontes
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Footontes
            //
            // The following CommonMark:
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

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Footontes");
			TestParser.TestSpec("Here is a footnote reference,[^1] and another.[^longnote]\n\nThis is another reference to [^1]\n\n[^1]: Here is the footnote.\n\nAnd another reference to [^longnote]\n\n[^longnote]: Here's one with multiple blocks.\n\n    Subsequent paragraphs are indented to show that they\nbelong to the previous footnote.\n\n    > This is a block quote\n    > Inside a footnote\n\n        { some.code }\n\n    The whole paragraph can be indented, or just the first\n    line.  In this way, multi-paragraph footnotes work like\n    multi-paragraph list items.\n\nThis paragraph won't be part of the note, because it\nisn't indented.", "<p>Here is a footnote reference,<a id=\"fnref:1\" href=\"#fn:1\" class=\"footnote-ref\"><sup>1</sup></a> and another.<a id=\"fnref:3\" href=\"#fn:2\" class=\"footnote-ref\"><sup>2</sup></a></p>\n<p>This is another reference to <a id=\"fnref:2\" href=\"#fn:1\" class=\"footnote-ref\"><sup>1</sup></a></p>\n<p>And another reference to <a id=\"fnref:4\" href=\"#fn:2\" class=\"footnote-ref\"><sup>2</sup></a></p>\n<p>This paragraph won't be part of the note, because it\nisn't indented.</p>\n<div class=\"footnotes\">\n<hr />\n<ol>\n<li id=\"fn:1\">\n<p>Here is the footnote.<a href=\"#fnref:1\" class=\"footnote-back-ref\">&#8617;</a><a href=\"#fnref:2\" class=\"footnote-back-ref\">&#8617;</a></p>\n</li>\n<li id=\"fn:2\">\n<p>Here's one with multiple blocks.</p>\n<p>Subsequent paragraphs are indented to show that they\nbelong to the previous footnote.</p>\n<blockquote>\n<p>This is a block quote\nInside a footnote</p>\n</blockquote>\n<pre><code>{ some.code }\n</code></pre>\n<p>The whole paragraph can be indented, or just the first\nline.  In this way, multi-paragraph footnotes work like\nmulti-paragraph list items.<a href=\"#fnref:3\" class=\"footnote-back-ref\">&#8617;</a><a href=\"#fnref:4\" class=\"footnote-back-ref\">&#8617;</a></p>\n</li>\n</ol>\n</div>", "footnotes|advanced");
        }
    }
        // Check with mulitple consecutive footnotes:
    [TestFixture]
    public partial class TestExtensionsFootontes
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions Footontes
            //
            // The following CommonMark:
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

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions Footontes");
			TestParser.TestSpec("Here is a footnote[^1]. And another one[^2]. And a third one[^3]. And a fourth[^4].\n\n[^1]: Footnote 1 text\n\n[^2]: Footnote 2 text\n\na\n\n[^3]: Footnote 3 text\n\n[^4]: Footnote 4 text", "<p>Here is a footnote<a id=\"fnref:1\" href=\"#fn:1\" class=\"footnote-ref\"><sup>1</sup></a>. And another one<a id=\"fnref:2\" href=\"#fn:2\" class=\"footnote-ref\"><sup>2</sup></a>. And a third one<a id=\"fnref:3\" href=\"#fn:3\" class=\"footnote-ref\"><sup>3</sup></a>. And a fourth<a id=\"fnref:4\" href=\"#fn:4\" class=\"footnote-ref\"><sup>4</sup></a>.</p>\n<p>a</p>\n<div class=\"footnotes\">\n<hr />\n<ol>\n<li id=\"fn:1\">\n<p>Footnote 1 text<a href=\"#fnref:1\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n<li id=\"fn:2\">\n<p>Footnote 2 text<a href=\"#fnref:2\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n<li id=\"fn:3\">\n<p>Footnote 3 text<a href=\"#fnref:3\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n<li id=\"fn:4\">\n<p>Footnote 4 text<a href=\"#fnref:4\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n</ol>\n</div>", "footnotes|advanced");
        }
    }
        // Another test with consecutive footnotes without a blank line separator:
    [TestFixture]
    public partial class TestExtensionsFootontes
    {
        [Test]
        public void Example003()
        {
            // Example 3
            // Section: Extensions Footontes
            //
            // The following CommonMark:
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

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Extensions Footontes");
			TestParser.TestSpec("Here is a footnote[^1]. And another one[^2]. And a third one[^3]. And a fourth[^4].\n\n[^1]: Footnote 1 text\n[^2]: Footnote 2 text\n[^3]: Footnote 3 text\n[^4]: Footnote 4 text", "<p>Here is a footnote<a id=\"fnref:1\" href=\"#fn:1\" class=\"footnote-ref\"><sup>1</sup></a>. And another one<a id=\"fnref:2\" href=\"#fn:2\" class=\"footnote-ref\"><sup>2</sup></a>. And a third one<a id=\"fnref:3\" href=\"#fn:3\" class=\"footnote-ref\"><sup>3</sup></a>. And a fourth<a id=\"fnref:4\" href=\"#fn:4\" class=\"footnote-ref\"><sup>4</sup></a>.</p>\n<div class=\"footnotes\">\n<hr />\n<ol>\n<li id=\"fn:1\">\n<p>Footnote 1 text<a href=\"#fnref:1\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n<li id=\"fn:2\">\n<p>Footnote 2 text<a href=\"#fnref:2\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n<li id=\"fn:3\">\n<p>Footnote 3 text<a href=\"#fnref:3\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n<li id=\"fn:4\">\n<p>Footnote 4 text<a href=\"#fnref:4\" class=\"footnote-back-ref\">&#8617;</a></p></li>\n</ol>\n</div>", "footnotes|advanced");
        }
    }
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
    [TestFixture]
    public partial class TestExtensionsGenericAttributes
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Generic Attributes
            //
            // The following CommonMark:
            //     # This is a heading with an an attribute{#heading-link}
            //     
            //     # This is a heading # {#heading-link2}
            //     
            //     [This is a link](http://google.com){#a-link .myclass data-lang=fr data-value="This is a value"}
            //     
            //     This is a heading{#heading-link2}
            //     -----------------
            //     
            //     This is a paragraph with an attached attributes {#myparagraph attached-bool-property}
            //
            // Should be rendered as:
            //     <h1 id="heading-link">This is a heading with an an attribute</h1>
            //     <h1 id="heading-link2">This is a heading</h1>
            //     <p><a href="http://google.com" id="a-link" class="myclass" data-lang="fr" data-value="This is a value">This is a link</a></p>
            //     <h2 id="heading-link2">This is a heading</h2>
            //     <p id="myparagraph" attached-bool-property>This is a paragraph with an attached attributes </p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Generic Attributes");
			TestParser.TestSpec("# This is a heading with an an attribute{#heading-link}\n\n# This is a heading # {#heading-link2}\n\n[This is a link](http://google.com){#a-link .myclass data-lang=fr data-value=\"This is a value\"}\n\nThis is a heading{#heading-link2}\n-----------------\n\nThis is a paragraph with an attached attributes {#myparagraph attached-bool-property}", "<h1 id=\"heading-link\">This is a heading with an an attribute</h1>\n<h1 id=\"heading-link2\">This is a heading</h1>\n<p><a href=\"http://google.com\" id=\"a-link\" class=\"myclass\" data-lang=\"fr\" data-value=\"This is a value\">This is a link</a></p>\n<h2 id=\"heading-link2\">This is a heading</h2>\n<p id=\"myparagraph\" attached-bool-property>This is a paragraph with an attached attributes </p>", "attributes|advanced");
        }
    }
        // The following shows that attributes can be attached to the next block if they are used inside a single line just preceding the block (and preceded by a blank line or beginning of a block container):
    [TestFixture]
    public partial class TestExtensionsGenericAttributes
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions Generic Attributes
            //
            // The following CommonMark:
            //     {#fenced-id .fenced-class}
            //     ~~~
            //     This is a fenced with attached attributes
            //     ~~~ 
            //
            // Should be rendered as:
            //     <pre><code id="fenced-id" class="fenced-class">This is a fenced with attached attributes
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions Generic Attributes");
			TestParser.TestSpec("{#fenced-id .fenced-class}\n~~~\nThis is a fenced with attached attributes\n~~~ ", "<pre><code id=\"fenced-id\" class=\"fenced-class\">This is a fenced with attached attributes\n</code></pre>", "attributes|advanced");
        }
    }
        // # Extensions
        //
        // The following additional emphasis are supported:
        //
        // ## Strikethrough
        //
        // Allows to strikethrough a span of text by surrounding it by `~~`. The semantic used for the generated HTML is the tag `<del>`.
    [TestFixture]
    public partial class TestExtensionsStrikethrough
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Strikethrough
            //
            // The following CommonMark:
            //     The following text ~~is deleted~~
            //
            // Should be rendered as:
            //     <p>The following text <del>is deleted</del></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Strikethrough");
			TestParser.TestSpec("The following text ~~is deleted~~", "<p>The following text <del>is deleted</del></p>", "emphasisextras|advanced");
        }
    }
        // ## Superscript and Subscript
        //
        // Superscripts can be written by surrounding a text by ^ characters; subscripts can be written by surrounding the subscripted text by ~ characters
    [TestFixture]
    public partial class TestExtensionsSuperscriptandSubscript
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions Superscript and Subscript
            //
            // The following CommonMark:
            //     H~2~O is a liquid. 2^10^ is 1024
            //
            // Should be rendered as:
            //     <p>H<sub>2</sub>O is a liquid. 2<sup>10</sup> is 1024</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions Superscript and Subscript");
			TestParser.TestSpec("H~2~O is a liquid. 2^10^ is 1024", "<p>H<sub>2</sub>O is a liquid. 2<sup>10</sup> is 1024</p>", "emphasisextras|advanced");
        }
    }
        // ## Inserted
        //
        // Inserted text can be used to specify that a text has been added to a document.  The semantic used for the generated HTML is the tag `<ins>`.
    [TestFixture]
    public partial class TestExtensionsInserted
    {
        [Test]
        public void Example003()
        {
            // Example 3
            // Section: Extensions Inserted
            //
            // The following CommonMark:
            //     ++Inserted text++
            //
            // Should be rendered as:
            //     <p><ins>Inserted text</ins></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Extensions Inserted");
			TestParser.TestSpec("++Inserted text++", "<p><ins>Inserted text</ins></p>", "emphasisextras|advanced");
        }
    }
        // ## Marked
        //
        // Marked text can be used to specify that a text has been marked in a document.  The semantic used for the generated HTML is the tag `<mark>`.
    [TestFixture]
    public partial class TestExtensionsMarked
    {
        [Test]
        public void Example004()
        {
            // Example 4
            // Section: Extensions Marked
            //
            // The following CommonMark:
            //     ==Marked text==
            //
            // Should be rendered as:
            //     <p><mark>Marked text</mark></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 4, "Extensions Marked");
			TestParser.TestSpec("==Marked text==", "<p><mark>Marked text</mark></p>", "emphasisextras|advanced");
        }
    }
        // # Extensions
        //
        // This section describes the different extensions supported:
        //
        // ## Hardline break
        //
        // When this extension is used, a new line in a paragraph block will result in a hardline break `<br>`:
    [TestFixture]
    public partial class TestExtensionsHardlinebreak
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Hardline break
            //
            // The following CommonMark:
            //     This is a paragraph
            //     with a break inside
            //
            // Should be rendered as:
            //     <p>This is a paragraph<br />
            //     with a break inside</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Hardline break");
			TestParser.TestSpec("This is a paragraph\nwith a break inside", "<p>This is a paragraph<br />\nwith a break inside</p>", "hardlinebreak|advanced+hardlinebreak");
        }
    }
        // # Extensions
        //
        // This section describes the different extensions supported:
        //
        // ## Grid Table
        //
        // A grid table allows to have multiple lines per cells and allows to span cells over multiple columns. The following shows a simple grid table
        //
        // ```
        // +---------+---------+
        // | Header  | Header  |
        // | Column1 | Column2 |
        // +=========+=========+
        // | 1. ab   | > This is a quote
        // | 2. cde  | > For the second column
        // | 3. f    |
        // +---------+---------+
        // | Second row spanning
        // | on two columns
        // +---------+---------+
        // | Back    |         |
        // | to      |         |
        // | one     |         |
        // | column  |         |
        // ```
        //
        // **Rule #1**
        // The first line of a grid table must a **row separator**. It must start with the column separator character `+` used to separate columns in a row separator. Each column separator is:
        // - starting by optional spaces
        // - followed by an optional `:` to specify left align, followed by optional spaces
        // - followed by a sequence of at least one `-` character, followed by optional spaces
        // - followed by an optional `:` to specify right align (or center align if left align is also defined)
        // - ending by optional spaces
        //
        // The first row separator must be followed by a *regular row*. A regular row must start with the character `|` that is starting at the same position than the column separator `+` of the first row separator.
        //
        // The following is a valid row separator
    [TestFixture]
    public partial class TestExtensionsGridTable
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Grid Table
            //
            // The following CommonMark:
            //     +---------+---------+
            //     | This is | a table |
            //
            // Should be rendered as:
            //     <table>
            //     <col style="width:50%">
            //     <col style="width:50%">
            //     <tbody>
            //     <tr>
            //     <td>This is</td>
            //     <td>a table</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Grid Table");
			TestParser.TestSpec("+---------+---------+\n| This is | a table |", "<table>\n<col style=\"width:50%\">\n<col style=\"width:50%\">\n<tbody>\n<tr>\n<td>This is</td>\n<td>a table</td>\n</tr>\n</tbody>\n</table>", "gridtables|advanced");
        }
    }
        // The following is not a valid row separator
    [TestFixture]
    public partial class TestExtensionsGridTable
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions Grid Table
            //
            // The following CommonMark:
            //     |-----xxx----+---------+
            //     | This is    | not a table
            //
            // Should be rendered as:
            //     <p>|-----xxx----+---------+
            //     | This is    | not a table</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions Grid Table");
			TestParser.TestSpec("|-----xxx----+---------+\n| This is    | not a table", "<p>|-----xxx----+---------+\n| This is    | not a table</p>", "gridtables|advanced");
        }
    }
        // **Rule #2**
        // A regular row can continue a previous regular row when column separator `|` are positioned at the same  position than the previous line. If they are positioned at the same location, the column may span over multiple columns:
    [TestFixture]
    public partial class TestExtensionsGridTable
    {
        [Test]
        public void Example003()
        {
            // Example 3
            // Section: Extensions Grid Table
            //
            // The following CommonMark:
            //     +---------+---------+---------+
            //     | Col1    | Col2    | Col3    |
            //     | Col1a   | Col2a   | Col3a   |
            //     | Col1b             | Col3b   |
            //     | Col1c                       |
            //
            // Should be rendered as:
            //     <table>
            //     <col style="width:33.33%">
            //     <col style="width:33.33%">
            //     <col style="width:33.33%">
            //     <tbody>
            //     <tr>
            //     <td>Col1
            //     Col1a</td>
            //     <td>Col2
            //     Col2a</td>
            //     <td>Col3
            //     Col3a</td>
            //     </tr>
            //     <tr>
            //     <td colspan="2">Col1b</td>
            //     <td>Col3b</td>
            //     </tr>
            //     <tr>
            //     <td colspan="3">Col1c</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Extensions Grid Table");
			TestParser.TestSpec("+---------+---------+---------+\n| Col1    | Col2    | Col3    |\n| Col1a   | Col2a   | Col3a   |\n| Col1b             | Col3b   |\n| Col1c                       |", "<table>\n<col style=\"width:33.33%\">\n<col style=\"width:33.33%\">\n<col style=\"width:33.33%\">\n<tbody>\n<tr>\n<td>Col1\nCol1a</td>\n<td>Col2\nCol2a</td>\n<td>Col3\nCol3a</td>\n</tr>\n<tr>\n<td colspan=\"2\">Col1b</td>\n<td>Col3b</td>\n</tr>\n<tr>\n<td colspan=\"3\">Col1c</td>\n</tr>\n</tbody>\n</table>", "gridtables|advanced");
        }
    }
        // A row header is separated using `+========+` instead of `+---------+`:
    [TestFixture]
    public partial class TestExtensionsGridTable
    {
        [Test]
        public void Example004()
        {
            // Example 4
            // Section: Extensions Grid Table
            //
            // The following CommonMark:
            //     +---------+---------+
            //     | This is | a table |
            //     +=========+=========+
            //
            // Should be rendered as:
            //     <table>
            //     <col style="width:50%">
            //     <col style="width:50%">
            //     <thead>
            //     <tr>
            //     <th>This is</th>
            //     <th>a table</th>
            //     </tr>
            //     </thead>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 4, "Extensions Grid Table");
			TestParser.TestSpec("+---------+---------+\n| This is | a table |\n+=========+=========+", "<table>\n<col style=\"width:50%\">\n<col style=\"width:50%\">\n<thead>\n<tr>\n<th>This is</th>\n<th>a table</th>\n</tr>\n</thead>\n</table>", "gridtables|advanced");
        }
    }
        // The last column separator `|` may be omitted:
    [TestFixture]
    public partial class TestExtensionsGridTable
    {
        [Test]
        public void Example005()
        {
            // Example 5
            // Section: Extensions Grid Table
            //
            // The following CommonMark:
            //     +---------+---------+
            //     | This is | a table with a longer text in the second column
            //
            // Should be rendered as:
            //     <table>
            //     <col style="width:50%">
            //     <col style="width:50%">
            //     <tbody>
            //     <tr>
            //     <td>This is</td>
            //     <td>a table with a longer text in the second column</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 5, "Extensions Grid Table");
			TestParser.TestSpec("+---------+---------+\n| This is | a table with a longer text in the second column", "<table>\n<col style=\"width:50%\">\n<col style=\"width:50%\">\n<tbody>\n<tr>\n<td>This is</td>\n<td>a table with a longer text in the second column</td>\n</tr>\n</tbody>\n</table>", "gridtables|advanced");
        }
    }
        // The respective width of the columns are calculated from the ratio between the total size of the first table row without counting the `+`: `+----+--------+----+` would be divided between:
        //
        // Total size is : 16
        //
        // - `----` -> 4
        // - `--------` -> 8
        // - `----` -> 4
        //
        // So the width would be 4/16 = 25%, 8/16 = 50%, 4/16 = 25%
    [TestFixture]
    public partial class TestExtensionsGridTable
    {
        [Test]
        public void Example006()
        {
            // Example 6
            // Section: Extensions Grid Table
            //
            // The following CommonMark:
            //     +----+--------+----+
            //     | A  |  B C D | E  |
            //     +----+--------+----+
            //
            // Should be rendered as:
            //     <table>
            //     <col style="width:25%">
            //     <col style="width:50%">
            //     <col style="width:25%">
            //     <tbody>
            //     <tr>
            //     <td>A</td>
            //     <td>B C D</td>
            //     <td>E</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 6, "Extensions Grid Table");
			TestParser.TestSpec("+----+--------+----+\n| A  |  B C D | E  |\n+----+--------+----+", "<table>\n<col style=\"width:25%\">\n<col style=\"width:50%\">\n<col style=\"width:25%\">\n<tbody>\n<tr>\n<td>A</td>\n<td>B C D</td>\n<td>E</td>\n</tr>\n</tbody>\n</table>", "gridtables|advanced");
        }
    }
        // Alignment might be specified on the first row using the character `:`:
    [TestFixture]
    public partial class TestExtensionsGridTable
    {
        [Test]
        public void Example007()
        {
            // Example 7
            // Section: Extensions Grid Table
            //
            // The following CommonMark:
            //     +-----+:---:+-----+
            //     |  A  |  B  |  C  |
            //     +-----+-----+-----+
            //
            // Should be rendered as:
            //     <table>
            //     <col style="width:33.33%">
            //     <col style="width:33.33%">
            //     <col style="width:33.33%">
            //     <tbody>
            //     <tr>
            //     <td>A</td>
            //     <td style="text-align: center;">B</td>
            //     <td>C</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 7, "Extensions Grid Table");
			TestParser.TestSpec("+-----+:---:+-----+\n|  A  |  B  |  C  |\n+-----+-----+-----+", "<table>\n<col style=\"width:33.33%\">\n<col style=\"width:33.33%\">\n<col style=\"width:33.33%\">\n<tbody>\n<tr>\n<td>A</td>\n<td style=\"text-align: center;\">B</td>\n<td>C</td>\n</tr>\n</tbody>\n</table>", "gridtables|advanced");
        }
    }
        // # Extensions
        //
        // This section describes the different extensions supported:
        //
        // ## Custom Container
        //
        // A custom container is similar to a fenced code block, but it is using the character `:` to declare a block (with at least 3 characters), and instead of generating a `<pre><code>...</code></pre>` it will generate a `<div>...</dib>` block.
    [TestFixture]
    public partial class TestExtensionsCustomContainer
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Custom Container
            //
            // The following CommonMark:
            //     :::spoiler
            //     This is a *spoiler*
            //     :::
            //
            // Should be rendered as:
            //     <div class="spoiler"><p>This is a <em>spoiler</em></p>
            //     </div>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Custom Container");
			TestParser.TestSpec(":::spoiler\nThis is a *spoiler*\n:::", "<div class=\"spoiler\"><p>This is a <em>spoiler</em></p>\n</div>", "customcontainers+attributes|advanced");
        }
    }
        // The text following the opened custom container is optional:
    [TestFixture]
    public partial class TestExtensionsCustomContainer
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions Custom Container
            //
            // The following CommonMark:
            //     :::
            //     This is a regular div
            //     :::
            //
            // Should be rendered as:
            //     <div><p>This is a regular div</p>
            //     </div>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions Custom Container");
			TestParser.TestSpec(":::\nThis is a regular div\n:::", "<div><p>This is a regular div</p>\n</div>", "customcontainers+attributes|advanced");
        }
    }
        // Like for fenced code block, you can use more than 3 `:` characters as long as the closing has the same number of characters:
    [TestFixture]
    public partial class TestExtensionsCustomContainer
    {
        [Test]
        public void Example003()
        {
            // Example 3
            // Section: Extensions Custom Container
            //
            // The following CommonMark:
            //     ::::::::::::spoiler
            //     This is a spoiler
            //     ::::::::::::
            //
            // Should be rendered as:
            //     <div class="spoiler"><p>This is a spoiler</p>
            //     </div>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Extensions Custom Container");
			TestParser.TestSpec("::::::::::::spoiler\nThis is a spoiler\n::::::::::::", "<div class=\"spoiler\"><p>This is a spoiler</p>\n</div>", "customcontainers+attributes|advanced");
        }
    }
        // Like for fenced code block, a custom container can span over multiple empty lines in a list block:
    [TestFixture]
    public partial class TestExtensionsCustomContainer
    {
        [Test]
        public void Example004()
        {
            // Example 4
            // Section: Extensions Custom Container
            //
            // The following CommonMark:
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

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 4, "Extensions Custom Container");
			TestParser.TestSpec("- This is a list\n  :::spoiler\n  This is a spoiler\n  - item1\n  - item2\n  :::\n- A second item in the list", "<ul>\n<li>This is a list\n<div class=\"spoiler\">This is a spoiler\n<ul>\n<li>item1</li>\n<li>item2</li>\n</ul>\n</div>\n</li>\n<li>A second item in the list</li>\n</ul>", "customcontainers+attributes|advanced");
        }
    }
        // Attributes extension is also supported for Custom Container, as long as the Attributes extension is activated after the CustomContainer extension (`.UseCustomContainer().UseAttributes()`)
    [TestFixture]
    public partial class TestExtensionsCustomContainer
    {
        [Test]
        public void Example005()
        {
            // Example 5
            // Section: Extensions Custom Container
            //
            // The following CommonMark:
            //     :::spoiler {#myspoiler myprop=yes}
            //     This is a spoiler
            //     :::
            //
            // Should be rendered as:
            //     <div id="myspoiler" class="spoiler" myprop="yes"><p>This is a spoiler</p>
            //     </div>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 5, "Extensions Custom Container");
			TestParser.TestSpec(":::spoiler {#myspoiler myprop=yes}\nThis is a spoiler\n:::", "<div id=\"myspoiler\" class=\"spoiler\" myprop=\"yes\"><p>This is a spoiler</p>\n</div>", "customcontainers+attributes|advanced");
        }
    }
        // The content of a custom container can contain any blocks:
    [TestFixture]
    public partial class TestExtensionsCustomContainer
    {
        [Test]
        public void Example006()
        {
            // Example 6
            // Section: Extensions Custom Container
            //
            // The following CommonMark:
            //     :::mycontainer
            //     <p>This is a raw spoiler</p>
            //     :::
            //
            // Should be rendered as:
            //     <div class="mycontainer"><p>This is a raw spoiler</p>
            //     </div>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 6, "Extensions Custom Container");
			TestParser.TestSpec(":::mycontainer\n<p>This is a raw spoiler</p>\n:::", "<div class=\"mycontainer\"><p>This is a raw spoiler</p>\n</div>", "customcontainers+attributes|advanced");
        }
    }
        // ## Inline Custom Container
        //
        // A custom container can also be used within an inline container (e.g: paragraph, heading...) by enclosing a text by a new emphasis `::`
    [TestFixture]
    public partial class TestExtensionsInlineCustomContainer
    {
        [Test]
        public void Example007()
        {
            // Example 7
            // Section: Extensions Inline Custom Container 
            //
            // The following CommonMark:
            //     This is a text ::with special emphasis::
            //
            // Should be rendered as:
            //     <p>This is a text <span>with special emphasis</span></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 7, "Extensions Inline Custom Container ");
			TestParser.TestSpec("This is a text ::with special emphasis::", "<p>This is a text <span>with special emphasis</span></p>", "customcontainers+attributes|advanced");
        }
    }
        // Any other emphasis inline can be used within this emphasis inline container:
    [TestFixture]
    public partial class TestExtensionsInlineCustomContainer
    {
        [Test]
        public void Example008()
        {
            // Example 8
            // Section: Extensions Inline Custom Container 
            //
            // The following CommonMark:
            //     This is a text ::with special *emphasis*::
            //
            // Should be rendered as:
            //     <p>This is a text <span>with special <em>emphasis</em></span></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 8, "Extensions Inline Custom Container ");
			TestParser.TestSpec("This is a text ::with special *emphasis*::", "<p>This is a text <span>with special <em>emphasis</em></span></p>", "customcontainers+attributes|advanced");
        }
    }
        // Attributes can be attached to a inline custom container:
    [TestFixture]
    public partial class TestExtensionsInlineCustomContainer
    {
        [Test]
        public void Example009()
        {
            // Example 9
            // Section: Extensions Inline Custom Container 
            //
            // The following CommonMark:
            //     This is a text ::with special emphasis::{#myId .myemphasis}
            //
            // Should be rendered as:
            //     <p>This is a text <span id="myId" class="myemphasis">with special emphasis</span></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 9, "Extensions Inline Custom Container ");
			TestParser.TestSpec("This is a text ::with special emphasis::{#myId .myemphasis}", "<p>This is a text <span id=\"myId\" class=\"myemphasis\">with special emphasis</span></p>", "customcontainers+attributes|advanced");
        }
    }
        // # Extensions
        //
        // This section describes the different extensions supported:
        //
        // ## Definition lists
        //
        // A custom container is similar to a fenced code block, but it is using the character `:` to declare a block (with at least 3 characters), and instead of generating a `<pre><code>...</code></pre>` it will generate a `<div>...</dib>` block.
    [TestFixture]
    public partial class TestExtensionsDefinitionlists
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Definition lists
            //
            // The following CommonMark:
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

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Definition lists");
			TestParser.TestSpec("Term 1\n:   This is a definition item\n    With a paragraph\n    > This is a block quote\n\n    - This is a list\n    - with an item2\n\n    ```java\n    Test\n\n\n    ```\n\n    And a last line\n:   This ia another definition item\n\nTerm2\nTerm3 *with some inline*\n:   This is another definition for term2", "<dl>\n<dt>Term 1</dt>\n<dd><p>This is a definition item\nWith a paragraph</p>\n<blockquote>\n<p>This is a block quote</p>\n</blockquote>\n<ul>\n<li>This is a list</li>\n<li>with an item2</li>\n</ul>\n<pre><code class=\"language-java\">Test\n\n\n</code></pre>\n<p>And a last line</p>\n</dd>\n<dd>This ia another definition item</dd>\n<dt>Term2</dt>\n<dt>Term3 <em>with some inline</em></dt>\n<dd>This is another definition for term2</dd>\n</dl>", "definitionlists+attributes|advanced");
        }
    }
        // A definition term can be followed at most by one blank line. Lazy continuations are supported for definitions:
    [TestFixture]
    public partial class TestExtensionsDefinitionlists
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions Definition lists
            //
            // The following CommonMark:
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

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions Definition lists");
			TestParser.TestSpec("Term 1\n\n:   Definition\nwith lazy continuation.\n\n    Second paragraph of the definition.", "<dl>\n<dt>Term 1</dt>\n<dd><p>Definition\nwith lazy continuation.</p>\n<p>Second paragraph of the definition.</p>\n</dd>\n</dl>", "definitionlists+attributes|advanced");
        }
    }
        // The definition must be indented to 4 characters including the `:`.
    [TestFixture]
    public partial class TestExtensionsDefinitionlists
    {
        [Test]
        public void Example003()
        {
            // Example 3
            // Section: Extensions Definition lists
            //
            // The following CommonMark:
            //     Term 1
            //     :  Invalid with less than 3 characters
            //
            // Should be rendered as:
            //     <p>Term 1
            //     :  Invalid with less than 3 characters</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Extensions Definition lists");
			TestParser.TestSpec("Term 1\n:  Invalid with less than 3 characters", "<p>Term 1\n:  Invalid with less than 3 characters</p>", "definitionlists+attributes|advanced");
        }
    }
        // The `:` can be indented up to 3 spaces:
    [TestFixture]
    public partial class TestExtensionsDefinitionlists
    {
        [Test]
        public void Example004()
        {
            // Example 4
            // Section: Extensions Definition lists
            //
            // The following CommonMark:
            //     Term 1
            //        : Valid even if `:` starts at most 3 spaces
            //
            // Should be rendered as:
            //     <dl>
            //     <dt>Term 1</dt>
            //     <dd>Valid even if <code>:</code> starts at most 3 spaces</dd>
            //     </dl>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 4, "Extensions Definition lists");
			TestParser.TestSpec("Term 1\n   : Valid even if `:` starts at most 3 spaces", "<dl>\n<dt>Term 1</dt>\n<dd>Valid even if <code>:</code> starts at most 3 spaces</dd>\n</dl>", "definitionlists+attributes|advanced");
        }
    }
        // But more than 3 spaces before `:` will trigger an indented code block:
    [TestFixture]
    public partial class TestExtensionsDefinitionlists
    {
        [Test]
        public void Example005()
        {
            // Example 5
            // Section: Extensions Definition lists
            //
            // The following CommonMark:
            //     Term 1
            //     
            //         : Not valid
            //
            // Should be rendered as:
            //     <p>Term 1</p>
            //     <pre><code>: Not valid
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 5, "Extensions Definition lists");
			TestParser.TestSpec("Term 1\n\n    : Not valid", "<p>Term 1</p>\n<pre><code>: Not valid\n</code></pre>", "definitionlists+attributes|advanced");
        }
    }
        // # Extensions
        //
        // This section describes the different extensions supported:
        //
        // ## Emoji
        //
        // Emoji and smiley can be converted to their respective unicode characters:
    [TestFixture]
    public partial class TestExtensionsEmoji
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Emoji
            //
            // The following CommonMark:
            //     This is a test with a :) and a :angry: smiley
            //
            // Should be rendered as:
            //     <p>This is a test with a 😃 and a 😠 smiley</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Emoji");
			TestParser.TestSpec("This is a test with a :) and a :angry: smiley", "<p>This is a test with a 😃 and a 😠 smiley</p>", "emojis|advanced+emojis");
        }
    }
        // An emoji needs to be preceded by a space and followed by a space:
    [TestFixture]
    public partial class TestExtensionsEmoji
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions Emoji
            //
            // The following CommonMark:
            //     These are not:) an :)emoji with a:) x:angry:x
            //
            // Should be rendered as:
            //     <p>These are not:) an :)emoji with a:) x:angry:x</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions Emoji");
			TestParser.TestSpec("These are not:) an :)emoji with a:) x:angry:x", "<p>These are not:) an :)emoji with a:) x:angry:x</p>", "emojis|advanced+emojis");
        }
    }
        // # Extensions
        //
        // This section describes the different extensions supported:
        //
        // ## Abbreviation
        //
        // Abbreviation can be declared by using the `*[Abbreviation Label]: Abbreviation description`
        //
        // Abbreviation definition will be removed from the original document. Any Abbreviation label found in literals will be replaced by the abbreviation description:
    [TestFixture]
    public partial class TestExtensionsAbbreviation
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Abbreviation
            //
            // The following CommonMark:
            //     *[HTML]: Hypertext Markup Language
            //     
            //     Later in a text we are using HTML and it becomes an abbr tag HTML
            //
            // Should be rendered as:
            //     <p>Later in a text we are using <abbr title="Hypertext Markup Language">HTML</abbr> and it becomes an abbr tag <abbr title="Hypertext Markup Language">HTML</abbr></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Abbreviation");
			TestParser.TestSpec("*[HTML]: Hypertext Markup Language\n\nLater in a text we are using HTML and it becomes an abbr tag HTML", "<p>Later in a text we are using <abbr title=\"Hypertext Markup Language\">HTML</abbr> and it becomes an abbr tag <abbr title=\"Hypertext Markup Language\">HTML</abbr></p>", "abbreviations|advanced");
        }
    }
        // An abbreviation definition can be indented at most 3 spaces
    [TestFixture]
    public partial class TestExtensionsAbbreviation
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions Abbreviation
            //
            // The following CommonMark:
            //     *[HTML]: Hypertext Markup Language
            //         *[This]: is not an abbreviation
            //
            // Should be rendered as:
            //     <pre><code>*[This]: is not an abbreviation
            //     </code></pre>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions Abbreviation");
			TestParser.TestSpec("*[HTML]: Hypertext Markup Language\n    *[This]: is not an abbreviation", "<pre><code>*[This]: is not an abbreviation\n</code></pre>", "abbreviations|advanced");
        }
    }
        // An abbreviation may contain spaces:
    [TestFixture]
    public partial class TestExtensionsAbbreviation
    {
        [Test]
        public void Example003()
        {
            // Example 3
            // Section: Extensions Abbreviation
            //
            // The following CommonMark:
            //     *[SUPER HTML]: Super Hypertext Markup Language
            //     
            //     This is a SUPER HTML document    
            //
            // Should be rendered as:
            //     <p>This is a <abbr title="Super Hypertext Markup Language">SUPER HTML</abbr> document</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Extensions Abbreviation");
			TestParser.TestSpec("*[SUPER HTML]: Super Hypertext Markup Language\n\nThis is a SUPER HTML document    ", "<p>This is a <abbr title=\"Super Hypertext Markup Language\">SUPER HTML</abbr> document</p>", "abbreviations|advanced");
        }
    }
        // Abbreviation may contain any unicode characters:
    [TestFixture]
    public partial class TestExtensionsAbbreviation
    {
        [Test]
        public void Example004()
        {
            // Example 4
            // Section: Extensions Abbreviation
            //
            // The following CommonMark:
            //     *[😃 HTML]: Hypertext Markup Language
            //     
            //     This is a 😃 HTML document    
            //
            // Should be rendered as:
            //     <p>This is a <abbr title="Hypertext Markup Language">😃 HTML</abbr> document</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 4, "Extensions Abbreviation");
			TestParser.TestSpec("*[😃 HTML]: Hypertext Markup Language\n\nThis is a 😃 HTML document    ", "<p>This is a <abbr title=\"Hypertext Markup Language\">😃 HTML</abbr> document</p>", "abbreviations|advanced");
        }
    }
        // # Extensions
        //
        // The following additional list items are supported:
        //
        // ## Ordered list with alpha letter
        //
        // Allows to use a list using an alpha letter instead of a number
    [TestFixture]
    public partial class TestExtensionsOrderedlistwithalphaletter
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Ordered list with alpha letter
            //
            // The following CommonMark:
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

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Ordered list with alpha letter");
			TestParser.TestSpec("a. First item\nb. Second item\nc. Last item", "<ol type=\"a\">\n<li>First item</li>\n<li>Second item</li>\n<li>Last item</li>\n</ol>", "listextras|advanced");
        }
    }
        // It works also for uppercase alpha:
    [TestFixture]
    public partial class TestExtensionsOrderedlistwithalphaletter
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions Ordered list with alpha letter
            //
            // The following CommonMark:
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

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions Ordered list with alpha letter");
			TestParser.TestSpec("A. First item\nB. Second item\nC. Last item", "<ol type=\"A\">\n<li>First item</li>\n<li>Second item</li>\n<li>Last item</li>\n</ol>", "listextras|advanced");
        }
    }
        // Like for numbered list, a list can start with a different letter
    [TestFixture]
    public partial class TestExtensionsOrderedlistwithalphaletter
    {
        [Test]
        public void Example003()
        {
            // Example 3
            // Section: Extensions Ordered list with alpha letter
            //
            // The following CommonMark:
            //     b. First item
            //     c. Second item
            //
            // Should be rendered as:
            //     <ol type="a" start="b">
            //     <li>First item</li>
            //     <li>Second item</li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Extensions Ordered list with alpha letter");
			TestParser.TestSpec("b. First item\nc. Second item", "<ol type=\"a\" start=\"b\">\n<li>First item</li>\n<li>Second item</li>\n</ol>", "listextras|advanced");
        }
    }
        // A different type of list will break the existing list:
    [TestFixture]
    public partial class TestExtensionsOrderedlistwithalphaletter
    {
        [Test]
        public void Example004()
        {
            // Example 4
            // Section: Extensions Ordered list with alpha letter
            //
            // The following CommonMark:
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

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 4, "Extensions Ordered list with alpha letter");
			TestParser.TestSpec("a. First item1\nb. Second item\nA. First item2", "<ol type=\"a\">\n<li>First item1</li>\n<li>Second item</li>\n</ol>\n<ol type=\"A\">\n<li>First item2</li>\n</ol>", "listextras|advanced");
        }
    }
        // ## Ordered list with roman letter
        //
        // Allows to use a list using a roman number instead of a number.
    [TestFixture]
    public partial class TestExtensionsOrderedlistwithromanletter
    {
        [Test]
        public void Example005()
        {
            // Example 5
            // Section: Extensions Ordered list with roman letter
            //
            // The following CommonMark:
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

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 5, "Extensions Ordered list with roman letter");
			TestParser.TestSpec("i. First item\nii. Second item\niii. Third item\niv. Last item", "<ol type=\"i\">\n<li>First item</li>\n<li>Second item</li>\n<li>Third item</li>\n<li>Last item</li>\n</ol>", "listextras|advanced");
        }
    }
        // It works also for uppercase alpha:
    [TestFixture]
    public partial class TestExtensionsOrderedlistwithromanletter
    {
        [Test]
        public void Example006()
        {
            // Example 6
            // Section: Extensions Ordered list with roman letter
            //
            // The following CommonMark:
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

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 6, "Extensions Ordered list with roman letter");
			TestParser.TestSpec("I. First item\nII. Second item\nIII. Third item\nIV. Last item", "<ol type=\"I\">\n<li>First item</li>\n<li>Second item</li>\n<li>Third item</li>\n<li>Last item</li>\n</ol>", "listextras|advanced");
        }
    }
        // Like for numbered list, a list can start with a different letter
    [TestFixture]
    public partial class TestExtensionsOrderedlistwithromanletter
    {
        [Test]
        public void Example007()
        {
            // Example 7
            // Section: Extensions Ordered list with roman letter
            //
            // The following CommonMark:
            //     ii. First item
            //     iii. Second item
            //
            // Should be rendered as:
            //     <ol type="i" start="ii">
            //     <li>First item</li>
            //     <li>Second item</li>
            //     </ol>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 7, "Extensions Ordered list with roman letter");
			TestParser.TestSpec("ii. First item\niii. Second item", "<ol type=\"i\" start=\"ii\">\n<li>First item</li>\n<li>Second item</li>\n</ol>", "listextras|advanced");
        }
    }
        // # Extensions
        //
        // The following the figure extension:
        //
        // ## Figures
        //
        // A figure can be defined by using a pattern equivalent to a fenced code block but with the character `^`
    [TestFixture]
    public partial class TestExtensionsFigures
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Figures
            //
            // The following CommonMark:
            //     ^^^
            //     This is a figure
            //     ^^^ This is a *caption*
            //
            // Should be rendered as:
            //     <figure>
            //     <p>This is a figure</p>
            //     <figcaption>This is a <em>caption</em></figcaption>
            //     </figure>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Figures");
			TestParser.TestSpec("^^^\nThis is a figure\n^^^ This is a *caption*", "<figure>\n<p>This is a figure</p>\n<figcaption>This is a <em>caption</em></figcaption>\n</figure>", "figures+footers+citations|advanced");
        }
    }
        // ## Footers
        //
        // A footer equivalent to a block quote parsing but starts with double character ^^
    [TestFixture]
    public partial class TestExtensionsFooters
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions Footers
            //
            // The following CommonMark:
            //     ^^ This is a footer
            //     ^^ multi-line
            //
            // Should be rendered as:
            //     <footer>This is a footer
            //     multi-line</footer>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions Footers");
			TestParser.TestSpec("^^ This is a footer\n^^ multi-line", "<footer>This is a footer\nmulti-line</footer>", "figures+footers+citations|advanced");
        }
    }
        // ## Cite
        //
        // A cite is working like an emphasis but using the double character ""
    [TestFixture]
    public partial class TestExtensionsCite
    {
        [Test]
        public void Example003()
        {
            // Example 3
            // Section: Extensions Cite
            //
            // The following CommonMark:
            //     This is a ""citation of someone""
            //
            // Should be rendered as:
            //     <p>This is a <cite>citation of someone</cite></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Extensions Cite");
			TestParser.TestSpec("This is a \"\"citation of someone\"\"", "<p>This is a <cite>citation of someone</cite></p>", "figures+footers+citations|advanced");
        }
    }
        // # Extensions
        //
        // Adds support for mathematics spans:
        //
        // ## Math Inline
        //
        // Allows to define a mathematic block embraced by `$...$`
    [TestFixture]
    public partial class TestExtensionsMathInline
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Math Inline
            //
            // The following CommonMark:
            //     This is a $math block$
            //
            // Should be rendered as:
            //     <p>This is a <span class="math">math block</span></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Math Inline");
			TestParser.TestSpec("This is a $math block$", "<p>This is a <span class=\"math\">math block</span></p>", "mathematics|advanced");
        }
    }
        // Or by `$$...$$` embracing it by:
    [TestFixture]
    public partial class TestExtensionsMathInline
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions Math Inline
            //
            // The following CommonMark:
            //     This is a $$math block$$
            //
            // Should be rendered as:
            //     <p>This is a <span class="math">math block</span></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions Math Inline");
			TestParser.TestSpec("This is a $$math block$$", "<p>This is a <span class=\"math\">math block</span></p>", "mathematics|advanced");
        }
    }
        // The opening `$` and closing `$` is following the rules of the emphasis delimiter `_`:
    [TestFixture]
    public partial class TestExtensionsMathInline
    {
        [Test]
        public void Example003()
        {
            // Example 3
            // Section: Extensions Math Inline
            //
            // The following CommonMark:
            //     This is not a $ math block $
            //
            // Should be rendered as:
            //     <p>This is not a $ math block $</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Extensions Math Inline");
			TestParser.TestSpec("This is not a $ math block $", "<p>This is not a $ math block $</p>", "mathematics|advanced");
        }
    }
        // For the opening `$` it requires a space or a punctuation before (but cannot be used within a word):
    [TestFixture]
    public partial class TestExtensionsMathInline
    {
        [Test]
        public void Example004()
        {
            // Example 4
            // Section: Extensions Math Inline
            //
            // The following CommonMark:
            //     This is not a m$ath block$
            //
            // Should be rendered as:
            //     <p>This is not a m$ath block$</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 4, "Extensions Math Inline");
			TestParser.TestSpec("This is not a m$ath block$", "<p>This is not a m$ath block$</p>", "mathematics|advanced");
        }
    }
        // For the closing `$` it requires a space after or a punctuation (but cannot be preceded by a space and cannot be used within a word):
    [TestFixture]
    public partial class TestExtensionsMathInline
    {
        [Test]
        public void Example005()
        {
            // Example 5
            // Section: Extensions Math Inline
            //
            // The following CommonMark:
            //     This is not a $math bloc$k
            //
            // Should be rendered as:
            //     <p>This is not a $math bloc$k</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 5, "Extensions Math Inline");
			TestParser.TestSpec("This is not a $math bloc$k", "<p>This is not a $math bloc$k</p>", "mathematics|advanced");
        }
    }
        // For the closing `$` it requires a space after or a punctuation (but cannot be preceded by a space and cannot be used within a word):
    [TestFixture]
    public partial class TestExtensionsMathInline
    {
        [Test]
        public void Example006()
        {
            // Example 6
            // Section: Extensions Math Inline
            //
            // The following CommonMark:
            //     This is should not match a 16$ or a $15
            //
            // Should be rendered as:
            //     <p>This is should not match a 16$ or a $15</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 6, "Extensions Math Inline");
			TestParser.TestSpec("This is should not match a 16$ or a $15", "<p>This is should not match a 16$ or a $15</p>", "mathematics|advanced");
        }
    }
        // A `$` can be escaped between a math inline block by using the escape `\\`
    [TestFixture]
    public partial class TestExtensionsMathInline
    {
        [Test]
        public void Example007()
        {
            // Example 7
            // Section: Extensions Math Inline
            //
            // The following CommonMark:
            //     This is a $math \$ block$
            //
            // Should be rendered as:
            //     <p>This is a <span class="math">math \$ block</span></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 7, "Extensions Math Inline");
			TestParser.TestSpec("This is a $math \\$ block$", "<p>This is a <span class=\"math\">math \\$ block</span></p>", "mathematics|advanced");
        }
    }
        // At most, two `$` will be matched for the opening and closing:
    [TestFixture]
    public partial class TestExtensionsMathInline
    {
        [Test]
        public void Example008()
        {
            // Example 8
            // Section: Extensions Math Inline
            //
            // The following CommonMark:
            //     This is a $$$math block$$$
            //
            // Should be rendered as:
            //     <p>This is a <span class="math">$math block$</span></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 8, "Extensions Math Inline");
			TestParser.TestSpec("This is a $$$math block$$$", "<p>This is a <span class=\"math\">$math block$</span></p>", "mathematics|advanced");
        }
    }
        // A mathematic block takes precedence over standard emphasis `*` `_`:
    [TestFixture]
    public partial class TestExtensionsMathInline
    {
        [Test]
        public void Example009()
        {
            // Example 9
            // Section: Extensions Math Inline
            //
            // The following CommonMark:
            //     This is *a $math* block$
            //
            // Should be rendered as:
            //     <p>This is *a <span class="math">math* block</span></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 9, "Extensions Math Inline");
			TestParser.TestSpec("This is *a $math* block$", "<p>This is *a <span class=\"math\">math* block</span></p>", "mathematics|advanced");
        }
    }
        // ## Math Block
        //
        // The match block can spawn on multiple lines by having a $$ starting on a line.
        // It is working as a fenced code block.
    [TestFixture]
    public partial class TestExtensionsMathBlock
    {
        [Test]
        public void Example010()
        {
            // Example 10
            // Section: Extensions Math Block
            //
            // The following CommonMark:
            //     $$
            //     \begin{equation}
            //       \int_0^\infty \frac{x^3}{e^x-1}\,dx = \frac{\pi^4}{15}
            //       \label{eq:sample}
            //     \end{equation}
            //     $$
            //
            // Should be rendered as:
            //     <div class="math">\begin{equation}
            //       \int_0^\infty \frac{x^3}{e^x-1}\,dx = \frac{\pi^4}{15}
            //       \label{eq:sample}
            //     \end{equation}
            //     </div>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 10, "Extensions Math Block");
			TestParser.TestSpec("$$\n\\begin{equation}\n  \\int_0^\\infty \\frac{x^3}{e^x-1}\\,dx = \\frac{\\pi^4}{15}\n  \\label{eq:sample}\n\\end{equation}\n$$", "<div class=\"math\">\\begin{equation}\n  \\int_0^\\infty \\frac{x^3}{e^x-1}\\,dx = \\frac{\\pi^4}{15}\n  \\label{eq:sample}\n\\end{equation}\n</div>", "mathematics|advanced");
        }
    }
        // # Extensions
        //
        // Adds support for outputing bootstrap ready tags:
        //
        // ## Bootstrap
        //
        // Adds bootstrap `.table` class to `<table>`:
    [TestFixture]
    public partial class TestExtensionsBootstrap
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Bootstrap
            //
            // The following CommonMark:
            //     Name | Value
            //     -----| -----
            //     Abc  | 16
            //
            // Should be rendered as:
            //     <table class="table">
            //     <thead>
            //     <tr>
            //     <th>Name</th>
            //     <th>Value</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>Abc</td>
            //     <td>16</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Bootstrap");
			TestParser.TestSpec("Name | Value\n-----| -----\nAbc  | 16", "<table class=\"table\">\n<thead>\n<tr>\n<th>Name</th>\n<th>Value</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>Abc</td>\n<td>16</td>\n</tr>\n</tbody>\n</table>", "bootstrap+pipetables+figures+attributes");
        }
    }
        // Adds bootstrap `.blockquote` class to `<blockquote>`:
    [TestFixture]
    public partial class TestExtensionsBootstrap
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions Bootstrap
            //
            // The following CommonMark:
            //     > This is a blockquote
            //
            // Should be rendered as:
            //     <blockquote class="blockquote">
            //     <p>This is a blockquote</p>
            //     </blockquote>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions Bootstrap");
			TestParser.TestSpec("> This is a blockquote", "<blockquote class=\"blockquote\">\n<p>This is a blockquote</p>\n</blockquote>", "bootstrap+pipetables+figures+attributes");
        }
    }
        // Adds bootstrap `.figure` class to `<figure>` and `.figure-caption` to `<figcaption>`
    [TestFixture]
    public partial class TestExtensionsBootstrap
    {
        [Test]
        public void Example003()
        {
            // Example 3
            // Section: Extensions Bootstrap
            //
            // The following CommonMark:
            //     ^^^
            //     This is a text in a caption
            //     ^^^ This is the caption
            //
            // Should be rendered as:
            //     <figure class="figure">
            //     <p>This is a text in a caption</p>
            //     <figcaption class="figure-caption">This is the caption</figcaption>
            //     </figure>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Extensions Bootstrap");
			TestParser.TestSpec("^^^\nThis is a text in a caption\n^^^ This is the caption", "<figure class=\"figure\">\n<p>This is a text in a caption</p>\n<figcaption class=\"figure-caption\">This is the caption</figcaption>\n</figure>", "bootstrap+pipetables+figures+attributes");
        }
    }
        // Adds the `.img-fluid` class to all image links `<img>`
    [TestFixture]
    public partial class TestExtensionsBootstrap
    {
        [Test]
        public void Example004()
        {
            // Example 4
            // Section: Extensions Bootstrap
            //
            // The following CommonMark:
            //     ![Image Link](/url)
            //
            // Should be rendered as:
            //     <p><img src="/url" class="img-fluid" alt="Image Link" /></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 4, "Extensions Bootstrap");
			TestParser.TestSpec("![Image Link](/url)", "<p><img src=\"/url\" class=\"img-fluid\" alt=\"Image Link\" /></p>", "bootstrap+pipetables+figures+attributes");
        }
    }
        // # Extensions
        //
        // Adds support for media links:
        //
        // ## Media links
        //
        // Allows to embed audio/video links to popular website:
    [TestFixture]
    public partial class TestExtensionsMedialinks
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Media links
            //
            // The following CommonMark:
            //     ![Video1](https://www.youtube.com/watch?v=mswPy5bt3TQ)
            //     
            //     ![Video2](https://vimeo.com/8607834)
            //
            // Should be rendered as:
            //     <p><iframe src="https://www.youtube.com/embed/mswPy5bt3TQ" width="500" height="281" frameborder="0" allowfullscreen></iframe></p>
            //     <p><iframe src="https://player.vimeo.com/video/8607834" width="500" height="281" frameborder="0" allowfullscreen></iframe></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Media links");
			TestParser.TestSpec("![Video1](https://www.youtube.com/watch?v=mswPy5bt3TQ)\n\n![Video2](https://vimeo.com/8607834)", "<p><iframe src=\"https://www.youtube.com/embed/mswPy5bt3TQ\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen></iframe></p>\n<p><iframe src=\"https://player.vimeo.com/video/8607834\" width=\"500\" height=\"281\" frameborder=\"0\" allowfullscreen></iframe></p>", "medialinks|advanced+medialinks");
        }
    }
        // # Extensions
        //
        // Adds support for smarty pants:
        //
        // ## SmartyPants Quotes
        //
        // Converts the following character to smarty pants:
    [TestFixture]
    public partial class TestExtensionsSmartyPantsQuotes
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions SmartyPants Quotes
            //
            // The following CommonMark:
            //     This is a "text"
            //
            // Should be rendered as:
            //     <p>This is a &ldquo;text&rdquo;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions SmartyPants Quotes");
			TestParser.TestSpec("This is a \"text\"", "<p>This is a &ldquo;text&rdquo;</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
    [TestFixture]
    public partial class TestExtensionsSmartyPantsQuotes
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions SmartyPants Quotes
            //
            // The following CommonMark:
            //     This is a 'text'
            //
            // Should be rendered as:
            //     <p>This is a &lsquo;text&rsquo;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions SmartyPants Quotes");
			TestParser.TestSpec("This is a 'text'", "<p>This is a &lsquo;text&rsquo;</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
    [TestFixture]
    public partial class TestExtensionsSmartyPantsQuotes
    {
        [Test]
        public void Example003()
        {
            // Example 3
            // Section: Extensions SmartyPants Quotes
            //
            // The following CommonMark:
            //     This is a <<text>>
            //
            // Should be rendered as:
            //     <p>This is a &laquo;text&raquo;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Extensions SmartyPants Quotes");
			TestParser.TestSpec("This is a <<text>>", "<p>This is a &laquo;text&raquo;</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
        // Unbalanced quotes are not changed:
    [TestFixture]
    public partial class TestExtensionsSmartyPantsQuotes
    {
        [Test]
        public void Example004()
        {
            // Example 4
            // Section: Extensions SmartyPants Quotes
            //
            // The following CommonMark:
            //     This is a "text
            //
            // Should be rendered as:
            //     <p>This is a &quot;text</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 4, "Extensions SmartyPants Quotes");
			TestParser.TestSpec("This is a \"text", "<p>This is a &quot;text</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
    [TestFixture]
    public partial class TestExtensionsSmartyPantsQuotes
    {
        [Test]
        public void Example005()
        {
            // Example 5
            // Section: Extensions SmartyPants Quotes
            //
            // The following CommonMark:
            //     This is a 'text
            //
            // Should be rendered as:
            //     <p>This is a 'text</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 5, "Extensions SmartyPants Quotes");
			TestParser.TestSpec("This is a 'text", "<p>This is a 'text</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
    [TestFixture]
    public partial class TestExtensionsSmartyPantsQuotes
    {
        [Test]
        public void Example006()
        {
            // Example 6
            // Section: Extensions SmartyPants Quotes
            //
            // The following CommonMark:
            //     This is a <<text
            //
            // Should be rendered as:
            //     <p>This is a &lt;&lt;text</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 6, "Extensions SmartyPants Quotes");
			TestParser.TestSpec("This is a <<text", "<p>This is a &lt;&lt;text</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
        // Unbalanced quotes inside other quotes are not changed:
    [TestFixture]
    public partial class TestExtensionsSmartyPantsQuotes
    {
        [Test]
        public void Example007()
        {
            // Example 7
            // Section: Extensions SmartyPants Quotes
            //
            // The following CommonMark:
            //     This is a "text 'with" a another text'
            //
            // Should be rendered as:
            //     <p>This is a &ldquo;text 'with&rdquo; a another text'</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 7, "Extensions SmartyPants Quotes");
			TestParser.TestSpec("This is a \"text 'with\" a another text'", "<p>This is a &ldquo;text 'with&rdquo; a another text'</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
    [TestFixture]
    public partial class TestExtensionsSmartyPantsQuotes
    {
        [Test]
        public void Example008()
        {
            // Example 8
            // Section: Extensions SmartyPants Quotes
            //
            // The following CommonMark:
            //     This is a 'text <<with' a another text>>
            //
            // Should be rendered as:
            //     <p>This is a &lsquo;text &lt;&lt;with&rsquo; a another text&gt;&gt;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 8, "Extensions SmartyPants Quotes");
			TestParser.TestSpec("This is a 'text <<with' a another text>>", "<p>This is a &lsquo;text &lt;&lt;with&rsquo; a another text&gt;&gt;</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
    [TestFixture]
    public partial class TestExtensionsSmartyPantsQuotes
    {
        [Test]
        public void Example009()
        {
            // Example 9
            // Section: Extensions SmartyPants Quotes
            //
            // The following CommonMark:
            //     This is a <<text 'with>> a another text'
            //
            // Should be rendered as:
            //     <p>This is a &laquo;text 'with&raquo; a another text'</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 9, "Extensions SmartyPants Quotes");
			TestParser.TestSpec("This is a <<text 'with>> a another text'", "<p>This is a &laquo;text 'with&raquo; a another text'</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
        // Quotes requires to have the same rules than emphasis `_` regarding left/right frankling rules:
    [TestFixture]
    public partial class TestExtensionsSmartyPantsQuotes
    {
        [Test]
        public void Example010()
        {
            // Example 10
            // Section: Extensions SmartyPants Quotes
            //
            // The following CommonMark:
            //     It's not quotes'
            //
            // Should be rendered as:
            //     <p>It's not quotes'</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 10, "Extensions SmartyPants Quotes");
			TestParser.TestSpec("It's not quotes'", "<p>It's not quotes'</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
    [TestFixture]
    public partial class TestExtensionsSmartyPantsQuotes
    {
        [Test]
        public void Example011()
        {
            // Example 11
            // Section: Extensions SmartyPants Quotes
            //
            // The following CommonMark:
            //     They are ' not matching quotes '
            //
            // Should be rendered as:
            //     <p>They are ' not matching quotes '</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 11, "Extensions SmartyPants Quotes");
			TestParser.TestSpec("They are ' not matching quotes '", "<p>They are ' not matching quotes '</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
    [TestFixture]
    public partial class TestExtensionsSmartyPantsQuotes
    {
        [Test]
        public void Example012()
        {
            // Example 12
            // Section: Extensions SmartyPants Quotes
            //
            // The following CommonMark:
            //     They are' not matching 'quotes
            //
            // Should be rendered as:
            //     <p>They are' not matching 'quotes</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 12, "Extensions SmartyPants Quotes");
			TestParser.TestSpec("They are' not matching 'quotes", "<p>They are' not matching 'quotes</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
        // An emphasis starting inside left/right quotes will span over the right quote:
    [TestFixture]
    public partial class TestExtensionsSmartyPantsQuotes
    {
        [Test]
        public void Example013()
        {
            // Example 13
            // Section: Extensions SmartyPants Quotes
            //
            // The following CommonMark:
            //     This is "a *text" with an emphasis*
            //
            // Should be rendered as:
            //     <p>This is &ldquo;a <em>text&rdquo; with an emphasis</em></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 13, "Extensions SmartyPants Quotes");
			TestParser.TestSpec("This is \"a *text\" with an emphasis*", "<p>This is &ldquo;a <em>text&rdquo; with an emphasis</em></p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
        // ## SmartyPants Separators
    [TestFixture]
    public partial class TestExtensionsSmartyPantsSeparators
    {
        [Test]
        public void Example014()
        {
            // Example 14
            // Section: Extensions SmartyPants Separators
            //
            // The following CommonMark:
            //     This is a -- text
            //
            // Should be rendered as:
            //     <p>This is a &ndash; text</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 14, "Extensions SmartyPants Separators");
			TestParser.TestSpec("This is a -- text", "<p>This is a &ndash; text</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
    [TestFixture]
    public partial class TestExtensionsSmartyPantsSeparators
    {
        [Test]
        public void Example015()
        {
            // Example 15
            // Section: Extensions SmartyPants Separators
            //
            // The following CommonMark:
            //     This is a --- text
            //
            // Should be rendered as:
            //     <p>This is a &mdash; text</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 15, "Extensions SmartyPants Separators");
			TestParser.TestSpec("This is a --- text", "<p>This is a &mdash; text</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
    [TestFixture]
    public partial class TestExtensionsSmartyPantsSeparators
    {
        [Test]
        public void Example016()
        {
            // Example 16
            // Section: Extensions SmartyPants Separators
            //
            // The following CommonMark:
            //     This is a en ellipsis...
            //
            // Should be rendered as:
            //     <p>This is a en ellipsis&hellip;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 16, "Extensions SmartyPants Separators");
			TestParser.TestSpec("This is a en ellipsis...", "<p>This is a en ellipsis&hellip;</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
        // Check that a smartypants are not breaking pipetable parsing:
    [TestFixture]
    public partial class TestExtensionsSmartyPantsSeparators
    {
        [Test]
        public void Example017()
        {
            // Example 17
            // Section: Extensions SmartyPants Separators
            //
            // The following CommonMark:
            //     a  | b
            //     -- | --
            //     0  | 1
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 17, "Extensions SmartyPants Separators");
			TestParser.TestSpec("a  | b\n-- | --\n0  | 1", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "pipetables+smartypants|advanced+smartypants");
        }
    }
        // Check quotes and dash:
    [TestFixture]
    public partial class TestExtensionsSmartyPantsSeparators
    {
        [Test]
        public void Example018()
        {
            // Example 18
            // Section: Extensions SmartyPants Separators
            //
            // The following CommonMark:
            //     A "quote" with a ---
            //
            // Should be rendered as:
            //     <p>A &ldquo;quote&rdquo; with a &mdash;</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 18, "Extensions SmartyPants Separators");
			TestParser.TestSpec("A \"quote\" with a ---", "<p>A &ldquo;quote&rdquo; with a &mdash;</p>", "pipetables+smartypants|advanced+smartypants");
        }
    }
        // # Extensions
        //
        // This section describes the auto identifier extension
        //
        // ## Heading Auto Identifiers
        //
        // Allows to automatically creates an identifier for a heading:
    [TestFixture]
    public partial class TestExtensionsHeadingAutoIdentifiers
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Heading Auto Identifiers
            //
            // The following CommonMark:
            //     # This is a heading
            //
            // Should be rendered as:
            //     <h1 id="this-is-a-heading">This is a heading</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Heading Auto Identifiers");
			TestParser.TestSpec("# This is a heading", "<h1 id=\"this-is-a-heading\">This is a heading</h1>", "autoidentifiers|advanced");
        }
    }
        // Only punctuation `-`, `_` and `.` is kept, all over non letter characters are discarded.
        // Consecutive same character `-`, `_` or `.` are rendered into a single one
        // Characters `-`, `_` and `.` at the end of the string are also discarded.
    [TestFixture]
    public partial class TestExtensionsHeadingAutoIdentifiers
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions Heading Auto Identifiers
            //
            // The following CommonMark:
            //     # This - is a &@! heading _ with . and ! -
            //
            // Should be rendered as:
            //     <h1 id="this-is-a-heading_with.and">This - is a &amp;@! heading _ with . and ! -</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions Heading Auto Identifiers");
			TestParser.TestSpec("# This - is a &@! heading _ with . and ! -", "<h1 id=\"this-is-a-heading_with.and\">This - is a &amp;@! heading _ with . and ! -</h1>", "autoidentifiers|advanced");
        }
    }
        // Formatting (emphasis) are also discarded:
    [TestFixture]
    public partial class TestExtensionsHeadingAutoIdentifiers
    {
        [Test]
        public void Example003()
        {
            // Example 3
            // Section: Extensions Heading Auto Identifiers
            //
            // The following CommonMark:
            //     # This is a *heading*
            //
            // Should be rendered as:
            //     <h1 id="this-is-a-heading">This is a <em>heading</em></h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 3, "Extensions Heading Auto Identifiers");
			TestParser.TestSpec("# This is a *heading*", "<h1 id=\"this-is-a-heading\">This is a <em>heading</em></h1>", "autoidentifiers|advanced");
        }
    }
        // Links are also removed:
    [TestFixture]
    public partial class TestExtensionsHeadingAutoIdentifiers
    {
        [Test]
        public void Example004()
        {
            // Example 4
            // Section: Extensions Heading Auto Identifiers
            //
            // The following CommonMark:
            //     # This is a [heading](/url)
            //
            // Should be rendered as:
            //     <h1 id="this-is-a-heading">This is a <a href="/url">heading</a></h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 4, "Extensions Heading Auto Identifiers");
			TestParser.TestSpec("# This is a [heading](/url)", "<h1 id=\"this-is-a-heading\">This is a <a href=\"/url\">heading</a></h1>", "autoidentifiers|advanced");
        }
    }
        // If multiple heading have the same text, -1, -2...-n will be postfix to the header id.
    [TestFixture]
    public partial class TestExtensionsHeadingAutoIdentifiers
    {
        [Test]
        public void Example005()
        {
            // Example 5
            // Section: Extensions Heading Auto Identifiers
            //
            // The following CommonMark:
            //     # This is a heading
            //     # This is a heading
            //
            // Should be rendered as:
            //     <h1 id="this-is-a-heading">This is a heading</h1>
            //     <h1 id="this-is-a-heading-1">This is a heading</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 5, "Extensions Heading Auto Identifiers");
			TestParser.TestSpec("# This is a heading\n# This is a heading", "<h1 id=\"this-is-a-heading\">This is a heading</h1>\n<h1 id=\"this-is-a-heading-1\">This is a heading</h1>", "autoidentifiers|advanced");
        }
    }
        // The heading Id will start on the first letter character of the heading, all previous characters will be discarded:
    [TestFixture]
    public partial class TestExtensionsHeadingAutoIdentifiers
    {
        [Test]
        public void Example006()
        {
            // Example 6
            // Section: Extensions Heading Auto Identifiers
            //
            // The following CommonMark:
            //     # 1.0 This is a heading
            //
            // Should be rendered as:
            //     <h1 id="this-is-a-heading">1.0 This is a heading</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 6, "Extensions Heading Auto Identifiers");
			TestParser.TestSpec("# 1.0 This is a heading", "<h1 id=\"this-is-a-heading\">1.0 This is a heading</h1>", "autoidentifiers|advanced");
        }
    }
        // If the heading is all stripped by the previous rules, the id `section` will be used instead:
    [TestFixture]
    public partial class TestExtensionsHeadingAutoIdentifiers
    {
        [Test]
        public void Example007()
        {
            // Example 7
            // Section: Extensions Heading Auto Identifiers
            //
            // The following CommonMark:
            //     # 1.0 & ^ % *
            //     # 1.0 & ^ % *
            //
            // Should be rendered as:
            //     <h1 id="section">1.0 &amp; ^ % *</h1>
            //     <h1 id="section-1">1.0 &amp; ^ % *</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 7, "Extensions Heading Auto Identifiers");
			TestParser.TestSpec("# 1.0 & ^ % *\n# 1.0 & ^ % *", "<h1 id=\"section\">1.0 &amp; ^ % *</h1>\n<h1 id=\"section-1\">1.0 &amp; ^ % *</h1>", "autoidentifiers|advanced");
        }
    }
        // When the options "AutoLink" is setup, it is possible to link to an existing heading by using the
        // exact same Label text as the heading:
    [TestFixture]
    public partial class TestExtensionsHeadingAutoIdentifiers
    {
        [Test]
        public void Example008()
        {
            // Example 8
            // Section: Extensions Heading Auto Identifiers
            //
            // The following CommonMark:
            //     # This is a heading
            //     [This is a heading]
            //
            // Should be rendered as:
            //     <h1 id="this-is-a-heading">This is a heading</h1>
            //     <p><a href="#this-is-a-heading">This is a heading</a></p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 8, "Extensions Heading Auto Identifiers");
			TestParser.TestSpec("# This is a heading\n[This is a heading]", "<h1 id=\"this-is-a-heading\">This is a heading</h1>\n<p><a href=\"#this-is-a-heading\">This is a heading</a></p>", "autoidentifiers|advanced");
        }
    }
        // Links before the heading are also working:
    [TestFixture]
    public partial class TestExtensionsHeadingAutoIdentifiers
    {
        [Test]
        public void Example009()
        {
            // Example 9
            // Section: Extensions Heading Auto Identifiers
            //
            // The following CommonMark:
            //     [This is a heading]
            //     # This is a heading
            //
            // Should be rendered as:
            //     <p><a href="#this-is-a-heading">This is a heading</a></p>
            //     <h1 id="this-is-a-heading">This is a heading</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 9, "Extensions Heading Auto Identifiers");
			TestParser.TestSpec("[This is a heading]\n# This is a heading", "<p><a href=\"#this-is-a-heading\">This is a heading</a></p>\n<h1 id=\"this-is-a-heading\">This is a heading</h1>", "autoidentifiers|advanced");
        }
    }
        // The text of the link can be changed:
    [TestFixture]
    public partial class TestExtensionsHeadingAutoIdentifiers
    {
        [Test]
        public void Example010()
        {
            // Example 10
            // Section: Extensions Heading Auto Identifiers
            //
            // The following CommonMark:
            //     [With a new text][This is a heading]
            //     # This is a heading
            //
            // Should be rendered as:
            //     <p><a href="#this-is-a-heading">With a new text</a></p>
            //     <h1 id="this-is-a-heading">This is a heading</h1>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 10, "Extensions Heading Auto Identifiers");
			TestParser.TestSpec("[With a new text][This is a heading]\n# This is a heading", "<p><a href=\"#this-is-a-heading\">With a new text</a></p>\n<h1 id=\"this-is-a-heading\">This is a heading</h1>", "autoidentifiers|advanced");
        }
    }
        // # Extensions
        //
        // Adds support for task lists:
        //
        // ## TaskLists
        //
        // A task list item consist of `[ ]` or `[x]` or `[X]` inside a list item (ordered or unordered)
    [TestFixture]
    public partial class TestExtensionsTaskLists
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions TaskLists
            //
            // The following CommonMark:
            //     - [ ] Item1
            //     - [x] Item2
            //     - [ ] Item3
            //     - Item4
            //
            // Should be rendered as:
            //     <ul class="contains-task-list">
            //     <li class="task-list-item"><input disabled="disabled" type="checkbox" /> Item1</li>
            //     <li class="task-list-item"><input disabled="disabled" type="checkbox" checked="checked" /> Item2</li>
            //     <li class="task-list-item"><input disabled="disabled" type="checkbox" /> Item3</li>
            //     <li>Item4</li>
            //     </ul>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions TaskLists");
			TestParser.TestSpec("- [ ] Item1\n- [x] Item2\n- [ ] Item3\n- Item4", "<ul class=\"contains-task-list\">\n<li class=\"task-list-item\"><input disabled=\"disabled\" type=\"checkbox\" /> Item1</li>\n<li class=\"task-list-item\"><input disabled=\"disabled\" type=\"checkbox\" checked=\"checked\" /> Item2</li>\n<li class=\"task-list-item\"><input disabled=\"disabled\" type=\"checkbox\" /> Item3</li>\n<li>Item4</li>\n</ul>", "tasklists|advanced");
        }
    }
        // A task is not recognized outside a list item:
    [TestFixture]
    public partial class TestExtensionsTaskLists
    {
        [Test]
        public void Example002()
        {
            // Example 2
            // Section: Extensions TaskLists
            //
            // The following CommonMark:
            //     [ ] This is not a task list
            //
            // Should be rendered as:
            //     <p>[ ] This is not a task list</p>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 2, "Extensions TaskLists");
			TestParser.TestSpec("[ ] This is not a task list", "<p>[ ] This is not a task list</p>", "tasklists|advanced");
        }
    }
        // # Extensions
        //
        // Adds support for diagrams extension:
        //
        // ## Mermaid diagrams
        //
        // Using a fenced code block with the `mermaid` language info will output a `<div class='mermaid'>` instead of a `pre/code` block:
    [TestFixture]
    public partial class TestExtensionsMermaiddiagrams
    {
        [Test]
        public void Example001()
        {
            // Example 1
            // Section: Extensions Mermaid diagrams
            //
            // The following CommonMark:
            //     ```mermaid
            //     graph TD;
            //         A-->B;
            //         A-->C;
            //         B-->D;
            //         C-->D;
            //     ```
            //
            // Should be rendered as:
            //     <div class="mermaid">graph TD;
            //         A-->B;
            //         A-->C;
            //         B-->D;
            //         C-->D;
            //     </div>

            Console.WriteLine("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 1, "Extensions Mermaid diagrams");
			TestParser.TestSpec("```mermaid\ngraph TD;\n    A-->B;\n    A-->C;\n    B-->D;\n    C-->D;\n```", "<div class=\"mermaid\">graph TD;\n    A-->B;\n    A-->C;\n    B-->D;\n    C-->D;\n</div>", "diagrams|advanced");
        }
    }
        // TODO: Add other text diagram languages
}
