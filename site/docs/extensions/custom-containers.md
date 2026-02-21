---
title: Custom containers
---

# Custom containers

Enable with `.UseCustomContainers()` (included in `UseAdvancedExtensions()`).

Custom containers generate `<div>` elements from fenced `:::` blocks, similar to how fenced code blocks work. They are inspired by this [CommonMark discussion](https://talk.commonmark.org/t/custom-container-for-block-and-inline/2051).

## Block containers

Use `:::` to open and close a container block:

```markdown
::: warning
This is a warning container. You can put **any Markdown** content here.

- Including lists
- And other blocks
:::
```

::: warning
This is a warning container. You can put **any Markdown** content here.

- Including lists
- And other blocks
:::

### HTML output

```html
<div class="warning">
  <p>This is a warning container...</p>
</div>
```

The text after `:::` becomes the CSS class of the `<div>`.

## Inline containers

Use a single `:` pair for inline containers:

```markdown
This has a :custom-span[styled word]{.highlight} in it.
```

## Nesting

Containers can be nested using more colons:

```markdown
:::: outer
::: inner
Nested content.
:::
::::
```

## Attributes

Combine with [Generic attributes](generic-attributes.md) for full control:

```markdown
::: {.alert .alert-info #my-alert role="alert"}
This is an informational alert.
:::
```

This produces:

```html
<div class="alert alert-info" id="my-alert" role="alert">
  <p>This is an informational alert.</p>
</div>
```
