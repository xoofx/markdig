---
title: Definition lists
---

# Definition lists

Enable with `.UseDefinitionLists()` (included in `UseAdvancedExtensions()`).

Definition lists render as `<dl>` / `<dt>` / `<dd>` HTML elements. Inspired by [PHP Markdown Extra](https://michelf.ca/projects/php-markdown/extra/#def-list).

## Syntax

A definition list consists of terms followed by their definitions. Definitions are prefixed with `:` (colon followed by a space):

```markdown
Term 1
:   Definition of term 1.

Term 2
:   Definition of term 2.
:   Another definition of term 2.
```

Term 1
:   Definition of term 1.

Term 2
:   Definition of term 2.
:   Another definition of term 2.

## Multi-line definitions

Definitions can span multiple lines and contain block-level content:

```markdown
Apple
:   A fruit that grows on trees.

    Apples come in many varieties including
    Granny Smith and Fuji.

Orange
:   A citrus fruit.
```

Apple
:   A fruit that grows on trees.

    Apples come in many varieties including
    Granny Smith and Fuji.

Orange
:   A citrus fruit.

## Multiple terms per definition

```markdown
Term A
Term B
:   Shared definition for both terms.
```

Term A
Term B
:   Shared definition for both terms.

## HTML output

```html
<dl>
  <dt>Term 1</dt>
  <dd>Definition of term 1.</dd>
  <dt>Term 2</dt>
  <dd>Definition of term 2.</dd>
  <dd>Another definition of term 2.</dd>
</dl>
```

## Rules

- There must be a blank line before the first term (or it will be treated as a paragraph).
- The `:` marker must be followed by at least one space.
- Continuation lines must be indented.
