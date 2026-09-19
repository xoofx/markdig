# Native GFM table differential verification

This development-only runner compares strict Markdig pipe tables with the actual
`github/cmark-gfm` executable. It does not download, build, or install native code.
It is intentionally outside the main solution and adds no package dependencies.

## Oracle and reproduction

The oracle is upstream revision **499789b49373bfa045d0e7547e5ee63444c77bca**,
invoked with **`--unsafe -e table`**. Only the table extension is enabled on both
sides; unsafe HTML rendering matches Markdig's default HTML behavior. Do not run
this test against an untrusted executable.

With Git, CMake, a C compiler and the .NET 10 SDK already available, use a separate
scratch directory for the upstream checkout and build:

```sh
git clone https://github.com/github/cmark-gfm.git cmark-gfm
git -C cmark-gfm checkout 499789b49373bfa045d0e7547e5ee63444c77bca
cmake -S cmark-gfm -B native-build -DCMARK_SHARED=OFF -DCMARK_STATIC=ON -DCMARK_TESTS=OFF -DCMAKE_BUILD_TYPE=Release
cmake --build native-build --config Release
```

The verified Windows build used Visual Studio 18 2026. Its executable is
`native-build/src/Release/cmark-gfm.exe`; single-configuration generators normally
place it in `native-build/src/cmark-gfm` instead.

From the Markdig repository root, pass absolute paths to the executable, upstream
checkout and an external report directory:

```sh
dotnet run -c Release --project tools/GfmTableDifferential -- <executable> <checkout> <reports>
```

The runner checks the checkout's revision. Ensure the executable was built from
that clean checkout. A fourth argument explicitly regenerates the checked-in
snapshot file:

```sh
dotnet run -c Release --project tools/GfmTableDifferential -- <executable> <checkout> <reports> src/Markdig.Tests/Specs/GfmPipeTableDifferential.json
```

Review regenerated expectations before committing. Native outputs are cached in
the report directory by input and pinned revision. Use a new report directory to
force a fresh native run. Reports include the entire corpus, full HTML/trivia
mismatches, table HTML mismatches, and inline normalization mismatches. Exit code
1 indicates table or inline-normalization differences; **exit code 0 does not
mean all full-document outputs match**. Full HTML differences are always reported.

## Coverage and results (2026-09-19)

The fixed corpus contains **11,038 distinct inputs**:

- 653 distinct applicable `test/spec.txt` examples (including all eight official
  table examples), with the spec's arrow-to-tab conversion;
- 16 additional distinct table examples from `test/extensions.txt`;
- systematic header/delimiter combinations, empty and single-column tables,
  mismatched column counts and failed-header recovery;
- spaces, tabs, vertical tabs, form feeds, NBSP and Unicode spaces;
- body padding/truncation, pipe-less rows, terminating blocks and indentation;
- paragraphs, quotes, ordered/unordered lists and nested combinations;
- code, emphasis, HTML, links, images, entities, references and autolinks;
- backslash runs of length zero through eight in six inline contexts;
- 4,000 seeded random combinations (seed 955), deduplicated with the other inputs.

After the fixes:

- **0 table HTML differences**, both normally and with trivia tracking;
- **0 inline normalization differences** over 866 inline/backslash inputs;
- **9,704 full HTML matches**, with **1,334 full-document differences** remaining.
  Of these, 170 contain identical table HTML but different surrounding HTML.
  The other 1,164 have no tables and reproduce with Markdig's default pipeline.
  These are not hidden by the table-only result.

Comparison canonicalizes CRLF, equivalent `align`/`style` alignment attributes,
outer HTML whitespace and a formatting newline after `<li>` before a block tag.
It does not collapse cell whitespace, remove paragraphs, or rewrite links.
The table-only diagnostic extracts complete `<table>...</table>` HTML fragments;
it does **not** prove that tables occupy the same position in the enclosing AST.

Known differences outside table parsing include thematic breaks escaping indented
list items, HTML/list block interactions, whitespace trimming in ordinary
paragraphs, trivia-preserved indentation, and some CommonMark inline behavior.
For example, a thematic break immediately after a table in a list can be placed
outside the list by Markdig. These need separate core-parser fixes rather than
changing the permissive table parser or masking the HTML comparison.

Finite testing is not a proof of equivalence. The corpus is bounded, not exhaustive
fuzzing, and does not verify every other extension combination or native source
position. Normalization, widths and Markdig source locations have separate tests.
The native column and auto-padding limits are mirrored; the large auto-padding
threshold is not exercised by this small-input differential corpus.

## Permanent regression coverage

`TestGfmPipeTableDifferential` runs **2,975 native-output snapshots** in normal CI,
without requiring CMake or `cmark-gfm`. Selection is fixed by input category, not
by whether Markdig passes. **1,703 cases compare full HTML**; the remaining cases
compare table presence/content to isolate known surrounding core differences.
Every case also compares table output with trivia tracking. The fixture records
the full native HTML even when only table content is asserted.

Additional tests exercise normalization and idempotence, source spans after pipe
unescaping, pooled-parser reuse, widths, column limits and the permissive defaults.
The existing official eight-example spec fixture is retained separately.

```sh
dotnet test src/Markdig.Tests -c Release -f net10.0 --filter FullyQualifiedName~TestGfmPipeTable
```

Upstream specification examples are licensed under
[CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/). The checked-in
snapshot selection uses the synthetic corpus; the official examples retain their
attribution in `src/Markdig.Tests/Specs/GfmTableSpecs.md`.
