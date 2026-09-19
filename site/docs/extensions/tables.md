---
title: Tables
---

# Tables

Markdig supports two kinds of tables: **pipe tables** (GitHub-style) and **grid tables** (Pandoc-style).

## Pipe tables

Enable with `.UsePipeTables()` (included in `UseAdvancedExtensions()`).

### Basic syntax

Columns are separated by `|`. A header row is separated from the body by a line of dashes:

```markdown
| Name     | Language | Stars |
|----------|----------|-------|
| Markdig  | C#       | 4.5k  |
| cmark    | C        | 1.6k  |
| markdown-it | JavaScript | 18k |
```

| Name     | Language | Stars |
|----------|----------|-------|
| Markdig  | C#       | 4.5k  |
| cmark    | C        | 1.6k  |
| markdown-it | JavaScript | 18k |

For backward compatibility, Markdig allows individual separator cells to be empty or contain only whitespace, provided at least one cell in the separator row contains dashes:

```markdown
| Field | PersonShared | Person |
| --- | | --- |
| Name | Master | Inherit |
```

An empty separator cell has no explicit alignment. A separator row containing only pipes and whitespace does not define a table. This compatibility behavior is more permissive than strict GFM syntax and is disabled by `UseGfmRules` (see below).

### Strict GFM mode

