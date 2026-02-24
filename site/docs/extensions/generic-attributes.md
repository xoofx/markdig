---
title: Generic attributes
---

# Generic attributes

Enable with `.UseGenericAttributes()` (included in `UseAdvancedExtensions()`).

This extension allows attaching CSS classes, IDs, and arbitrary HTML attributes to nearly any Markdown element using `{...}` syntax. Inspired by [PHP Markdown Extra — Special Attributes](https://michelf.ca/projects/php-markdown/extra/#spe-attr).

> [!IMPORTANT]
> `UseGenericAttributes()` should be the **last** extension added to the pipeline, as it modifies other parsers to recognize attribute syntax.

## Syntax

Place `{...}` after a Markdown element:

```markdown
# Heading {#custom-id .special-class}

A paragraph with attributes. {.lead}

[A link](https://example.com){target="_blank" rel="noopener"}
```

## Supported attribute types

{.table}
| Syntax | Meaning | Example |
|---|---|---|
| `#value` | HTML `id` | `{#my-id}` |
| `.value` | CSS class | `{.my-class}` |
| `key=value` | HTML attribute | `{data-count=5}` |
| `key="value"` | Quoted attribute | `{title="Hello World"}` |

Multiple attributes can be combined:

```markdown
A paragraph. {#intro .highlight data-section="overview"}
```

## Applying to blocks

### Headings

```markdown
## My Section {#section-1 .special}
```

Renders as: `<h2 id="section-1" class="special">My Section</h2>`

### Paragraphs

Place the attributes at the end of the paragraph:

```markdown
This is a styled paragraph. {.lead .text-center}
```

### Code blocks

````markdown
```csharp {.highlight-lines}
var x = 42;
```
````

### Blockquotes

```markdown
> A styled blockquote {.fancy-quote}
```

### Tables

The `.table` class is commonly used to apply Bootstrap-style table formatting:

```markdown
{.table .table-striped}
| A | B |
|---|---|
| 1 | 2 |
```

## Applying to inlines

### Links

```markdown
[Click me](https://example.com){.btn .btn-primary}
```

### Images

```markdown
![Photo](photo.jpg){.rounded width="200"}
```

### Emphasis

```markdown
**Important text**{.text-danger}
```

## HTML output

```markdown
A paragraph. {.lead #intro}
```

Produces:

```html
<p class="lead" id="intro">A paragraph.</p>
```
