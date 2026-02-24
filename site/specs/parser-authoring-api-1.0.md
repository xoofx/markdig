---
title: Parser + AST authoring API specification (Draft)
---

# Parser + Markdown model (AST) authoring API specification

Status: Draft

This document specifies a set of improvements to Markdig’s public API for:

- authoring block/inline parsers and extensions with feature parity to built-in extensions, and
- safely and efficiently manipulating Markdig’s Markdown model (AST) during parsing and post-processing.

The intent is to preserve Markdig’s current performance characteristics while making extension authoring more predictable, less error-prone, and more composable.

---

## 1. Problem statement

Markdig is highly extensible, but parser/extension authoring remains harder than necessary:

- **Parity gaps:** Some capabilities used by built-in extensions are `internal` and not available to third parties (notably container replacement during inline parsing and trivia capture for custom block parsers).
- **Implicit contracts:** Correctness depends on subtle engine invariants (e.g., `BlockState` semantics, `NewBlocks` invariants, inline emission rules) that are not formally documented as authoring contracts.
- **AST surgery is under-supported:** Extensions often need to replace blocks, transfer children, or insert siblings safely. The inline model provides useful mutation primitives (`Inline.Remove`, `Inline.ReplaceBy`), but the block model does not. Current patterns are verbose and can be accidentally inefficient (e.g., `RemoveAt(0)` loops on `ContainerBlock`).

---

## 2. Goals

1. **Third‑party parity:** Anything implemented by built-in extensions should be implementable by third parties using public, supported APIs.
2. **Explicit contracts:** Document stable, implementable contracts for block parsing, inline parsing, and post-processing hook selection.
3. **No performance regressions:**
   - Preserve opening-character dispatch (`OpeningCharacters` → `CharacterMap`).
   - Avoid per-character allocations in hot loops.
   - Keep object pooling (`BlockProcessor`/`InlineProcessor`) intact.
4. **Safer AST manipulation:** Provide small, focused helpers that preserve ownership invariants and avoid accidental O(n²) patterns.
5. **Minimal surface-area risk:** Prefer additive APIs and access-level changes; avoid breaking changes where possible.

---

## 3. Non-goals

- Rewriting the parsing engine.
- Changing CommonMark behavior/semantics.
- Removing existing low-level APIs (they may remain as advanced entry points).
- Introducing reflection-heavy or allocation-heavy extension mechanisms.

---

## 4. Background (current architecture, summarized)

### 4.1 Two phases

1. **Block phase** (`BlockProcessor`): line-by-line parsing using `BlockParser` instances. The engine:
   - continues currently open blocks (`TryContinue`),
   - opens new blocks (`TryOpen`) using opening-character dispatch,
   - closes blocks that are no longer open (`Close`/`Closed`).

2. **Inline phase** (`InlineProcessor`): per-leaf-block parsing using `InlineParser` instances. The engine:
   - dispatches parsers by opening character and falls back to `LiteralInlineParser`,
   - runs `IPostInlineProcessor` instances after character scanning,
   - performs leaf replacement (`BlockNew`) and container replacement stack synchronization.

### 4.2 Dispatch model

Both phases use `ParserList<TParser, TProcessor>`:

- Parsers that define `OpeningCharacters` are indexed into a character map.
- Parsers with no `OpeningCharacters` are treated as **global parsers** and are tried on every line/character after character-specific candidates.
- Parser ordering is composable via `OrderedList<T>` (`InsertBefore`, `InsertAfter`, `Replace`, `TryRemove`, …).

### 4.3 Hook points (conceptual map)

- **During block parsing:** `BlockParser.TryOpen`, `BlockParser.TryContinue`, `BlockParser.Close`, `BlockParser.Closed`.
- **During inline parsing:** `InlineParser.Match`, `LiteralInlineParser.PostMatch`, `IPostInlineProcessor.PostProcess`.
- **Cross-phase / per-block:** `Block.ProcessInlinesBegin` and `Block.ProcessInlinesEnd` (one-shot callbacks).
- **Pipeline-level:** `MarkdownPipelineBuilder.DocumentProcessed`.

