---
title: "Figures, footers & citations"
---

# Figures, footers & citations

These three related extensions add HTML5 semantic elements to Markdown.

## Figures

Enable with `.UseFigures()` (included in `UseAdvancedExtensions()`).

Use `^^^` to create `<figure>` blocks with optional `<figcaption>`:

```markdown
^^^
![A scenic mountain](mountain.jpg)
^^^ A beautiful mountain landscape
```

### HTML output

```html
<figure>
  <p><img src="mountain.jpg" alt="A scenic mountain" /></p>
  <figcaption>A beautiful mountain landscape</figcaption>
</figure>
```

### Multiple items in a figure

```markdown
^^^
![Photo 1](photo1.jpg)
![Photo 2](photo2.jpg)
^^^ A gallery of photos
```

## Footers

Enable with `.UseFooters()` (included in `UseAdvancedExtensions()`).

Use `^^` at the start of a line to create `<footer>` elements:

```markdown
^^ This is a footer element.
```

### HTML output

```html
<footer>This is a footer element.</footer>
```

### Multi-line footers

```markdown
^^ This is the first line of the footer.
^^ This is the second line.
```

## Citations

Enable with `.UseCitations()` (included in `UseAdvancedExtensions()`).

Wrap text in double quotes `""` to create a `<cite>` element:

```markdown
""The Art of Computer Programming""
```

### HTML output

```html
<p><cite>The Art of Computer Programming</cite></p>
```

### In context

```markdown
As described in ""Design Patterns"" by the Gang of Four, the Observer pattern
is used to define a one-to-many dependency between objects.
```

## Combining figures with citations

```markdown
^^^
> "The best way to predict the future is to invent it." — Alan Kay
^^^ ""Computing in the 21st Century""
```
