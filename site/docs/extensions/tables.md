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
| `RequireHeaderSeparator`         | `true`  | Whether the dashed separator row is required. Set to `false` for Kramdown-style tables that allow headerless tables. |
| `UseHeaderForColumnCount`        | `false` | When `true`, the header row's column count is authoritative — short rows are padded with empty cells and extra cells in wider rows are dropped. When `false`, the widest row determines the column count. |
| `InferColumnWidthsFromSeparator` | `false` | When `true`, populates `TableColumnDefinition.Width` based on the dash count of each column in the separator row, normalized to percentages that sum to 100. When `false`, `Width` stays `0` and no width information is emitted. |

#### Inferring column widths from the separator

With `InferColumnWidthsFromSeparator = true`, the width of each column is proportional to the number of `-` characters under it in the separator row. This is useful when you want authors to control relative column widths directly in the Markdown source.

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
