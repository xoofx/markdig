# Markdig — Codex Agent Instructions

Markdig is a fast, powerful, CommonMark compliant, extensible Markdown processor for .NET.

Paths/commands below are relative to this directory.

## Orientation

- Workspace: `src/markdig.slnx` (solution/workspace listing the projects below)
- Library: `src/Markdig/`
- Tests: `src/Markdig.Tests/` (NUnit; includes generated spec tests)
- Signed library build: `src/Markdig.Signed/` (strong-named build of `src/Markdig/` sources)
- Benchmarks: `src/Markdig.Benchmarks/` (BenchmarkDotNet perf comparisons)
- Fuzzing: `src/Markdig.Fuzzing/` (SharpFuzz/libFuzzer harness for parser/renderers)
- Web app: `src/Markdig.WebApp/` (ASP.NET Core API for converting Markdown → HTML)
- Tooling: `src/mdtoc/` (CLI to generate a Markdown TOC from a local file or GitHub URL)
- Tooling: `src/SpecFileGen/` (regenerates `*.generated.cs` spec tests from `src/Markdig.Tests/**.md`)
- Tooling: `src/UnicodeNormDApp/` (generates the Unicode→ASCII mapping used by `Markdig.Helpers.CharNormalizer`)
- Docs to keep in sync with behavior: `readme.md` and the docs under `doc/` (e.g., `doc/**/*.md`)

## Build & Test

```sh
# from the project root (this folder)
cd src
dotnet build -c Release
dotnet test -c Release
```

All tests must pass and docs must be updated before submitting.

## Contribution Rules (Do/Don't)

- Keep diffs focused; avoid drive-by refactors/formatting and unnecessary dependencies.
- Follow existing patterns and naming; prefer clarity over cleverness.
- New/changed behavior requires tests; bug fix = regression test first, then fix.
- All public APIs require XML docs (avoid CS1591) and should document thrown exceptions.

## C# Conventions (Project Defaults)

- Naming: `PascalCase` public/types/namespaces, `camelCase` locals/params, `_camelCase` private fields, `I*` interfaces.
- Style: file-scoped namespaces; `using` outside namespace (`System` first); `var` when the type is obvious.
- Nullability: enabled — respect annotations; use `ArgumentNullException.ThrowIfNull()`; prefer `is null`/`is not null`; don't suppress warnings without a justification comment.
- Exceptions: validate inputs early; throw specific exceptions (e.g., `ArgumentException`/`ArgumentNullException`) with meaningful messages.
- Async: `Async` suffix; no `async void` (except event handlers); use `ConfigureAwait(false)` in library code; consider `ValueTask<T>` on hot paths.

## Performance / AOT / Trimming

- Minimize allocations (`Span<T>`, `stackalloc`, `ArrayPool<T>`, `StringBuilder` in loops).
- Keep code AOT/trimmer-friendly: avoid reflection; prefer source generators; use `[DynamicallyAccessedMembers]` when reflection is unavoidable.
- Use `sealed` for non-inheritable classes; prefer `ReadOnlySpan<char>` for parsing.

## API Design

- Follow .NET guidelines; keep APIs small and hard to misuse.
- Prefer overloads over optional parameters (binary compatibility); consider `Try*` methods alongside throwing versions.
- Mark APIs `[Obsolete("message", error: false)]` before removal once stable (can be skipped while pre-release).

## Git / Pre-Submit

- Commits: commit after each self-contained logical step; imperative subject, < 72 chars; one logical change per commit; reference issues when relevant; don't delete unrelated local files.
- Checklist: each self-contained step is committed; build+tests pass; docs updated if behavior changed; public APIs have XML docs; changes covered by unit tests.
