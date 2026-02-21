# Parser + AST API Migration Notes (Potential Breaking Changes)

This document tracks compatibility risks for consumers adopting the new parser/AST authoring APIs and for future releases that may tighten contracts.

## Current Release Impact

The implemented changes are additive/public-surface expansions:

- `InlineProcessor.ReplaceParentContainer(...)` is now public.
- `BlockProcessor.TakeLinesBefore()` is public.
- `BlockProcessor.IsOpen(Block)` is public.
- `BlockProcessor.TryDiscard(Block)` is new.
- `InlineProcessor.GetParserState(...)` and `InlineProcessor.Emit(...)` are new.
- `Block.Remove()`, `Block.ReplaceBy(...)`, `ContainerBlock.TransferChildrenTo(...)`, and `ContainerInline.TransferChildrenTo(...)` are new.
- `MarkdownPipelineBuilder.TrackTrivia` setter is public.
- Typed metadata helpers (`DataKey<T>`, `MarkdownObjectDataExtensions`) are new.

No runtime behavior change is required for existing consumers unless they opt into these APIs.

## Potential Future Breaking Changes

These are not implemented in this release, but consumers should avoid relying on ambiguous behavior.

### 1. `ReplaceParentContainer` replacement limits

Current contract allows a single replacement request per leaf pass. If this evolves to support multiple requests, error behavior and ordering guarantees may change.

Guidance:

- Keep replacements localized and deterministic.
- Avoid depending on repeated replacement attempts in one pass.

### 2. Mutation-helper metadata behavior

Mutation helpers currently do not recalculate spans/trivia automatically. A future strict mode might validate or enforce metadata consistency.

Guidance:

- Explicitly set/update `Span`, `Line`, `Column`, and trivia fields in transforms where source fidelity matters.

### 3. Internal helper cleanup

Internal compatibility shims may be removed in a later major release after migration (for example, internal aliases kept during transition).

Guidance:

- Use public methods (`TakeLinesBefore`, `TryDiscard`, etc.) directly.

### 4. Typed metadata API naming and overload resolution

Some helper overloads share names with existing instance methods. Advanced consumers should prefer explicit generic invocations to avoid ambiguity.

Guidance:

- Prefer explicit generic calls:
  - `node.SetData<MyType>(value)`
  - `node.SetData<MyType>(key, value)`
  - `node.GetData<MyType>(key)`
  - `node.TryGetData<MyType>(key, out var value)`

## Migration Checklist for Extension Authors

1. Replace custom open-stack discard patterns with `TryDiscard` where appropriate.
2. Use `TakeLinesBefore()` instead of internal trivia helpers.
3. For inline-driven parent replacement:
   - replace the parent container in the AST first,
   - then call `ReplaceParentContainer(old, @new)`.
4. Replace manual child-move loops (`RemoveAt(0)` patterns) with transfer helpers.
5. Add regression tests for transformed AST shape (parent/child ownership and ordering).

## Versioning Guidance

For a future major version, consider:

- deprecating internal transition shims,
- optionally adding strict validation mode for parser invariants and mutation metadata,
- documenting any tightened contracts as migration steps in this file.
