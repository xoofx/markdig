---
title: Parser authoring API
---

# Parser + AST authoring API

This page documents parser authoring contracts and the public APIs intended to give third-party extensions parity with built-in extensions (and to make safe AST manipulation less error-prone).

If you're new to writing extensions, start with [Creating extensions](creating-extensions.md) and then come back here for the "engine contract" details.

## Scope

The authoring model remains two-phase:

1. Block parsing (`BlockProcessor` + `BlockParser`).
2. Inline parsing (`InlineProcessor` + `InlineParser` + `IPostInlineProcessor`).

Performance characteristics are unchanged: parser dispatch still relies on opening-character maps and pooled processors.

## Public API Additions

### `InlineProcessor.ReplaceParentContainer(...)`

Use this when an inline parser replaces a parent container block during inline processing.

Contract:

- Call only while processing the current leaf (`ProcessInlineLeaf` pass).
- Replace the container in the AST first, then call `ReplaceParentContainer(old, @new)` to synchronize traversal state.
- Only one replacement request is allowed per leaf processing pass.

### `BlockProcessor.TakeLinesBefore()`

Use this in `TryOpen` when `TrackTrivia` is enabled and the new block should own pending leading blank/trivia lines.

Contract:

- Returns current `LinesBefore` and clears it.
- Returns `null` when there is no pending list.

### `BlockProcessor.IsOpen(Block)` and `BlockProcessor.TryDiscard(Block)`

Use these to safely reason about/modify the open block stack.

Contract:

- `IsOpen` is a read-only stack membership check.
- `TryDiscard` removes an open non-root block from both parent container and open stack, returning `true` only when a discard happened.

### `MarkdownPipelineBuilder.TrackTrivia`

`TrackTrivia` is now publicly settable and flows into the built pipeline/processors.

## Block Parser Contract

### `BlockState` semantics

- `None`: no match for this parser/block.
- `Skip`: skip parser for this line and continue with others.
- `Continue`: block stays open; leaf blocks append line content.
- `ContinueDiscard`: block stays open; line consumed but not appended.
- `Break`: block closes; current line remains available for further parsing.
- `BreakDiscard`: block closes; current line is consumed.

### `NewBlocks` invariants

- Every pushed block must have `Parser` set to the creating parser.
- Leaf blocks must be pushed last.
- Push order is outer container to inner container to leaf (LIFO pop in processor).

### Trivia rules

When `TrackTrivia` is enabled:

- Use `TakeLinesBefore()` in `TryOpen` to assign pending leading blank/trivia lines.
- Use `UseTrivia(end)` when a parser needs exact trivia slices around syntax markers.

## Inline Parser Contract

### `Match` behavior

- On success: advance `slice` and set `processor.Inline` when emitting a node.
- On failure: return `false` without mutating parser output state.

### Emission behavior

- If a matched parser sets a parentless `processor.Inline`, the processor appends it to the deepest open inline container.
- For explicit emission, use `processor.Emit(inline)`.

### Parser state

Use `processor.GetParserState<TState>(this)` or `processor.GetParserState(this, factory)` for per-leaf parser state.
Parser states are cleared at the beginning of each `ProcessInlineLeaf`.

### Block transforms from inline parsing

- `processor.BlockNew` replaces the current leaf block after the current leaf pass returns.
- `processor.ReplaceParentContainer(old, @new)` keeps traversal coherent when a parent container has already been replaced in the AST.

## Hook Selection

| Goal | Preferred hook |
|---|---|
| Add block syntax | `BlockParser` in `MarkdownPipelineBuilder.BlockParsers` |
| Add inline syntax | `InlineParser` in `MarkdownPipelineBuilder.InlineParsers` |
| Deferred inline resolution | `IPostInlineProcessor` |
| Replace current leaf block | `InlineProcessor.BlockNew` |
| Replace parent container from inline | `InlineProcessor.ReplaceParentContainer` |
| Literal post-processing | `LiteralInlineParser.PostMatch` |
| Post-close block transform | `BlockParser.Closed` |
| Per-block inline begin/end behavior | `Block.ProcessInlinesBegin` / `Block.ProcessInlinesEnd` |
| Post-document transform | `MarkdownPipelineBuilder.DocumentProcessed` |

## AST Mutation Helpers

### Block-level

- `Block.Remove()`: remove from parent container.
- `Block.ReplaceBy(replacement, moveChildren: true)`: replace in parent container and optionally move children.
- `ContainerBlock.TransferChildrenTo(destination)`: move child blocks in order.

### Inline-level

- `ContainerInline.TransferChildrenTo(destination)`: move child inlines in order.

Mutation helpers do not auto-recompute spans/trivia; callsites remain responsible for source metadata correctness when required.

## Typed Metadata Helpers

For extension state attached to AST nodes:

- `SetData<T>(value)` / `GetData<T>()`.
- `TryGetData<T>(key, out value)` / `GetData<T>(key)` for explicit object keys.
- `DataKey<T>` for collision-resistant typed keys.

Example:

```csharp
var key = new DataKey<MyState>();
block.SetData<MyState>(key, state);
if (block.TryGetData<MyState>(key, out var existing))
{
    // use existing
}
```
