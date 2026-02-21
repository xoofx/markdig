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

## Inline footnotes

You can also define footnotes inline (though this is less common):

```markdown
This has an inline footnote^[This is the inline footnote content].
```

## HTML output

Footnote references become superscript links, and footnote definitions are collected into a `<section>` at the end of the page:

```html
<p>Text with a footnote<a href="#fn:1" class="footnote-ref"><sup>1</sup></a>.</p>

<section class="footnotes">
  <ol>
    <li id="fn:1">
      <p>This is the footnote content.
        <a href="#fnref:1" class="footnote-back-ref">↩</a></p>
    </li>
  </ol>
</section>
```

## Rules

- Footnote labels are case-insensitive.
- Footnote definitions can appear anywhere in the document — they are always rendered at the end.
- Unused footnote definitions are not rendered.
- Multiple references to the same footnote share the same content.
