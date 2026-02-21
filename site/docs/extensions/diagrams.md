---
title: Diagrams
---

# Diagrams

Enable with `.UseDiagrams()` (included in `UseAdvancedExtensions()`).

When a fenced code block uses a recognized diagram language as its info string, Markdig wraps the content in a `<div>` with the language as a CSS class instead of rendering it as `<pre><code>`.

## Supported languages

{.table}
| Language | Info string |
|---|---|
| [Mermaid](https://mermaid.js.org/) | `mermaid` |
| [nomnoml](https://github.com/skanaar/nomnoml) | `nomnoml` |

## Mermaid example

````markdown
```mermaid
graph LR
    A[Parse] --> B[AST]
    B --> C[Render]
    C --> D[HTML]
```
````

This renders as:

```html
<div class="mermaid">graph LR
    A[Parse] --> B[AST]
    B --> C[Render]
    C --> D[HTML]
</div>
```

To display the diagram in a browser, include the Mermaid JavaScript library:

```html
<script src="https://cdn.jsdelivr.net/npm/mermaid/dist/mermaid.min.js"></script>
<script>mermaid.initialize({ startOnLoad: true });</script>
```

## nomnoml example

````markdown
```nomnoml
[Markdown] -> [Parser]
[Parser] -> [AST]
[AST] -> [Renderer]
[Renderer] -> [HTML]
```
````

## HTML output

Instead of the usual code block rendering:

```html
<pre><code class="language-mermaid">...</code></pre>
```

The diagrams extension produces:

```html
<div class="mermaid">...</div>
```

This allows client-side diagram libraries to find and render the content.
