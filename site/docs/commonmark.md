---
title: CommonMark syntax
---

# CommonMark syntax

Markdig is fully compliant with the [CommonMark specification](https://spec.commonmark.org/) (v0.31.2), passing 600+ spec tests. This page is a reference for all core Markdown syntax supported out of the box — no extensions required.

## Headings

### ATX headings

Use `#` characters (1–6) followed by a space:

```markdown
# Heading 1
## Heading 2
### Heading 3
#### Heading 4
##### Heading 5
###### Heading 6
```

### Setext headings

Underline text with `=` (level 1) or `-` (level 2):

```markdown
Heading 1
=========

Heading 2
---------
```

## Paragraphs

Paragraphs are separated by one or more blank lines. A single newline within a paragraph does **not** create a line break — it's treated as a space.

```markdown
This is the first paragraph.

This is the second paragraph. This sentence
continues on the next line but renders inline.
```

## Line breaks

To create a hard line break within a paragraph, end a line with **two or more spaces** or a backslash `\`:

```markdown
First line  
Second line (two trailing spaces above)

First line\
Second line (backslash above)
```

## Emphasis and strong emphasis

```markdown
*italic text* or _italic text_
**bold text** or __bold text__
***bold and italic*** or ___bold and italic___
```

Renders as:

- *italic text*
- **bold text**
- ***bold and italic***

## Links

### Inline links

```markdown
[Link text](https://example.com)
[Link with title](https://example.com "Example Title")
```

### Reference links

```markdown
[Link text][ref]
[Another link][ref]

[ref]: https://example.com "Optional Title"
```

### Autolinks

Angle brackets around a URL or email:

```markdown
<https://example.com>
<user@example.com>
```

## Images

```markdown
![Alt text](https://example.com/image.png)
![Alt text](https://example.com/image.png "Title")

![Alt text][imgref]

[imgref]: https://example.com/image.png "Title"
```

## Code

### Inline code

```markdown
Use the `Markdown.ToHtml()` method.
```

Use the `Markdown.ToHtml()` method.

### Fenced code blocks

Use triple backticks or triple tildes, optionally with a language identifier:

````markdown
```csharp
var html = Markdown.ToHtml("Hello **world**!");
```
````

Or with tildes:

````markdown
~~~python
print("Hello, world!")
~~~
````

### Indented code blocks

Indent every line by 4 spaces or 1 tab:

```markdown
    var x = 42;
    Console.WriteLine(x);
```

## Blockquotes

Prefix lines with `>`:

```markdown
> This is a blockquote.
>
> It can span multiple paragraphs.
>
> > And be nested.
```

> This is a blockquote.
>
> It can span multiple paragraphs.
>
> > And be nested.

## Lists

### Unordered lists

Use `-`, `*`, or `+` as markers:

```markdown
- Item one
- Item two
  - Nested item
- Item three
```

- Item one
- Item two
  - Nested item
- Item three

### Ordered lists

Use numbers followed by `.` or `)`:

```markdown
1. First item
2. Second item
3. Third item

1) Also valid
2) With parentheses
```

1. First item
2. Second item
3. Third item

### List continuation

Indent content to align with the list item text:

```markdown
1. First paragraph of item.

   Second paragraph of the same item.

2. Another item.
```

## Thematic breaks

Three or more `-`, `*`, or `_` on a line (optionally with spaces):

```markdown
---
***
___
```

---

## HTML blocks

Raw HTML can be included directly:

```markdown
<div class="custom">
  This is raw HTML.
</div>
```

## Inline HTML

HTML tags can appear within inline content:

```markdown
This is <em>inline HTML</em> in a paragraph.
```

## Escaping

Use a backslash `\` to escape special characters:

```markdown
\*Not italic\*
\# Not a heading
\[Not a link\]
```

The following characters can be escaped: `` \ ` * _ { } [ ] ( ) # + - . ! | ``

## Entities

HTML entities are supported:

```markdown
&copy; &amp; &lt; &gt; &nbsp;
&#169; &#x00A9;
```

## Blank lines

Blank lines separate block elements. Multiple consecutive blank lines are treated the same as a single blank line.

## Next steps

- [Extensions](extensions/readme.md) — Enable tables, task lists, math, footnotes, and 20+ more features
- [Getting started](getting-started.md) — Install and configure Markdig
