---
title: Alert blocks
---

# Alert blocks

Enable with `.UseAlertBlocks()` (included in `UseAdvancedExtensions()`).

Alert blocks are GitHub-style callouts for highlighting important content in documentation. They are rendered as styled `<div>` elements.

## Syntax

Alert blocks are blockquotes that begin with a special `[!TYPE]` marker:

```markdown
> [!NOTE]
> Useful information that users should know, even when skimming content.

> [!TIP]
> Helpful advice for doing things better or more easily.

> [!IMPORTANT]
> Key information users need to know to achieve their goal.

> [!WARNING]
> Urgent info that needs immediate user attention to avoid problems.

> [!CAUTION]
> Advises about risks or negative outcomes of certain actions.
```

> [!NOTE]
> Useful information that users should know, even when skimming content.

> [!TIP]
> Helpful advice for doing things better or more easily.

> [!IMPORTANT]
> Key information users need to know to achieve their goal.

> [!WARNING]
> Urgent info that needs immediate user attention to avoid problems.

> [!CAUTION]
> Advises about risks or negative outcomes of certain actions.

## Alert types

{.table}
| Type | Purpose |
|---|---|
| `[!NOTE]` | Supplementary information |
| `[!TIP]` | Helpful suggestions |
| `[!IMPORTANT]` | Critical information |
| `[!WARNING]` | Potential problems |
| `[!CAUTION]` | Risk of negative outcomes |

## HTML output

Each alert renders as:

```html
<div class="markdown-alert markdown-alert-note">
  <p class="markdown-alert-title">Note</p>
  <p>Your alert content here.</p>
</div>
```

The `markdown-alert-{type}` CSS class allows you to apply custom styling for each alert kind.

## Content within alerts

Alerts support full Markdown content — paragraphs, lists, code blocks, emphasis, etc.:

```markdown
> [!TIP]
> You can use **bold**, *italic*, and `code` in alerts.
>
> - Even lists work
> - Inside alert blocks
>
> ```csharp
> var x = 42; // And code blocks too!
> ```
```

> [!TIP]
> You can use **bold**, *italic*, and `code` in alerts.
>
> - Even lists work
> - Inside alert blocks
>
> ```csharp
> var x = 42; // And code blocks too!
> ```

## Custom rendering

Pass a custom renderer delegate to `UseAlertBlocks` to override the kind rendering:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseAlertBlocks(renderKind: (renderer, kind) =>
    {
        renderer.Write($"<span class=\"icon\">{kind}</span>");
    })
    .Build();
```
