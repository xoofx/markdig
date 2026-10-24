# Extensions

This section describes the different extensions supported:

## Table of Contents

The table of contents extension replaces a paragraph that consists of `[TOC]` alone with a nested list of the headings of the document. It is enabled with `UseTableOfContents()`. The examples also enable `UseAutoIdentifiers()`, which gives every heading an ID.

The list is wrapped in a `nav` element with the `doc-toc` role. An entry contains the text of its heading, and the entries of the headings below it form a nested list:

```````````````````````````````` example
[TOC]

# Foo
## Bar
## Baz
.
<nav class="toc" role="doc-toc">
<ul>
<li>Foo
<ul>
<li>Bar</li>
<li>Baz</li>
</ul>
</li>
</ul>
</nav>
<h1 id="foo">Foo</h1>
<h2 id="bar">Bar</h2>
<h2 id="baz">Baz</h2>
````````````````````````````````

Every heading of the document is listed, including the headings before the marker:

```````````````````````````````` example
# Intro

[TOC]

## Usage
.
<h1 id="intro">Intro</h1>
<nav class="toc" role="doc-toc">
<ul>
<li>Intro
<ul>
<li>Usage</li>
</ul>
</li>
</ul>
</nav>
<h2 id="usage">Usage</h2>
````````````````````````````````

A heading is nested below the closest preceding heading with a lower level, so a skipped level produces no empty entry:

```````````````````````````````` example
[TOC]

# Foo
### Bar
## Baz
.
<nav class="toc" role="doc-toc">
<ul>
<li>Foo
<ul>
<li>Bar</li>
<li>Baz</li>
</ul>
</li>
</ul>
</nav>
<h1 id="foo">Foo</h1>
<h3 id="bar">Bar</h3>
<h2 id="baz">Baz</h2>
````````````````````````````````

A heading without such a preceding heading is a top-level entry, even if its level is higher than the level of a later heading:

```````````````````````````````` example
[TOC]

### Foo
# Bar
## Baz
.
<nav class="toc" role="doc-toc">
<ul>
<li>Foo</li>
<li>Bar
<ul>
<li>Baz</li>
</ul>
</li>
</ul>
</nav>
<h3 id="foo">Foo</h3>
<h1 id="bar">Bar</h1>
<h2 id="baz">Baz</h2>
````````````````````````````````

Headings inside container blocks are listed too. Headings without content are left out:

```````````````````````````````` example
[TOC]

> # Foo

- ## Bar

#
.
<nav class="toc" role="doc-toc">
<ul>
<li>Foo
<ul>
<li>Bar</li>
</ul>
</li>
</ul>
</nav>
<blockquote>
<h1 id="foo">Foo</h1>
</blockquote>
<ul>
<li>
<h2 id="bar">Bar</h2>
</li>
</ul>
<h1 id="section"></h1>
````````````````````````````````

An entry contains the plain text of its heading. The formatting is dropped, so that all entries look the same:

```````````````````````````````` example
[TOC]

# Use *the* `<T>` & co
.
<nav class="toc" role="doc-toc">
<ul>
<li>Use the &lt;T&gt; &amp; co</li>
</ul>
</nav>
<h1 id="use-the-t-co">Use <em>the</em> <code>&lt;T&gt;</code> &amp; co</h1>
````````````````````````````````

Headings with the same text get an entry each:

```````````````````````````````` example
[TOC]

# Foo
# Foo
.
<nav class="toc" role="doc-toc">
<ul>
<li>Foo</li>
<li>Foo</li>
</ul>
</nav>
<h1 id="foo">Foo</h1>
<h1 id="foo-1">Foo</h1>
````````````````````````````````

Every marker of a document renders the same list:

```````````````````````````````` example
[TOC]

# Foo

[TOC]
.
<nav class="toc" role="doc-toc">
<ul>
<li>Foo</li>
</ul>
</nav>
<h1 id="foo">Foo</h1>
<nav class="toc" role="doc-toc">
<ul>
<li>Foo</li>
</ul>
</nav>
````````````````````````````````

A marker inside a container block is recognized as well:

```````````````````````````````` example
- [TOC]

> [TOC]

# Foo
.
<ul>
<li>
<nav class="toc" role="doc-toc">
<ul>
<li>Foo</li>
</ul>
</nav>
</li>
</ul>
<blockquote>
<nav class="toc" role="doc-toc">
<ul>
<li>Foo</li>
</ul>
</nav>
</blockquote>
<h1 id="foo">Foo</h1>
````````````````````````````````

The marker needs no blank line after it when the next line starts a block that interrupts a paragraph:

```````````````````````````````` example
[TOC]
# Foo
.
<nav class="toc" role="doc-toc">
<ul>
<li>Foo</li>
</ul>
</nav>
<h1 id="foo">Foo</h1>
````````````````````````````````

A document without headings renders no table of contents:

```````````````````````````````` example
[TOC]

Text
.
<p>Text</p>
````````````````````````````````

The marker takes precedence over a link to a heading with the text `TOC`:

```````````````````````````````` example
[TOC]

# TOC
.
<nav class="toc" role="doc-toc">
<ul>
<li>TOC</li>
</ul>
</nav>
<h1 id="toc">TOC</h1>
````````````````````````````````

It also takes precedence over a link reference definition with the label `TOC`. An escaped bracket keeps the text, and an explicit collapsed reference keeps the link:

```````````````````````````````` example
[TOC]

# Foo

\[TOC]

[TOC][]

[TOC]: /url
.
<nav class="toc" role="doc-toc">
<ul>
<li>Foo</li>
</ul>
</nav>
<h1 id="foo">Foo</h1>
<p>[TOC]</p>
<p><a href="/url">TOC</a></p>
````````````````````````````````

Only a paragraph that consists of `[TOC]` alone is a marker. More text in the paragraph, another casing or spacing, and a code span render as usual:

```````````````````````````````` example
[TOC] text

[TOC]
text

[toc]

[ TOC ]

`[TOC]`

# Foo
.
<p>[TOC] text</p>
<p>[TOC]
text</p>
<p>[toc]</p>
<p>[ TOC ]</p>
<p><code>[TOC]</code></p>
<h1 id="foo">Foo</h1>
````````````````````````````````

A lazy continuation line belongs to the paragraph before it:

```````````````````````````````` example
> Foo
[TOC]

# Bar
.
<blockquote>
<p>Foo
[TOC]</p>
</blockquote>
<h1 id="bar">Bar</h1>
````````````````````````````````

An indented code block and a setext heading are not markers either:

```````````````````````````````` example
    [TOC]

[TOC]
=====
.
<pre><code>[TOC]
</code></pre>
<h1 id="toc">[TOC]</h1>
````````````````````````````````
