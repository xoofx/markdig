---
title: Abstract syntax tree
---

# Abstract syntax tree

The `Markdown.Parse(...)` method returns a `MarkdownDocument` — the root of the abstract syntax tree (AST). The AST is a tree of `Block` and `Inline` nodes that fully represents the semantic structure of the Markdown input.

## AST structure

There are two general categories of node:

- **Block nodes** — Represent block-level constructs: paragraphs, headings, lists, code blocks, blockquotes, etc.
- **Inline nodes** — Represent inline constructs within a block: emphasis, links, code spans, images, line breaks, etc.

Blocks may contain other blocks (container blocks) or inlines (leaf blocks). Inlines may contain other inlines (container inlines) but never blocks.

```
MarkdownDocument (ContainerBlock)
├── HeadingBlock (LeafBlock)
│   └── LiteralInline
├── ParagraphBlock (LeafBlock)
│   ├── LiteralInline
│   ├── EmphasisInline (ContainerInline)
│   │   └── LiteralInline
│   └── LiteralInline
├── ListBlock (ContainerBlock)
│   ├── ListItemBlock (ContainerBlock)
│   │   └── ParagraphBlock
│   └── ListItemBlock (ContainerBlock)
│       └── ParagraphBlock
└── FencedCodeBlock (LeafBlock)
```

## Node hierarchy

All AST nodes inherit from `MarkdownObject`, which provides:

{.table}
| Member | Type | Description |
|---|---|---|
| `Span` | `SourceSpan` | Start and end positions (inclusive) in the source text |
| `Line` | `int` | Zero-based line number in the source |
| `Column` | `int` | Zero-based column number |

### Block types

{.table}
| Base class | Description | Examples |
|---|---|---|
| `ContainerBlock` | Contains child blocks | `MarkdownDocument`, `ListBlock`, `ListItemBlock`, `QuoteBlock` |
| `LeafBlock` | Contains inlines, no child blocks | `ParagraphBlock`, `HeadingBlock`, `CodeBlock`, `FencedCodeBlock` |

A `LeafBlock` has an `Inline` property (`ContainerInline?`) that is the root of its inline content.

### Inline types

{.table}
| Base class | Description | Examples |
|---|---|---|
| `ContainerInline` | Contains child inlines | `EmphasisInline`, `LinkInline` |
| `LeafInline` | No children | `LiteralInline`, `CodeInline`, `LineBreakInline` |

Inlines are stored as a **doubly-linked list** — each inline has `PreviousSibling` and `NextSibling` properties, plus a `Parent` (`ContainerInline?`).

## Traversing the AST

### The Descendants API

The `Descendants` extension methods provide the easiest way to traverse the tree. They yield nodes in **depth-first** order.

#### All descendants

```csharp
var document = Markdown.Parse(markdownText);

foreach (var node in document.Descendants())
{
    Console.WriteLine($"{node.GetType().Name} at {node.Line}:{node.Column}");
}
```

#### Filter by type

```csharp
// All headings
foreach (var heading in document.Descendants<HeadingBlock>())
{
    Console.WriteLine($"H{heading.Level}: line {heading.Line}");
}

// All links (not images)
foreach (var link in document.Descendants<LinkInline>().Where(l => !l.IsImage))
{
    Console.WriteLine($"Link: {link.Url}");
}

// All images
foreach (var image in document.Descendants<LinkInline>().Where(l => l.IsImage))
{
    Console.WriteLine($"Image: {image.Url}");
}
```

#### Querying from any node

`Descendants` works from any `MarkdownObject`, not just the root:

```csharp
// Find all emphasis inside list items
var items = document.Descendants<ListItemBlock>()
    .SelectMany(item => item.Descendants<EmphasisInline>());

// Find emphasis whose direct parent block is a list item
var other = document.Descendants<EmphasisInline>()
    .Where(em => em.ParentBlock is ListItemBlock);
```

### Manual traversal

For containers you can iterate children directly:

```csharp
// Block children of a ContainerBlock
foreach (var child in document)
{
    // child is a Block
}

// Inline children of a ContainerInline
var paragraph = document.Descendants<ParagraphBlock>().First();
var inline = paragraph.Inline; // ContainerInline?
if (inline != null)
{
    var child = inline.FirstChild;
    while (child != null)
    {
        Console.WriteLine(child.GetType().Name);
        child = child.NextSibling;
    }
}
```

## Common block types