Opt in to the [GitHub Flavored Markdown table rules](https://github.github.com/gfm/#tables-extension-):

```csharp
using Markdig;
using Markdig.Extensions.Tables;

var pipeline = new MarkdownPipelineBuilder()
    .UsePipeTables(new PipeTableOptions { UseGfmRules = true })
    .Build();
```

Alternatively, use `.Configure("gfm-pipetables")`. This preset previously enabled
only header-based column counting; it now selects the full GFM table mode.
Plain `.UsePipeTables()` and `.UseAdvancedExtensions()` remain permissive by default.
When combining strict tables with `.UseAdvancedExtensions()`, call the configured
`.UsePipeTables(...)` first, since an already registered table extension is not replaced.

In this mode:

- A header and a delimiter row are required, with exactly the same number of cells.
- Every delimiter cell needs at least one dash, optionally surrounded by alignment
  colons. Empty delimiter cells and spaces between dashes/colons are rejected.
- The header determines the width: short body rows are padded and excess cells are
  ignored. Body rows do not need to contain a pipe.
- Unescaped pipes split cells **before** inline parsing, even inside code, HTML,
  or link labels. Use `\|` for a literal pipe, including inside code spans.
- As in `cmark-gfm`, a pipe immediately preceded by a backslash stays in its cell
  regardless of the length of the backslash run. One backslash is removed before
  inline parsing, including reference-link lookup and autolinks.
- List markers take precedence: `- | -` starts a list, not a delimiter row.
  Use `|-|-|` or `-- | --` to avoid that ambiguity.
- Blank lines and other block-level structures end the table. Inline spans cannot
  cross cell or row boundaries. A lone `|` also ends the table; `||` represents
  one empty cell and can continue it.
- Cell-edge spaces and tabs are trimmed; non-breaking and other Unicode spaces
  are preserved. A rejected header/delimiter cell-count match is not retried
  within the same paragraph.

`UseGfmRules` overrides `RequireHeaderSeparator` and `UseHeaderForColumnCount`
without modifying those option values. Width inference is still available as an
additional, non-GFM rendering feature. This option changes table parsing only;
it does not enable other GFM extensions such as task lists or strikethrough.

Strict mode is differentially tested against a pinned native `cmark-gfm` build,
not just the published specification examples. The repository's
`tools/GfmTableDifferential/README.md` records the corpus, reproduction commands,
and known limitations. This does **not** promise identical whole-document HTML:
Markdig's other block parsers, trivia mode and renderers still have differences.

### Column alignment

Use colons in the separator row to control alignment:

```markdown
| Left   | Center  | Right  |
|:-------|:-------:|-------:|
| one    | two     | three  |
| four   | five    | six    |
```

| Left   | Center  | Right  |
|:-------|:-------:|-------:|
| one    | two     | three  |
| four   | five    | six    |

### Optional leading/trailing pipes

The outer pipes are optional:

```markdown
Name | Language
-----|--------
Markdig | C#
cmark | C
```

Name | Language
-----|--------
Markdig | C#
cmark | C

### Inline formatting in cells

Cells support inline Markdown — emphasis, code, links, etc.:

```markdown
| Feature       | Status        |
|---------------|---------------|
| **Bold**      | ~~removed~~   |
| `code`        | [link](#)     |
```

| Feature       | Status        |
|---------------|---------------|
| **Bold**      | ~~removed~~   |
| `code`        | [link](#)     |

### Escaped pipes

Use `\|` to include a literal pipe inside a cell:

```markdown
| Expression   | Result |
|-------------|--------|
| `a \| b`    | a or b |
```

### Options

`UsePipeTables` accepts a `PipeTableOptions` object:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UsePipeTables(new PipeTableOptions
    {
        UseHeaderForColumnCount = true // GFM-compatible column counting
    })
    .Build();
```

| Option                           | Default | Description |
|----------------------------------|---------|-------------|
| `UseGfmRules`                    | `false` | Use strict GFM table parsing, requiring a matching delimiter row and header-based column counting regardless of the next two options. |
| `RequireHeaderSeparator`         | `true`  | Whether the dashed separator row is required. Set to `false` for Kramdown-style tables that allow headerless tables. |
| `UseHeaderForColumnCount`        | `false` | When `true`, the header row's column count is authoritative — short rows are padded with empty cells and extra cells in wider rows are dropped. When `false`, the widest row determines the column count. |
| `InferColumnWidthsFromSeparator` | `false` | When `true`, populates `TableColumnDefinition.Width` based on the dash count of each column in the separator row, normalized to percentages that sum to 100. When `false`, `Width` stays `0` and no width information is emitted. |

#### Inferring column widths from the separator

With `InferColumnWidthsFromSeparator = true`, the width of each column is proportional to the number of `-` characters under it in the separator row. This is useful when you want authors to control relative column widths directly in the Markdown source.

An empty separator cell contributes no dashes and gets `Width = 0`; the nonempty separator cells determine the remaining widths.

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UsePipeTables(new PipeTableOptions { InferColumnWidthsFromSeparator = true })
    .Build();
```

Given this input:

```markdown
| A | B |
|---|--------|
| 1 | 2      |
```

the first column gets `Width = 25` and the second `Width = 75` (a 3:9 ratio of dashes, normalized to 100). The HTML renderer emits a `<colgroup>` with `<col style="width:N%" />` entries so the widths flow through to the rendered table. The values are also available on `Table.ColumnDefinitions[i].Width` for custom renderers.

### Normalizing pipe tables

`Markdown.Normalize(markdown, pipeline: pipeline)` writes pipe tables with outer
pipes and consistent cell spacing. It preserves explicit column alignments and
emits a separator for every header cell, including columns added to accommodate
wider body rows. With `InferColumnWidthsFromSeparator` enabled, the original
separator dash counts (including zero for empty cells) are retained for parsed
columns so their inferred width proportions survive normalization. Without width inference, separators use three
dashes.

For tables parsed in GFM mode, normalization also escapes pipes in code spans and
inline HTML so those pipes do not become cell boundaries when parsed again.

This support targets pipe tables, not grid tables. When both extensions are
enabled (including via `UseAdvancedExtensions()`), the normalizer also attempts
to write grid tables as pipe tables. Multiline cells, spans, and richer grid-table
content are not reliably preserved; do not use this conversion for lossless
grid-table round-tripping.

## Grid tables

Enable with `.UseGridTables()` (included in `UseAdvancedExtensions()`).

Grid tables use `+`, `-`, and `|` characters to draw a grid. They support multi-line cells, column spanning, and richer content than pipe tables.

### Basic grid table

```markdown
+-----------+-----------+
| Header 1  | Header 2  |
+===========+===========+
| Cell 1    | Cell 2    |
+-----------+-----------+
| Cell 3    | Cell 4    |
+-----------+-----------+
```

+-----------+-----------+
| Header 1  | Header 2  |
+===========+===========+
| Cell 1    | Cell 2    |
+-----------+-----------+
| Cell 3    | Cell 4    |
+-----------+-----------+

### Multi-line cells

Grid table cells can contain multiple lines and block-level content:

```markdown
+-----------+-------------------+
| Name      | Description       |
+===========+===================+
| Markdig   | A fast, powerful  |
|           | Markdown parser.  |
+-----------+-------------------+
| cmark     | The C reference   |
|           | implementation.   |
+-----------+-------------------+
```

+-----------+-------------------+
| Name      | Description       |
+===========+===================+
| Markdig   | A fast, powerful  |
|           | Markdown parser.  |
+-----------+-------------------+
| cmark     | The C reference   |
|           | implementation.   |
+-----------+-------------------+

### Column spanning

Use a continuous line (without `+` separators) to span columns:

```markdown
+-------+-------+
| A     | B     |
+=======+=======+
| Cell spanning |
+-------+-------+
```
+-------+-------+
| A     | B     |
+=======+=======+
| Cell spanning |
+-------+-------+

### Header separator

Use `=` instead of `-` for the header separator line (`+===+===+`).
