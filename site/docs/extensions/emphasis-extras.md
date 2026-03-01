---
title: Emphasis extras
---

# Emphasis extras

Enable with `.UseEmphasisExtras()` (included in `UseAdvancedExtensions()`).

This extension adds several emphasis styles beyond standard bold and italic.

If you need improved emphasis parsing for Chinese/Japanese/Korean (CJK) text, see [CJK friendly emphasis](cjk-friendly-emphasis.md).

## Strikethrough

Wrap text with `~~` for strikethrough:

```markdown
This is ~~deleted~~ text.
```

This is ~~deleted~~ text.

## Subscript

Wrap text with `~` for subscript:

```markdown
H~2~O is water.
```

H~2~O is water.

## Superscript

Wrap text with `^` for superscript:

```markdown
2^10^ is 1024.
```

2^10^ is 1024.

## Inserted text

Wrap text with `++` for inserted/underlined text:

```markdown
This text has been ++inserted++.
```

This text has been ++inserted++.

## Marked/highlighted text

Wrap text with `==` for marked/highlighted text:

```markdown
This is ==highlighted== text.
```

This is ==highlighted== text.

## HTML output

{.table}
| Syntax | HTML output |
|---|---|
| `~~text~~` | `<del>text</del>` |
| `~text~` | `<sub>text</sub>` |
| `^text^` | `<sup>text</sup>` |
| `++text++` | `<ins>text</ins>` |
| `==text==` | `<mark>text</mark>` |

## Selective activation

By default, all emphasis extras are enabled. Use `EmphasisExtraOptions` to enable only specific ones:

```csharp
using Markdig.Extensions.EmphasisExtras;

var pipeline = new MarkdownPipelineBuilder()
    .UseEmphasisExtras(EmphasisExtraOptions.Strikethrough | EmphasisExtraOptions.Superscript)
    .Build();
```

Available options:

- `EmphasisExtraOptions.Strikethrough`
- `EmphasisExtraOptions.Subscript`
- `EmphasisExtraOptions.Superscript`
- `EmphasisExtraOptions.Inserted`
- `EmphasisExtraOptions.Marked`
- `EmphasisExtraOptions.Default` (all of the above)
