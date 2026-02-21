---
title: Pipeline architecture
---

# Pipeline architecture

This guide explains how the `MarkdownPipeline` works, how extensions configure it, and how the parsing flow proceeds from source text to AST to rendered output.

## Overview

Markdig's processing involves three objects:

1. **`MarkdownPipelineBuilder`** — A mutable builder where you configure extensions, parsers, and options.
2. **`MarkdownPipeline`** — An immutable, thread-safe object produced by the builder. Contains the final parser and extension lists.
3. **`Markdown`** — The static class that uses a pipeline to parse and render.

```csharp
// 1. Configure
var builder = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions();

// 2. Build (immutable from here on)
var pipeline = builder.Build();

// 3. Use (thread-safe, reusable)
var html = Markdown.ToHtml(markdownText, pipeline);
```

## What the pipeline holds

The `MarkdownPipeline` contains:

{.table}
| Component | Type | Description |
|---|---|---|
| Block parsers | `BlockParserList` | Ordered list of `BlockParser` objects |
| Inline parsers | `InlineParserList` | Ordered list of `InlineParser` objects |
| Extensions | `OrderedList<IMarkdownExtension>` | Registered extensions |
| TrackTrivia | `bool` | Whether trivia parsing is enabled |
| DocumentProcessed | `ProcessDocumentDelegate?` | Callback after parsing completes |

## How Build() works

When you call `builder.Build()`:

1. Each registered extension's `Setup(MarkdownPipelineBuilder)` method is called **in order**.
2. Extensions may add/remove/modify block parsers, inline parsers, or other settings.
3. The builder's mutable state is frozen into an immutable `MarkdownPipeline`.
4. The builder caches the result — calling `Build()` again returns the same instance.

This is why extension ordering matters: an extension may look for parsers added by a previous extension.

## How Parse() works

`Markdown.Parse(string, pipeline)` executes these steps:

### Step 1: Block parsing

A `BlockProcessor` is created with the pipeline's block parsers. It processes the source text **line by line**:

1. For each line, the processor checks all open blocks to see if they continue (`TryContinue`).
2. Blocks that don't continue are closed.
3. The processor tries each `BlockParser` to see if a new block opens at the current position (`TryOpen`).
4. Characters are dispatched based on `OpeningCharacters` — parsers are only tried when their opening character matches.

The result is a tree of `Block` nodes rooted at the `MarkdownDocument`.

### Step 2: Trivia expansion (optional)

If `TrackTrivia` is enabled, blocks are expanded to absorb neighboring whitespace and trivia. This supports lossless roundtripping.

### Step 3: Inline parsing

An `InlineProcessor` visits each `LeafBlock` and runs the pipeline's inline parsers over the block's text content:

1. Starting from the first character, the processor finds `InlineParser` objects whose `OpeningCharacters` match.
2. The `Match` method is called on each candidate parser (in order) until one returns `true`.
3. The matched inline is added to the block's `Inline` container.
4. If no parser matches, a `LiteralInlineParser` consumes the character.
5. After all characters are consumed, post-processing runs (e.g., emphasis restructuring).

### Step 4: Post-processing

The `DocumentProcessed` delegate (if set) is invoked on the completed document.

### Step 5: Return

The `MarkdownDocument` is returned.

## How rendering works

Rendering is a separate phase. When you call `document.ToHtml(pipeline)`:

1. An `HtmlRenderer` is created (or borrowed from a pool).
2. `pipeline.Setup(renderer)` is called — this invokes `Setup(MarkdownPipeline, IMarkdownRenderer)` on every registered extension, giving each one the chance to register its `ObjectRenderer`.
3. The renderer walks the AST, dispatching each node to the appropriate `ObjectRenderer` by type.
4. The rendered output is returned.

> [!IMPORTANT]
> This is why the **same pipeline** must be used for both parsing and rendering. The parse-phase `Setup` registers parsers that produce custom AST node types. The render-phase `Setup` registers renderers that know how to output those types. Mismatched pipelines = missing renderers.

## Extension setup: two phases

Every `IMarkdownExtension` has two `Setup` methods:

```csharp
public interface IMarkdownExtension
{
    // Phase 1: Called during Build() — register/modify parsers
    void Setup(MarkdownPipelineBuilder pipeline);

    // Phase 2: Called during rendering — register object renderers
    void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer);
}
```

### Phase 1 example

```csharp
public void Setup(MarkdownPipelineBuilder pipeline)
{
    // Add a new block parser
    pipeline.BlockParsers.AddIfNotAlready<MyBlockParser>();

    // Or modify an existing parser
    var emphasisParser = pipeline.InlineParsers.FindExact<EmphasisInlineParser>();
    if (emphasisParser != null)
    {
        emphasisParser.EmphasisDescriptors.Add(
            new EmphasisDescriptor('%', 3, 3, false));
    }
}
```

### Phase 2 example

```csharp
public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
{
    if (renderer is HtmlRenderer htmlRenderer)
    {
        htmlRenderer.ObjectRenderers.AddIfNotAlready<HtmlMyBlockRenderer>();
    }
}
```

## The OrderedList&lt;T&gt; collection

Both parser and extension lists are `OrderedList<T>`, a custom Markdig collection with methods for type-safe insertion:

{.table}
| Method | Description |
|---|---|
| `AddIfNotAlready<T>()` | Add if no instance of `T` exists |
| `InsertBefore<TBefore>(item)` | Insert before a specific type |
| `InsertAfter<TAfter>(item)` | Insert after a specific type |
| `Find<T>()` | Find the first instance of type `T` |
| `FindExact<T>()` | Find an exact type match (not subclasses) |
| `TryFind<T>(out T?)` | Try to find, returning success |
| `Replace<T>(newItem)` | Replace an existing item of type `T` |
| `ReplaceOrAdd<T>(newItem)` | Replace or add if not found |
| `TryRemove<T>()` | Remove the first instance of type `T` |
| `Contains<T>()` | Check if an instance of type `T` exists |

## Default parsers

Without any extensions, the pipeline contains these parsers:

### Default block parsers

1. `ThematicBreakParser`
2. `HeadingBlockParser`
3. `QuoteBlockParser`
4. `ListBlockParser`
5. `HtmlBlockParser`
6. `FencedCodeBlockParser`
7. `IndentedCodeBlockParser`
8. `ParagraphBlockParser`

### Default inline parsers

1. `HtmlEntityParser`
2. `LinkInlineParser`
3. `EscapeInlineParser`
4. `EmphasisInlineParser`
5. `CodeInlineParser`
6. `AutolinkInlineParser`
7. `LineBreakInlineParser`

Extensions add to or modify these lists.

## Trivia and roundtripping

Enable trivia tracking for lossless parse→render roundtrips:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .EnableTrackTrivia()
    .Build();

var document = Markdown.Parse(markdownText, pipeline);
var normalized = Markdown.Normalize(markdownText, pipeline: pipeline);
```

When trivia is tracked, whitespace, extra heading characters, and other non-semantic elements are stored on the AST nodes, allowing the document to be re-rendered as close to the original as possible.