---

## 5. Required public API changes (parity + configurability)

This section defines the minimal changes required for third‑party parity.

### 5.1 `InlineProcessor.ReplaceParentContainer` becomes public

**Current state:** container replacement during inline parsing is implemented by `InlineProcessor.ReplaceParentContainer(...)` and honored by the inline tree walk; the method is not publicly accessible.

**Change:**

```csharp
public void ReplaceParentContainer(ContainerBlock previousParentContainer, ContainerBlock newParentContainer)
```

**Behavioral contract (must be documented in XML docs):**

- **When callable:** only during `InlineProcessor.ProcessInlineLeaf(...)` (i.e., while a leaf is being processed).
- **Ownership:** this method does **not** perform the AST replacement. The caller must have already:
  1) replaced the container in its parent container (`parent[index] = newContainer`), and
  2) moved/transferred children as desired.
- **Purpose:** the method requests **traversal-stack synchronization** so that `MarkdownParser.ProcessInlines` continues the walk against the replacement container.
- **Limitations:** only **one** parent-container replacement may be requested per leaf processing pass; subsequent calls must throw `InvalidOperationException`.
- **Timing:** the requested replacement is applied after the leaf returns from `ProcessInlineLeaf` during the `MarkdownParser.ProcessInlines` walk.

**Implementation notes:**

- This can remain a lightweight state-setter on `InlineProcessor` (it already stores a single replacement request).
- The tree-walk logic must keep its current behavior; the public API simply formalizes and documents it.

### 5.2 `BlockProcessor.TakeLinesBefore` becomes public (trivia parity)

**Current state:** Markdig tracks blank lines and trivia into `BlockProcessor.LinesBefore` when `TrackTrivia` is enabled. Built-in parsers attach those lines to blocks via an `internal` helper.

**Change:**

Add a public method that transfers ownership of `LinesBefore`:

```csharp
public List<StringSlice>? TakeLinesBefore()
```

**Contract:**

- Returns the accumulated `LinesBefore` list and clears `LinesBefore` to `null`.
- Returns `null` if there is no pending trivia/blank-line list.
- Intended usage: call from `BlockParser.TryOpen` after deciding a block match, to attach the lines to the newly created block’s `LinesBefore`.
- Meaningful only when trivia tracking is enabled (`MarkdownPipeline.TrackTrivia == true`).

**Implementation notes:**

- If an existing internal helper exists, it may be retained as an internal wrapper around the public method to avoid internal churn.
- Update built-in block parsers and end-of-document trivia logic to use the public method name consistently.

### 5.3 `BlockProcessor.IsOpen(Block)` becomes public

**Change:**

```csharp
public bool IsOpen(Block block)
```

**Contract:**

- Returns `true` if the specified block is present on the open-block stack for the current `BlockProcessor`.
- Read-only query; no side effects.

**Rationale:** advanced parsers and transforms often need to reason about whether a predecessor block is still open.

### 5.4 `MarkdownPipelineBuilder.TrackTrivia` becomes publicly settable

**Change:**

```csharp
public bool TrackTrivia { get; set; }
```

**Contract:**

- Enables/disables trivia tracking during parsing (whitespace/trivia slices, `LinesBefore`/`LinesAfter`, etc.).
- Must flow into the built `MarkdownPipeline` and into processors (`BlockProcessor`/`InlineProcessor`) exactly as today.

**Rationale:** trivia tracking should be an application/pipeline choice, not restricted to Markdig internals.

---

## 6. Authoring contracts (documentation requirements)

The public API changes above are necessary but not sufficient: correct authoring depends on stable contracts.

This section defines what must be documented (either as XML docs on types/members and/or as Markdown documentation under `doc/`).

### 6.1 Block parser contract

**Required documentation:**

1. **`BlockState` semantics** (explicit, per enum value):
   - whether the current line is consumed or re-processed,
   - whether a leaf block appends the line,
   - how discard variants interact with line storage,
   - how `Skip` behaves (and where it is appropriate).