{.table}
| Type | Description |
|---|---|
| `MarkdownDocument` | Root node, a `ContainerBlock` |
| `ParagraphBlock` | A paragraph (`<p>`) |
| `HeadingBlock` | A heading (`<h1>`–`<h6>`); has `Level` property |
| `ListBlock` | An ordered or unordered list |
| `ListItemBlock` | A single list item |
| `QuoteBlock` | A blockquote |
| `FencedCodeBlock` | A fenced code block; has `Info` (language) and `Lines` properties |
| `CodeBlock` | An indented code block |
| `ThematicBreakBlock` | A horizontal rule (`<hr>`) |
| `HtmlBlock` | A raw HTML block |

## Common inline types

{.table}
| Type | Description |
|---|---|
| `LiteralInline` | Plain text content |
| `EmphasisInline` | Emphasis (`<em>` or `<strong>`); has `DelimiterChar` and `DelimiterCount` |
| `CodeInline` | Inline code span |
| `LinkInline` | A link or image; has `Url`, `Title`, `IsImage` |
| `AutolinkInline` | An autolink (`<url>`) |
| `LineBreakInline` | A line break (hard or soft) |
| `HtmlInline` | Inline raw HTML |
| `HtmlEntityInline` | An HTML entity |

## Attached data

Every `MarkdownObject` supports attaching arbitrary key-value data:

```csharp
// Store data
node.SetData("my-key", someValue);

// Retrieve data
var value = node.GetData("my-key");

// Check existence
if (node.ContainsData("my-key")) { ... }

// Remove
node.RemoveData("my-key");
```

### Typed metadata helpers

For extension state, prefer collision-resistant typed keys via `DataKey<T>` and the typed helper methods:

```csharp
using Markdig.Syntax;

public sealed class MyExtensionState
{
    public int Value { get; set; }
}

static readonly DataKey<MyExtensionState> StateKey = new();

// Store
node.SetData<MyExtensionState>(StateKey, new MyExtensionState { Value = 123 });

// Retrieve
if (node.TryGetData<MyExtensionState>(StateKey, out var state))
{
    Console.WriteLine(state.Value);
}
```

> [!TIP]
> Use the explicit generic calls (`SetData<T>`, `TryGetData<T>`, `GetData<T>`) to avoid ambiguity with the untyped `IMarkdownObject` methods.

### HTML attributes

The most common attached data is `HtmlAttributes`, used by the [Generic attributes](../extensions/generic-attributes.md) extension:

```csharp
using Markdig.Renderers.Html;

var attrs = node.GetAttributes(); // Creates if not present
attrs.AddClass("my-class");
attrs.Id = "my-id";
attrs.AddProperty("data-value", "42");
```

## Mutating the AST safely

Markdig provides a few small helpers for common AST "surgery" operations:

```csharp
// Remove a block from its parent container
block.Remove();

// Replace a block in its parent container (optionally transferring children)
block.ReplaceBy(replacementBlock, moveChildren: true);

// Transfer children efficiently (preserves order)
sourceContainerBlock.TransferChildrenTo(destinationContainerBlock);
sourceContainerInline.TransferChildrenTo(destinationContainerInline);
```

These helpers preserve parent/child ownership invariants and avoid common O(n²) patterns (for example repeatedly calling `RemoveAt(0)` on a `ContainerBlock`).

> [!IMPORTANT]
> These helpers do not automatically recompute spans or trivia. If your transform needs exact source fidelity after mutation, update `Span`, `Line`, `Column`, and trivia properties explicitly.

## The SourceSpan struct

Every node has a `Span` property of type `SourceSpan` with `Start` and `End` positions (inclusive) in the original source. When `.UsePreciseSourceLocation()` is enabled, these values are accurate for all nodes.

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UsePreciseSourceLocation()
    .Build();

var document = Markdown.Parse(markdownText, pipeline);

foreach (var heading in document.Descendants<HeadingBlock>())
{
    var span = heading.Span;
    var sourceText = markdownText[span.Start..(span.End + 1)];
    Console.WriteLine($"Heading source: '{sourceText}'");
}
```

## Block properties

### IsOpen

While a block is being parsed, `IsOpen` is `true`. Once the parser finishes building the block, `IsOpen` is set to `false`. In a fully parsed document, all blocks have `IsOpen == false`.

### IsBreakable

Indicates whether the block can be interrupted by new blocks. `FencedCodeBlock` is the only built-in non-breakable block — its parent container cannot be closed while the code block is still open, because content inside the fence is treated as literal code.

### Parent

Every block has a `Parent` (`ContainerBlock?`) property for upward traversal. The root `MarkdownDocument` has `Parent == null`.

### Parser

Every block stores a reference to the `BlockParser` that created it. This is useful when post-processing needs to identify which parser produced a given node.
