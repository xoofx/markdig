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

### Header separator

Use `=` instead of `-` for the header separator line (`+===+===+`).