2. **`NewBlocks` invariants** (must be formalized as a contract):
   - every pushed block must have its `Parser` set to the creating parser (non-null for extension-authored blocks),
   - leaf blocks must be pushed last (the engine assumes the stack is empty after handling leaf blocks),
   - blocks are popped in LIFO order; outer container first, then inner, then leaf.

3. **Indentation APIs and state:**
   - `Column`, `ColumnBeforeIndent`, `Indent`, `IsCodeIndent`, and how `ParseIndent`, `GoToColumn`, `GoToCodeIndent`, `RestartIndent` should be used.

4. **Trivia authoring rules (when `TrackTrivia` is enabled):**
   - how and when to attach `LinesBefore` using `TakeLinesBefore`,
   - how `UseTrivia(end)` is used to capture trivia slices,
   - which trivia fields on blocks are expected to be set by parsers (`TriviaBefore`, `TriviaAfter`, `LinesBefore`, `LinesAfter`).

### 6.2 Inline parser contract

**Required documentation:**

1. **`InlineParser.Match` contract:**
   - on success: advance `slice` and optionally set `processor.Inline`,
   - on failure: return `false` and do not mutate `processor.Inline` or `slice`.

2. **Inline emission rules after a successful match:**
   - if `processor.Inline` is non-null and parentless, the engine appends it to the deepest open `ContainerInline`,
   - if `processor.Inline` is null, the engine sets it to the “current” inline position (last child of deepest container).

3. **Per-leaf parser state:**
   - `InlineProcessor.ParserStates` is cleared for each `ProcessInlineLeaf` call,
   - a parser’s state slot is indexed by `InlineParser.Index`.

4. **Post-processing model:**
   - `IPostInlineProcessor.PostProcess` runs after the character scan,
   - post-processors may re-run inline post-processing on subtrees (`InlineProcessor.PostProcessInlines`), as pipe tables do for per-cell reprocessing.

5. **Block-tree transforms triggered during inline parsing:**
   - `InlineProcessor.BlockNew` replaces the current leaf block after `ProcessInlineLeaf` returns,
   - `InlineProcessor.ReplaceParentContainer` requests traversal-stack synchronization for a container that the parser has already replaced in the AST.

**Critical contract notes (must be stated explicitly):**

- `BlockNew` is applied by the `ProcessInlines` walk after `ProcessInlineLeaf` returns; the replaced block will not have its inlines processed again in the same walk. Use `BlockNew` only when the replacement is already in its final form.
- Avoid mutating the current container’s child list in a way that invalidates traversal indices. Prefer deferring insertion/removal via `Block.ProcessInlinesEnd` on the container being traversed (the pipe table pattern).

### 6.3 Hook selection guide

Provide a single authoritative table mapping intent → hook:

| You want to… | Preferred mechanism |
|---|---|
| Add a new block syntax | `BlockParser` in `MarkdownPipelineBuilder.BlockParsers` |
| Add a new fenced block | `FencedBlockParserBase<TBlock>` |
| Add a new inline syntax | `InlineParser` in `MarkdownPipelineBuilder.InlineParsers` |
| Add delimiter syntax (emphasis-like) | Extend `EmphasisInlineParser` descriptors/delegates |
| Defer inline resolution | `InlineParser` + `IPostInlineProcessor` |
| Replace a leaf block based on inline content | `InlineProcessor.BlockNew` (typically from `IPostInlineProcessor`) |
| Replace a parent container during inline parsing | `InlineProcessor.ReplaceParentContainer(old, @new)` |
| Scan/transform literal runs | `LiteralInlineParser.PostMatch` (registered during `ProcessInlinesBegin`) |
| Transform the tree after a block is closed | `BlockParser.Closed` |
| Transform per-block after inline processing | `Block.ProcessInlinesBegin` / `Block.ProcessInlinesEnd` |
| Decorate the document after full parse | `MarkdownPipelineBuilder.DocumentProcessed` |

---

## 7. Helper APIs (recommended, additive)

These helpers reduce common error-prone patterns while preserving performance.

### 7.1 `InlineProcessor.GetParserState<TState>(...)`

