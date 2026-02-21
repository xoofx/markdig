---
title: Auto-identifiers
---

# Auto-identifiers

Enable with `.UseAutoIdentifiers()` (included in `UseAdvancedExtensions()`).

This extension automatically generates `id` attributes for all headings, similar to [Pandoc auto identifiers](https://pandoc.org/MANUAL.html#extension-auto_identifiers). This is essential for linking to specific sections.

## How it works

Every heading gets an `id` attribute derived from its text content:

```markdown
## Getting Started
```

Renders as:

```html
<h2 id="getting-started">Getting Started</h2>
```

You can then link to it:

```markdown
See [Getting Started](#getting-started).
```

## ID generation rules

1. Convert to lowercase
2. Remove anything that is not a letter, number, space, or hyphen
3. Replace spaces with hyphens
4. Remove leading/trailing hyphens

**Examples:**

{.table}
| Heading | Generated ID |
|---|---|
| `# Hello World` | `hello-world` |
| `## C# Tips & Tricks` | `c-tips--tricks` |
| `### 2. Installation` | `2-installation` |

## Duplicate handling

If two headings produce the same ID, a numeric suffix is appended:

```markdown
## Section
## Section
## Section
```

Produces: `section`, `section-1`, `section-2`.

## Options

`UseAutoIdentifiers` accepts an `AutoIdentifierOptions` flags enum:

```csharp
using Markdig.Extensions.AutoIdentifiers;

var pipeline = new MarkdownPipelineBuilder()
    .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
    .Build();
```

Available options:

{.table}
| Option | Description |
|---|---|
| `AutoIdentifierOptions.Default` | Standard auto-identifier behavior |
| `AutoIdentifierOptions.GitHub` | GitHub-compatible ID generation |
| `AutoIdentifierOptions.AutoLink` | Also create a self-link anchor |
| `AutoIdentifierOptions.AllowOnlyAscii` | Strip non-ASCII characters from IDs |

## Combining with generic attributes

You can override the auto-generated ID using [Generic attributes](generic-attributes.md):

```markdown
## My Heading {#custom-id}
```

The explicit `#custom-id` takes precedence over the auto-generated one.
