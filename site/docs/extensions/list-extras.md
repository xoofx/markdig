---
title: List extras
---

# List extras

Enable with `.UseListExtras()` (included in `UseAdvancedExtensions()`).

This extension adds support for additional ordered list item types beyond the standard numeric `1.` markers.

## Alpha lists

Use lowercase or uppercase letters followed by `.`:

```markdown
a. First item
b. Second item
c. Third item
```

a. First item
b. Second item
c. Third item

### Uppercase

```markdown
A. First item
B. Second item
C. Third item
```

A. First item
B. Second item
C. Third item

## Roman numeral lists

Use lowercase or uppercase Roman numerals followed by `.`:

```markdown
i. First item
ii. Second item
iii. Third item
iv. Fourth item
```

i. First item
ii. Second item
iii. Third item
iv. Fourth item

### Uppercase Roman

```markdown
I. First item
II. Second item
III. Third item
```

I. First item
II. Second item
III. Third item

## HTML output

The `type` attribute is set on the `<ol>` element:

```html
<ol type="a">
  <li>First item</li>
  <li>Second item</li>
</ol>
```

{.table}
| Marker | HTML `type` |
|---|---|
| `a.` | `a` |
| `A.` | `A` |
| `i.` | `i` |
| `I.` | `I` |