Add a helper that centralizes the “index into `ParserStates`” pattern:

```csharp
public TState GetParserState<TState>(InlineParser parser, Func<TState> factory) where TState : class
```

Recommended additional overload to avoid delegate creation:

```csharp
public TState GetParserState<TState>(InlineParser parser) where TState : class, new()
```

**Contract:**

- Returns the cached state object for the current leaf, or creates/stores a new one.
- The stored state is scoped to the current `ProcessInlineLeaf` call only.

### 7.2 `InlineProcessor.Emit(Inline inline)`

Add an explicit emission helper:

```csharp
public void Emit(Inline inline)
```

**Contract:**

- Appends `inline` to the deepest open `ContainerInline`.
- Sets `InlineProcessor.Inline` to the emitted node.

**Rationale:** makes intent explicit and avoids relying on the engine’s post-match append behavior.

### 7.3 Debug-only validation of `NewBlocks`

Add debug assertions in the block creation path to fail fast when a parser violates invariants:

- block pushed to `NewBlocks` without `Parser` set,
- leaf block pushed but followed by additional blocks.

These assertions must be `Debug.Assert(...)` only.

### 7.4 `BlockProcessor.TryDiscard(Block)`

Add a helper to make “discard if open” explicit:

```csharp
public bool TryDiscard(Block block)
```

**Contract:**

- If `block` is on the open stack, discard it (remove from parent + open stack) and return `true`.
- Otherwise do nothing and return `false`.

This pairs naturally with a public `IsOpen(Block)` and reduces duplicated patterns.

---

## 8. Markdown model (AST) API improvements (recommended, additive)

### 8.1 Block mutation helpers

Add a small set of block mutation helpers to mirror the usability of inline mutations:

1. `Block.Remove()`  
   Detach the block from its parent container (no-op if `Parent` is null).

2. `Block.ReplaceBy(Block replacement, bool moveChildren = true)`  
   Replace this block in its parent container with `replacement`.

   - `replacement.Parent` must be null (otherwise throw `ArgumentException`).
   - If `moveChildren == true` and both blocks are `ContainerBlock`, move children from the old container to the new one preserving order.
   - This method must not implicitly synchronize traversal state; if called during inline traversal and replacing a parent container, the caller must still call `InlineProcessor.ReplaceParentContainer(...)` as specified in §5.1.

### 8.2 Efficient child transfer for `ContainerBlock`

Add:

```csharp
public void TransferChildrenTo(ContainerBlock destination)
```

**Contract:**

- Moves all child blocks from this container to `destination` preserving order.
- Must be O(n) with respect to the number of transferred children (no `RemoveAt(0)` loops).
- Enforces ownership invariants:
  - after transfer, source has `Count == 0`,
  - all moved children have `Parent == destination`.

**Rationale:** enables safe and efficient transforms such as “replace quote with alert and move children” without quadratic removal patterns.

### 8.3 Optional: efficient child transfer for `ContainerInline`

Inlining mutation is already O(n) using `Remove()` per child, but common operations repeatedly rebuild sibling links. If a measurable benefit exists, add:

```csharp
public void TransferChildrenTo(ContainerInline destination)
```

**Contract:**

- Moves all children to `destination` preserving order.
- Must preserve sibling links and set `Parent` on moved nodes correctly.

**Implementation note:** since `Inline.PreviousSibling` has a private setter, an efficient splice implementation may require adding internal helper(s) within `Inline` to update sibling pointers safely.

### 8.4 Span/trivia policy for mutation helpers

Mutation helpers must **not** automatically recompute spans/trivia by default (hidden O(n) work is undesirable).

- The contract should state: callers are responsible for updating `Span`, `Line`, `Column`, and trivia fields if they require exact values after transforms.
- Consider adding optional debug-only validation helpers if span/trivia correctness becomes a recurring source of bugs.

---

## 9. Typed metadata helpers (optional, additive)

Markdig’s `SetData(object, object)` and `MarkdownParserContext.Properties` are flexible but require casting and can risk key collisions.

Add opt-in typed helpers (prefer extension methods to avoid inflating core interfaces):

