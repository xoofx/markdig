---
title: Footnotes
---

# Footnotes

Enable with `.UseFootnotes()` (included in `UseAdvancedExtensions()`).

Footnotes allow you to add references that appear at the bottom of the document, inspired by [PHP Markdown Extra](https://michelf.ca/projects/php-markdown/extra/#footnotes).

## Syntax

Define a footnote reference inline with `[^label]` and the footnote content elsewhere:

```markdown
Here is a sentence with a footnote[^1].

And another with a named footnote[^note].

[^1]: This is the first footnote content.
[^note]: Footnotes can have any label, not just numbers.
```

Here is a sentence with a footnote[^1].

And another with a named footnote[^note].

[^1]: This is the first footnote content.
[^note]: Footnotes can have any label, not just numbers.

## Multi-line footnotes

Indent continuation lines to include multiple paragraphs in a footnote:

```markdown
This has a long footnote[^long].

[^long]: This is the first paragraph of the footnote.

    This is the second paragraph. It must be indented to be
    included in the footnote.

    - Even lists work in footnotes
    - Like this one
```

This has a long footnote[^long].

[^long]: This is the first paragraph of the footnote.

    This is the second paragraph. It must be indented to be
    included in the footnote.

    - Even lists work in footnotes
    - Like this one

## HTML output

Footnote references become superscript links, and footnote definitions are collected into a `<div>` at the end of the page:

```html
<p>Text with a footnote<a id="fnref:1" href="#fn:1" class="footnote-ref"><sup>1</sup></a>.</p>

<div class="footnotes">
  <hr />
  <ol>
    <li id="fn:1">
      <p>This is the footnote content.<a href="#fnref:1" class="footnote-back-ref">&#8617;</a></p>
    </li>
  </ol>
</div>
```

## Rules

- Footnote labels are case-insensitive.
- Footnote definitions can appear anywhere in the document — they are always rendered at the end.
- Unused footnote definitions are not rendered.
- Multiple references to the same footnote share the same content.
