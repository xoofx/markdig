---
title: Developer guide
---

# Developer guide

Markdig is designed to be deeply extensible. This guide covers how the parsing pipeline works, how to traverse and manipulate the AST, and how to create your own extensions with custom block parsers, inline parsers, and renderers.

## Architecture overview

Markdig's processing flow has three stages:

```
Markdown text → [Block Parsing] → [Inline Parsing] → AST (MarkdownDocument)
                                                        ↓
                                              [Rendering] → Output (HTML, etc.)
```

1. **Block parsing** — The `BlockProcessor` walks through the source text line by line, using registered `BlockParser` objects to identify block-level elements (paragraphs, headings, lists, code blocks, etc.) and build the AST skeleton.

2. **Inline parsing** — The `InlineProcessor` visits every `LeafBlock` in the AST and runs registered `InlineParser` objects over the block's text to identify inline elements (emphasis, links, code spans, etc.).

3. **Rendering** — A renderer (typically `HtmlRenderer`) walks the complete AST and dispatches each node to a matching `ObjectRenderer` for output.

Extensions can modify any of these stages: adding new parsers, modifying existing ones, or registering custom renderers.

## Guides

{.table}
| Guide | What you'll learn |
|---|---|
| [Abstract syntax tree](ast.md) | Structure of block/inline nodes, traversal with `Descendants`, source spans |
| [Pipeline architecture](pipeline.md) | How `MarkdownPipeline`, `MarkdownPipelineBuilder`, and extensions interact |
| [Creating extensions](creating-extensions.md) | Implement `IMarkdownExtension` — from simple to complex |
| [Block parsers](block-parsers.md) | Write custom `BlockParser` subclasses — `TryOpen`, `TryContinue`, `BlockState` |
| [Inline parsers](inline-parsers.md) | Write custom `InlineParser` subclasses — `Match`, `StringSlice`, post-processing |
| [Renderers](renderers.md) | Implement `HtmlObjectRenderer<T>` or build a completely custom renderer |
| [Performance](performance.md) | Tips for maintaining high throughput — allocation-free patterns, pooling, Span-based parsing |

## Quick reference: key types

{.table}
| Type | Role |
|---|---|
| `Markdown` | Static entry point — `Parse`, `ToHtml`, `Convert` |
| `MarkdownPipeline` | Immutable, thread-safe configuration object |
| `MarkdownPipelineBuilder` | Fluent builder for `MarkdownPipeline` |
| `IMarkdownExtension` | Interface all extensions implement |
| `BlockParser` | Abstract base for block-level parsers |
| `InlineParser` | Abstract base for inline-level parsers |
| `MarkdownDocument` | Root AST node (a `ContainerBlock`) |
| `Block` | Base for all block AST nodes |
| `Inline` | Base for all inline AST nodes |
| `MarkdownObject` | Base for all AST nodes — provides `Span`, `Line`, `Column`, data storage |
| `HtmlRenderer` | Built-in HTML output renderer |
| `HtmlObjectRenderer<T>` | Base for per-type HTML rendering |
| `IMarkdownRenderer` | Interface for custom renderers |