- `TryGetData<T>(this IMarkdownObject obj, object key, out T value)`
- `GetData<T>(this IMarkdownObject obj, object key)` (throws or returns null based on chosen semantics)
- `GetData<T>(this IMarkdownObject obj)` / `SetData<T>(this IMarkdownObject obj, T value)` using `typeof(T)` as a conventional key

Optional stronger keying:

```csharp
public sealed class DataKey<T>
{
    public object Key { get; } = new object();
}
```

Extensions can hold a static `DataKey<T>` and use `obj.SetData(key.Key, value)` without risk of collision.

---

## 10. Implementation plan (for maintainers)

### Phase 1: Parity + configuration (small, low-risk)

1. Make `InlineProcessor.ReplaceParentContainer` public and add XML docs specifying the contract.
2. Add `BlockProcessor.TakeLinesBefore` public API and update built-in parsers to use it when trivia is enabled.
3. Make `BlockProcessor.IsOpen(Block)` public.
4. Make `MarkdownPipelineBuilder.TrackTrivia` setter public.

### Phase 2: Documentation (medium effort, zero runtime risk)

1. Write/extend the authoring documentation described in §6.
2. Add/upgrade XML docs for:
   - `BlockState` enum members,
   - `BlockProcessor` and `InlineProcessor` authoring-relevant members,
   - `MarkdownPipelineBuilder.TrackTrivia`.

### Phase 3: Helpers (small, low-risk)

1. Add `InlineProcessor.GetParserState` overload(s).
2. Add `InlineProcessor.Emit`.
3. Add debug-only invariants validation for `NewBlocks`.
4. Add `BlockProcessor.TryDiscard`.

### Phase 4: Markdown model (AST) helpers (small-medium, low-risk)

1. Add `Block.Remove` and `Block.ReplaceBy`.
2. Add `ContainerBlock.TransferChildrenTo`.
3. (Optional) Add `ContainerInline.TransferChildrenTo` if justified by benchmarks/usage.

### Phase 5: Typed metadata (optional)

1. Add typed helper extension methods and, optionally, `DataKey<T>`.
2. Update a few built-in extensions to use the helpers as reference patterns.

---

## 11. Test plan

Add focused tests in `src/Markdig.Tests/` for:

1. **Public parity APIs:**
   - `TrackTrivia` is configurable from `MarkdownPipelineBuilder`.
   - `TakeLinesBefore` attaches trivia lines correctly for a custom block parser.
   - `ReplaceParentContainer` enables alert-like container replacement from an external extension without internal access.

2. **Helper methods:**
   - `GetParserState` caches state per leaf and resets between leaves.
   - `TryDiscard` matches `Discard` behavior only when the block is open.

3. **AST mutation helpers:**
   - `ContainerBlock.TransferChildrenTo` preserves order and ownership invariants.
   - `Block.ReplaceBy(..., moveChildren: true)` moves children and leaves the old container empty.

All tests must pass in Release configuration (`cd src; dotnet test -c Release`).

---

## 12. Compatibility and migration notes

- All proposed changes are **additive** or access-level changes, except internal renames (if any) which do not affect external callers.
- If internal helpers are renamed (e.g., trivia transfer), consider keeping internal wrappers to reduce churn and avoid breaking internal consumers.
- Existing extensions continue to work unchanged; new APIs primarily improve what third‑party extensions can do and how safely they can do it.

---

## 13. Open questions

1. Should `ReplaceParentContainer` support multiple replacements per leaf pass, or remain limited to one for simplicity? 
   - Recommendation: limited to one for 1.0
3. Should mutation helpers provide optional debug-only span/trivia validation to catch common mistakes?
   - Recommendation: consider after initial helper implementation and usage patterns are observed.
5. Is an efficient `ContainerInline.TransferChildrenTo` implementation worth the additional internal pointer-manipulation helpers?
   - Recommendation: Probably not. This is not a performance critical path.
6. Should Markdig provide a small “parser test harness” utility for extension authors (input → AST dump) to simplify regression testing?  
   - Recommendation: Probably not in 1.0.
