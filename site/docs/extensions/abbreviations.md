---
title: Abbreviations
---

# Abbreviations

Enable with `.UseAbbreviations()` (included in `UseAdvancedExtensions()`).

Abbreviations define expansions for acronyms and short terms. When the abbreviation appears in the text, it is wrapped in an `<abbr>` tag with a `title` attribute. Inspired by [PHP Markdown Extra](https://michelf.ca/projects/php-markdown/extra/#abbr).

## Syntax

Define abbreviations anywhere in the document using `*[ABBR]: Full text`:

```markdown
*[HTML]: Hyper Text Markup Language
*[CSS]: Cascading Style Sheets

This page uses HTML and CSS.
```

*[HTML]: Hyper Text Markup Language
*[CSS]: Cascading Style Sheets

This page uses HTML and CSS.

## HTML output

```html
<p>This page uses <abbr title="Hyper Text Markup Language">HTML</abbr>
and <abbr title="Cascading Style Sheets">CSS</abbr>.</p>
```

## Rules

- Abbreviation definitions are not rendered as visible content.
- Matching is case-sensitive and matches whole words only.
- Abbreviation definitions can appear anywhere in the document — they apply globally.
- Multiple abbreviations can be defined in the same document.
