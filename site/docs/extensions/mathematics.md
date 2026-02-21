---
title: Mathematics
---

# Mathematics

Enable with `.UseMathematics()` (included in `UseAdvancedExtensions()`).

This extension supports LaTeX-style math using `$` for inline and `$$` for display (block) math.

## Inline math

Wrap math expressions with single `$`:

```markdown
The quadratic formula is $x = \frac{-b \pm \sqrt{b^2 - 4ac}}{{ "}{" }}2a}$.
```

The quadratic formula is $x = \frac{-b \pm \sqrt{b^2 - 4ac}}{{ "}{" }}2a}$.

## Block math

Wrap display equations with `$$` on their own lines:

```markdown
$$
\int_0^\infty e^{-x^2} dx = \frac{\sqrt{\pi}}{{ "}{" }}2}
$$
```

$$
\int_0^\infty e^{-x^2} dx = \frac{\sqrt{\pi}}{{ "}{" }}2}
$$

## HTML output

- **Inline math** renders as `<span class="math">\(...\)</span>`
- **Block math** renders as `<div class="math">\[...\]</div>`

This HTML is designed to be consumed by math rendering libraries such as [KaTeX](https://katex.org/) or [MathJax](https://www.mathjax.org/).

### Example output

```html
<p>The formula is <span class="math">\(E = mc^2\)</span>.</p>
```

```html
<div class="math">\[
\sum_{i=1}^n i = \frac{n(n+1)}{2}
\]</div>
```

## Rules

- Inline math (`$...$`) must not have a space immediately after the opening `$` or before the closing `$`.
- Block math (`$$...$$`) uses `$$` on separate lines.
- A single `$` surrounded by spaces is treated as a literal dollar sign, not math.

## Integrating with KaTeX or MathJax

After rendering to HTML, include a math library in your page to render the formulas:

```html
<!-- KaTeX -->
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/katex/dist/katex.min.css">
<script src="https://cdn.jsdelivr.net/npm/katex/dist/katex.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/katex/dist/contrib/auto-render.min.js"></script>
<script>
  renderMathInElement(document.body, {
    delimiters: [
      { left: "\\(", right: "\\)", display: false },
      { left: "\\[", right: "\\]", display: true }
    ]
  });
</script>
```
